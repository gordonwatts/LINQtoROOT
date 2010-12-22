﻿using TTreeParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ROOTNET.Interface;
using System.Collections.Generic;
using System.Linq;

namespace TTreeParser.Tests
{
    
    
    /// <summary>
    ///This is a test class for UtilsTest and is intended
    ///to contain all UtilsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UtilsTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        class FileCreator : IDisposable
        {
            ROOTNET.NTFile _file = null;
            public FileCreator(string name)
            {
                _file = new ROOTNET.NTFile(name, "RECREATE");
            }

            int object_index = 0;

            /// <summary>
            /// Add some number of objects into the current file
            /// </summary>
            /// <typeparam name="T"></typeparam>
            public void AddObject<T>(int count = 1)
                where T : ROOTNET.Interface.NTObject, new()
            {
                for (int i = 0; i < count; i++)
                {
                    var obj = new T();
                    obj.Write("obj_" + object_index.ToString());
                    object_index += 1;
                }
            }

            /// <summary>
            /// Get rid of it all!
            /// </summary>
            public void Dispose()
            {
                _file.Write();
                _file.Close();
            }
        }

        [TestMethod]
        public void TestForNoObjects()
        {
            using (var f = new FileCreator("TestForNoObjects_empty.root"))
            {
            }
            var fin = new ROOTNET.NTFile("TestForNoObjects_empty.root", "READ");
            Assert.AreEqual(0, Utils.FindAllOfType<ROOTNET.Interface.NTObject>(fin).Count(), "was an empty file");
            fin.Close();

            using (var f = new FileCreator("TestForNoObjects_wrong.root"))
            {
                f.AddObject<ROOTNET.NTObject>(5);
            }
            fin = new ROOTNET.NTFile("TestForNoObjects_wrong.root", "READ");
            Assert.AreEqual(0, Utils.FindAllOfType<ROOTNET.Interface.NTH1F>(fin).Count(), "was a file with incorrect objects file");
            fin.Close();
        }

        [TestMethod]
        public void TestForSomeObjects()
        {
            using (var f = new FileCreator("TestForSomeObjects_only.root"))
            {
                f.AddObject<ROOTNET.NTObject>(5);
            }
            var fin = new ROOTNET.NTFile("TestForSomeObjects_only.root", "READ");
            Assert.AreEqual(5, Utils.FindAllOfType<ROOTNET.Interface.NTObject>(fin).Count(), "was just the right types in it");
            fin.Close();

            using (var f = new FileCreator("TestForSomeObjects_combo.root"))
            {
                f.AddObject<ROOTNET.NTObject>(5);
                f.AddObject<ROOTNET.NTH1F>(5);
            }
            fin = new ROOTNET.NTFile("TestForSomeObjects_combo.root", "READ");
            Assert.AreEqual(10, Utils.FindAllOfType<ROOTNET.Interface.NTObject>(fin).Count(), "was just the right types in it");
            Assert.AreEqual(5, Utils.FindAllOfType<ROOTNET.Interface.NTH1F>(fin).Count(), "was just the mostly the types in it");
            fin.Close();
        }

        /// <summary>
        /// Do basic testing here!
        /// </summary>
        /// <param name="numberOfItems"></param>
        /// <param name="numberofNulls"></param>
        public void DoEnumerableTest(int numberOfItems, ROOTNET.Interface.NTList list)
        {
            for (int i = 0; i < numberOfItems; i++)
            {
                list.Add(new ROOTNET.NTObject());
            }

            int countedItems = list.AsEnumerable().Where(n => n != null).Count();
            int countedNulls = list.AsEnumerable().Where(n => n == null).Count();

            Assert.AreEqual(numberOfItems, countedItems, "# of items not right");
            Assert.AreEqual(0, countedNulls, "# of nulls not right!");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AsEnumerableTestNull()
        {
            DoEnumerableTest(0, null);
        }

        [TestMethod]
        public void AsEnumerableTestItems()
        {
            DoEnumerableTest(10, new ROOTNET.NTList());
            DoEnumerableTest(0, new ROOTNET.NTList());
        }
    }
}
