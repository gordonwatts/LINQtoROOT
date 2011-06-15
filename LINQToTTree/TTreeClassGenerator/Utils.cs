
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

    }
}
