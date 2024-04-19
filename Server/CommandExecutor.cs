using System;
using Domain.DTOs;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

public class CommandExecutor
{
    string simulatorBasePath= "./../../../simplesim-3.0/";

    public CommandExecutor()
    {
    }

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

        Process process = Process.Start(simulatorBasePath + "sim-bpred", prepareArguments(param, uniqueId));
        process.Start();
        process.WaitForExit();

        StreamReader simoutReader = new StreamReader($"./{uniqueId}-sim.res");
        var simoutRes = simoutReader.ReadToEnd();

        StreamReader progoutReader = new StreamReader($"./{uniqueId}-prog.res");
        var progoutRes = progoutReader.ReadToEnd();

        cleanup(uniqueId);

        return new Result() {
            benchName = param.benchName,
            simoutFile = simoutRes,
            progoutFile = progoutRes
        };
    }
}
