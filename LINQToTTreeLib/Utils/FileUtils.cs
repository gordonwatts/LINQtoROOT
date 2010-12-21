using System.IO;
using System;

namespace LINQToTTreeLib.Utils
{
    /// <summary>
    /// Utilities to help deal with files we write out or read in. All extension methods.
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// Write out a text file. Only modify the file on disk if it is different
        /// from the file we are writing out. This means that the timestamp won't
        /// get updated unless new data is being written out.
        /// 
        /// This works only for "small" files that fit in memory.
        /// </summary>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        public static TextWriter WriteTextIfNotDuplicate(this FileInfo outputFile)
        {
            if (outputFile == null)
                throw new ArgumentNullException("outputFile must not be null when WriteTextIfNotDuplicate is called!");

            return new StreamWriter(new MyWriter(outputFile));
        }

        /// <summary>
        /// Deals with the actual writing
        /// </summary>
        private class MyWriter : MemoryStream
        {
            private FileInfo outputFile;

            public MyWriter(FileInfo outputFile)
            {
                this.outputFile = outputFile;
            }

            bool alreadyClosing = false;

            /// <summary>
            /// They have finished writing the file. Save all our internal data to an
            /// output file if it has changed from whatever is currently on disk.
            /// </summary>
            public override void Close()
            {
                ///
                /// Prevent a double close from going through b/c we have to re-read
                /// the stream from the start
                /// 

                if (alreadyClosing)
                {
                    return;
                }
                alreadyClosing = true;

                ///
                /// Protect against an exception leaving this is an unknown state - though I imagine
                /// if one occured there would be some serious problems here! :-)
                /// 

                try
                {
                    ///
                    /// First, get the complete contents of the file into a string for
                    /// easy comparison and use.
                    /// 

                    Seek(0, SeekOrigin.Begin);
                    string contentsOfFile;
                    using (var reader = new StreamReader(this))
                    {
                        contentsOfFile = reader.ReadToEnd();
                    }

                    ///
                    /// Next, we see what shape the output file is in
                    /// 

                    bool mustWrite = false;

                    outputFile.Refresh();
                    if (!mustWrite && !outputFile.Exists)
                    {
                        mustWrite = true;
                    }

                    if (!mustWrite)
                    {
                        using (var reader = outputFile.OpenText())
                        {
                            string currentContents = reader.ReadToEnd();
                            mustWrite = currentContents != contentsOfFile;
                        }
                    }

                    ///
                    /// Do it if need be!
                    /// 

                    if (mustWrite)
                    {
                        using (var writer = outputFile.CreateText())
                        {
                            writer.Write(contentsOfFile);
                        }
                    }

                    ///
                    /// Ok - now close off everything!
                    ///

                    base.Close();

                }
                finally
                {
                    alreadyClosing = false;
                }
            }
        }
    }
}
