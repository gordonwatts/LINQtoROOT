using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// For adding together results from a run
    /// </summary>
    public interface IAddResults
    {
        /// <summary>
        /// Return true if we can add items of type t
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        bool CanHandle(Type t);

        /// <summary>
        /// return accumulator = accumulator + o2. o2 may not be altered in any way.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="accumulator">The accumulator. Add </param>
        /// <param name="o2"></param>
        /// <returns></returns>
        void Update<T>(T accumulator, T o2);

        /// <summary>
        /// Make a clone, if appropriate, of the first value. FOr example, if we are looking at integers, then this
        /// does not matter. But if histograms, one could clone this, and then not have ot use clone in the Add, as the
        /// object will always be passed into the accumulator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        T Clone<T>(T o);
    }
}
