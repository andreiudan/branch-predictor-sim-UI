using System;
using Domain.DTOs;
using System.Threading;

public class CommandExecutor
{
    public CommandExecutor()
    {
    }

    public Result execute(Params param)
    {
        Thread.Sleep(5000);

        Console.WriteLine("I was here");

        return new Result();
    }
}
