
namespace TTreeDataModel
{
    /// <summary>
    /// Info that is needed to write out info about a ntuple. Basically, the list
    /// of classes and any C++ files that need to be loaded or compiled.
    /// </summary>
    public class NtupleTreeInfo
    {
        /// <summary>
        /// The list of classes that will need to be used
        /// </summary>
        public ROOTClassShell[] Classes;

        /// <summary>
        /// List of the files required to run this guy
        /// </summary>
        public string[] ClassImplimintationFiles;
    }
}
