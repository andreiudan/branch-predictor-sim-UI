using System;
using System.Collections.Generic;
public class Benchmark
{
    public string benchmarkFile { get; set; }
    public string inFile { get; set; }

    public static Dictionary<string, Benchmark> Benchmarks = new Dictionary<string, Benchmark>()
    {
        {
            "Applu",
            new Benchmark
            {
                benchmarkFile = "benchmarks/applu.ss",
                inFile = "inputs/applu.in"
            }
        },
        {
            "Apsi",
            new Benchmark
            {
                benchmarkFile = "benchmarks/apsi.ss",
                inFile = "inputs/apsi.in"
            }
        },
        {
            "Cc1",
            new Benchmark
            {
                benchmarkFile = "benchmarks/cc1.ss",
                inFile = "inputs/1stmt.i"
            }
        },
        {
            "Fpppp",
            new Benchmark
            {
                benchmarkFile = "benchmarks/fpppp.ss",
                inFile = "inputs/natoms.in"
            }
        },
        {
            "Go",
            new Benchmark
            {
                benchmarkFile = "benchmarks/go.ss",
                inFile = "inputs/9stone21.in"
            }
        }
    };

    public Benchmark()
    {
    }
}