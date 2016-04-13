using System;
using System.IO;

namespace LINQToTreeHelpers.FutureUtils
{
    /// <summary>
    /// Future TFile - really just a normal TFile, but gets written out in the future...
    /// </summary>
    public class FutureTFile : FutureTDirectory, IDisposable
    {
        private static ROOTNET.Interface.NTFile CreateOpenFile(string name)
        {
            var oldDir = ROOTNET.NTDirectory.CurrentDirectory();
            try
            {
                var f = ROOTNET.NTFile.Open(name, "RECREATE");
                if (!f.IsOpen())
                {
                    throw new InvalidOperationException(string.Format("Unable to create file '{0}'. It could be the file is locked by another process (like ROOT!!??)", name));
                }
                return f;
            }
            finally
            {
                oldDir.cd();
            }
        }

        /// <summary>
        /// Creates a new ROOT file and attaches a future value container to it. This container
        /// can be used to store future values that get evaluated at a later time.
        /// </summary>
        /// <remarks>
        /// After this call your global directory will point back to Rint: - the base directory for
        /// all of root.
        /// </remarks>
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

            // Since we've shut it down, we don't need to do the dispose any longer.
            _disposed = true;
        }

        /// <summary>
        /// Finalizer that makes sure the class has been closed.
        /// </summary>
        ~FutureTFile()
        {
            Dispose();
        }

        /// <summary>
        /// Have we done a dispose yet?
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Release the file resouce (and write it out).
        /// </summary>
        public void Dispose()
        {
            if (!_disposed) {
                Close();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
