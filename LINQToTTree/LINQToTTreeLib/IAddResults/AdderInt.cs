using LinqToTTreeInterfacesLib;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace LINQToTTreeLib.IAddResults
{
    [Export(typeof(IAddResult))]
    class AdderInt : IAddResult
    {
        // We deal only with integers
        public bool CanHandle(Type t)
        {
            return t == typeof(int);
        }

        /// <summary>
        /// A clone of an int is an int.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public Task<T> Clone<T>(T o)
        {
            return Task.FromResult(o);
        }

        /// <summary>
        /// Add the integers together.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="accumulator"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public T Update<T>(T accumulator, T o2)
        {
            var a = accumulator as int?;
            var o = o2 as int?;

            object r = a.Value + o.Value;

            return (T)r;
        }
    }
}
