﻿using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.QueryVisitors;
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
            public string[] IncludeFiles()
            {
                return null;
            }

            /// <summary>
            /// Return the lines of code
            /// </summary>
            /// <param name="methodName"></param>
            /// <returns></returns>
            public IEnumerable<string> LinesOfCode(string methodName)
            {
                Assert.AreEqual("MultBy2", methodName);
                yield return "MultBy2 = i*2;";
            }

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
        /// Holder class for the test methods.
        /// </summary>
        class MyModifiableCode : IOnTheFlyCPPObject
        {
            public string[] Includes = null;

            public string[] IncludeFiles()
            {
                return Includes;
            }

            public string[] LOC = new string[0];

            /// <summary>
            /// Return the lines of code
            /// </summary>
            /// <param name="methodName"></param>
            /// <returns></returns>
            public IEnumerable<string> LinesOfCode(string methodName)
            {
                Assert.AreEqual("MultBy2", methodName);
                return LOC;
            }

            /// <summary>
            /// Do a simple times 2
            /// </summary>
            /// <param name="i"></param>
            /// <returns></returns>
            public int MultBy2(int i)
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

        [TestMethod]
        public void CacheWhenCPPChanges()
        {
            // Make sure that when the C++ code changes, the cache key for lookup of results will also change.
            var q = new QueriableDummy<ntup>();
            var mym = new MyModifiableCode();
            mym.LOC = new string[] { "int i = 0;", "MultBy2 = 10;" };
            var i = q.Select(e => mym.MultBy2(e.run)).Where(x => x > 2).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Set the LOC to something.
            var str1 = FormattingQueryVisitor.Format(query);
            mym.LOC = new string[] { "int j = 10;", "MultBy2 = 10;" };
            var str2 = FormattingQueryVisitor.Format(query);
            Console.WriteLine(str1);
            Console.WriteLine(str2);
            Assert.AreNotEqual(str1, str2);
        }

        [TestMethod]
        public void CacheWhenIncludesChange()
        {
            // Make sure that when the C++ code changes, the cache key for lookup of results will also change.
            var q = new QueriableDummy<ntup>();
            var mym = new MyModifiableCode();
            mym.LOC = new string[] { "int i = 0;", "MultBy2 = 10;" };
            var i = q.Select(e => mym.MultBy2(e.run)).Where(x => x > 2).Count();
            var query = DummyQueryExectuor.LastQueryModel;

            // Set the LOC to something.
            mym.Includes = new string[] { "TMVAReader.h" };
            var str1 = FormattingQueryVisitor.Format(query);
            mym.Includes = new string[] { "TFile.h" };
            var str2 = FormattingQueryVisitor.Format(query);
            Console.WriteLine(str1);
            Console.WriteLine(str2);
            Assert.AreNotEqual(str1, str2);
        }
    }
}
