
using LinqToTTreeInterfacesLib;
namespace LINQToTTreeLib
{
    /// <summary>
    /// A future that holds onto a query and a result.
    /// The result is not evaluated right away. Access the result
    /// (with Value) will cause it to be evaulated if it hasn't already
    /// been done. When it is evaluated, any other results on the same
    /// query source will also be evaluated.
    /// </summary>
    internal class FutureValue<T> : IFutureValue<T>
    {
        /// <summary>
        /// Return the value. Execute the query if needed
        /// </summary>
        public T Value
        {
            get { return default(T); }
        }

        /// <summary>
        /// A way of checking to see if we already know about the query.
        /// </summary>
        public bool HasValue
        {
            get { return false; }
        }
    }
}
