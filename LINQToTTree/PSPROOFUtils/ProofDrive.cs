using System.Collections.Generic;
using System.Management.Automation;

namespace PSPROOFUtils
{

    /// <summary>
    /// Custom drive that is used to hold connection info. This is returned to powershell.
    /// </summary>
    class ProofDrive : PSDriveInfo
    {
        /// <summary>
        /// Create a proof drive that PS knows what to do with.
        /// </summary>
        /// <param name="basicInfo"></param>
        /// <param name="connection"></param>
        public ProofDrive(PSDriveInfo basicInfo, ROOTNET.Interface.NTProof connection)
            : base(basicInfo)
        {
            Cache = new DSCache();
            ProofConnection = connection;
            Cache.ProofConnection = connection;
        }

        /// <summary>
        /// Close our connection to the proof server.
        /// </summary>
        internal void Close()
        {
            Cache.ProofConnection = null;
            Cache = null;
            ProofConnection.Close();
            ProofConnection = null;
        }

        /// <summary>
        /// The proof connection this drive represents.
        /// </summary>
        public ROOTNET.Interface.NTProof ProofConnection { get; private set; }

        /// <summary>
        /// The backing cache for this drive.
        /// </summary>
        private DSCache Cache { get; set; }

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

            Cache.Update();
            return Cache.HasDataset(path);
        }

        /// <summary>
        /// Get a list of the datasets we know about!
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<ProofDataSetItem> GetDSItems()
        {
            Cache.Update();
            return Cache.GetDSItems();
        }

        /// <summary>
        /// Get the item for a dataset.
        /// </summary>
        /// <param name="path"></param>
        internal ProofDataSetItem GetDSItem(string path, bool fullInformation = false)
        {
            Cache.Update();
            return Cache.GetDSItem(path, fullInformation);
        }
    }
}
