using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.TypeHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Tests.TypeHandlers.CPPCode
{
    [TestClass]
    public class TypeHandlerOnTheFlyCPPTest
    {
        [TestInitialize]
        public void TestInit()
        {
            TestUtils.ResetLINQLibrary();
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        /// <summary>
        /// Holder class for the test methods.
        /// </summary>
        class MyCode : IOnTheFlyCPPObject
        {
            /// <summary>
            /// Do a simple times 2
            /// </summary>
            /// <param name="i"></param>
            /// <returns></returns>
            public int MultBy2 (int i)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Code that doesn't inherit, so it should be bad to call this.
        /// </summary>
        class MyBrokenCode
        {
            public int MultBy2 (int i)
            {
                throw new InvalidOperationException();
            }
        }

        [TestMethod]
        public void SimpleMultBy2()
        {
            // An end-to-end code test to see if we can generate code to multiply by 2.
            var q = new QueriableDummy<ntup>();

            var mym = new MyCode();
            var i = q.Select(e => mym.MultBy2(e.run)).Where(x => x > 2).Count();

            DummyQueryExectuor.FinalResult.DumpCodeToConsole();
            Assert.IsTrue(DummyQueryExectuor.FinalResult.DumpCode().Where(l => l.Contains("*2")).Any());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BadObjectMultBy2()
        {
            // An end-to-end code test to see if we can generate code to multiply by 2.
            var q = new QueriableDummy<ntup>();

            var mym = new MyBrokenCode();
            var i = q.Select(e => mym.MultBy2(e.run)).Where(x => x > 2).Count();
        }
    }
}
