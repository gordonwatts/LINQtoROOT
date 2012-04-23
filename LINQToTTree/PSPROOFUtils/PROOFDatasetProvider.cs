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
        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            Console.WriteLine("In new drive");
            return base.NewDrive(drive);
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
    }
}
