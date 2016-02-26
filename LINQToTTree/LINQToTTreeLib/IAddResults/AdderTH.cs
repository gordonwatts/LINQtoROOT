using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.IAddResults
{
    /// <summary>
    /// Add generic histograms
    /// </summary>
    [Export(typeof(IAddResult))]
    class AdderTH : IAddResult
    {
        // We deal only with integers
        public bool CanHandle(Type t)
        {
            return t.GetInterfaces().Contains(typeof(ROOTNET.Interface.NTH1));
        }

        /// <summary>
        /// A clone of an int is an int.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public T Clone<T>(T o)
        {
            var h = o as ROOTNET.Interface.NTH1;
            return (T) h.Clone();
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
            var a = accumulator as ROOTNET.Interface.NTH1;
            var o = o2 as ROOTNET.Interface.NTH1;

            a.Add(o);

            return accumulator;
        }
    }
}
