using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LINQToTTreeLib.ExecutionCommon
{
    /// <summary>
    /// Some utilities to help with running code and copying and compiling, etc.
    /// </summary>
    class ExecutionUtilities
    {
        /// <summary>
        /// Copies a source file to a directory. Also copies over any "valid" includes we can find.
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destDirectory"></param>
        public static FileInfo CopyToDirectory(FileInfo sourceFile, DirectoryInfo destDirectory)
        {
            ///
            /// See if the dest file is already there. If so, don't copy over
            /// 

            FileInfo destFile = new FileInfo(destDirectory.FullName + "\\" + sourceFile.Name);
            if (destFile.Exists)
            {
                if (destFile.LastWriteTime >= sourceFile.LastWriteTime
                    && destFile.Length == sourceFile.Length)
                {
                    return destFile;
                }
            }
            sourceFile.CopyTo(destFile.FullName, true);

            ///
            /// Next, if there are any include files we need to move
            /// 

            CopyIncludedFilesToDirectory(sourceFile, destDirectory);

            ///
            /// Return what we know!
            /// 

            destFile.Refresh();
            return destFile;
        }

        /// <summary>
        /// Copy over any files that are included in the source file to the destination
        /// directory
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destDirectory"></param>
        public static void CopyIncludedFilesToDirectory(FileInfo sourceFile, DirectoryInfo destDirectory)
        {
            var includeFiles = FindIncludeFiles(sourceFile);
            var goodIncludeFiles = from f in includeFiles
                                   where !Path.IsPathRooted(f)
                                   let full = new FileInfo(sourceFile.DirectoryName + "\\" + f)
                                   where full.Exists
                                   select full;

            foreach (var item in goodIncludeFiles)
            {
                CopyToDirectory(item, destDirectory);
            }
        }

        /// <summary>
        /// Return the include files that we find in this guy.
        /// </summary>
        /// <param name="_proxyFile"></param>
        /// <returns></returns>
        private static IEnumerable<string> FindIncludeFiles(FileInfo _proxyFile)
        {
            Regex reg = new Regex("#include \"(?<file>[^\"]+)\"");
            using (var reader = _proxyFile.OpenText())
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        continue;

                    var m = reg.Match(line);
                    if (m.Success)
                    {
                        var s = m.Groups["file"].Value;
                        yield return s;
                    }
                }
            }
        }

    }
}
