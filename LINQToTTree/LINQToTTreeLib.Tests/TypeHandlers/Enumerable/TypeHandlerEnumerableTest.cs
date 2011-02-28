// <copyright file="TypeHandlerEnumerableTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.TypeHandlers.Enumerable
{
    /// <summary>This class contains parameterized unit tests for TypeHandlerEnumerable</summary>
    [PexClass(typeof(TypeHandlerEnumerable))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class TypeHandlerEnumerableTest
    {
        /// <summary>Test stub for CanHandle(Type)</summary>
        [PexMethod]
        internal bool CanHandle([PexAssumeUnderTest]TypeHandlerEnumerable target, Type t)
        {
            bool result = target.CanHandle(t);
            return result;
            // TODO: add assertions to method TypeHandlerEnumerableTest.CanHandle(TypeHandlerEnumerable, Type)
        }

        /// <summary>Test stub for ProcessConstantReference(ConstantExpression, IGeneratedCode)</summary>
        [PexMethod]
        internal IValue ProcessConstantReference(
            [PexAssumeUnderTest]TypeHandlerEnumerable target,
            ConstantExpression expr,
            IGeneratedCode codeEnv
        )
        {
            IValue result = target.ProcessConstantReference(expr, codeEnv);
            return result;
            // TODO: add assertions to method TypeHandlerEnumerableTest.ProcessConstantReference(TypeHandlerEnumerable, ConstantExpression, IGeneratedCode)
        }

        /// <summary>Test stub for ProcessMethodCall(MethodCallExpression, IValue&amp;, IGeneratedCode, ICodeContext)</summary>
        [PexMethod]
        internal Expression ProcessMethodCall(
            [PexAssumeUnderTest]TypeHandlerEnumerable target,
            MethodCallExpression expr,
            out IValue result,
            IGeneratedCode gc,
            ICodeContext context
        )
        {
            Expression result01 = target.ProcessMethodCall(expr, out result, gc, context, null);
            return result01;
            // TODO: add assertions to method TypeHandlerEnumerableTest.ProcessMethodCall(TypeHandlerEnumerable, MethodCallExpression, IValue&, IGeneratedCode, ICodeContext)
        }

        [TestMethod]
        public void TestEnumerableGoodType()
        {
            Assert.IsTrue(CanHandle(new TypeHandlerEnumerable(), typeof(System.Linq.Enumerable)), "can deal with enumerable!");
            Assert.IsFalse(CanHandle(new TypeHandlerEnumerable(), typeof(int)), "Should not deal with int!");
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestBadConstReference()
        {
            var gc = new GeneratedCode();
            ProcessConstantReference(new TypeHandlerEnumerable(), Expression.Constant(10), gc);
        }

        [TestMethod]
        public void TestSimpleCount()
        {
            var cgmethod = (from m in typeof(System.Linq.Enumerable).GetMethods()
                            where m.Name == "Count" && m.GetParameters().Count() == 1
                            select m).First();
            var cmethod = cgmethod.MakeGenericMethod(new Type[] { typeof(int) });

            /// The result that we are going to get back
            IValue result;

            /// Where we expect code to show up
            var gc = new GeneratedCode();
            var context = new CodeContext();

            int[] myArray = new int[] { 0, 1, 2, 3 };

            var call = Expression.Call(cmethod, Expression.Variable(typeof(int[]), "dude"));

            context.Add("dude", new Variables.ValEnumerableVector("fork", typeof(int[])));

            ProcessMethodCall(new TypeHandlerEnumerable(), call, out result, gc, context);

            Assert.AreEqual(typeof(int), result.Type, "bad type coming back");
            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "only 1 statement expected");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.First(), typeof(Statements.StatementLoopOnVector), "bad loop type");

            var statements = gc.CodeBody;
            Assert.IsNotNull(statements, "That was odd");
            Assert.AreEqual(1, statements.DeclaredVariables.Count(), "Expected the variable iterating to be declared!");

            Assert.Inconclusive("Make sure the scope has been popped so we don't put something inside the count!");
        }
    }
}
