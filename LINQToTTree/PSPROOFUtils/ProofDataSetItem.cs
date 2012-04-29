using System.Collections.Generic;
using System.Linq;
namespace PSPROOFUtils
{
    public class ProofDataSetItem
    {
        /// <summary>
        /// Get the name of this dataset.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Create a new dataset with only the name info valid.
        /// </summary>
        /// <param name="name"></param>
        public ProofDataSetItem(string name, long nFiles, long nCorruptFiles, long nStagedFiles, long totalSize)
        {
            Name = name;
            NumberFiles = nFiles;
            NumberCorruptFiles = nCorruptFiles;
            NumberStagedFiles = nStagedFiles;
            TotalSizeBytes = totalSize;

            //
            // Other initalization.
            //

            InformationComplete = false;
        }

        /// <summary>
        /// Is the information we are storing complete or not?
        /// </summary>
        internal bool InformationComplete { get; private set; }

        /// <summary>
        /// A list of all files we know about.
        /// </summary>
        private ROOTNET.Interface.NTFileInfo[] _files;

        /// <summary>
        /// Save the full data info for this dataset.
        /// </summary>
        /// <param name="files"></param>
        internal void SetFullData(System.Collections.Generic.IEnumerable<ROOTNET.Interface.NTFileInfo> files)
        {
            InformationComplete = true;
            _files = files.ToArray();
        }

        /// <summary>
        /// Enumerate through the list of files.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<ROOTNET.Interface.NTFileInfo> GetFileInfoEnumerator()
        {
            return _files;
        }

        /// <summary>
        /// Total number of files in the dataset
        /// </summary>
        public long NumberFiles { get; private set; }

        /// <summary>
        /// Total number of corrupt files in the dataset
        /// </summary>
        public long NumberCorruptFiles { get; private set; }

        /// <summary>
        /// Total number of staged files in the dataset.
        /// </summary>
        public long NumberStagedFiles { get; private set; }

        /// <summary>
        /// Total size in bytes of the dataset.
        /// </summary>
        public long TotalSizeBytes { get; private set; }
    }
}
