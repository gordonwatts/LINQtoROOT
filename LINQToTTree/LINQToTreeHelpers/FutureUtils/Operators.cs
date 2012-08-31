using System;
using System.Linq;
using LinqToTTreeInterfacesLib;

namespace LINQToTreeHelpers.FutureUtils
{
    /// <summary>
    /// Some operators to make future operations "work"... Can't quite do this as well as C++!
    /// </summary>
    public static class Operators
    {
        /// <summary>
        /// Divide to fture values that are integers. Return a dobule (as we should be!!).
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
        /// Divide to fture values that are integers. Return a dobule (as we should be!!).
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
        /// Divide to fture values that are integers. Return a dobule (as we should be!!).
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
        /// Divide to fture values that are integers. Return a dobule (as we should be!!).
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
        /// Divide to fture values that are integers. Return a dobule (as we should be!!).
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
        /// Divide to fture values that are integers. Return a dobule (as we should be!!).
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
        /// Divide to fture values that are integers. Return a dobule (as we should be!!).
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
        /// Divide to fture values that are integers. Return a dobule (as we should be!!).
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
        /// Divide to fture values that are integers. Return a dobule (as we should be!!).
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
        /// Divide to fture values that are integers. Return a dobule (as we should be!!).
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
        /// Divide to fture values that are integers. Return a dobule (as we should be!!).
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
        /// Multipley a future value by a value.
        /// </summary>
        /// <param name="op1"></param>
        /// <param name="op2"></param>
        /// <returns></returns>
        public static IFutureValue<double> MultiplyBy(this IFutureValue<double> op1, double op2)
        {
            return new DoFutureOperator<double>(
                () => op1.Value * op2,
                () => op1.HasValue
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
        /// Apply an operation to a future value in the future. For fluent programing's sake,
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
        private class DoFutureOperator<T> : IFutureValue<T>
        {
            /// <summary>
            /// Future operator - will run the getResult guy when it is time to run.
            /// </summary>
            internal DoFutureOperator(Func<T> genValue, Func<bool> hasValue)
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
