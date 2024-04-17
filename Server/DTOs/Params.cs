using System;
public class Params
{
    public string benchName { get; set; }

    public int maxInst { get; set; }
    public string bpred { get; set; }
    public int l1Size { get; set; }
    public int l2Size { get; set; }

    public int tableSize { get; set; }
    public int metaTableSize { get; set; }
    public int rasSize { get; set; }

    public int numSets { get; set; }
    public int histSize { get; set; }
    public int xor { get; set; }
    public int assoc { get; set; }
}
