// <copyright file="ValSimpleTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Variables
{
    /// <summary>This class contains parameterized unit tests for ValSimple</summary>
    [TestClass]
    public partial class ValSimpleTest
    {
#if false
        /// <summary>Test stub for .ctor(String)</summary>
        [PexMethod]
        internal ValSimple Constructor(string v)
        {
            ValSimple target = new ValSimple(v, typeof(int));
            Assert.AreEqual(v, target.RawValue, "Should have been set to the same thing!");
            return target;
        }

        [PexMethod]
        internal ValSimple TestCTorWithType(string v, Type t)
        {
            ValSimple target = new ValSimple(v, t);
            Assert.AreEqual(v, target.RawValue, "Should have been set to the same thing!");
            Assert.IsNotNull(target.Type, "Expected some value for the type!");
            return target;
        }
#endif
        [TestMethod]
        public void RenameMethodCall()
        {
            var target = new ValSimple("(*aNTH1F_1233).Fill(((double)aInt32_326),1.0*((1.0*1.0)*1.0))", typeof(int));
            target.RenameRawValue("aInt32_326", "aInt32_37");
            Assert.AreEqual("(*aNTH1F_1233).Fill(((double)aInt32_37),1.0*((1.0*1.0)*1.0))", target.RawValue);
        }
    }
}
