using System;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Given a translation dictionary, do a replacement on the incoming stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="replaceDict"></param>
        /// <returns></returns>
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, IEnumerable<Tuple<T,T>> replaceDict)
        {
            var dict = replaceDict.ToDictionary(i => i.Item1, i => i.Item2);
            foreach (var s in source)
            {
                if (dict.ContainsKey(s))
                {
                    yield return dict[s];
                } else
                {
                    yield return s;
                }
            }
        }
    }
}
