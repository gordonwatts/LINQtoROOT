﻿using System;
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
    }
}
