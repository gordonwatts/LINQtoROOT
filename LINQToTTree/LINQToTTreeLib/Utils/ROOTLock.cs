using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
        /// Grab the lock
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static AwaitableDisposable<IDisposable> LockAsync ([CallerMemberName] string memberName = "")
        {
            try
            {
                Debug.WriteLine($"Going to get the ROOT lock from {memberName}.");
                return _lock.LockAsync();
            } finally
            {
                Debug.WriteLine($"  -> Got to get the ROOT lock from {memberName}.");
            }
        }

        /// <summary>
        /// Grab the lock right away
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static IDisposable Lock([CallerMemberName] string memberName = "")
        {
            try
            {
                Debug.WriteLine($"Going to get the ROOT lock from {memberName}.");
                return _lock.Lock();
            }
            finally
            {
                Debug.WriteLine($"  -> Got to get the ROOT lock from {memberName}.");
            }
        }

        /// <summary>
        /// Return a async lock you can await on. Make sure to do this before doing anything
        /// with a root file.
        /// </summary>
        //public static AsyncLock Lock => _lock;
    }
}
