﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using Remotion.Data.Linq;
using System.Diagnostics;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Implement the caching algorithm for queries. This cache works by using hash's of strings. And we will keep text files around
    /// so that you can search the directory for results. Default cache location is the users data directory.
    /// </summary>
    [Export(typeof(IQueryResultCache))]
    class QueryResultCache : IQueryResultCache
    {
        /// <summary>
        /// Create the cache manager
        /// </summary>
        public QueryResultCache()
        {
            if (!_cache_dir_good)
                CacheDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\LINQToTTree\\QueryCache");
        }

        /// <summary>
        /// Private cache of the key info for 
        /// </summary>
        class KeyInfo : IQueryResultCacheKey
        {
            /// <summary>
            /// Directory where the cache files can be found.
            /// </summary>
            public DirectoryInfo CacheDirectory { get; set; }

            public FileInfo RootFile { get; set; }

            public DateTime OldestSourceFileDate { get; set; }

            public string[] DescriptionLines { get; set; }

            public string QueryText { get; set; }
        }

        /// <summary>
        /// Return the key object
        /// </summary>
        /// <param name="rootfiles"></param>
        /// <param name="treename"></param>
        /// <param name="inputObjects"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryResultCacheKey GetKey(FileInfo[] rootfiles, string treename, object[] inputObjects, QueryModel query)
        {
            // <pex>
            Debug.Assert(rootfiles[0] != (object)null, "rootfiles[0]");
            // </pex>

            ///
            /// Build the hash, which is a bit of a pain in the butt.
            /// 

            StringBuilder fullSourceName = new StringBuilder();
            foreach (var f in rootfiles)
            {
                fullSourceName.Append(f.FullName);
            }

            var hash = fullSourceName.ToString().GetHashCode();

            ///
            /// Save the names of the files for a descriptor we will write out.
            /// 

            KeyInfo result = new KeyInfo();

            result.DescriptionLines = (from f in rootfiles
                                       select f.FullName).ToArray();

            ///
            /// Text for the query
            /// 

            result.QueryText = query.ToString();

            ///
            /// And the directory name - we use the first name of the file.
            /// 

            result.CacheDirectory = new DirectoryInfo(CacheDirectory.FullName + "\\" + hash + " - " + treename + "-" + Path.GetFileNameWithoutExtension(rootfiles[0].Name));

            ///
            /// Scan the files that we are input and find the oldest one there
            /// 

            result.OldestSourceFileDate = GetRecentFileDates(rootfiles).Max();

            ///
            /// And now the file that the query should be cached in
            /// 

            var queryHash = query.ToString().GetHashCode();
            string queryNameBase = @"\\query " + queryHash.ToString() + "-" + CalcObjectHash(inputObjects).ToString();
            result.RootFile = new FileInfo(result.CacheDirectory.FullName + queryNameBase + ".root");

            return result;
        }

        /// <summary>
        /// Get the most recent modified date
        /// </summary>
        /// <param name="rootfiles"></param>
        /// <returns></returns>
        private IEnumerable<DateTime> GetRecentFileDates(FileInfo[] rootfiles)
        {
            foreach (var f in rootfiles)
            {
                f.Refresh();
                yield return f.LastWriteTime;
            }
        }

        /// <summary>
        /// Lookup in our cache. Fail if we can't find things by returning null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceRootFiles"></param>
        /// <param name="queryModel"></param>
        /// <param name="varSaver"></param>
        /// <returns></returns>
        public Tuple<bool, T> Lookup<T>(IQueryResultCacheKey akey, IVariableSaver varSaver, IVariable theVar)
        {
            if (akey == null || (akey as KeyInfo) == null)
                throw new ArgumentNullException("the key cannot be null");
            var key = akey as KeyInfo;

            ///
            /// First, check to make sure none of the source root files have actually be altered since
            /// the cache line was written (if there is a cache line, indeed!).
            /// 

            if (!ROOTFileDatesOK(key))
                return new Tuple<bool, T>(false, default(T));

            ///
            /// Ok. Next task is to see if the cache file exists right now
            /// 

            key.RootFile.Refresh();
            if (!key.RootFile.Exists)
                return new Tuple<bool, T>(false, default(T));

            ///
            /// At this point we think that the file contains a valid cache line. As long as it isn't
            /// corrupt! :-)
            /// 

            var tf = new ROOTNET.NTFile(key.RootFile.FullName, "READ");
            if (!tf.IsOpen())
                return new Tuple<bool, T>(false, default(T));

            ///
            /// Find and load the object. Proect against an error in ROOT while this is going on
            /// causing us to leave something open. Note b/c of the way ROOT works we need to disconnect
            /// the object from the file. There is a uniform way to do this, but it involves a call-back
            /// and the ROOT.NET translation wrapper doesn't implement this yet. So we have to use the clone
            /// technique, after we change our default directory. :(
            /// 

            try
            {

                var k = tf.ListOfKeys.FirstOrDefault() as ROOTNET.Interface.NTKey;
                if (k == null)
                    return new Tuple<bool, T>(false, default(T));

                var cachedObject = k.ReadObj();
                ROOTNET.NTROOT.gROOT.cd();
                return new Tuple<bool, T>(true, varSaver.LoadResult<T>(theVar, k.ReadObj()));
            }
            finally
            {
                tf.Close();
            }
        }

        /// <summary>
        /// Check to see if any source root files have had their date updated. If a source file has
        /// a date more modern than the cache file, then we return false - the cache line is out of date.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool ROOTFileDatesOK(KeyInfo key)
        {
            var df = GetCacheInfoFileDescriptor(key);
            if (df.Exists)
            {
                if (key.OldestSourceFileDate < df.LastWriteTime)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the file that contains the basic text for the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static FileInfo GetCacheInfoFileDescriptor(KeyInfo key)
        {
            var df = new FileInfo(key.CacheDirectory.FullName + "\\file.txt");
            return df;
        }

        /// <summary>
        /// Location of the cache directory.
        /// </summary>
        private static DirectoryInfo _cache_dir;

        /// <summary>
        /// Has the cache directory been set once, and set well? :-)
        /// </summary>
        private static bool _cache_dir_good = false;

        /// <summary>
        /// Get/Set the root directory where we will cache our results. Defaults to local app data directory.
        /// </summary>
        public static DirectoryInfo CacheDirectory
        {
            get { return _cache_dir; }
            set
            {
                value.Refresh();
                if (!value.Exists)
                    value.Create();
                value.Refresh();
                _cache_dir = value;
                _cache_dir_good = true;
            }
        }

        /// <summary>
        /// Cache an item for later retrevial.
        /// </summary>
        /// <param name="sourceFiles"></param>
        /// <param name="qm"></param>
        /// <param name="o"></param>
        public void CacheItem(IQueryResultCacheKey akey, ROOTNET.Interface.NTObject o)
        {
            var key = akey as KeyInfo;
            if (key == null)
                throw new ArgumentNullException("The key must be valid to cache an item");

            ///
            /// See if teh directory exists
            /// 

            ///
            /// Now, write out the text file that tells everyone what files are here. Do that only
            /// if the thing isn't there already.
            /// 

            var df = GetCacheInfoFileDescriptor(key);
            if (!df.Exists)
            {
                if (!df.Directory.Exists)
                    df.Directory.Create();

                using (var writer = df.CreateText())
                {
                    writer.WriteLine("This cache directory is for queries against the file:");
                    foreach (var f in key.DescriptionLines)
                    {
                        writer.WriteLine(f);
                    }
                    writer.Close();
                }
            }

            ///
            /// Write out the query
            /// 

            using (var writer = File.CreateText(Path.ChangeExtension(key.RootFile.FullName, "txt")))
            {
                writer.WriteLine(key.QueryText);
            }

            ///
            /// Ok, now save it to the root file
            /// 

            var osaver = o.Clone();
            var trf = new ROOTNET.NTFile(key.RootFile.FullName, "RECREATE");
            osaver.Write();
            trf.Write();
            trf.Close();
        }

        /// <summary>
        /// Calculate the hash value for a bunch of objects
        /// </summary>
        /// <param name="inputObjects"></param>
        /// <returns></returns>
        private int CalcObjectHash(object[] inputObjects)
        {
            if (inputObjects == null)
                return 0;

            ObjectHashCalculator cq = new ObjectHashCalculator();
            foreach (var o in inputObjects)
            {
                cq.AccumutlateHash(o);
            }
            return cq.Hash;
        }
    }
}
