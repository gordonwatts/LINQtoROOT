using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Parsing.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq.Expressions;

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
            TestUtils.ResetLINQLibrary();
            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new DealWithInt32());
            MEFUtilities.AddPart(new ArrayArrayInfoFactory());
            MEFUtilities.AddPart(new SubQueryArrayTypeFactory());
            MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
            MEFUtilities.AddPart(new LINQToTTreeLib.ResultOperators.ROTakeSkipOperators());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);
        }

        [Export(typeof(ITypeHandler))]
        class DealWithInt32 : ITypeHandler
        {
            public bool CanHandle(Type t)
            {
                return t == typeof(Int32[]);
            }

            public Expression ProcessMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, ICodeContext context, System.ComponentModel.Composition.Hosting.CompositionContainer container)
            {
                return expr;
            }

            public IValue CodeMethodCall(MethodCallExpression expr, IGeneratedQueryCode gc, System.ComponentModel.Composition.Hosting.CompositionContainer container)
            {
                throw new NotImplementedException();
            }


            public Expression ProcessNew(NewExpression expression, out IValue result, IGeneratedQueryCode gc, System.ComponentModel.Composition.Hosting.CompositionContainer container)
            {
                throw new NotImplementedException();
            }


            public IValue ProcessConstantReference(ConstantExpression expr, IGeneratedQueryCode codeEnv, System.ComponentModel.Composition.Hosting.CompositionContainer container)
            {
                return new Variables.ValSimple("35", typeof(int[]));
            }

            public Expression ProcessConstantReferenceExpression(ConstantExpression expr, System.ComponentModel.Composition.Hosting.CompositionContainer container)
            {
                return expr;
            }


            public IValue ProcessMemberReference(MemberExpression expr, IGeneratedQueryCode gc, ICodeContext cc, System.ComponentModel.Composition.Hosting.CompositionContainer container)
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestForNonArray()
        {
            var gc = new GeneratedCode();
            var cc = new CodeContext();
            IQuerySource s = new DummyQueryReference() { ItemName = "q", ItemType = typeof(int) };
            ArrayExpressionParser.ParseArrayExpression(s, Expression.Variable(typeof(int), "d"), gc, cc, MEFUtilities.MEFContainer);
        }

        class DummyQueryReference : IQuerySource
        {
            public string ItemName { get; set; }
            public Type ItemType { get; set; }
        };

        [TestMethod]
        public void TestRunForNormalArray()
        {
            var gc = new GeneratedCode();
            var cc = new CodeContext();
            IQuerySource s = new DummyQueryReference() { ItemName = "q", ItemType = typeof(int) };
            ArrayExpressionParser.ParseArrayExpression(s, Expression.Variable(typeof(int[]), "d"), gc, cc, MEFUtilities.MEFContainer);
            Assert.IsNotNull(cc.LoopVariable, "loop variable");
        }

        [TestMethod]
        public void TestRunForConvertedArray()
        {
            var gc = new GeneratedCode();
            var cc = new CodeContext();
            IQuerySource s = new DummyQueryReference() { ItemName = "q", ItemType = typeof(int) };
            var arr = Expression.Variable(typeof(int[]), "d");
            var cvt = Expression.Convert(arr, typeof(IEnumerable<int>));
            ArrayExpressionParser.ParseArrayExpression(s, cvt, gc, cc, MEFUtilities.MEFContainer);
            Assert.IsNotNull(cc.LoopVariable, "loop variable");
        }

        private QueryModel GetModel<T>(Expression<Func<T>> expr)
        {
            var parser = QueryParser.CreateDefault();
            return parser.GetParsedQuery(expr.Body);
        }

        class dummyntup
        {
#pragma warning disable 0649
            public int run;
            public int[] vals;
#pragma warning restore 0649
        }
    }
}
