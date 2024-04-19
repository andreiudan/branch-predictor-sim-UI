namespace Domain.DTOs
{
    public class Params
    {
        public string benchName { get; set; }

        public int maxInst { get; set; } = 1000000000;

        public string bpred { get; set; } = "bimod";

        public int tableSize { get; set; } = 2048;

        public int l1Size { get; set; } = 1;
        public int l2Size { get; set; } = 1024;
        public int histSize { get; set; } = 8;
        public int xor { get; set; } = 0;

        public int metaTableSize { get; set; } = 1024;

        public int rasSize { get; set; } = 8;

        public int numSets { get; set; } = 512;
        public int assoc { get; set; } = 4;
    }
}
