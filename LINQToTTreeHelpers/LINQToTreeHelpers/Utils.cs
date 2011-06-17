
namespace LINQToTreeHelpers
{
    internal static class Utils
    {
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
    }
}
