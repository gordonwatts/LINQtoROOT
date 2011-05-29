
using System;
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
        /// Keeps track of the tree executor
        /// </summary>
        public TTreeQueryExecutor TreeExecutor { get; private set; }

        /// <summary>
        /// Create a FV that has already gotten the result. No need for lookup here!
        /// </summary>
        /// <param name="result"></param>
        internal FutureValue(T result)
        {
            Value = result;
            HasValue = true;
        }

        /// <summary>
        /// Create an unfilled future, connected with a tree executor that
        /// can do the job of filling it in.
        /// </summary>
        public FutureValue(TTreeQueryExecutor tTreeQueryExecutor)
        {
            if (tTreeQueryExecutor == null)
                throw new ArgumentException("tree executor must not be null!");

            Value = default(T);
            HasValue = false;
            TreeExecutor = tTreeQueryExecutor;
        }

        /// <summary>
        /// Internal storage for our value.
        /// </summary>
        private T _value;

        /// <summary>
        /// Return the value. Execute the query if needed
        /// </summary>
        public T Value
        {
            get
            {
                if (!HasValue)
                {
                    TreeExecutor.ExecuteQueuedQueries();
                    if (!HasValue)
                        throw new InvalidOperationException("Queued query was not executed when all queued queries were run! Can't do this query now!");
                }
                return _value;
            }
            private set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Set the value - someone else has run the query! :-)
        /// </summary>
        /// <param name="val"></param>
        internal void SetValue(T val)
        {
            HasValue = true;
            Value = val;
        }

        /// <summary>
        /// A way of checking to see if we already know about the query.
        /// </summary>
        public bool HasValue { get; private set; }
    }
}
