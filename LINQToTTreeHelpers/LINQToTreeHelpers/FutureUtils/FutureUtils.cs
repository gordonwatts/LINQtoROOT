using LinqToTTreeInterfacesLib;

namespace LINQToTreeHelpers.FutureUtils
{
    public static class FutureTDirectoryUtils
    {
        /// <summary>
        /// Helper function to add a future to a directory for later writing.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dir"></param>
        public static IFutureValue<T> SaveToROOTDirectory<T>(this IFutureValue<T> obj, FutureTDirectory dir, string tag = null)
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
        /// Holds onto a future value which is not in teh future! ;-)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class ImmediateFutureValue<T> : IFutureValue<T>
        {
            public bool HasValue { get { return true; } }

            public T Value { get; set; }
        }

        /// <summary>
        /// Save a regular old TObject type values to one of our directories.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dir"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static IFutureValue<T> SaveToROOTDirectory<T>(this T obj, FutureTDirectory dir, string tag = null)
            where T : ROOTNET.Interface.NTObject
        {
            IFutureValue<T> fv = new ImmediateFutureValue<T> { Value = obj };
            var r = fv.SaveToROOTDirectory(dir, tag);
            return r;
        }

        /// <summary>
        /// Do an immediate write to a root directory
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dir"></param>
        public static void WriteToROOTDirectory<T>(this T obj, ROOTNET.Interface.NTDirectory dir)
            where T : ROOTNET.Interface.NTObject
        {
            dir.WriteTObject(obj);
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
