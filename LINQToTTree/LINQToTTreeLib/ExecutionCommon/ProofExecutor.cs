using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
        public IDictionary<string, ROOTNET.Interface.NTObject> Execute(FileInfo templateFile, DirectoryInfo queryDirectory, IEnumerable<KeyValuePair<string, object>> varsToTransfer)
        {
            //
            // x-check parameters
            //

            if (templateFile == null)
                return null;
            if (!templateFile.Exists)
                throw new FileNotFoundException("Unable to find C++ code to use for PROOF query", templateFile.FullName);

            //
            // Make sure everythign is inialized
            //

            OrganizeUris();

            //
            // Now run the PROOF query
            //

            var pc = GetProofConnection();
            var r = pc.Process(PROOFDSNames(), string.Format("{0}+", templateFile.Name));
            if (r == -1)
                throw new InvalidOperationException("Faild to run PROOF query");

            //
            // Clean up
            //

            if (Environment.CleanupQuery)
            {
                if (queryDirectory != null)
                    if (queryDirectory.Exists)
                        queryDirectory.Delete();
            }

            //
            // Return back all the objects that came back from the script
            //

            return null;
        }

        /// <summary>
        /// Generate the dataset names for PROOF to address in the proper format.
        /// </summary>
        /// <returns></returns>
        private string PROOFDSNames()
        {
            //
            // Some x-checks to make sure we are not doing anything crazy
            //

            if (_datasets.Length != 1)
                throw new InvalidOperationException("Currently can't deal a PROOF query that isn't just one dataset");

            if (Environment.TreeName == null)
                throw new ArgumentNullException("Tree name can't be null");

            return string.Format("{0}#{1}", _datasets[0], Environment.TreeName);
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
            // We can deal with multiple data-sets, but only a single host. Strip the leading "/" off the dataset name when we
            // look at this as a URI segment (the substring below).
            //

            var hosts = (from u in Environment.RootFiles
                         group u.PathAndQuery.Substring(1) by u.Host).ToArray();
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
                _proofConnection = new ROOTNET.NTProof(string.Format("proof://{0}", _proofHost));
            }
            return _proofConnection;
        }

        /// <summary>
        /// Contains the environment needed to run the execution.
        /// </summary>
        public ExecutionEnvironment Environment { get; set; }
    }
}
