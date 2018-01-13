using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Utils
{
    /// <summary>
    /// A helper class to help lock down ROOT from multiple access
    /// </summary>
    public static class ROOTLock
    {
        private static AsyncLock _lock = new AsyncLock();

        /// <summary>
        /// Return a async lock you can await on. Make sure to do this before doing anything
        /// with a root file.
        /// </summary>
        public static AsyncLock Lock => _lock;
    }
}
