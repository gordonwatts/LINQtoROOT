using System.Collections.Generic;

namespace LINQToTreeHelpers
{
    /// <summary>
    /// A coninuous iterator - that keeps iterating over and over. :-)
    /// </summary>
    static class ContinuousIteratorHelper
    {
        /// <summary>
        /// A looping iterator. Constantly re-runs the iterator that is passed in. Runs forever.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<T> ContinuousIterator<T>(this IEnumerable<T> source)
        {
            //
            // Iterates through the sequence and keeps repeating. Runs forever.
            // 

            while (true)
            {
                foreach (var item in source)
                {
                    yield return item;
                }
            }
        }
    }
}
