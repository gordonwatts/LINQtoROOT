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

        /// <summary>
        /// Run through a iterator, and if the iterator is modified under us (and throws), just
        /// re-run from the start.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> RepeatIfModified<T> (this IEnumerable<T> source)
        {
            bool done = false;
            while (!done)
            {
                var next = source.GetEnumerator();
                while (!done)
                {
                    T obj = default(T);
                    try
                    {
                        done = !next.MoveNext();
                        if (!done)
                        {
                            obj = next.Current;
                        }
                    } catch (NotImplementedException)
                    {
                        break;
                    }
                    if (!done)
                    {
                        yield return obj;
                    }
                }
            }
        }

        /// <summary>
        /// Run through a iterator, and if the iterator is modified under us (and throws), just
        /// re-run from the start. When we do, don't re-run any objects we've already seen.
        /// Whatever object we were operating on that causes the change we will retry.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> RetryIfModified<T>(this IEnumerable<T> source)
            where T : class
        {
            bool done = false;
            var seen = new HashSet<T>();
            while (!done)
            {
                var next = source.GetEnumerator();
                T objOld = default(T);
                while (!done)
                {
                    T obj = default(T);
                    try
                    {
                        done = !next.MoveNext();
                        if (!done)
                        {
                            obj = next.Current;
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        break;
                    }
                    if (!done && !seen.Contains(obj))
                    {
                        if (objOld != null)
                        {
                            seen.Add(objOld);
                        }
                        objOld = obj;
                        yield return obj;
                    }
                }
            }
        }

        /// <summary>
        /// Run through a iterator, and if the iterator is modified under us (and throws), just
        /// re-run from the start.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> RetryFromStartIfModified<T>(this IEnumerable<T> source)
            where T : class
        {
            bool done = false;
            while (!done)
            {
                var next = source.GetEnumerator();
                while (!done)
                {
                    T obj = default(T);
                    try
                    {
                        done = !next.MoveNext();
                        if (!done)
                        {
                            obj = next.Current;
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        break;
                    }
                    if (!done)
                    {
                        yield return obj;
                    }
                }
            }
        }
    }
}
