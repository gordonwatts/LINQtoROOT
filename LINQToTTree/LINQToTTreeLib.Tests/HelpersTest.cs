using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Tests
{
    [TestClass]
    public class HelpersTest
    {
        [TestInitialize]
        public void Setup()
        {
            TestUtils.ResetLINQLibrary();
        }

        [TestCleanup]
        public void Cleanup()
        {
            MEFUtilities.MyClassDone();
        }

        [TestMethod]
        public void PrettyPrintSimpleQuery()
        {
            var q = new QueriableDummy<ntup>();

            var r = q.Select(rq => rq.run * 2).PrettyPrintQuery();
            Console.WriteLine(r);
            Assert.IsTrue(r.Contains("select"));
            Assert.IsTrue(r.Contains("[rq].run * 2"));
        }

        [TestMethod]
        public void PrettyPrintComplexQueryI()
        {
            var q = new QueriableDummy<dummyntup>();

            var r = from e in q
                    from s in e.valC1D
                    where s > 22
                    select s * 2;

            var sr = r.PrettyPrintQuery();
            Console.WriteLine(sr);
            Assert.AreEqual(4, CountLines(sr));
        }

        [TestMethod]
        public void PrettyPrintComplexQueryII()
        {
            var q = new QueriableDummy<dummyntup>();

            var r = from e in q
                    let rf = (from s in e.valC1D where s > 22 select s * 2).Sum()
                    where rf > 20
                    select e.run;

            var sr = r.PrettyPrintQuery();
            Console.WriteLine(sr);
        }

        [TestMethod]
        public void PrettyPrintHideTuples()
        {
            var q = new QueriableDummy<ntup>();

            var r = q.Select(rq => Tuple.Create(rq.run * 2, 1)).Select(rq => rq.Item1).PrettyPrintQuery();
            Console.WriteLine(r);
            Assert.IsFalse(r.Contains("Tuple"));
            Assert.IsTrue(r.Contains("[rq].run * 2"));

        }

        /// <summary>
        /// Count the number of lines.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private int CountLines (string text)
        {
            return text.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).Count();
        }
    }
}
