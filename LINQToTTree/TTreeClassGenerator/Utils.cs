
namespace TTreeClassGenerator
{
    static class Utils
    {
        /// <summary>
        /// It is possible to use C# reserved keywords as leaf names and other
        /// illegal C++ things. We have to alter them. In general, they need to be altered in a way
        /// that the ROOT make proxy would alter them as well.
        /// </summary>
        /// <param name="lName"></param>
        /// <returns></returns>
        public static string FixupLeafName(this string lName)
        {
            return lName.Replace(":", "_");
        }

        /// <summary>
        /// Fix up the string used for classes
        /// </summary>
        /// <param name="cname"></param>
        /// <returns></returns>
        public static string FixupClassName(this string cname)
        {
            var n = cname.Replace("#", "_");
            return n;
        }

    }
}
