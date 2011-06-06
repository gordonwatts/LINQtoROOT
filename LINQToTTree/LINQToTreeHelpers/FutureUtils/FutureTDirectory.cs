﻿
using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;

namespace LINQToTreeHelpers.FutureUtils
{
    /// <summary>
    /// Holds a series of Future TObject like guys and when the time
    /// comes (you call write) will write them into the root directory
    /// </summary>
    public class FutureTDirectory
    {
        public ROOTNET.Interface.NTDirectory Directory { get; set; }

        public FutureTDirectory(ROOTNET.Interface.NTDirectory dir)
        {
            Directory = dir;
        }

        abstract class FVHolderBase
        {
            public void Save(ROOTNET.Interface.NTDirectory dir)
            {
                dir.WriteTObject(AsTObject);
            }

            public abstract ROOTNET.Interface.NTObject AsTObject { get; }
        }

        class FVHolder<T> : FVHolderBase
            where T : ROOTNET.Interface.NTObject
        {
            public IFutureValue<T> Value { get; set; }

            public override ROOTNET.Interface.NTObject AsTObject
            {
                get { return Value.Value; }
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
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void AddFuture<T>(IFutureValue<T> obj, string[] tags = null)
            where T : ROOTNET.Interface.NTObject
        {
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
        public void Write()
        {
            ///
            /// Local values
            /// 

            foreach (var item in _heldValues)
            {
                item.Save(Directory);
            }
            _heldValues.Clear();

            ///
            /// Next, the subdirectories
            /// 

            if (_subDirs.IsValueCreated)
            {
                foreach (var item in _subDirs.Value)
                {
                    item.Write();
                }
            }
        }

        /// <summary>
        /// Keep track of subdirectories in this file.
        /// </summary>
        Lazy<List<FutureTDirectory>> _subDirs = new Lazy<List<FutureTDirectory>>();

        /// <summary>
        /// Create a new directory
        /// </summary>
        /// <param name="subdirname"></param>
        /// <returns></returns>
        public FutureTDirectory mkdir(string subdirname)
        {
            var rootDir = Directory.mkdir(subdirname);
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
