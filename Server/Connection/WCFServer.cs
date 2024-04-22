using Domain.DTOs;
using Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

public class WCFServer : IWCFServer
{
    static int processes = 0;
    CommandExecutor commandExecutor;

    public WCFServer()
    {
        commandExecutor = new CommandExecutor();
    }

    public int getProcessesNo()
    {
        return processes;
    }

    public string executeCommands(string serializedParams)
    {
        List<Params> args = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Params>>(serializedParams);

        List<Task<Result>> resultTasks = new List<Task<Result>>();

        foreach(var param in args)
        {
            var newTask = Task.Run(() =>
            {
                processes++;

                var result = commandExecutor.execute(param);

                processes--;

                return result;
            });

            resultTasks.Add(newTask);
        }

        Task.WaitAll(resultTasks.ToArray());

        List<Result> results = new List<Result>();

        foreach(var resultTask in resultTasks)
        {
            results.Add(resultTask.Result);
        }

        return Newtonsoft.Json.JsonConvert.SerializeObject(results);
    }
}
