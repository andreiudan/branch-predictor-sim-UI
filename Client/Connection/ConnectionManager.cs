using System;
using System.Collections.Generic;

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

    private Server getNextAvailableServer()
    {
        int minProc = servers[0].getProcesses();
        Server result = servers[0];

        for (int i = 1; i < servers.Count; i++)
        {
            int temp = servers[i].getProcesses();

            if (temp < minProc)
            {
                minProc = temp;
                result = servers[i];
            }
        }

        return result;
    }

    public List<Result> executeCommands(List<Params> args)
    {
        List<Result> results = new List<Result>();

        foreach (var arg in args)
        {
            Server availableServer = getNextAvailableServer();

            results.Add(availableServer.executeCommand(arg));
        }

        return results;
    }
}
