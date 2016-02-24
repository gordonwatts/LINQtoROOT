using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Utils
{
    static class ExceptionUtils
    {
        /// <summary>
        /// Throw if null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="obj"></param>
        /// <param name="generateException"></param>
        /// <returns></returns>
        public static T ThrowIfNull<T,E> (this T obj, Func<E> generateException)
            where E : Exception
        {
            if (obj != null)
                return obj;

            throw generateException();
        }
    }
}
