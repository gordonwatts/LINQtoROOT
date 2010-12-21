// <copyright file="VarSimpleTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Variables
{
    /// <summary>This class contains parameterized unit tests for VarSimple</summary>
    [PexClass(typeof(VarSimple))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class VarSimpleTest
    {
        /// <summary>Test stub for .ctor(Type)</summary>
        [PexMethod]
        public VarSimple Constructor(Type type)
        {
            VarSimple target = new VarSimple(type);
            return target;
            // TODO: add assertions to method VarSimpleTest.Constructor(Type)
        }
    }
}
