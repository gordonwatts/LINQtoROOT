﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Utils
{
    static class HashSetUtils
    {
        /// <summary>
        /// Add a range to the hash set. Will return the modified hash set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">Set to be added to.</param>
        /// <param name="newItems"></param>
        /// <returns></returns>
        public static HashSet<T> AddRange<T> (this HashSet<T> source, IEnumerable<T> newItems)
        {
            foreach (var item in newItems)
            {
                source.Add(item);
            }

            return source;
        }

        /// <summary>
        /// So we can do it functionally.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static HashSet<T> ToHashSet<T> (this IEnumerable<T> source)
        {
            var r = new HashSet<T>();

            foreach (var item in source)
            {
                r.Add(item);
            }

            return r;
        }
    }
}
