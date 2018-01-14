
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
            Value = default;
            HasValue = false;
            TreeExecutor = tTreeQueryExecutor ?? throw new ArgumentException("tree executor must not be null!");
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
                    // We are running just one value - so wait.
                    FireOffQueryTask().Wait();
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
        /// Return the task that will run when everything is done.
        /// </summary>
        /// <returns></returns>
        public Task GetAvailibleTask()
        {
            return FireOffQueryTask();
        }

        private Task _queryExecutionTask = null;

        /// <summary>
        /// Fire off a query execution task
        /// </summary>
        private Task FireOffQueryTask()
        {
            if (HasValue)
            {
                return Task.FromResult(Value);
            }
            if (_queryExecutionTask == null)
            {
                _queryExecutionTask = (TreeExecutor ?? throw new ArgumentNullException($"TreeExecutor for type {typeof(T).Name} for FutureValue has a null value!"))
                    .ExecuteQueuedQueries();
            }
            return _queryExecutionTask;
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

    public static class IFutureValueUtils
    {
        public class IFutureValueAwaiter<T> : INotifyCompletion
        {
            private readonly IFutureValue<T> _ifv;

            public IFutureValueAwaiter (IFutureValue<T> fv)
            {
                _ifv = fv;
            }

            public bool IsCompleted => _ifv.HasValue;

            public void OnCompleted(Action continuation)
            {
                _ifv.GetAvailibleTask().ContinueWith(t => continuation());
            }

            public T GetResult()
            {
                return _ifv.Value;
            }
        }

        /// <summary>
        /// Return a task awaiter.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static IFutureValueAwaiter<T> GetAwaiter<T> (this IFutureValue<T> v)
        {
            return new IFutureValueAwaiter<T>(v);
        }
    }
}
