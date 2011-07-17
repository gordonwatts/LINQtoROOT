using System.IO;

namespace LINQToTreeHelpers.FutureUtils
{
    /// <summary>
    /// Future TFile - really just a normal TFile, but gets written out in the future...
    /// </summary>
    public class FutureTFile : FutureTDirectory
    {
        private static ROOTNET.Interface.NTFile CreateOpenFile(string name)
        {
            var f = ROOTNET.NTFile.Open(name, "RECREATE");
            return f;
        }

        /// <summary>
        /// Creates a new ROOT file and attaches a future value container to it. This container
        /// can be used to store future values that get evaluated at a later time.
        /// </summary>
        /// <param name="outputRootFile"></param>
        public FutureTFile(FileInfo outputRootFile)
            : base(CreateOpenFile(outputRootFile.FullName))
        {
        }

        /// <summary>
        /// Creates a new ROOT file and attaches a future value container to it. This container
        /// can be used to store future values that get evaluated at a later time.
        /// </summary>
        /// <param name="outputRootFile"></param>
        public FutureTFile(string outputRootFile)
            : base(CreateOpenFile(outputRootFile))
        {
        }

        /// <summary>
        /// Close this file. Resolves all futures, writes the directories, and closes the file
        /// </summary>
        public void Close()
        {
            //
            // Write out this guy and close it!
            // 

            Write();
            Directory.Close();
        }
    }
}
