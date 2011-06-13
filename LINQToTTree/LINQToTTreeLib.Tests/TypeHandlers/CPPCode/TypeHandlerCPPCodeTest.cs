// <copyright file="TypeHandlerCPPCodeTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.Utils;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.TypeHandlers.CPPCode
{
    /// <summary>This class contains parameterized unit tests for TypeHandlerCPPCode</summary>
    [PexClass(typeof(TypeHandlerCPPCode))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class TypeHandlerCPPCodeTest
    {
        /// <summary>Test stub for CanHandle(Type)</summary>
        [PexMethod]
        internal bool CanHandle([PexAssumeUnderTest]TypeHandlerCPPCode target, Type t)
        {
            bool result = target.CanHandle(t);

            var attr = t.TypeHasAttribute<CPPHelperClassAttribute>();
            Assert.AreEqual(attr != null, result, "type attribute not correct");

            return result;
        }

        static class FreeClass
        {
            public static int DoIt(int arg)
            {
                throw new NotImplementedException();
            }
        }

        [CPPHelperClass]
        static class DoItClass
        {
            [CPPCode(Code = new string[] { "DoIt = arg*2;" }, IncludeFiles = new string[] { "TLorentzVector.h" })]
            public static int DoIt(int arg)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void TestCanHandleNo()
        {
            CanHandle(new TypeHandlerCPPCode(), typeof(FreeClass));
        }
        [TestMethod]
        public void TestCanHandleYes()
        {
            CanHandle(new TypeHandlerCPPCode(), typeof(DoItClass));
        }

        /// <summary>Test stub for ProcessConstantReference(ConstantExpression, IGeneratedQueryCode, ICodeContext, CompositionContainer)</summary>
        [PexMethod]
        internal IValue ProcessConstantReference(
            [PexAssumeUnderTest]TypeHandlerCPPCode target,
            ConstantExpression expr,
            GeneratedCode codeEnv,
            CodeContext context
        )
        {
            IValue result
               = target.ProcessConstantReference(expr, codeEnv, context, MEFUtilities.MEFContainer);
            return result;
            // TODO: add assertions to method TypeHandlerCPPCodeTest.ProcessConstantReference(TypeHandlerCPPCode, ConstantExpression, IGeneratedQueryCode, ICodeContext, CompositionContainer)
        }

        [TestMethod]
        public void TestSimpleCodeAddon()
        {
            var target = new TypeHandlerCPPCode();
            var gc = new GeneratedCode();
            var context = new CodeContext();

            var param = Expression.Parameter(typeof(int), "p");
            var expr = Expression.Call(typeof(DoItClass).GetMethod("DoIt"), param);

            IValue result;

            target.ProcessMethodCall(expr, out result, gc, context, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            Assert.IsNotNull(result, "result!");
            var vname = result.RawValue;

            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "# of statements that came back");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.First(), typeof(Statements.StatementSimpleStatement), "statement type #1");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.Skip(1).First(), typeof(Statements.StatementSimpleStatement), "statement type #2");
            var st1 = gc.CodeBody.Statements.First() as Statements.StatementSimpleStatement;
            var st2 = gc.CodeBody.Statements.Skip(1).First() as Statements.StatementSimpleStatement;

            Assert.AreEqual("int " + vname, st1.Line, "line #1 is incorrect");

            var expected = new StringBuilder();
            expected.AppendFormat("{0} = p*2", vname);
            Assert.AreEqual(expected.ToString(), st2.Line, "statement line incorrect");

            Assert.AreEqual(1, gc.IncludeFiles.Count(), "# of include files");
            Assert.AreEqual("TLorentzVector.h", gc.IncludeFiles.First(), "include file name");
        }

        /// <summary>Test stub for ProcessMethodCall(MethodCallExpression, IValue&amp;, IGeneratedQueryCode, ICodeContext, CompositionContainer)</summary>
        [PexMethod]
        internal Expression ProcessMethodCall(
            [PexAssumeUnderTest]TypeHandlerCPPCode target,
            MethodCallExpression expr,
            out IValue result,
            IGeneratedQueryCode gc,
            ICodeContext context,
            CompositionContainer container
        )
        {
            Expression result01
               = target.ProcessMethodCall(expr, out result, gc, context, container);
            return result01;
            // TODO: add assertions to method TypeHandlerCPPCodeTest.ProcessMethodCall(TypeHandlerCPPCode, MethodCallExpression, IValue&, IGeneratedQueryCode, ICodeContext, CompositionContainer)
        }

        /// <summary>Test stub for ProcessNew(NewExpression, IValue&amp;, IGeneratedQueryCode, ICodeContext, CompositionContainer)</summary>
        [PexMethod]
        internal Expression ProcessNew(
            [PexAssumeUnderTest]TypeHandlerCPPCode target,
            NewExpression expression,
            out IValue result,
            IGeneratedQueryCode gc,
            ICodeContext context,
            CompositionContainer container
        )
        {
            Expression result01
               = target.ProcessNew(expression, out result, gc, context, container);
            return result01;
            // TODO: add assertions to method TypeHandlerCPPCodeTest.ProcessNew(TypeHandlerCPPCode, NewExpression, IValue&, IGeneratedQueryCode, ICodeContext, CompositionContainer)
        }
    }
}
