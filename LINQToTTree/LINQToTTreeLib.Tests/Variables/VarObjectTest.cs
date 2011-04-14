// <copyright file="VarObjectTest.cs" company="Microsoft">Copyright � Microsoft 2010</copyright>
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Variables
{
    /// <summary>This class contains parameterized unit tests for VarObject</summary>
    [TestClass]
    public partial class VarObjectTest
    {
        /// <summary>Test stub for .ctor(Type)</summary>
        ///[PexMethod]
        internal VarObject Constructor(Type type)
        {
            VarObject target = new VarObject(type);
            return target;
            // TODO: add assertions to method VarObjectTest.Constructor(Type)
        }

        [TestMethod]
        public void TestForSimple()
        {
            var r = Constructor(typeof(int));
            Assert.IsTrue(r.RawValue.IndexOf("*") < 0, "There should be no pointer things in '" + r.RawValue + "'");
        }
    }
}
