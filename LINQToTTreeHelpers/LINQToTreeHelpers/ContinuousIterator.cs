using System.Collections.Generic;

namespace LINQToTreeHelpers
{
    /// <summary>
    /// A coninuous iterator - that keeps iterating over and over. :-)
    /// </summary>
    public static class ContinuousIteratorHelper
    {
        public static IEnumerable<T> ContinuousIterator<T>(this IEnumerable<T> source)
        {
            ///
            /// Iterates through the sequence and keeps repeating. Runs forever.
            /// 

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
