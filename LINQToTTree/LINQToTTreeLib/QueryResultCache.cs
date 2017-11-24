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
using ROOTNET.Interface;

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

            public string UniqueHashString { get; set; }

            public string GetUniqueHashString()
            {
                return UniqueHashString;
            }
        }

        /// <summary>
        /// Return the key object
        /// </summary>
        /// <param name="rootfiles"></param>
        /// <param name="treename"></param>
        /// <param name="inputObjects"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryResultCacheKey GetKey(Uri[] unsortedRootfiles, string treename, object[] inputObjects,
            string[] unsortedCrumbs,
            QueryModel query,
            bool recheckDates = false,
            Func<Uri, DateTime> dateChecker = null)
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
                             orderby r.OriginalString ascending
                             select r).ToArray();

            TraceHelpers.TraceInfo(24, "GetKey: Creating big string file name and calculating hash");
            int fnameLength = rootfiles.Select(f => f.OriginalString).Sum(w => w.Length) + 100;
            StringBuilder fullSourceName = new StringBuilder(fnameLength);
            foreach (var f in rootfiles)
            {
                fullSourceName.Append(f.OriginalString);
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
            var fpathName = Path.GetFileNameWithoutExtension(rootfiles[0].PathAndQuery.SanitizedPathName());
            result.CacheDirectory = new DirectoryInfo(CacheDirectory.FullName + "\\" + flieHash + " - " + treename + "-" + fpathName);

            ///
            /// Scan the files that we are input and find the oldest one there
            /// 

            TraceHelpers.TraceInfo(28, "GetKey: calculating the most recent file dates");
            result.OldestSourceFileDate = GetRecentFileDates(rootfiles, recheckDates, dateChecker).Max();

            ///
            /// And now the file that the query should be cached in
            /// 

            TraceHelpers.TraceInfo(29, "GetKey: Calculating query hash");
            var queryHash = result.QueryText.GetHashCode();
            TraceHelpers.TraceInfo(30, "GetKey: Calculating the input object hash");
            var inputObjectHash = CalcObjectHash(inputObjects);
            string queryNameBase = string.Format(@"\\query {0}-inp{1}-crm{2}", queryHash.ToString(), inputObjectHash, crumbHash);
            result.RootFile = new FileInfo(result.CacheDirectory.FullName + queryNameBase + "_%%CYCLE%%.root");

            // And a complete unique hash string.
            result.UniqueHashString = $"files{flieHash.ToString()}-query{queryHash.ToString()}-objs{inputObjectHash}-crm{crumbHash}";

            TraceHelpers.TraceInfo(31, "GetKey: Done");
            return result;
        }

        /// <summary>
        /// Get the most recent modified date
        /// </summary>
        /// <param name="rootfiles"></param>
        /// <returns></returns>
        private IEnumerable<DateTime> GetRecentFileDates(Uri[] rootfiles, bool recheckDates, Func<Uri, DateTime> dateChecker)
        {
            return from f in rootfiles
                   select ConvertToLastUpdateTime(f, recheckDates, dateChecker);
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
        private DateTime ConvertToLastUpdateTime(Uri u, bool attemptDateRecheck, Func<Uri, DateTime> dateChecker)
        {
            if (attemptDateRecheck || !_uriToFileInfo.ContainsKey(u))
            {
                if (dateChecker == null)
                {
                    throw new InvalidOperationException($"Attempt to look at DateTime for dataset {u.OriginalString}. But no method to check it provided!");
                }
                var result = dateChecker(u);
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
        /// <param name="akey"></param>
        /// <returns></returns>
        public Tuple<bool, T> Lookup<T>(IQueryResultCacheKey akey, IVariableSaver varSaver, IDeclaredParameter theVar,
            Func<IAddResult> generateAdder = null)
        {
            if (akey == null || (akey as KeyInfo) == null)
                throw new ArgumentNullException("the key cannot be null");
            var key = akey as KeyInfo;

            // Check to make sure none of the source root files have actually be altered since
            // the cache line was written (if there is a cache line, indeed!).
            if (!ROOTFileDatesOK(key))
                return new Tuple<bool, T>(false, default(T));

            // Next, read in as many of the cycles of cache files as there are, 
            // grabbing all the objects we need.
            var cycleObjects =
                Enumerable.Range(0, 1000)
                .Select(index => LoadCacheData<T>(key, index, varSaver, theVar))
                .TakeWhile(objs => objs.found)
                .ToArray();

            if (cycleObjects.Length == 0 && cycleObjects.All(cval => cval.found == true && cval.val != null))
            {
                return new Tuple<bool, T>(false, default(T));
            }

            // Special case for where this a single cycle. We just read them in and push them through the saver
            // and return them.
            if (cycleObjects.Length == 1)
            {
                return new Tuple<bool, T>(true, cycleObjects[0].val);
            }
            else
            {
                // More than one cycle object. We will have to add things together before we can do anything with them.
                if (generateAdder == null)
                {
                    throw new InvalidOperationException($"Unable to combine data types {typeof(T).Name} because an IAddResult wasn't passed to me!");
                }

                var adder = generateAdder();
                var addedValue = cycleObjects.Skip(1)
                    .Aggregate(adder.Clone(cycleObjects[0].val), (acc, newv) => adder.Update(acc, newv.val));
                return new Tuple<bool, T>(true, addedValue);
            }
        }

        /// <summary>
        /// Read in the data for a particular cycle. Return it along with the file. This does no work to clone the objects
        /// or anything similar. Returns null if it couldn't find the cycle file or anythng at all went wrong.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private (bool found, T val) LoadCacheData<T>(KeyInfo key, int index, IVariableSaver svr, IDeclaredParameter prm)
        {
            // Get the file name for this cycle and see if it exists.
            var cycleFilename = FileForCycle(key, index);
            if (!File.Exists(cycleFilename))
            {
                return (false, default(T));
            }

            // Load all the objects in this file.
            var tf = NTFile.Open(cycleFilename, "READ");
            try
            {
                var keys = tf.ListOfKeys;
                if (keys.Size == 0)
                    return (false, default(T));

                var cachedObjects = keys.Cast<ROOTNET.Interface.NTKey>().Select(k => k.ReadObj()).ToArray();

                // Now do the pick up. Make sure we are in the root directory when we do it, however!
                // We do this b.c. sometimes the saver will Clone an object, and if it becomes attached to a file,
                // it will be deleted when the file is closed on the way out of this routine.
                ROOTNET.NTROOT.gROOT.cd();
                return (true, svr.LoadResult<T>(prm, cachedObjects, index));
            }
            finally
            {
                // In case bad things happened, try not to leave anything behind.
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
            CacheItem(akey, new[] { objs });
        }

        /// <summary>
        /// Cache item with cycles - in short, the same thing several times.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cycleOfItems"></param>
        public void CacheItem(IQueryResultCacheKey akey, IEnumerable<NTObject[]> cycleOfItems)
        {
            // Fail if we can't get a key of our own type.
            var key = (akey as KeyInfo)
                .ThrowIfNull(() => new ArgumentNullException("The key must be valid to cache an item"));

            // Now, write out the text file that tells everyone what files are here. Do that only
            // if the thing isn't there already. If the contents of the file change, then th key has
            // changed - we are assuming no hash collisions!
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

            // Write out the query and any extra info so the user can see when debugging. There is no use for
            // the caching infrastructure for this file.
            using (var writer = File.CreateText(Path.ChangeExtension(key.RootFile.FullName, "txt")))
            {
                writer.WriteLine(key.QueryText);
                foreach (var r in key.ExtraQueryInfoLines)
                {
                    writer.WriteLine(r);
                }
            }

            // Next, write out the cache files themselves. We do one for each cycle.
            foreach (var cycleItems in cycleOfItems.Zip(Enumerable.Range(0,1000), (c, idx) => (items: c, index:idx)))
            {
                if (cycleItems.items.Length == 0)
                {
                    throw new InvalidOperationException("Can't deal with caching zero objects!");
                }

                var clones = cycleItems.items.Select(o => o.Clone()).ToArray();
                var trf = new ROOTNET.NTFile(FileForCycle(key, cycleItems.index), "RECREATE");
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
        }

        /// <summary>
        /// Generate the filename for a particular cycle.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cycle"></param>
        /// <returns></returns>
        private string FileForCycle(KeyInfo key, int cycle)
        {
            return key.RootFile.FullName.Replace("%%CYCLE%%", cycle.ToString());
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
