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
        // Se deserializeaza lista parametrilor simularilor
        List<Params> args = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Params>>(serializedParams);

        List<Task<Result>> resultTasks = new List<Task<Result>>();

        // pentru fiecare instructiune se creaza un thread nou si se modifica numarul de procese de pe server
        foreach(var param in args)
        {
            var newTask = Task.Run(() =>
            {
                processes++;

                // Executia efectiva a comenzii
                var result = commandExecutor.execute(param);

                processes--;

                return result;
            });

            resultTasks.Add(newTask);
        }

        // Asteapta pana cand toate thread-urile termina executia simularii 
        Task.WaitAll(resultTasks.ToArray());

        // Creaza obiectul Result
        List<Result> results = new List<Result>();

        foreach(var resultTask in resultTasks)
        {
            results.Add(resultTask.Result);
        }

        // Serializarea rezultatului
        return Newtonsoft.Json.JsonConvert.SerializeObject(results);
    }

    public string ping()
    {
        return "OK";
    }
}
