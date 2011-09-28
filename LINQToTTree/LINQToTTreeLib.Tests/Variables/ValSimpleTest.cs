// <copyright file="ValSimpleTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Variables
{
    /// <summary>This class contains parameterized unit tests for ValSimple</summary>
    [PexClass(typeof(ValSimple))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class ValSimpleTest
    {
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
    }
}
