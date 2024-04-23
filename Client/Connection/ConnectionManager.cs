using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DTOs;
using Domain.EventArgs;
using System.IO;
using System.Threading;

namespace Client.Connection
{
    public delegate void ConnectionModifiedHandler(object sender, ConnectionStatusEventArgs args);

    public class ConnectionManager
    {
        public event ConnectionModifiedHandler ConnectionStatusModified;

        List<Server> servers = new List<Server>();

        public void addConnection(string IP, string port)
        {
            var server = new Server(IP, port);

            servers.Add(server);

            Task.Run(() =>
            {
                while (true)
                {
                    var status = server.ping();

                    OnConnectionStatusModified(IP, port, status.ToString());

                    Thread.Sleep(1000);
                }
            });
        }

        protected virtual void OnConnectionStatusModified(string ip, string port, string status)
        {
            ConnectionStatusModified?.Invoke(this, new ConnectionStatusEventArgs(ip, port, status));
        }

        private Dictionary<Server, List<Params>> balanceCommands(List<Params> paramList)
        {
            // Obtine numarul de procese curente de pe fiecare server
            Dictionary<Server, int> processMap = getProcessMap();

            // Creaza un dictionar pentru a atribui fiecarui server o lista de simulari
            Dictionary<Server, List<Params>> balancedMap = new Dictionary<Server, List<Params>>();

            foreach (var process in processMap)
            {
                balancedMap.Add(process.Key, new List<Params>());
            }

            foreach (var param in paramList)
            {
                // Se gaseste serverul care are cele mai putine procese la momenul curent
                int minimum = int.MaxValue;
                Server server = null;

                foreach (var process in processMap)
                {
                    if (process.Value < minimum)
                    {
                        server = process.Key;
                        minimum = process.Value;
                    }
                }

                // Se adauga simulare in lista proceselor serverului
                processMap[server]++;
                balancedMap[server].Add(param);
            }

            return balancedMap;
        }

        private Dictionary<Server, int> getProcessMap()
        {
            Dictionary<Server, int> processesMap = new Dictionary<Server, int>();

            foreach (var server in servers)
            {
                int processes = server.getProcessesNo();

                processesMap.Add(server, processes);
            }

            return processesMap;
        }

        public List<Result> executeCommands(List<Params> args)
        {
            var balancedCommands = balanceCommands(args);
            List<Result> results = new List<Result>();

            foreach (var comm in balancedCommands)
            {
                OnConnectionStatusModified(comm.Key.Ip, comm.Key.Port, ServerStatus.Simulating.ToString());

                var temp = comm.Key.executeCommands(comm.Value);

                results.AddRange(temp);

                OnConnectionStatusModified(comm.Key.Ip, comm.Key.Port, ServerStatus.Simulated.ToString());
            }

            return results;
        }
    }

}