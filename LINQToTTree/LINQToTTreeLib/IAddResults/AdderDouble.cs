using LinqToTTreeInterfacesLib;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace LINQToTTreeLib.IAddResults
{
    [Export(typeof(IAddResult))]
    class AdderDouble : IAddResult
    {
        // We deal only with double
        public bool CanHandle(Type t)
        {
            return t == typeof(double);
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
            var a = accumulator as double?;
            var o = o2 as double?;

            object r = a.Value + o.Value;

            return (T)r;
        }
    }
}
