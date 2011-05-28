
namespace LinqToTTreeInterfacesLib
{
    public interface IFutureValue<T>
    {
        /// <summary>
        /// Returns the value of the query. May be very fast if the result has already
        /// been evaluated, or may be very slow b/c it has to be evaluated.
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Returns true if the value has already been rendered
        /// </summary>
        bool HasValue { get; }
    }
}
