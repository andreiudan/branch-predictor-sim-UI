const exec = require('child_process').exec;

const benchmarks = [
    {
        name: "applu",
        inFile: "applu.in",
        bench: "applu.ss"
    },
    {
        name: "apsi",
        inFile: "apsi.in",
        bench: "apsi.ss"
    },
    {
        name: "cc1",
        inFile: "1stmt.i",
        bench: "cc1.ss"
    },
    {
        name: "fpppp",
        inFile: "natoms.in",
        bench: "fpppp.ss"
    },
    {
        name: "go",
        inFile: "9stone21.in",
        bench: "go.ss"
    }
]

const sims = {
    cache: (bench) => {return `./sim-fast -redir:sim ./cache-${bench.name}/simout.res -redir:prog ./cache-${bench.name}/progout.res ./benchmarks/${bench.bench}<./inputs/${bench.inFile}`},
    // cheetah: () => {},
    // bpred: () => {},
    // outorder: () => {}
}

benchmarks.forEach(benchmark=>{
    for(const key in sims){
        exec(sims[key](benchmark), (err, stdout, stderr)=>{
            if(err)
                console.log(err);

            if(stderr)
                console.log(stderr);
        })
    }
})