using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TTreeParser.Tests
{


    /// <summary>
    ///This is a test class for UtilsTest and is intended
    ///to contain all UtilsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UtilsTest
    {
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

        [TestMethod]
        public void TestIniFile()
        {
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.WriteLine("[Section1]");
            writer.WriteLine("Line 1");
            writer.WriteLine("");
            writer.WriteLine("[Section2]");
            writer.WriteLine("hi");
            writer.Flush();

            ms.Position = 0;
            var reader = new StreamReader(ms);
            ms.Position = 0;

            var results = reader.ParseINIFormat();

            Assert.IsNotNull(results, "results");
            Assert.AreEqual(2, results.Keys.Count, "# of sections");
            Assert.IsTrue(results.ContainsKey("Section1"), "section 1 key missing");
            Assert.IsTrue(results.ContainsKey("Section2"), "section 2 key missing");

            Assert.AreEqual(2, results["Section1"].Length, "# of lines in section 1");
            Assert.AreEqual("Line 1", results["Section1"][0], "#1 line of section 1");
            Assert.AreEqual("", results["Section1"][1], "#2 line of section 1");

            Assert.AreEqual(1, results["Section2"].Length, "# of lines in section 2");
            Assert.AreEqual("hi", results["Section2"][0], "#1 line of section 2");

        }

        [TestMethod]
        public void TestSanitizedName()
        {
            var t = new ROOTNET.NTTree("dork", "Fork");
            Assert.AreEqual("dork", t.SanitizedName(), "plain tree");

            t = new ROOTNET.NTTree("##Shapes", "dude");
            Assert.IsTrue(CleanName(t.SanitizedName()), "hashes: " + t.Name);
        }

        /// <summary>
        /// return false if the name isn't clean!
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool CleanName(string p)
        {
            if (p.Contains("."))
                return false;
            if (p.Contains("#"))
                return false;
            return true;
        }
    }
}
