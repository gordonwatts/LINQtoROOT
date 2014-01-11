using System.Collections.Generic;

namespace LINQToTTreeLib.Utils
{
    /// <summary>
    /// Help with enumerables to make life easier.
    /// </summary>
    static class EnumerableUtils
    {
        /// <summary>
        /// Wrap a single object into a sequence of one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<T> Return<T>(this T obj)
        {
            return new T[] { obj };
        }
    }
}
