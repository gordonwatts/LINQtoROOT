using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using LinqToTTreeInterfacesLib;
using Remotion.Data.Linq;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Implement the caching algorithm for queries. This cache works by using hash's of strings. And we will keep text files around
    /// so that you can search the directory for results. Default cache location is the users data directory.
    /// </summary>
    [Export(typeof(IQueryResultCache))]
    class QueryResultCache : IQueryResultCache
    {
        public QueryResultCache()
        {
            CacheDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\LINQToTTree\\QueryCache");

        }

        /// <summary>
        /// Lookup in our cache. Fail if we can't find things by returning null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceRootFile"></param>
        /// <param name="queryModel"></param>
        /// <param name="varSaver"></param>
        /// <returns></returns>
        public Tuple<bool, T> Lookup<T>(FileInfo sourceRootFile, QueryModel queryModel, IVariableSaver varSaver, IVariable theVar)
        {
            ///
            /// Get the ROOT file
            /// 

            FileInfo rootFileInfo = GetROOTCacheFile(sourceRootFile, queryModel, false);
            if (rootFileInfo == null)
                return new Tuple<bool, T>(false, default(T));

            ///
            /// Next, load it up. There should be only one object in there, so it should be easy
            /// 

            var tf = new ROOTNET.NTFile(rootFileInfo.FullName, "READ");
            if (!tf.IsOpen())
                return new Tuple<bool, T>(false, default(T));

            try
            {

                var k = tf.ListOfKeys.FirstOrDefault() as ROOTNET.Interface.NTKey;
                if (k == null)
                    return new Tuple<bool, T>(false, default(T));

                return new Tuple<bool, T>(true, varSaver.LoadResult<T>(theVar, k.ReadObj()));
            }
            finally
            {
                tf.Close();
            }
        }

        /// <summary>
        /// Location of the cache directory.
        /// </summary>
        private DirectoryInfo _cache_dir;

        /// <summary>
        /// Get/Set the root directory where we will cache our results. Defaults to local app data directory.
        /// </summary>
        public DirectoryInfo CacheDirectory
        {
            get { return _cache_dir; }
            set
            {
                value.Refresh();
                if (!value.Exists)
                    value.Create();
                value.Refresh();
                _cache_dir = value;
            }
        }

        /// <summary>
        /// Cache an item for later retrevial.
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="qm"></param>
        /// <param name="o"></param>
        public void CacheItem(FileInfo sourceFile, QueryModel qm, ROOTNET.Interface.NTObject o)
        {
            var osaver = o.Clone();
            var rootFile = GetROOTCacheFile(sourceFile, qm, true);
            var trf = new ROOTNET.NTFile(rootFile.FullName, "RECREATE");
            osaver.Write();
            trf.Write();
            trf.Close();
        }

        /// <summary>
        /// Get the root file FileInfo for the item we are going to query. If "create it all" is set, then
        /// create all the meta-data files and other things along the way.
        /// </summary>
        /// <param name="_rootFile"></param>
        /// <param name="qm"></param>
        /// <param name="createEverything"></param>
        private FileInfo GetROOTCacheFile(FileInfo _rootFile, QueryModel qm, bool createEverything)
        {
            ///
            /// Get the directory to cache.
            ///

            var dfile = FindCachForFile(_rootFile, createEverything);
            if (dfile == null)
                return null;

            ///
            /// And now the file that the query should be cached in
            /// 

            var queryHash = qm.ToString().GetHashCode();
            string queryNameBase = @"\\query " + queryHash.ToString();
            var rootFile = new FileInfo(dfile.FullName + queryNameBase + ".root");
            if (rootFile.Exists)
                return rootFile;

            ///
            /// Ok, the root file doesn't exist. If we are not creating, then we fail. Otherwise
            /// we create the meta-data file and return.
            /// 

            if (!createEverything)
            {
                return null;
            }

            using (var writer = new FileInfo(dfile.FullName + queryNameBase + ".txt").CreateText())
            {
                writer.WriteLine(qm.ToString());
                writer.Close();
            }

            return rootFile;
        }

        /// <summary>
        /// Returns the cache directory for a particular file. Create it if requested. Check the modification
        /// date - if the file was modified, then we need to kill off everything we already know about the
        /// file as the file has been altered. :(
        /// </summary>
        /// <param name="sourceRootFile"></param>
        /// <returns></returns>
        private DirectoryInfo FindCachForFile(FileInfo sourceRootFile, bool create = false)
        {
            var hash = sourceRootFile.FullName.GetHashCode();
            DirectoryInfo df = new DirectoryInfo(CacheDirectory.FullName + "\\" + hash + " - " + sourceRootFile.Name);

            ///
            /// If the directory does exist, then check the modification date.
            /// 

            var descriptor = new FileInfo(df.FullName + "\\file.txt");
            if (df.Exists)
            {
                sourceRootFile.Refresh();
                if (sourceRootFile.LastWriteTime > descriptor.LastWriteTime)
                {
                    df.Delete(true);
                    df.Refresh();
                }
            }

            ///
            /// Ok, if the directory doesn't exist, then we should do some work on it.
            /// 

            if (!df.Exists)
            {
                if (!create)
                    return null;
                df.Create();
                df.Refresh();

                using (var writer = descriptor.CreateText())
                {
                    writer.WriteLine("This cache directory is for queries against the file:");
                    writer.WriteLine(sourceRootFile.FullName);
                    writer.Close();
                }
            }

            return df;
        }
    }
}
