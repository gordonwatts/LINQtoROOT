
namespace LINQToTreeHelpers
{
    /// <summary>
    /// Some helper routines for dealing with root.
    /// </summary>
    public static class ROOTUtils
    {
        /// <summary>
        /// Add a histogram to a ROOT directory.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="h"></param>
        public static void Add(this ROOTNET.Interface.NTDirectory dir, ROOTNET.Interface.NTH1 h)
        {
            h.SetDirectory(dir);
        }

        /// <summary>
        /// Save a plot to a TDirectory. Return the plot so it can also be used in other
        /// places.
        /// 
        /// Temp Fix: We need to set the object owner so that this object won't be cleaned up during
        /// GC, however, ROOT.NET doesn't support it yet. So instead we will just set it to null and
        /// return null for now. To be fixed when we update ROOT.NET.
        /// </summary>
        /// <param name="hist"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static ROOTNET.Interface.NTH1 SaveToROOTDirectory(this ROOTNET.Interface.NTH1 hist, ROOTNET.Interface.NTDirectory dir)
        {
            hist.SetDirectory(dir);
            hist.SetNull();
            return null;
        }

        /// <summary>
        /// Do an immediate write to a root directory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dir"></param>
        public static void WriteToROOTDirectory<T>(this T obj, ROOTNET.Interface.NTDirectory dir)
            where T : ROOTNET.Interface.NTObject
        {
            dir.WriteTObject(obj);
        }

    }
}
