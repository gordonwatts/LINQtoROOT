using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTreeParser
{
    /// <summary>
    /// Utilities for helping deal with ROOT...
    /// </summary>
    static class Utils
    {
        /// <summary>
        /// Attempt to find all the objects that are of type T or inherrit from T in the input file,
        /// and return them. Note that the file or owner of the directory needs to stay open as long as the sequence
        /// is being parsed!! This is an iterator, and processes on an as-need basis!!!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootDir"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindAllOfType<T>(ROOTNET.Interface.NTDirectory rootDir)
            where T: ROOTNET.Interface.NTObject
        {
            var objectKeys = rootDir.GetListOfKeys();
            string basename = typeof(T).Name.Substring(1);

            foreach (var item in objectKeys.AsEnumerable().Cast<ROOTNET.Interface.NTKey>())
            {
                var c = ROOTNET.NTClass.GetClass(item.GetClassName());
                if (c.InheritsFrom(basename))
                {
                    yield return (T)item.ReadObj();
                }
            }
        }

        /// <summary>
        /// Turn the list into something we can enumerate over!
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<ROOTNET.Interface.NTObject> AsEnumerable(this ROOTNET.Interface.NTCollection list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            var iter = new ROOTNET.NTIter(list);
            bool done = false;
            while (!done)
            {
                var result = iter.Next() as ROOTNET.Interface.NTObject;
                if (result != null)
                {
                    yield return result;
                }
                else
                {
                    done = true;
                }
            }
        }
    }
}
