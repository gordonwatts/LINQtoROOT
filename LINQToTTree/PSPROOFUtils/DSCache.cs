
using System;
using System.Collections.Generic;
using System.Linq;
namespace PSPROOFUtils
{
    /// <summary>
    /// A cache that helps with keeping track of datasets on the server. The datasets
    /// are powershell items.
    /// </summary>
    class DSCache
    {
        /// <summary>
        /// The time of the last cache update.
        /// </summary>
        private DateTime _lastCacheUpdate;

        /// <summary>
        /// The max age our cache is allowed to get to.
        /// </summary>
        private TimeSpan _maxCacheAge = new TimeSpan(0, 5, 0);

        /// <summary>
        /// Track the actual cache
        /// </summary>
        private Dictionary<string, ProofDataSetItem> _cache = new Dictionary<string, ProofDataSetItem>();

        /// <summary>
        /// How many times should we attempt to get the dataset list before giving up and throwing?
        /// </summary>
        private int _maxDatasetRetries = 10;

        /// <summary>
        /// The proof connection.
        /// </summary>
        public ROOTNET.Interface.NTProof ProofConnection { get; set; }

        /// <summary>
        /// Update the internal cache, if we need to...
        /// </summary>
        /// <param name="ProofConnection">This is heald onto for teh rest of the lifetime of the cache</param>
        internal void Update()
        {
            if (ProofConnection == null)
                throw new InvalidOperationException("Can't try to update the proof cache if the connection to proof hasn't been established");

            //
            // Is it time?
            //

            if ((DateTime.Now - _lastCacheUpdate) < _maxCacheAge)
                return;

            //
            // Get a listing of all the datasets
            //

            _cache.Clear();
            var proofDS = LoadProofDSList(ProofConnection);
            foreach (var dsname in proofDS.Cast<ROOTNET.Interface.NTObjString>())
            {
                //
                // Ignore namespaces for now ("/users/gwatts/default/HG123");
                //

                var name = dsname.Name.Split('/').Last();

                //
                // Fetch back the global metadata
                //

                var fc = proofDS.GetValue(dsname) as ROOTNET.Interface.NTFileCollection;

                //
                // This guy brings back only global meta-data information, so only store that!
                //

                _cache[name] = new ProofDataSetItem(name, fc.NFiles, fc.NCorruptFiles, fc.NStagedFiles, fc.TotalSize);
            }

            //
            // Rest the cache time
            //

            _lastCacheUpdate = DateTime.Now;
        }

        /// <summary>
        /// Sometimes proof seems to have some trouble getting us back a dataset list... so
        /// keep trying...
        /// </summary>
        /// <param name="proof"></param>
        /// <returns></returns>
        private ROOTNET.Interface.NTMap LoadProofDSList(ROOTNET.Interface.NTProof proof)
        {
            for (int i = 0; i < _maxDatasetRetries; i++)
            {
                var r = proof.DataSets;
                if (r != null)
                    return r;
                System.Threading.Thread.Sleep(100);
            }

            throw new InvalidOperationException("Unable to get the dataset list from the proof server");
        }

        /// <summary>
        /// Return true if the dataset exists on this proof server.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal bool HasDataset(string path)
        {
            return _cache.ContainsKey(path);
        }

        /// <summary>
        /// Return a list of the proof datasets.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<ProofDataSetItem> GetDSItems()
        {
            return _cache.Values;
        }

        /// <summary>
        /// Return an item in our cache, or throw.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal ProofDataSetItem GetDSItem(string path, bool fullInformation)
        {
            var item = _cache[path];

            if (fullInformation && !item.InformationComplete)
            {
                var ds = ProofConnection.GetDataSet(item.Name);
                var files = ds.List.Cast<ROOTNET.Interface.NTFileInfo>();
                item.SetFullData(files);
            }

            return item;
        }
    }
}
