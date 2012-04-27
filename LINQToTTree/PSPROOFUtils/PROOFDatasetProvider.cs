using System;
using System.Management.Automation;
using System.Management.Automation.Provider;

namespace PSPROOFUtils
{
    /// <summary>
    /// Provider that implements datasets as a list of datasets...
    /// </summary>
    [CmdletProvider("PROOFDS", ProviderCapabilities.None)]
    public class PROOFDatasetProvider : ItemCmdletProvider
    {
        /// <summary>
        /// Create a new powershell drive for our proof server.
        /// </summary>
        /// <param name="drive"></param>
        /// <returns></returns>
        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            var b = new ProofDrive(drive);
            b.ProofConnection = ROOTNET.NTProof.Open(drive.Root);
            if (b.ProofConnection == null || !b.ProofConnection.IsValid())
                throw new ArgumentException(string.Format("Unable to connect to PROOF server at '{0}'", drive.Root));
            return b;
        }

        /// <summary>
        /// Check to see if the path is valid or not.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected override bool IsValidPath(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Custom drive that is used to hold connection info.
        /// </summary>
        class ProofDrive : PSDriveInfo
        {
            public ProofDrive (PSDriveInfo basicInfo)
                : base(basicInfo)
            {}

            public ROOTNET.Interface.NTProof ProofConnection { get; set; }
        }
    }
}
