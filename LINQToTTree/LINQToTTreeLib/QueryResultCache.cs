using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.QueryVisitors;
using LINQToTTreeLib.Utils;
using Remotion.Linq;
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

            public string[] ExtraQueryInfoLines { get; set; }

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
        public IQueryResultCacheKey GetKey(Uri[] unsortedRootfiles, string treename, object[] inputObjects, string[] unsortedCrumbs, QueryModel query, bool recheckDates = false)
        {
            ///
            /// Quick check to make sure everything is good
            /// 

            TraceHelpers.TraceInfo(23, "GetKey: Initial query calculation");
            if (unsortedRootfiles.Any(f => f == null))
                throw new ArgumentException("one of the root files is null");
            if (string.IsNullOrWhiteSpace(treename))
                throw new ArgumentException("tree name must be valid");
            if (inputObjects != null && inputObjects.Any(o => o == null))
                throw new ArgumentException("one of the input objects is null - not allowed");

            ///
            /// Build the hash, which is a bit of a pain in the butt.
            /// For the root files we don't care about the order given to us in or the order they
            /// are processed in. What we care about is what is there!
            /// 

            var rootfiles = (from r in unsortedRootfiles
                             orderby r.AbsolutePath ascending
                             select r).ToArray();

            TraceHelpers.TraceInfo(24, "GetKey: Creating big string file name and calculating hash");
            int fnameLength = rootfiles.Select(f => f.AbsolutePath).Sum(w => w.Length) + 100;
            StringBuilder fullSourceName = new StringBuilder(fnameLength);
            foreach (var f in rootfiles)
            {
                fullSourceName.Append(f.AbsolutePath);
            }

            var flieHash = fullSourceName.ToString().GetHashCode();

            //
            // Next, the crumbs. They should also be sorted in order, and we will need
            // a hash code for them too.
            //

            string[] crumbs = null;
            int crumbHash = 0;
            if (unsortedCrumbs == null)
            {
                crumbs = new string[0];
            }
            else
            {
                crumbs = (from c in unsortedCrumbs orderby c select c).Distinct().ToArray();
                StringBuilder crumbString = new StringBuilder();
                foreach (var c in crumbs)
                {
                    crumbString.Append(c);
                }
                crumbHash = crumbString.ToString().GetHashCode();
            }

            ///
            /// Save the names of the files for a descriptor we will write out.
            /// 

            KeyInfo result = new KeyInfo();

            TraceHelpers.TraceInfo(25, "GetKey: Saving description lines");
            result.DescriptionLines = (from f in rootfiles
                                       select f.OriginalString).ToArray();
            result.ExtraQueryInfoLines = crumbs;

            ///
            /// Text for the query. There are strings like "generated_x" where x is a number. These get incremented each time they are used,
            /// so to protect the caching we need to swap those out with a dummy.
            /// 

            TraceHelpers.TraceInfo(26, "GetKey: Pretty printing the query");
            result.QueryText = FormattingQueryVisitor.Format(query);
            result.QueryText = result.QueryText.SwapOutWithUninqueString("\\<generated\\>_[0-9]+");

            ///
            /// And the directory name - we use the first name of the file.
            /// 

            TraceHelpers.TraceInfo(27, "GetKey: Getting the cache directory");
            result.CacheDirectory = new DirectoryInfo(CacheDirectory.FullName + "\\" + flieHash + " - " + treename + "-" + Path.GetFileNameWithoutExtension(rootfiles[0].PathAndQuery));

            ///
            /// Scan the files that we are input and find the oldest one there
            /// 

            TraceHelpers.TraceInfo(28, "GetKey: calculating the most recent file dates");
            result.OldestSourceFileDate = GetRecentFileDates(rootfiles, recheckDates).Max();

            ///
            /// And now the file that the query should be cached in
            /// 

            TraceHelpers.TraceInfo(29, "GetKey: Calculating query hash");
            var queryHash = result.QueryText.GetHashCode();
            TraceHelpers.TraceInfo(30, "GetKey: Calculating the input object hash");
            string queryNameBase = string.Format(@"\\query {0}-inp{1}-crm{2}", queryHash.ToString(), CalcObjectHash(inputObjects), crumbHash);
            result.RootFile = new FileInfo(result.CacheDirectory.FullName + queryNameBase + ".root");

            TraceHelpers.TraceInfo(31, "GetKey: Done");
            return result;
        }

        /// <summary>
        /// Get the most recent modified date
        /// </summary>
        /// <param name="rootfiles"></param>
        /// <returns></returns>
        private IEnumerable<DateTime> GetRecentFileDates(Uri[] rootfiles, bool recheckDates)
        {
            return from f in rootfiles
                   select ConvertToLastUpdateTime(f, recheckDates);
        }

        /// <summary>
        /// Keep track of file info's we've created. We do this because checking the date on a FileInfo is
        /// very expensive. So unless explicitly asked, we don't want to do it over and over again in a single
        /// run (it can take many many seconds for files located on a high latency server).
        /// </summary>
        private Dictionary<Uri, DateTime> _uriToFileInfo = new Dictionary<Uri, DateTime>();

        /// <summary>
        /// Convert a Uri into a date/time. We cache the date and time we find when
        /// we first look it up unless we are asked to recheck.
        /// 
        /// For files we actually check the date.
        /// For proof datasets we assume the dataset is "stable" once created.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        private DateTime ConvertToLastUpdateTime(Uri u, bool attemptDateRecheck)
        {
            if (attemptDateRecheck || !_uriToFileInfo.ContainsKey(u))
            {
                DateTime result;
                if (u.Scheme == "file")
                {
                    result = File.GetLastWriteTime(u.LocalPath);
                }
                else if (u.Scheme == "proof")
                {
                    result = new DateTime(1990, 12, 1);
                }
                else
                {
                    throw new ArgumentException(string.Format("Do not know how to figure out the date for the URI '{0}'", u.OriginalString));
                }
                _uriToFileInfo[u] = result;
                return result;
            }

            // Cached! Use it!

            return _uriToFileInfo[u];
        }

        /// <summary>
        /// Lookup in our cache. Fail if we can't find things by returning null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceRootFiles"></param>
        /// <param name="queryModel"></param>
        /// <param name="varSaver"></param>
        /// <returns></returns>
        public Tuple<bool, T> Lookup<T>(IQueryResultCacheKey akey, IVariableSaver varSaver, IDeclaredParameter theVar)
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
            /// Find and load the object. Protect against an error in ROOT while this is going on
            /// causing us to leave something open. Note b/c of the way ROOT works we need to disconnect
            /// the object from the file. There is a uniform way to do this, but it involves a call-back
            /// and the ROOT.NET translation wrapper doesn't implement this yet. So we have to use the clone
            /// technique, after we change our default directory. :(
            /// 

            try
            {
                var keys = tf.ListOfKeys;
                if (keys.Size == 0)
                    return new Tuple<bool, T>(false, default(T));

                var cachedObjects = keys.Cast<ROOTNET.Interface.NTKey>().Select(k => k.ReadObj());
                ROOTNET.NTROOT.gROOT.cd();
                var v = varSaver.LoadResult<T>(theVar, cachedObjects.ToArray());
                return new Tuple<bool, T>(v != null, v);
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
            Trace.WriteLine("Failing the ROOTFileDatesOK check");
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
        /// Cache an item for later retrieval.
        /// </summary>
        /// <param name="sourceFiles"></param>
        /// <param name="qm"></param>
        /// <param name="o"></param>
        public void CacheItem(IQueryResultCacheKey akey, ROOTNET.Interface.NTObject[] objs)
        {
            var key = akey as KeyInfo;
            if (key == null)
                throw new ArgumentNullException("The key must be valid to cache an item");

            ///
            /// Now, write out the text file that tells everyone what files are here. Do that only
            /// if the thing isn't there already.
            /// 

            var df = GetCacheInfoFileDescriptor(key);
            if (!df.Exists || !ROOTFileDatesOK(key))
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
            /// Write out the query and any extra info so the user can see when debugging.
            /// 

            using (var writer = File.CreateText(Path.ChangeExtension(key.RootFile.FullName, "txt")))
            {
                writer.WriteLine(key.QueryText);
                foreach (var r in key.ExtraQueryInfoLines)
                {
                    writer.WriteLine(r);
                }
            }

            // Ok, now save all of them to the root file

            var clones = objs.Select(o => o.Clone()).ToArray();
            var trf = new ROOTNET.NTFile(key.RootFile.FullName, "RECREATE");
            try
            {
                foreach (var obj in clones)
                {
                    obj.Write();
                }
            }
            finally
            {
                trf.Write();
                trf.Close();
            }
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
