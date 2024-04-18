using System;
using System.Threading;
using Domain.DTOs;
using Interfaces;

public class WCFServer : IWCFServer
{
    public WCFServer()
    {
    }

    public string executeCommand(string serializedArg)
    {
        Params arg = Newtonsoft.Json.JsonConvert.DeserializeObject<Params>(serializedArg);

        Thread.Sleep(10000);

        Console.WriteLine(serializedArg);

        return "";

    }
}
