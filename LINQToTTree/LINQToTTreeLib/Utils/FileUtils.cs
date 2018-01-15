using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LINQToTTreeLib.Utils
{
    /// <summary>
    /// Utilities to help deal with files we write out or read in. All extension methods.
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// Iterator to read the lines from a file.
        /// </summary>
        /// <param name="input">Existing file to read</param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateTextFile(this FileInfo input)
        {
            using (var reader = input.OpenText())
            {
                foreach (var l in reader.EnumerateLines())
                {
                    yield return l;
                }
            }
        }

        /// <summary>
        /// Given a stream reader, return one line at a time.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateLines (this TextReader reader)
        {
            var line = reader.ReadLine();
            while (line != null)
            {
                yield return line;
                line = reader.ReadLine();
            }
        }

        /// <summary>
        /// Return all parent directories, one after the other.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static IEnumerable<DirectoryInfo> AllParentDirectories(this DirectoryInfo dir, string possibleStub = null)
        {
            var initialDirectory = dir;
            while (dir.Parent != null)
            {
                yield return dir;
                if (possibleStub != null)
                {
                    yield return new DirectoryInfo(Path.Combine(dir.FullName, possibleStub));
                }
                dir = dir.Parent;
            }
        }
    }

}
