using LinqToTTreeInterfacesLib;
using System;
using System.Linq;

namespace LINQToTreeHelpers.FutureUtils
{
    /// <summary>
    /// Some operators to make future operations "work"... Can't quite do this as well as C++!
    /// </summary>
    public static class Operators
    {
        /// <summary>
        /// Select pattern - so that one can access the monad in situ.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="M"></typeparam>
        /// <param name="self"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static IFutureValue<T> Select<T, M>(this IFutureValue<M> self, Func<M, T> map)
        {
            return new DoFutureOperator<T>(
                () => map(self.Value),
                () => self.HasValue
                );
        }

        /// <summary>
        /// Cache a call to Value to make sure it is only called once.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class FutureValueCache<T>
        {
            private T _cache;
            private bool _called = false;
            private IFutureValue<T> _fvalue;

            public FutureValueCache(IFutureValue<T> mfv)
            {
                _fvalue = mfv;
            }

            public T GetValue()
            {
                if (!_called)
                {
                    _cache = _fvalue.Value;
                    _called = true;
                }
                return _cache;
            }
        }

        /// <summary>
        /// Implement the select many pattern to allow monadic calculations
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="self"></param>
        /// <param name="select"></param>
        /// <param name="project"></param>
        /// <returns>A future value that applies the projection operator</returns>
        /// <remarks>
        /// The projection is straight forward SelectMany (not that it is straight forward in any way).
        /// Doing the HasValue guy, however, is not, as we want to be a little careful not to call Value
        /// on anything that doesn't have a value yet. So we are relying on the fact that self.Value won't
        /// get calculated until after the self.HasValue has been "ok"d.
        /// </remarks>
        public static IFutureValue<V> SelectMany<T, U, V>(
            this IFutureValue<T> self,
            Func<T, IFutureValue<U>> select,
            Func<T, U, V> project)
        {
            var fvcache = new FutureValueCache<T>(self);
            return new DoFutureOperator<V>(
                () =>
                {
                    var resT = fvcache.GetValue();
                    var resUOpt = select(resT);
                    var resU = resUOpt.Value;
                    var resV = project(resT, resU);
                    return resV;
                },
                () => self.HasValue && select(fvcache.GetValue()).HasValue
                );
        }

        /// <summary>
        /// Divide to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static IFutureValue<double> DivideBy(this IFutureValue<int> numerator, IFutureValue<int> denominator)
        {
            return new DoFutureOperator<double>(
                () => ((double)numerator.Value) / ((double)denominator.Value),
                () => numerator.HasValue && denominator.HasValue
                    );
        }

        /// <summary>
        /// Divide to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static IFutureValue<double> DivideBy(this IFutureValue<double> numerator, IFutureValue<double> denominator)
        {
            return new DoFutureOperator<double>(
                () => ((double)numerator.Value) / ((double)denominator.Value),
                () => numerator.HasValue && denominator.HasValue
                    );
        }

        /// <summary>
        /// Divide to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static IFutureValue<float> DivideBy(this IFutureValue<float> numerator, IFutureValue<float> denominator)
        {
            return new DoFutureOperator<float>(
                () => ((float)numerator.Value) / ((float)denominator.Value),
                () => numerator.HasValue && denominator.HasValue
                    );
        }

        /// <summary>
        /// Divide to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static IFutureValue<double> DivideBy(this IFutureValue<int> numerator, int denominator)
        {
            return new DoFutureOperator<double>(
                () => ((double)numerator.Value) / ((double)denominator),
                () => numerator.HasValue
                    );
        }

        /// <summary>
        /// Divide to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static IFutureValue<float> DivideBy(this IFutureValue<float> numerator, float denominator)
        {
            return new DoFutureOperator<float>(
                () => ((float)numerator.Value) / ((float)denominator),
                () => numerator.HasValue
                    );
        }

        /// <summary>
        /// Divide to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static IFutureValue<double> DivideBy(this IFutureValue<double> numerator, double denominator)
        {
            return new DoFutureOperator<double>(
                () => ((double)numerator.Value) / ((double)denominator),
                () => numerator.HasValue
                    );
        }

        /// <summary>
        /// Add two numbers together.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static IFutureValue<int> AddTo(this IFutureValue<int> v1, IFutureValue<int> v2)
        {
            return new DoFutureOperator<int>(
                () => v1.Value + v2.Value,
                () => v1.HasValue && v2.HasValue
                    );
        }

        /// <summary>
        /// Add to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static IFutureValue<double> AddTo(this IFutureValue<double> v1, IFutureValue<double> v2)
        {
            return new DoFutureOperator<double>(
                () => v1.Value + v2.Value,
                () => v1.HasValue && v2.HasValue
                    );
        }

        /// <summary>
        /// Add to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static IFutureValue<float> AddTo(this IFutureValue<float> v1, IFutureValue<float> v2)
        {
            return new DoFutureOperator<float>(
                () => v1.Value + v2.Value,
                () => v1.HasValue && v2.HasValue
                    );
        }

        /// <summary>
        /// Add to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static IFutureValue<float> AddTo(this IFutureValue<float> v1, float v2)
        {
            return new DoFutureOperator<float>(
                () => v1.Value + v2,
                () => v1.HasValue
                    );
        }

        /// <summary>
        /// Add to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static IFutureValue<int> AddTo(this IFutureValue<int> v1, int v2)
        {
            return new DoFutureOperator<int>(
                () => v1.Value + v2,
                () => v1.HasValue
                    );
        }

        /// <summary>
        /// Add to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static IFutureValue<double> AddTo(this IFutureValue<double> v1, double v2)
        {
            return new DoFutureOperator<double>(
                () => v1.Value + v2,
                () => v1.HasValue
                    );
        }

        /// <summary>
        /// Multiply to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static IFutureValue<int> MultiplyBy(this IFutureValue<int> numerator, IFutureValue<int> denominator)
        {
            return new DoFutureOperator<int>(
                () => (numerator.Value) * (denominator.Value),
                () => numerator.HasValue && denominator.HasValue
                    );
        }

        /// <summary>
        /// Multiply to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static IFutureValue<double> MultiplyBy(this IFutureValue<double> numerator, IFutureValue<double> denominator)
        {
            return new DoFutureOperator<double>(
                () => ((double)numerator.Value) * ((double)denominator.Value),
                () => numerator.HasValue && denominator.HasValue
                    );
        }

        /// <summary>
        /// Multiply to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static IFutureValue<float> MultiplyBy(this IFutureValue<float> numerator, IFutureValue<float> denominator)
        {
            return new DoFutureOperator<float>(
                () => ((float)numerator.Value) * ((float)denominator.Value),
                () => numerator.HasValue && denominator.HasValue
                    );
        }

        /// <summary>
        /// Multiply to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static IFutureValue<int> MultiplyBy(this IFutureValue<int> numerator, int denominator)
        {
            return new DoFutureOperator<int>(
                () => (numerator.Value) * (denominator),
                () => numerator.HasValue
                    );
        }

        /// <summary>
        /// Multiply to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static IFutureValue<float> MultiplyBy(this IFutureValue<float> numerator, float denominator)
        {
            return new DoFutureOperator<float>(
                () => ((float)numerator.Value) * ((float)denominator),
                () => numerator.HasValue
                    );
        }

        /// <summary>
        /// Multiply to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static IFutureValue<double> MultiplyBy(this IFutureValue<double> numerator, double denominator)
        {
            return new DoFutureOperator<double>(
                () => ((double)numerator.Value) * ((double)denominator),
                () => numerator.HasValue
                    );
        }

        /// <summary>
        /// Subtract two numbers together.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static IFutureValue<int> Subtract(this IFutureValue<int> v1, IFutureValue<int> v2)
        {
            return new DoFutureOperator<int>(
                () => v1.Value - v2.Value,
                () => v1.HasValue && v2.HasValue
                    );
        }

        /// <summary>
        /// Subtract to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static IFutureValue<double> Subtract(this IFutureValue<double> v1, IFutureValue<double> v2)
        {
            return new DoFutureOperator<double>(
                () => v1.Value - v2.Value,
                () => v1.HasValue && v2.HasValue
                    );
        }

        /// <summary>
        /// Subtract to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static IFutureValue<float> Subtract(this IFutureValue<float> v1, IFutureValue<float> v2)
        {
            return new DoFutureOperator<float>(
                () => v1.Value - v2.Value,
                () => v1.HasValue && v2.HasValue
                    );
        }

        /// <summary>
        /// Subtract to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static IFutureValue<float> Subtract(this IFutureValue<float> v1, float v2)
        {
            return new DoFutureOperator<float>(
                () => v1.Value - v2,
                () => v1.HasValue
                    );
        }

        /// <summary>
        /// Subtract to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static IFutureValue<int> Subtract(this IFutureValue<int> v1, int v2)
        {
            return new DoFutureOperator<int>(
                () => v1.Value - v2,
                () => v1.HasValue
                    );
        }

        /// <summary>
        /// Subtract to future values that are integers. Return a double (as we should be!!).
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static IFutureValue<double> Subtract(this IFutureValue<double> v1, double v2)
        {
            return new DoFutureOperator<double>(
                () => v1.Value - v2,
                () => v1.HasValue
                    );
        }

        /// <summary>
        /// When requested, we will extract the value from the source.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="extractor"></param>
        /// <returns></returns>
        public static IFutureValue<TResult> ExtractValue<TSource, TResult>(this IFutureValue<TSource> source, Func<TSource, TResult> extractor)
        {
            return new DoFutureOperator<TResult>(
                () => extractor(source.Value),
                () => source.HasValue
                );
        }

        /// <summary>
        /// Returns a future value that uses two different future values to calculate a third. A Func does the
        /// actual manipulation.
        /// </summary>
        /// <typeparam name="TSource1"></typeparam>
        /// <typeparam name="TSource2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <param name="extractor"></param>
        /// <returns></returns>
        public static IFutureValue<TResult> ExtractValue<TSource1, TSource2, TResult>(this IFutureValue<TSource1> source1,
            IFutureValue<TSource2> source2,
            Func<TSource1, TSource2, TResult> extractor)
        {
            return new DoFutureOperator<TResult>(
                () => extractor(source1.Value, source2.Value),
                () => source2.HasValue && source1.HasValue
                );
        }

        /// <summary>
        /// Returns a future value that uses a list of future values to calculate its result. A Func does the
        /// actual work.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="extractor"></param>
        /// <returns></returns>
        public static IFutureValue<TResult> ExtractValue<TSource, TResult>(this IFutureValue<TSource>[] source, Func<TSource[], TResult> extractor)
        {
            return new DoFutureOperator<TResult>(
                () => extractor(source.Select(s => s.Value).ToArray()),
                () => source.All(s => s.HasValue)
                );
        }

        /// <summary>
        /// Helper class for casting. When created, it will hold a FV, and use the "To"
        /// method to cast it properly.
        /// </summary>
        /// <typeparam name="TSource">The type of the original object</typeparam>
        /// <remarks>Need this because otherwise you will need to specify explicitly both types at </remarks>
        public class CastCapture<TSource>
        {
            /// <summary>
            /// The value we are holding onto.
            /// </summary>
            private readonly IFutureValue<TSource> _value;

            /// <summary>
            /// Hold onto a value.
            /// </summary>
            /// <param name="v"></param>
            internal CastCapture(IFutureValue<TSource> v)
            {
                _value = v;
            }

            /// <summary>
            /// Return a FV that contains the cast held object.
            /// </summary>
            /// <typeparam name="TResult">The type for the result of the cast</typeparam>
            /// <returns>A FV that holds onto the cast version of the original value</returns>
            public IFutureValue<TResult> To<TResult>()
                where TResult : class
            {
                return new DoFutureOperator<TResult>(
                    () => _value.Value as TResult,
                    () => _value.HasValue
                    );
            }
        }

        /// <summary>
        /// Cast an IFutureValue from one class to another. Apply the To operator to the result of this.
        /// </summary>
        /// <typeparam name="TSource">The source type for the FV that will be cast</typeparam>
        /// <param name="source">The original future value that is to have its value re-cast</param>
        /// <returns>A CastCapture object that you can use the To object to recast to the type you want</returns>
        public static CastCapture<TSource> Cast<TSource>(this IFutureValue<TSource> source)
        {
            return new CastCapture<TSource>(source);
        }

        /// <summary>
        /// Apply an operation to a future value in the future. For fluent programming's sake,
        /// return the same object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="application"></param>
        /// <returns></returns>
        public static IFutureValue<T> Apply<T>(this IFutureValue<T> obj, Action<T> application)
        {
            return new DoFutureOperator<T>(
                () => { var v = obj.Value; application(v); return v; },
                () => obj.HasValue
                );
        }

        /// <summary>
        /// The future value we return for all these operations.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class DoFutureOperator<T> : IFutureValue<T>
        {
            /// <summary>
            /// Future operator - will run the getResult guy when it is time to run.
            /// </summary>
            public DoFutureOperator(Func<T> genValue, Func<bool> hasValue)
            {
                _getResult = genValue;
                _hasValue = hasValue;
            }

            /// <summary>
            /// Holds onto the function that will generate our value.
            /// </summary>
            private Func<T> _getResult;

            private Func<bool> _hasValue;

            /// <summary>
            /// Returns true if we have a value.
            /// </summary>
            public bool HasValue { get { return _hasValue(); } }

            /// <summary>
            /// Return the value
            /// </summary>
            public T Value
            {
                get { return _getResult(); }
            }
        }
    }
}
