using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    /// Help out with setup/tear down of MEF in this environment
    /// </summary>
    class MEFUtilities
    {
        /// <summary>
        /// Holds the crap for MEF.
        /// </summary>
        private static CompositionContainer _container = null;

        /// <summary>
        /// Gets teh container use for composition
        /// </summary>
        public static CompositionContainer MEFContainer { get { return _container; } }

        /// <summary>
        /// Keep track of everyone we are going to compose.
        /// </summary>
        private static CompositionBatch _batch = null;

        private static AggregateCatalog _catalog;

        /// <summary>
        /// Call this at the start of the class in order to get MEF setup.
        /// </summary>
        /// <param name="context"></param>
        public static void MyClassInit()
        {
            _catalog = new AggregateCatalog();

            _container = new CompositionContainer(_catalog);
            _batch = new CompositionBatch();
        }

        /// <summary>
        /// We are done - make sure nothing gets accidentally used
        /// </summary>
        public static void MyClassDone()
        {
            _batch = null;
            _catalog = null;
            _container.Dispose();
            _container = null;
        }

        public static void AddAssemblyForType(Type myType)
        {
            _catalog.Catalogs.Add(new AssemblyCatalog(myType.Assembly));
        }

        /// <summary>
        /// Do the composition
        /// </summary>
        /// <param name="re"></param>
        public static void Compose<T>(T re)
        {
            _batch.AddPart(re);
            _container.Compose(_batch);
            _batch = new CompositionBatch();
        }

        public static void AddPart<T>(T o)
        {
            _batch.AddPart(o);
        }
    }
}
