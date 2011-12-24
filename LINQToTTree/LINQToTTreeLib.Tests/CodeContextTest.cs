// <copyright file="CodeContextTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Linq.Expressions;
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
        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public CodeContext Constructor()
        {
            CodeContext target = new CodeContext();
            return target;
            // TODO: add assertions to method CodeContextTest.Constructor()
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
    }
}
