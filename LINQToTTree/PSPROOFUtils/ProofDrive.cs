using System.Collections.Generic;
using System.Management.Automation;

namespace PSPROOFUtils
{

    /// <summary>
    /// Custom drive that is used to hold connection info. This is returned to powershell.
    /// </summary>
    class ProofDrive : PSDriveInfo
    {
        public ProofDrive(PSDriveInfo basicInfo)
            : base(basicInfo)
        {
            Cache = new DSCache();
        }

        public ROOTNET.Interface.NTProof ProofConnection { get; set; }
        public DSCache Cache { get; private set; }

        /// <summary>
        /// Return true if this dataset exists.
        /// </summary>
        /// <param name="path"></param>
        internal bool HasDataset(string path)
        {
            //
            // If we are dealing with the top level... well, that always exists on a valid
            // proof connection!
            //

            if (path == "")
                return true;

            //
            // Make sure the cache is up to date, and then check it!
            //

            Cache.Update(ProofConnection);
            return Cache.HasDataset(path);
        }

        /// <summary>
        /// Get a list of the datasets we know about!
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<ProofDataSetItem> GetDSItems()
        {
            Cache.Update(ProofConnection);
            return Cache.GetDSItems();
        }
    }
}
