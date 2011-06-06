using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private ROOTNET.Interface.NTFile _file;

        public FutureTFile(FileInfo outputRootFile)
            : base(CreateOpenFile(outputRootFile.FullName))
        {
        }

        /// <summary>
        /// Close this file. Resolves all futures, writes the directories, and closes the file
        /// </summary>
        public void Close()
        {
            ///
            /// Write out this guy and close it!
            /// 

            Write();
            Directory.Close();
        }
    }
}
