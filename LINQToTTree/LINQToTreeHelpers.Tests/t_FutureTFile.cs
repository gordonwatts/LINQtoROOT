using LINQToTreeHelpers.FutureUtils;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOTNET.Interface;
using System.IO;
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
            var f = new FileInfo("FutureFileWriteWithClose.root");
            if (f.Exists)
            {
                f.Delete();
            }
            while (f.Exists)
            {
                f.Refresh();
            }

            using (var ftf = new FutureTFile(f))
            {
                ftf.Close();
            }

            f.Refresh();
            Assert.IsTrue(f.Exists);
        }

        [TestMethod]
        public void DoNotHoldRootLock()
        {
            var fl = new FileInfo("CommandLineCommonExecutor.root");
            using (var f = new FutureTFile(fl))
            {
                var h = new LockingFutureValue() as IFutureValue<NTObject>;
                h.Save(f);
            }

            // Make sure it was written out!
            var fr = ROOTNET.NTFile.Open(fl.FullName, "READ");
            try
            {
                var myh = fr.Get("hi") as NTH1F;
                Assert.IsNotNull(myh);
                Assert.AreEqual(1, (int)myh.GetEntries());
            } finally
            {
                fr.Close();
            }
        }

        public class LockingFutureValue : IFutureValue<NTObject>
        {
            public NTObject Value
            {
                get
                {
                    if (_histo == null)
                    {
                        GenerateValue().Wait();
                    }
                    return _histo;
                }
            }

            public bool HasValue => _histo != null;

            private ROOTNET.Interface.NTH1F _histo = null;

            async Task GenerateValue()
            {
                using (await ROOTLock.Lock.LockAsync())
                {
                    _histo = new ROOTNET.NTH1F("hi", "there", 10, 0.0, 100.0);
                    _histo.Fill(5.0);
                }
            }

            public Task GetAvailibleTask()
            {
                return GenerateValue();
            }
        }
    }
}
