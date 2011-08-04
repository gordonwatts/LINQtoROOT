using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQToTTreeLib.ExecutionCommon
{
    /// <summary>
    /// Execute a query on a remote proof cluster.
    /// </summary>
    class ProofExecutor : IQueryExectuor
    {
        /// <summary>
        /// Run the execution on the PROOF cluster specified in the list of URI's we need to execute.
        /// </summary>
        /// <param name="templateFile"></param>
        /// <param name="queryDirectory"></param>
        /// <param name="varsToTransfer"></param>
        /// <returns></returns>
        public IDictionary<string, ROOTNET.Interface.NTObject> Execute(System.IO.FileInfo templateFile, System.IO.DirectoryInfo queryDirectory, IEnumerable<KeyValuePair<string, object>> varsToTransfer)
        {
            OrganizeUris();

            return null;
        }


        /// <summary>
        /// True if we've parsed the info we need from the environment.
        /// </summary>
        private bool _urisOrganized = false;

        /// <summary>
        /// The net name of the proof host.
        /// </summary>
        private string _proofHost;

        /// <summary>
        /// The list of PROOF datasets
        /// </summary>
        private string[] _datasets;

        /// <summary>
        /// Executed once, to x-check the Uri's we are going to be looking at.
        /// </summary>
        private void OrganizeUris()
        {
            if (_urisOrganized)
                return;

            //
            // Check for correctly formatted URL's
            //

            var allgood = (from u in Environment.RootFiles select u.Segments.Length).All(c => c == 2);
            if (!allgood)
                throw new ArgumentException("Dataset formats are not correct: only a proof machine name and a dataset name are allowed");

            //
            // We can deal with multiple data-sets, but only a single host.
            //

            var hosts = (from u in Environment.RootFiles
                         group u.PathAndQuery by u.Segments[1]).ToArray();
            if (hosts.Length != 1)
                throw new ArgumentException("There were more than one host for the list of PROOF datasets. This is not supported");

            _proofHost = hosts[0].Key;
            _datasets = hosts[0].ToArray();

            //
            // Finally, check to see if these datasets exist up there!
            //

            var proof = GetProofConnection();

            //
            // Next, see if the data sets are good.
            //

            foreach (var ds in _datasets)
            {
                if (!proof.ExistsDataSet(ds))
                {
                    throw new ArgumentException(string.Format("The proof server at {0} does not know about the dataset '{1}'", _proofHost, ds));
                }
            }

            //
            // Made it all the way through!
            //

            _urisOrganized = true;
        }

        /// <summary>
        /// Keep track of the local proof connection.
        /// </summary>
        private ROOTNET.Interface.NTProof _proofConnection;

        /// <summary>
        /// Open and return a connection to a proof cluster.
        /// </summary>
        /// <returns></returns>
        private ROOTNET.Interface.NTProof GetProofConnection()
        {
            if (_proofConnection == null)
            {
                _proofConnection = new ROOTNET.NTProof(_proofHost);
            }
            return _proofConnection;
        }

        /// <summary>
        /// Contains the environment needed to run the execution.
        /// </summary>
        public ExecutionEnvironment Environment { get; set; }
    }
}
