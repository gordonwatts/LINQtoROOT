﻿using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Parsing.Structure;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    ///This is a test class for TestArrayExpressionParser and is intended
    ///to contain all TestArrayExpressionParser Unit Tests
    ///</summary>
    [TestClass()]
    public class TestArrayExpressionParser
    {
        [TestInitialize]
        public void Setup()
        {
            MEFUtilities.MyClassInit();
            MEFUtilities.AddPart(new QVResultOperators());
            ExpressionVisitor.TypeHandlers = new TypeHandlerCache();
            MEFUtilities.AddPart(ExpressionVisitor.TypeHandlers);
            MEFUtilities.AddPart(new DealWithInt32());
            MEFUtilities.AddPart(new LINQToTTreeLib.ResultOperators.ROTakeSkipOperators());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            var qv = new QueryVisitor(gc, cc);
            MEFUtilities.Compose(qv);
        }

        [Export(typeof(ITypeHandler))]
        class DealWithInt32 : ITypeHandler
        {
            public bool CanHandle(Type t)
            {
                return t == typeof(Int32[]);
            }

            public IValue ProcessConstantReference(ConstantExpression expr, IGeneratedCode codeEnv, ICodeContext context, System.ComponentModel.Composition.Hosting.CompositionContainer container)
            {
                return new Variables.ValSimple("35", typeof(int[]));
            }

            public Expression ProcessMethodCall(MethodCallExpression expr, out IValue result, IGeneratedCode gc, ICodeContext context, System.ComponentModel.Composition.Hosting.CompositionContainer container)
            {
                throw new NotImplementedException();
            }
        }


        [TestCleanup]
        public void Cleanup()
        {
            MEFUtilities.MyClassDone();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestForNonArray()
        {
            var gc = new GeneratedCode();
            var cc = new CodeContext();
            ArrayExpressionParser.ParseArrayExpression(Expression.Variable(typeof(int), "d"), gc, cc, MEFUtilities.MEFContainer);
        }

        [TestMethod]
        public void TestRunForNormalArray()
        {
            var gc = new GeneratedCode();
            var cc = new CodeContext();
            ArrayExpressionParser.ParseArrayExpression(Expression.Variable(typeof(int[]), "d"), gc, cc, MEFUtilities.MEFContainer);
        }

        private QueryModel GetModel<T>(Expression<Func<T>> expr)
        {
            var parser = new QueryParser();
            return parser.GetParsedQuery(expr.Body);
        }

        class dummyntup
        {
            public int run;
            public int[] vals;
        }

        [TestMethod]
        public void TestSubQueryExpression()
        {
            var q = new dummyntup();
            q.vals = new int[] { 1, 2, 3, 4, 5 };
            var model = GetModel(() => (from j in q.vals select j).Take(1));
            var sq = new SubQueryExpression(model);

            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();

            var result = ArrayExpressionParser.ParseArrayExpression(sq, gc, cc, MEFUtilities.MEFContainer);
            Assert.IsNull(result, "result of parse array");

            Assert.IsNotNull(cc.LoopVariable, "loop variable");
        }
    }
}