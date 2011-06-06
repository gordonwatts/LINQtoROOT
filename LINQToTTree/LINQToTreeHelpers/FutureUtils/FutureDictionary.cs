
using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
namespace LINQToTreeHelpers.FutureUtils
{
    /// <summary>
    /// Helps save key-value pairs, where the value is one of our future markers.
    /// Renders to a standard dictionary.
    /// </summary>
    public class FutureDictionary
    {
        abstract class FutureHolderBase
        {
            public string Key { get; set; }

            /// <summary>
            /// Convert to type T...
            /// </summary>
            /// <typeparam name="T1"></typeparam>
            /// <returns></returns>
            public abstract T1 ConvertTo<T1>();
        }

        class FutureHolder<T> : FutureHolderBase
        {
            public IFutureValue<T> Value;

            public override T1 ConvertTo<T1>()
            {
                T v = Value.Value;
                return (T1)Convert.ChangeType(v, typeof(T1));
            }
        }

        List<FutureHolderBase> _items = new List<FutureHolderBase>();

        /// <summary>
        /// Add an item to the dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add<T>(string key, IFutureValue<T> value)
        {
            _items.Add(new FutureHolder<T>() { Key = key, Value = value });
        }

        /// <summary>
        /// Given the list of items, return them as a dictionary of a certian
        /// type. We will attempt to render all the types.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Dictionary<string, T> ToDictionary<T>()
        {
            var result = new Dictionary<string, T>();

            foreach (var item in _items)
            {
                result[item.Key] = item.ConvertTo<T>();
            }

            return result;
        }
    }
}
