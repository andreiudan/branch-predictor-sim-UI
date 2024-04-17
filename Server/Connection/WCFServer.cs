using System;
using System.ServiceModel;
using Newtonsoft;
using System.Threading;
using System.Threading.Tasks;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
                 ConcurrencyMode = ConcurrencyMode.Reentrant)]
public class WCFServer : IWCFServer
{
    int processes;

    public WCFServer()
    {
        processes = 0;
    }

    public async string executeCommand(string serializedArg)
    {
        processes++;

        Params arg = Newtonsoft.Json.JsonConvert.DeserializeObject<Params>(serializedArg);

        await Task.Run(() =>
        {
            Thread.Sleep(10000);
        });

        Console.WriteLine(serializedArg);

        processes--;

        return "";

    }

    public int getProcesses()
    {
        return processes;
    }
}
