// <copyright file="CodeContextTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for CodeContext</summary>
    [PexClass(typeof(CodeContext))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class CodeContextTest
    {
        /// <summary>Test stub for Add(String, IValue)</summary>
        [PexMethod]
        public void Add(
            [PexAssumeUnderTest]CodeContext target,
            string varName,
            IValue replacementName
        )
        {
            target.Add(varName, replacementName);
            // TODO: add assertions to method CodeContextTest.Add(CodeContext, String, IValue)
        }

        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public CodeContext Constructor()
        {
            CodeContext target = new CodeContext();
            return target;
            // TODO: add assertions to method CodeContextTest.Constructor()
        }

        /// <summary>Test stub for GetReplacement(String, Type)</summary>
        [PexMethod]
        public IValue GetReplacement(
            [PexAssumeUnderTest]CodeContext target,
            string varname,
            Type type
        )
        {
            IValue result = target.GetReplacement(varname, type);
            return result;
            // TODO: add assertions to method CodeContextTest.GetReplacement(CodeContext, String, Type)
        }

        [PexMethod]
        [PexAssertReachEventually("all", StopWhenAllReached=true)]
        public IValue TestRoundTrip([PexAssumeUnderTest]CodeContext target,
            string varName,
            [PexAssumeNotNull] string replName, 
            [PexAssumeNotNull] Type t)
        {
            var r1 = target.GetReplacement(varName, t);
            var replacement = new Variables.ValSimple(replName, t);
            target.Add(varName, replacement);
            var r2 = target.GetReplacement(varName, t);

            Assert.AreEqual(r1.RawValue, varName, "Incorrect null replacement looking for the name");
            Assert.AreEqual(r1.Type, t, "Incorrect created type");

            Assert.AreEqual(r2.RawValue, replName, "Incorrect cached variable");
            Assert.AreEqual(r2.Type, t, "Incorrect recalled type!");

            PexAssert.ReachEventually("all");

            return r2;
        }

        [TestMethod]
        public void OneRoundTripTest()
        {
            TestRoundTrip(new CodeContext(), "dude", "fork", typeof(int));
        }
    }
}
