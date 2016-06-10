using System;
using System.Collections.Generic;
using LINQToTTreeLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for TypeUtilsTest and is intended
    ///to contain all TypeUtilsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TypeUtilsTest
    {
        [TestMethod]
        public void TestUniqueVarNameForMap()
        {
            var name = typeof(Dictionary<int, double>).CreateUniqueVariableName();
            Console.WriteLine(name);
            Assert.IsFalse(name.Contains("`"), "Name contains a `");
        }

        [TestMethod]
        public void TestUniqueVarNameForArray()
        {
            var name = typeof(int[]).CreateUniqueVariableName();
            Console.WriteLine(name);
            Assert.IsFalse(name.Contains("["), "Name contains a [");
        }

        [TestMethod]
        public void NameSimple()
        {
            Assert.AreEqual("Int32", typeof(int).FullyQualifiedName());
        }

        [TestMethod]
        public void NameGeneric()
        {
            Assert.AreEqual("IEnumerable<Int32>", typeof(IEnumerable<int>).FullyQualifiedName());
        }

        [TestMethod]
        public void NameNestedeneric()
        {
            Assert.AreEqual("IEnumerable<IList<Int32>>", typeof(IEnumerable<IList<int>>).FullyQualifiedName());
        }
    }
}
