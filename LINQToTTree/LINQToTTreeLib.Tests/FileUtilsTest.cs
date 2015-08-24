using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Utils
{
    [TestClass]
    public partial class FileUtilsTest
    {
        public TextWriter WriteTextIfNotDuplicate(FileInfo outputFile)
        {
            TextWriter result = FileUtils.WriteTextIfNotDuplicate(outputFile);
            return result;
        }

        public void MakeSureFileIsRight(string textToWrite)
        {
            FileInfo f = new FileInfo("MakeSureFileIsRight.txt");
            if (f.Exists)
            {
                f.Delete();
            }

            using (TextWriter writer = f.WriteTextIfNotDuplicate())
            {
                writer.Write(textToWrite);
                writer.Close();
            }

            f.Refresh();
            Assert.IsTrue(f.Exists);

            using (TextReader reader = f.OpenText())
            {
                if (textToWrite == null)
                {
                    textToWrite = "";
                }
                Assert.AreEqual(textToWrite, reader.ReadToEnd(), "Incorrect text came back");
            }
        }

        public void MakeSureFileNotUpdated(string s1, string s2)
        {
            string compareS1 = s1;
            string compareS2 = s2;

            if (compareS1 == null)
                compareS1 = "";
            if (compareS2 == null)
                compareS2 = "";

            FileInfo f = new FileInfo("MakeSureFileNotUpdated.txt");
            if (f.Exists)
            {
                f.Delete();
            }

            using (TextWriter writer = f.WriteTextIfNotDuplicate())
            {
                writer.Write(s1);
                writer.Close();
            }

            f.Refresh();
            Assert.IsTrue(f.Exists);

            ///
            /// Now, knock back an hour the time it takes to actually run this guy
            /// 

            f.LastWriteTime = DateTime.Now - TimeSpan.FromDays(1);
            f.Refresh();
            Assert.IsTrue((DateTime.Now - f.LastWriteTime).TotalDays > 0.8, "Resetting last modified time didn't work");

            ///
            /// Next, do the write of string 2
            /// 

            using (TextWriter writer = f.WriteTextIfNotDuplicate())
            {
                writer.Write(s2);
                writer.Close();
            }

            ///
            /// Now, exactly what happens depends if s1 and s2 are equal or not!
            /// 

            f.Refresh();
            var delta = (DateTime.Now - f.LastWriteTime).TotalDays;
            if (compareS1 == compareS2)
            {
                Assert.IsTrue(delta > 0.8, "The strings are the same, but file shouldn't update");
            }
            else
            {
                Assert.IsTrue(delta < 0.1, "The strings aren't the same, but the file should have updated!");
            }

            using (TextReader reader = f.OpenText())
            {
                Assert.AreEqual(compareS2, reader.ReadToEnd(), "Incorrect text came back");
            }

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WriteTextIfNotDuplicateThrowsArgumentNullException466()
        {
            TextWriter textWriter;
            textWriter = this.WriteTextIfNotDuplicate((FileInfo)null);
            textWriter.Dispose();
        }
    }
}
