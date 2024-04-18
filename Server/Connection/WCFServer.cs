using System;
using System.Threading;
using Domain.DTOs;
using Interfaces;
using System.Collections.Generic;

public class WCFServer : IWCFServer
{
    static int processes = 0;

    public WCFServer()
    {
    }

    public int getProcessesNo()
    {
        return processes;
    }

    public string executeCommands(string serializedParams)
    {
        processes++;

        List<Params> arg = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Params>>(serializedParams);

        Thread.Sleep(10000);

        Console.WriteLine(serializedParams);

        processes--;

        return "";
    }
}
