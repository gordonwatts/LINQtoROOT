// <copyright file="TypeHandlerCacheTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Tests;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.TypeHandlers
{
    /// <summary>This class contains parameterized unit tests for TypeHandlerCache</summary>
    [PexClass(typeof(TypeHandlerCache))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class TypeHandlerCacheTest
    {
        [PexMethod]
        public IValue ProcessConstantReference(
            [PexAssumeUnderTest]TypeHandlerCache target,
            ConstantExpression expr,
            IGeneratedQueryCode codeEnv
        )
        {
            IValue result = target.ProcessConstantReference(expr, codeEnv, null, null);
            return result;
            // TODO: add assertions to method TypeHandlerCacheTest.ProcessConstantReference(TypeHandlerCache, ConstantExpression, IGeneratedCode)
        }
        [TestInitialize]
        public void Setup()
        {
            MEFUtilities.MyClassInit();
        }

        [TestCleanup]
        public void Cleanup()
        {
            MEFUtilities.MyClassDone();
        }

        [Export(typeof(ITypeHandler))]
        class BogusTT : ITypeHandler
        {
            public bool CanHandle(Type t)
            {
                return true;
            }

            public IValue ProcessConstantReference(ConstantExpression expr, IGeneratedQueryCode codeEnv, ICodeContext context, CompositionContainer container)
            {
                return new Variables.ValSimple("dude", expr.Type);
            }


            public Expression ProcessMethodCall(MethodCallExpression expr, out IValue result, IGeneratedQueryCode gc, ICodeContext context, CompositionContainer container)
            {
                result = new Variables.VarInteger();
                return expr;
            }
        }


        [TestMethod]
        public void ProcessConstantRefROOT()
        {
            var looker = new BogusTT();
            var target = new TypeHandlerCache();

            MEFUtilities.AddPart(looker);
            MEFUtilities.Compose(target);

            ConstantExpression expr = Expression.Constant(new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0));
            GeneratedCode gc = new GeneratedCode();

            var v = ProcessConstantReference(target, expr, gc);

            Assert.AreEqual("dude", v.RawValue);
            Assert.AreEqual(typeof(ROOTNET.NTH1F), v.Type);
        }

        [TestMethod]
        public void TestMethodCall()
        {
            var looker = new BogusTT();
            var target = new TypeHandlerCache();

            MEFUtilities.AddPart(looker);
            MEFUtilities.Compose(target);

            var m = typeof(int).GetMethods().Where(me => me.Name == "ToString").First();
            var expr = Expression.Call(Expression.Constant(10), m);
            GeneratedCode gc = new GeneratedCode();
            CodeContext c = new CodeContext();

            IValue result;

            var outExpr = ProcessMethodCall(target, expr, out result, gc, c);

            Assert.AreEqual(expr, outExpr);
            Assert.IsInstanceOfType(result, typeof(Variables.VarInteger));
        }

        [PexMethod]
        public Expression ProcessMethodCall(
            [PexAssumeUnderTest]TypeHandlerCache target,
            MethodCallExpression expr,
            out IValue result,
            IGeneratedQueryCode gc,
            ICodeContext cc
        )
        {
            Expression result01 = target.ProcessMethodCall(expr, out result, gc, cc, null);
            return result01;
            // TODO: add assertions to method TypeHandlerCacheTest.ProcessMethodCall(TypeHandlerCache, MethodCallExpression, IValue&, IGeneratedCode)
        }

        /// <summary>Test stub for ProcessNew(NewExpression, IValue&amp;, IGeneratedQueryCode, ICodeContext, CompositionContainer)</summary>
        [PexMethod]
        internal Expression ProcessNew(
            [PexAssumeUnderTest]TypeHandlerCache target,
            NewExpression expression,
            out IValue _result,
            IGeneratedQueryCode _codeEnv,
            ICodeContext _codeContext,
            CompositionContainer MEFContainer
        )
        {
            Expression result = target.ProcessNew
                                    (expression, out _result, _codeEnv, _codeContext, MEFContainer);
            return result;
            // TODO: add assertions to method TypeHandlerCacheTest.ProcessNew(TypeHandlerCache, NewExpression, IValue&, IGeneratedQueryCode, ICodeContext, CompositionContainer)
        }
    }
}
