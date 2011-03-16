// <copyright file="StatementLoopOnVectorTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Statements
{
    /// <summary>This class contains parameterized unit tests for StatementLoopOnVector</summary>
    [PexClass(typeof(StatementLoopOnVector))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class StatementLoopOnVectorTest
    {
        /// <summary>Test stub for CodeItUp()</summary>
        [PexMethod]
        public IEnumerable<string> CodeItUp([PexAssumeUnderTest]StatementLoopOnVector target)
        {
            IEnumerable<string> result = target.CodeItUp();
            return result;
            // TODO: add assertions to method StatementLoopOnVectorTest.CodeItUp(StatementLoopOnVector)
        }

        /// <summary>Test stub for .ctor(IValue, String)</summary>
        [PexMethod]
        public StatementLoopOnVector Constructor(IValue arrayToIterateOver, string iteratorVarName)
        {
            StatementLoopOnVector target
               = new StatementLoopOnVector(arrayToIterateOver, iteratorVarName);
            return target;
            // TODO: add assertions to method StatementLoopOnVectorTest.Constructor(IValue, String)
        }

        [TestMethod]
        public void TestEmptyStatements()
        {
            var st = new StatementLoopOnVector(new Variables.ValSimple("dude", typeof(IEnumerable<string>)), "fork");
            var result = st.CodeItUp().ToArray();
            Assert.AreEqual(0, result.Length, "should have empty statements");
        }

        [TestMethod]
        public void TestForWithStatements()
        {
            var st = new StatementLoopOnVector(new Variables.ValSimple("dude", typeof(IEnumerable<string>)), "fork");
            st.Add(new StatementSimpleStatement("dude"));
            var result = st.CodeItUp().ToArray();
            Assert.AreEqual(4, result.Length, "should have empty statements");
            Assert.AreEqual("for (int fork=0; fork < dude->size(); fork++)", result[0], "for statement incorrect");
        }

        [TestMethod]
        public void TestVectorReferenceWorks()
        {
            /// We assume a pointer-to-the-vector, so, it should come back...

            var st = new StatementLoopOnVector(new Variables.ValSimple("dude", typeof(IEnumerable<int>)), "fork");

            Assert.AreEqual("(dude)[fork]", st.ObjectReference.RawValue, "iterator isn't right");
            Assert.AreEqual(typeof(int), st.ObjectReference.Type, "type for object reference isn't correct");
        }

        [TestMethod]
        public void TestVectorReferenceWorksArrayType()
        {
            /// We assume a pointer-to-the-vector, so, it should come back...

            var st = new StatementLoopOnVector(new Variables.ValSimple("dude", typeof(int[])), "fork");

            Assert.AreEqual("(dude)[fork]", st.ObjectReference.RawValue, "iterator isn't right");
            Assert.AreEqual(typeof(int), st.ObjectReference.Type, "type for object reference isn't correct");
        }
    }
}
