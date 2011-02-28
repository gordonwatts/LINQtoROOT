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

            gc.Add(new Statements.StatementSimpleStatement("dude"));
            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "current scope pointer is incorrectly set");
        }

        [TestMethod]
        public void TestSimpleWhere()
        {
            var wgmethod = (from m in typeof(System.Linq.Enumerable).GetMethods()
                            where m.Name == "Where" && m.GetParameters().Count() == 2
                            where m.GetParameters()[1].ParameterType.GetGenericArguments().Length == 2
                            select m).First();
            var wmethod = wgmethod.MakeGenericMethod(new Type[] { typeof(int) });

            /// The result that we are going to get back
            IValue result;

            /// Where we expect code to show up
            var gc = new GeneratedCode();
            var context = new CodeContext();

            int[] myArray = new int[] { 0, 1, 2, 3 };

            Expression<Func<int, bool>> lambda = a => a == 0;

            var call = Expression.Call(wmethod, Expression.Variable(typeof(int[]), "dude"), lambda);

            context.Add("dude", new Variables.ValEnumerableVector("fork", typeof(int[])));

            ProcessMethodCall(new TypeHandlerEnumerable(), call, out result, gc, context);

            /// There should be an outter loop

            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "only 1 statement expected");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.First(), typeof(Statements.StatementLoopOnVector), "bad loop type");
            var loop = gc.CodeBody.Statements.First() as Statements.StatementLoopOnVector;
            Assert.AreEqual(1, loop.Statements.Count(), "Incorrect # of statements in the loop");
            Assert.IsInstanceOfType(loop.Statements.First(), typeof(Statements.StatementFilter), "no filter statement?");
            var filter = loop.Statements.First() as Statements.StatementFilter;
            Assert.AreEqual(0, filter.Statements.Count(), "bad # of filter statements - shouldn't be any under it!");

            /// Check that the scope is correct.

            gc.Add(new Statements.StatementSimpleStatement("dude"));
            Assert.AreEqual(1, filter.Statements.Count(), "Scope dosen't seem to be pointed to the inner statement here!");

            /// Make sure the type that comes back is an array!

            Assert.IsInstanceOfType(result, typeof(ISequenceAccessor), "return is not a sequence");
        }
    }
}
