// <copyright file="CodeContextTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using LinqToTTreeInterfacesLib;
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
            Assert.AreEqual(replacementName, target.GetReplacement(varName, replacementName.Type), "value didn't come out correctly");
            // TODO: add assertions to method CodeContextTest.Add(CodeContext, String, IValue)
        }

        /// <summary>
        /// Test for poping variables off and on.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="varName"></param>
        /// <param name="replacement1"></param>
        /// <param name="replacement2"></param>
        [PexMethod]
        public void AddWithPop([PexAssumeUnderTest]CodeContext target,
           [PexAssumeNotNull] string varName, [PexAssumeNotNull]IValue replacement1, [PexAssumeNotNull]IValue replacement2)
        {
            target.Add(varName, replacement1);
            Assert.AreEqual(replacement1, target.GetReplacement(varName, replacement1.Type), "value didn't come out correctly");
            var r = target.Add(varName, replacement2);
            Assert.AreEqual(replacement2, target.GetReplacement(varName, replacement2.Type), "value didn't come out correctly for 2nd replacement");
            r.Pop();
            Assert.AreEqual(replacement1, target.GetReplacement(varName, replacement1.Type), "pop didn't come out correctly");
        }

        [TestMethod]
        public void TestPop()
        {
            var replacement1 = new Variables.ValSimple("freakout", typeof(int));
            var replacement2 = new Variables.ValSimple("stuff", typeof(string));
            AddWithPop(new CodeContext(), "bogus", replacement1, replacement2);
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
        [PexAssertReachEventually("all", StopWhenAllReached = true)]
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
