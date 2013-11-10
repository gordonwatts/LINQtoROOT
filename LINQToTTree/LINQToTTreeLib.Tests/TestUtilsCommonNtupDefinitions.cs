using LINQToTTreeLib.CodeAttributes;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    /// An ntuple that contains an int, and a 1 and 2D array. The 1D array is indexed off the integer,
    /// and the 2D is constant size by the integer.
    /// </summary>
    public class dummyntup
    {
        public int run;
        public int[] vals;
        public int[][] val2D;

        [ArraySizeIndex("run")]
        public int[] valC1D;

        [ArraySizeIndex("20", IsConstantExpression = true)]
        public int[] valC1DConst;

        [ArraySizeIndex("20", IsConstantExpression = true, Index = 0)]
        [ArraySizeIndex("run", Index = 1)]
        public int[][] valC2D;
    }

    /// <summary>
    /// A very simple ntuple that contains only a single entry - runs, which is an integer.
    /// </summary>
    public class ntup
    {
        public int run;
    }

}
