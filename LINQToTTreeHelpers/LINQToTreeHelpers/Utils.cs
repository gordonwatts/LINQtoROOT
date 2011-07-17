
namespace LINQToTreeHelpers
{
    internal static class Utils
    {
        /// <summary>
        /// Write out an object. Eventually, with ROOTNET improvements this will work better and perahps
        /// won't be needed!
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dir"></param>
        public static void InternalWriteObject(this ROOTNET.Interface.NTObject obj, ROOTNET.Interface.NTDirectory dir)
        {
            var h = obj as ROOTNET.Interface.NTH1;
            if (h != null)
            {
                var copy = h.Clone();
                dir.WriteTObject(copy); // Ugly from a memory pov, but...
                copy.SetNull();
            }
            else
            {
                dir.WriteTObject(obj);
                obj.SetNull();
            }
        }

        /// <summary>
        /// Take a string and "sanatize" it for a root name.
        /// </summary>
        /// <param name="name">Text name to be used as a ROOT name</param>
        /// <returns>argument name with spaces removes, as well as other characters</returns>
        public static string FixupForROOTName(this string name)
        {
            var result = name.Replace(" ", "");
            result = result.Replace("_{", "");
            result = result.Replace("{", "");
            result = result.Replace("}", "");
            result = result.Replace("-", "");
            result = result.Replace("\\", "");
            result = result.Replace("%", "");
            result = result.Replace("<", "");
            result = result.Replace(">", "");
            return result;
        }
    }
}
