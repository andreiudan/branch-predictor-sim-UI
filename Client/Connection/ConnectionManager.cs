using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Domain.DTOs;

public class ConnectionManager
{
    List<Server> servers = new List<Server>();

    public ConnectionManager()
    {
    }

    public void addConnection(string IP, string port)
    {
        servers.Add(new Server(IP, port));
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

    public List<Result> executeCommands(List<Params> args)
    {
        var balancedCommands = balanceCommands(args);
        List<Result> results = new List<Result>();

        foreach(var comm in balancedCommands)
        {
            Task.Run(() =>
            {
                results.AddRange(comm.Key.executeCommands(comm.Value));
            });
        }

        return results;
    }
}
