using System;
using Domain.DTOs;
using System.Diagnostics;
using System.IO;

public class CommandExecutor
{
    string simulatorBasePath= "./../../../simplesim-3.0/";

    private string prepareArguments(Params param, string uniqueId)
    {
        string args = "";

        args += $" -max:inst {param.maxInst}";
        args += $" -bpred {param.bpred}";
        args += $" -bpred:bimod {param.tableSize}";
        args += $" -bpred:2lev {param.l1Size} {param.l2Size} {param.histSize} {param.xor}";
        args += $" -bpred:comb {param.metaTableSize}";
        args += $" -bpred:ras {param.rasSize}";
        args += $" -bpred:btb {param.numSets} {param.assoc}";
        args += $" -redir:prog ./{uniqueId}-prog.res";
        args += $" -redir:sim ./{uniqueId}-sim.res";

        args += $" {simulatorBasePath}{Benchmark.Benchmarks[param.benchName].benchmarkFile} < {simulatorBasePath}{Benchmark.Benchmarks[param.benchName].inFile}";

        return args;
    }

    private void cleanup(string uniqueId)
    {
        File.Delete($"./{uniqueId}-sim.res");
        File.Delete($"./{uniqueId}-prog.res");
    }

    public Result execute(Params param)
    {
        string uniqueId = Guid.NewGuid().ToString();

        var command = simulatorBasePath + "sim-bpred " + prepareArguments(param, uniqueId);

        Process process = Process.Start(simulatorBasePath + "sim-bpred", prepareArguments(param, uniqueId));
        process.Start();
        process.WaitForExit();

        StreamReader simoutReader = new StreamReader($"./{uniqueId}-sim.res");
        var simoutRes = simoutReader.ReadToEnd();

        StreamReader progoutReader = new StreamReader($"./{uniqueId}-prog.res");
        var progoutRes = progoutReader.ReadToEnd();

        Console.WriteLine(command);

        cleanup(uniqueId);

        return new Result() {
            benchName = param.benchName,
            simoutFile = simoutRes,
            progoutFile = progoutRes
        };
    }
}
