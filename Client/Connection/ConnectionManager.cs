using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DTOs;
using Domain.EventArgs;

namespace Client.Connection
{
    public delegate void ConnectionModifiedHandler(object sender, ConnectionStatusEventArgs args);

    public class ConnectionManager
    {
        public event ConnectionModifiedHandler ConnectionStatusModified;

        List<Server> servers = new List<Server>();

        public async Task<ServerStatus> addConnection(string IP, string port)
        {
            try
            {
                servers.Add(new Server(IP, port));
            }
            catch(Exception ex)
            {
                await Task.Run(() => OnConnectionStatusModified(IP, port, ServerStatus.ConnectionFailed.ToString()));
                return ServerStatus.ConnectionFailed;
            }

            await Task.Run(() => OnConnectionStatusModified(IP, port, ServerStatus.Connected.ToString()));
            return ServerStatus.Connected;
        }

        protected virtual void OnConnectionStatusModified(string ip, string port, string status)
        {
            ConnectionStatusModified?.Invoke(this, new ConnectionStatusEventArgs(ip, port, status));
        }

        private Dictionary<Server, List<Params>> balanceCommands(List<Params> paramList)
        {
            var processMap = getProcessMap();

            Dictionary<Server, List<Params>> balancedMap = new Dictionary<Server, List<Params>>();

            foreach (var process in processMap)
            {
                balancedMap.Add(process.Key, new List<Params>());
            }

            foreach (var param in paramList)
            {
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

        public async Task<List<Result>> executeCommands(List<Params> args)
        {
            var balancedCommands = balanceCommands(args);
            List<Result> results = new List<Result>();

            foreach (var comm in balancedCommands)
            {
                await Task.Run(() => OnConnectionStatusModified(comm.Key.Ip, comm.Key.Port, ServerStatus.Simulating.ToString()));

                await Task.Run(() =>
                {
                    results.AddRange(comm.Key.executeCommands(comm.Value));
                });

                await Task.Run(() => OnConnectionStatusModified(comm.Key.Ip, comm.Key.Port, ServerStatus.Simulated.ToString()));
            }

            return results;
        }
    }

}