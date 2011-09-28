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
            IValue result = target.ProcessConstantReference(expr, codeEnv, null);
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

            public Expression ProcessMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, ICodeContext context, System.ComponentModel.Composition.Hosting.CompositionContainer container)
            {
                return expr;
            }

            public IValue CodeMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, System.ComponentModel.Composition.Hosting.CompositionContainer container)
            {
                return new Variables.ValSimple("1", typeof(int));
            }

            public Expression ProcessNew(NewExpression expression, out IValue result, IGeneratedQueryCode gc, CompositionContainer container)
            {
                throw new NotImplementedException();
            }


            public IValue ProcessConstantReference(ConstantExpression expr, IGeneratedQueryCode codeEnv, CompositionContainer container)
            {
                return new Variables.ValSimple("dude", expr.Type);
            }

            public Expression ProcessConstantReferenceExpression(ConstantExpression expr, CompositionContainer container)
            {
                throw new NotImplementedException();
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

            var result = CodeMethodCall(target, expr, gc);

            Assert.AreEqual(result.Type, typeof(int), "result type");
        }

        [PexMethod]
        public Expression ProcessMethodCall(
            [PexAssumeUnderTest]TypeHandlerCache target,
            MethodCallExpression expr,
            IGeneratedQueryCode gc,
            ICodeContext cc
        )
        {
            return target.ProcessMethodCall(expr, gc, cc, null);
        }

        [PexMethod]
        public IValue CodeMethodCall(
            [PexAssumeUnderTest]TypeHandlerCache target,
            MethodCallExpression expr,
            IGeneratedQueryCode gc
        )
        {
            return target.CodeMethodCall(expr, gc, null);
        }

        /// <summary>Test stub for ProcessNew(NewExpression, IValue&amp;, IGeneratedQueryCode, ICodeContext, CompositionContainer)</summary>
        [PexMethod]
        internal Expression ProcessNew(
            [PexAssumeUnderTest]TypeHandlerCache target,
            NewExpression expression,
            out IValue _result,
            IGeneratedQueryCode _codeEnv,
            CompositionContainer MEFContainer
        )
        {
            Expression result = target.ProcessNew
                                    (expression, out _result, _codeEnv, MEFContainer);
            return result;
            // TODO: add assertions to method TypeHandlerCacheTest.ProcessNew(TypeHandlerCache, NewExpression, IValue&, IGeneratedQueryCode, ICodeContext, CompositionContainer)
        }
    }
}
