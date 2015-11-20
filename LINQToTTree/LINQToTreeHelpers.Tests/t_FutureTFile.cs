using LINQToTreeHelpers.FutureUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTreeHelpers.Tests
{
    [TestClass]
    public class t_FutureTFile
    {
        [TestMethod]
        public void FutureFileEmpty()
        {
            // Create an empty TFile, make sure that is good!
            var f = new FileInfo("FutureFileEmpty.root");
            if (f.Exists)
            {
                f.Delete();
            }

            var ftf = new FutureTFile(f);
            ftf.Write();
            ftf.Close();

            f.Refresh();
            Assert.IsTrue(f.Exists);
        }

        [TestMethod]
        public void FutureFileEmptyInUsing()
        {
            var f = new FileInfo("FutureFileWriteHisto.root");
            if (f.Exists)
            {
                f.Delete();
            }

            using (var ftf = new FutureTFile(f))
            {

            }

            f.Refresh();
            Assert.IsTrue(f.Exists);
        }

        [TestMethod]
        public void FutureFileWriteWithClose()
        {
            var f = new FileInfo("FutureFileWriteHisto.root");
            if (f.Exists)
            {
                f.Delete();
            }

            using (var ftf = new FutureTFile(f))
            {
                ftf.Close();
            }

            f.Refresh();
            Assert.IsTrue(f.Exists);
        }
    }
}
