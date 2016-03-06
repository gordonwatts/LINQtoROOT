using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.IAddResults
{
    [Export(typeof(IAddResult))]
    class AdderFileInfoArray : IAddResult
    {
        /// <summary>
        /// Deal only with arrays of file info.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool CanHandle(Type t)
        {
            return t == typeof(FileInfo[]);
        }

        /// <summary>
        /// In place clone - we don't care because we aren't going to modify
        /// anything here.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public T Clone<T>(T o)
        {
            return o;
        }

        /// <summary>
        /// Append the array lists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="accumulator"></param>
        /// <param name="o2"></param>
        /// <returns></returns>
        public T Update<T>(T accumulator, T o2)
        {
            var mo1 = accumulator as FileInfo[];
            var mo2 = o2 as FileInfo[];
            var r = mo1.Concat(mo2).ToArray();

            return (T)((object)r);
        }
    }
}
