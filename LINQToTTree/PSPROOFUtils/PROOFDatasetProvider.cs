using System;
using System.Management.Automation;
using System.Management.Automation.Provider;

namespace PSPROOFUtils
{
    /// <summary>
    /// Provider that implements datasets as a list of datasets...
    /// </summary>
    [CmdletProvider("PROOFDS", ProviderCapabilities.None)]
    public class PROOFDatasetProvider : ContainerCmdletProvider
    {
        #region Drive Overrides
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
        /// Helper function to save us typing - get at the drive object, but
        /// marked as our own type so we don't have to write the conversion over and over.
        /// </summary>
        private ProofDrive PROOFDrive
        {
            get { return this.PSDriveInfo as ProofDrive; }
        }


        /// <summary>
        /// Turn off the drive - close the connection to PROOF.
        /// </summary>
        /// <param name="drive"></param>
        /// <returns></returns>
        protected override PSDriveInfo RemoveDrive(PSDriveInfo drive)
        {
            var p = drive as ProofDrive;
            if (p == null)
                throw new InvalidOperationException("Attempt to remove a PROOF drive for a drive that isn't PROOF!");
            p.ProofConnection.Close();
            p.ProofConnection = null;

            return drive;
        }

        #endregion

        #region Item Overrides
        /// <summary>
        /// Return an item from our drive - we just get the item object here!
        /// </summary>
        /// <param name="path"></param>
        protected override void GetItem(string path)
        {
            var i = PROOFDrive.GetDSItems(path);
            WriteItemObject(i, i.Name, false);
        }

        protected override void SetItem(string path, object value)
        {
            base.SetItem(path, value);
        }

        /// <summary>
        /// Check to see if the item exists in our current proof guy.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected override bool ItemExists(string path)
        {
            return PROOFDrive.HasDataset(path);
        }

        protected override void ClearItem(string path)
        {
            base.ClearItem(path);
        }

        protected override void InvokeDefaultAction(string path)
        {
            base.InvokeDefaultAction(path);
        }

        protected override string[] ExpandPath(string path)
        {
            throw new NotSupportedException();
            return base.ExpandPath(path);
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
        /// Contain an extra parameters to be used for the -GetItem guy
        /// </summary>
        public class GetItemExtraParameters
        { }

        #endregion

        #region Container Overrides

        protected override void CopyItem(string path, string copyPath, bool recurse)
        {
            base.CopyItem(path, copyPath, recurse);
        }

        /// <summary>
        /// Return a list of child items in the store.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recurse"></param>
        /// <remarks>
        /// PROOF has no child items below the top level, so if there is anything below we can just return.
        /// </remarks>
        protected override void GetChildItems(string path, bool recurse)
        {
            //
            // Only top level for us!
            //

            if (path != "")
                return;

            //
            // Get a list of the dataset names
            //

            foreach (var item in PROOFDrive.GetDSItems())
            {
                WriteItemObject(item, item.Name, false);
            }
        }

        protected override void GetChildNames(string path, ReturnContainers returnContainers)
        {
            base.GetChildNames(path, returnContainers);
        }

        protected override void NewItem(string path, string itemTypeName, object newItemValue)
        {
            base.NewItem(path, itemTypeName, newItemValue);
        }

        protected override void RemoveItem(string path, bool recurse)
        {
            base.RemoveItem(path, recurse);
        }

        protected override void RenameItem(string path, string newName)
        {
            base.RenameItem(path, newName);
        }

        protected override bool HasChildItems(string path)
        {
            return base.HasChildItems(path);
        }

        protected override bool ConvertPath(string path, string filter, ref string updatedPath, ref string updatedFilter)
        {
            return base.ConvertPath(path, filter, ref updatedPath, ref updatedFilter);
        }

        #endregion
    }
}
