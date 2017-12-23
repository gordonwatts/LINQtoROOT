
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTTreeInterfacesLib;

namespace LINQToTreeHelpers.FutureUtils
{
    /// <summary>
    /// Holds a series of Future TObject like guys and when the time
    /// comes (you call write) will write them into the root directory
    /// </summary>
    public class FutureTDirectory
    {
        /// <summary>
        /// Get the underlying ROOT TDirectory.
        /// </summary>
        public ROOTNET.Interface.NTDirectory Directory { get; private set; }

        /// <summary>
        /// Create a new future directory container that will write its objects to the given root directory
        /// </summary>
        /// <remarks>
        /// In ROOT, when you create a new directory the gDirectory variable isn't changed; so the global directory
        /// will be unchanged by this operation.
        /// </remarks>
        /// <param name="dir"></param>
        public FutureTDirectory(ROOTNET.Interface.NTDirectory dir)
        {
            if (dir == null)
                throw new ArgumentNullException("Can't create a new FutureTDirectory pointing to a null directory");

            Directory = dir;
        }

        /// <summary>
        /// Hold onto a value
        /// </summary>
        abstract class FVHolderBase
        {
            public void Save(ROOTNET.Interface.NTDirectory dir)
            {
                AsTObject.InternalWriteObject(dir);
            }

            public abstract ROOTNET.Interface.NTObject AsTObject { get; }

            /// <summary>
            /// Return the task that waits for this value to be rendered.
            /// </summary>
            public abstract Task WaiterTask { get; }
        }

        class FVHolder<T> : FVHolderBase
            where T : ROOTNET.Interface.NTObject
        {
            public IFutureValue<T> Value { get; set; }

            public override ROOTNET.Interface.NTObject AsTObject
            {
                get { return Value.Value; }
            }

            public override Task WaiterTask
            {
                get { return Value.GetAvailibleTask(); }
            }
        }

        List<FVHolderBase> _heldValues = new List<FVHolderBase>();

        /// <summary>
        /// Awful, un-type safe (but this is the CLR) tagging system.
        /// </summary>
        Dictionary<string, List<object>> _taggedObjects = new Dictionary<string, List<object>>();

        /// <summary>
        /// Add a tobject to the directory to be written out.
        /// </summary>
        /// <typeparam name="T">Type of the future falve</typeparam>
        /// <param name="obj">Object that will be added to the directory</param>
        /// <param name="tags">A list of string tags that can be used to retreive the future value at a later time</param>
        public void AddFuture<T>(IFutureValue<T> obj, string[] tags = null)
            where T : ROOTNET.Interface.NTObject
        {
            if (obj == null)
                throw new ArgumentNullException("Unable to store null futures in a FutureTDirectory!");

            _heldValues.Add(new FVHolder<T>() { Value = obj });
            if (tags != null)
            {
                foreach (var t in tags)
                {
                    if (!_taggedObjects.ContainsKey(t))
                        _taggedObjects[t] = new List<object>();
                    _taggedObjects[t].Add(obj);
                }
            }
        }

        /// <summary>
        /// Gets all objects that are of the right type and have the right tag. Never
        /// returns null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        /// <param name="recursive">If true this and all known subdirectories will be searched</param>
        /// <returns></returns>
        public IFutureValue<T>[] GetTaggedObjects<T>(string tag, bool recursive = false)
        {
            IFutureValue<T>[] localResult = null;
            if (_taggedObjects.ContainsKey(tag))
            {
                localResult = (from t in _taggedObjects[tag]
                               let o = t as IFutureValue<T>
                               where o != null
                               select o).ToArray();
            }
            else
            {
                localResult = new IFutureValue<T>[0];
            }

            IFutureValue<T>[] recursiveResult = null;
            if (recursive)
            {
                recursiveResult = (from subdir in _subDirs.Value
                                   from taggedPlot in subdir.GetTaggedObjects<T>(tag, true)
                                   select taggedPlot).ToArray();
            }
            else
            {
                recursiveResult = new IFutureValue<T>[0];
            }

            return localResult.Concat(recursiveResult).ToArray();
        }

        /// <summary>
        /// Convert everything and save it to the directory! Also write out
        /// all the sub-directories.
        /// </summary>
        /// <param name="calculateEverything">If true, then trigger all calculations here and below. Normally call with false.</param>
        public void Write(bool calculateEverything = true)
        {
            // Write everything associated with this directory.
            Directory.Write();

            // Trigger all the calculatiosn that are needed for these directories.
            // This drives the ability for parallel calculation of everything.
            if (calculateEverything)
            {
                TriggerResolutions().Wait();
            }

            // Local values
            foreach (var item in _heldValues)
            {
                item.Save(Directory);
            }
            _heldValues.Clear();

            // Next, the subdirectories
            if (_subDirs.IsValueCreated)
            {
                foreach (var item in _subDirs.Value)
                {
                    item.Write();
                }
            }
        }

        /// <summary>
        /// Grab all the tasks and aysync wait on them.
        /// </summary>
        private Task TriggerResolutions()
        {
            var ourResults = _heldValues.Select(v => v.WaiterTask);
            var subDirTasks = _subDirs.IsValueCreated
                ? _subDirs.Value.Select(d => d.TriggerResolutions())
                : Enumerable.Empty<Task>();

            return Task.WhenAll(ourResults.Concat(subDirTasks));
        }

        /// <summary>
        /// Keep track of subdirectories in this file.
        /// </summary>
        Lazy<List<FutureTDirectory>> _subDirs = new Lazy<List<FutureTDirectory>>();

        /// <summary>
        /// Create a new directory
        /// </summary>
        /// <remarks>
        /// In ROOT, when you create a new directory the gDirectory variable isn't changed; so the global directory
        /// will be unchanged by this operation.
        /// </remarks>
        /// <param name="subdirname"></param>
        /// <returns></returns>
        public FutureTDirectory mkdir(string subdirname)
        {
            var rootDir = Directory.mkdir(subdirname);
            if (rootDir == null)
            {
                rootDir = Directory.Get(subdirname) as ROOTNET.Interface.NTDirectory;
                if (rootDir == null)
                    throw new ArgumentException("Unable to create directory '" + subdirname + "' because something with that name already exists in '" + Directory.Name + "'.");
            }
            var future = new FutureTDirectory(rootDir);
            _subDirs.Value.Add(future);

            return future;
        }

        /// <summary>
        /// Return null or a found sub directory!
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public FutureTDirectory FindSubDir(string ds)
        {
            return _subDirs.Value.Where(sd => sd.Directory.Name == ds).FirstOrDefault();
        }
    }
}
