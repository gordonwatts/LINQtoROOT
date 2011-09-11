// <copyright file="CodeContextTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Linq.Expressions;
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
        public void AddWithPop([PexAssumeUnderTest]CodeContext target,
           [PexAssumeNotNull] string varName,
            [PexAssumeNotNull]IValue replacement1,
            [PexAssumeNotNull]IValue replacement2)
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

        [TestMethod]
        public void TestPopOfNothing()
        {
            var replacement1 = new Variables.ValSimple("freakout", typeof(int));
            var cc = new CodeContext();
            var popper = cc.Add("dude", replacement1);
            Assert.AreEqual(1, cc.NumberOfParams, "Incorrect # after insertion");
            popper.Pop();
            Assert.AreEqual(0, cc.NumberOfParams, "After pop, expected everything to be gone");
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

        [TestMethod]
        public void TestLoopVarCTor()
        {
            var c = new CodeContext();
            Assert.IsNull(c.LoopVariable, "ctor isn't null");
        }

        [TestMethod]
        public void TestLoopVarSetting()
        {
            var c = new CodeContext();
            var v = Expression.Variable(typeof(int), "d");
            c.SetLoopVariable(v, null);
            Assert.AreEqual(v, c.LoopVariable, "set didn't work");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestLoopVarSetNull()
        {
            var c = new CodeContext();
            c.SetLoopVariable(null, null);
        }

        [TestMethod]
        public void TestSetExpressionAndPop()
        {
            var c = new CodeContext();
            Assert.IsNull(c.GetReplacement("d"), "initally get");
            var myvar = Expression.Variable(typeof(int), "fork");
            var p = c.Add("d", myvar);
            Assert.AreEqual(myvar, c.GetReplacement("d"), "callback after store");
            p.Pop();
            Assert.IsNull(c.GetReplacement("d"), "Final get");
        }

        [TestMethod]
        public void TestExpressionHiding()
        {
            var c = new CodeContext();
            var myvar1 = Expression.Variable(typeof(int), "d");
            var myvar2 = Expression.Variable(typeof(float), "dude");

            c.Add("p", myvar1);
            var p = c.Add("p", myvar2);
            Assert.AreEqual(myvar2, c.GetReplacement("p"), "replacement check");
            p.Pop();
            Assert.AreEqual(myvar1, c.GetReplacement("p"), "poped state");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddNullExpression()
        {
            var c = new CodeContext();
            c.Add("dude", (Expression)null);
        }

        [TestMethod]
        public void TestRemoveAndPopRemove()
        {
            var c = new CodeContext();
            var myvar = Expression.Variable(typeof(int), "d");
            c.Add("dude", myvar);

            var popper = c.Remove("dude");
            Assert.IsNull(c.GetReplacement("dude"), "incorrect dummy name");
            popper.Pop();
            Assert.AreEqual("d", (c.GetReplacement("dude") as ParameterExpression).Name, "incorrect dummy name");
        }

        [TestMethod]
        public void TestRemoveOfNothing()
        {
            var c = new CodeContext();
            var popper = c.Remove("dude");
            popper.Pop();
        }

        [TestMethod]
        public void TestPopAcrossDomainsExpression()
        {
            var c = new CodeContext();
            var myvar = new Variables.ValSimple("hi", typeof(int));
            c.Add("dude", myvar);
            var popper = c.Add("dude", Expression.Variable(typeof(int), "dude"));

            Assert.AreEqual("hi", c.GetReplacement("dude", typeof(int)).RawValue, "dude not there");
            popper.Pop();
            Assert.AreEqual("hi", c.GetReplacement("dude", typeof(int)).RawValue, "dude not there");
        }

        [TestMethod]
        public void TestPopAcrossDomainsValue()
        {
            var c = new CodeContext();
            var myvar = new Variables.ValSimple("hi", typeof(int));
            c.Add("dude", Expression.Variable(typeof(int), "dude"));
            var popper = c.Add("dude", myvar);

            Assert.IsNotNull(c.GetReplacement("dude"), "dude not there");
            popper.Pop();
            Assert.IsNotNull(c.GetReplacement("dude"), "dude not there");
        }
    }
}
