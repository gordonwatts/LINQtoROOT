using System.Collections.Generic;
using System.Threading.Tasks;
using LinqToTTreeInterfacesLib;

namespace LINQToTreeHelpers.FutureUtils
{
    /// <summary>
    /// Helper utilties for dealing with IFutureValue and FutureTDirectory objects. These
    /// are extension methods.
    /// </summary>
    public static class FutureTDirectoryUtils
    {
        /// <summary>
        /// Helper function to add a future to a directory for later writing.
        /// </summary>
        /// <typeparam name="T">Type of the future value</typeparam>
        /// <param name="dir">Directory where this future value should be saved</param>
        /// <param name="obj">Object to save</param>
        /// <param name="tag">List of tags which can be used to retreive the object later</param>
        public static IFutureValue<T> Save<T>(this IFutureValue<T> obj, FutureTDirectory dir, string tag = null)
            where T : ROOTNET.Interface.NTObject
        {
            if (tag == null)
            {
                dir.AddFuture(obj);
            }
            else
            {
                dir.AddFuture(obj, new string[] { tag });
            }
            return obj;
        }

        /// <summary>
        /// Helper function to write a bunch of values to a directory. NOTE that it returns an enumerable - so you'll
        /// need to loop through it!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="dir"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static IEnumerable<IFutureValue<T>> Save<T>(this IEnumerable<IFutureValue<T>> source, FutureTDirectory dir, string tag = null)
            where T : ROOTNET.Interface.NTObject
        {
            foreach (var s in source)
            {
                yield return s.Save(dir, tag);
            }
        }

        /// <summary>
        /// Holds onto a future value which is not in teh future! ;-)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class ImmediateFutureValue<T> : IFutureValue<T>
        {
            public bool HasValue { get { return true; } }

            public T Value { get; set; }

            public Task GetAvailibleTask()
            {
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// Save a regular old TObject type values to one of our directories.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dir"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static IFutureValue<T> Save<T>(this T obj, FutureTDirectory dir, string tag = null)
            where T : ROOTNET.Interface.NTObject
        {
            IFutureValue<T> fv = new ImmediateFutureValue<T> { Value = obj };
            var r = fv.Save(dir, tag);
            return r;
        }

        /// <summary>
        /// Add an item to a future dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="what"></param>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        public static IFutureValue<T> Add<T>(this IFutureValue<T> what, FutureDictionary dict, string key)
        {
            dict.Add(key, what);
            return what;
        }
    }
}
