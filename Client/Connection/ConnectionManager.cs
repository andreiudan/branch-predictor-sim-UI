using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ConnectionManager
{
    List<Server> servers = new List<Server>();
    int index;

    public ConnectionManager()
    {
        index = 0;
    }

    public void addConnection(string IP, string port)
    {
        servers.Add(new Server(IP, port));
    }

    private Server getNextAvailableServer()
    {
        var server = servers[index];

        index = (index + 1) % servers.Count;

        return server;
    }

    public List<Task<Result>> executeCommands(List<Params> args)
    {
        List<Task<Result>> results = new List<Task<Result>>();

        foreach (var arg in args)
        {
            Server availableServer = getNextAvailableServer();

            results.Add(availableServer.executeCommand(arg));
        }

        return results;
    }
}
