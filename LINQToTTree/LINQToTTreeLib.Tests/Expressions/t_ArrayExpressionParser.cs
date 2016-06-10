using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.ResultOperators;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
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
            MEFUtilities.AddPart(new SubQueryExpressionArrayInfoFactory());
            MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
            MEFUtilities.AddPart(new EnumerableRangeArrayTypeFactory());
            MEFUtilities.AddPart(new GroupByFactory());
            MEFUtilities.AddPart(new GroupByArrayFactory());
            MEFUtilities.AddPart(new MemberAccessArrayTypeFactory());

            MEFUtilities.AddPart(new ROTakeSkipOperators());
            MEFUtilities.AddPart(new ROFirstLast());
            MEFUtilities.AddPart(new ROCount());

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
                return new LINQToTTreeLib.Variables.ValSimple("35", typeof(int[]));
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
            var r = ArrayExpressionParser.ParseArrayExpression(s, cvt, gc, cc, MEFUtilities.MEFContainer);
            Assert.IsNotNull(r);
            Assert.IsNotNull(cc.LoopVariable, "loop variable");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IEnumerableWithNothingBehindItCheck()
        {
            // We can't loop over a random ienumerable - that isn't enough infomration
            // about where the loop came from (so we can't build the loop unless we know what we
            // are looping over!).
            var gc = new GeneratedCode();
            var cc = new CodeContext();
            IQuerySource s = new DummyQueryReference() { ItemName = "q", ItemType = typeof(int) };
            var arr = Expression.Variable(typeof(IEnumerable<int>), "d");
            var r = ArrayExpressionParser.ParseArrayExpression(s, arr, gc, cc, MEFUtilities.MEFContainer);
        }

        class ObjWithEnumerable
        {
            public IEnumerable<int> a;
        }

        [TestMethod]
        public void IEnumerableFromCustomObjectDirect()
        {
            // Custom object backed by a straight array.
            var e = GenerateExpression(d => new ObjWithEnumerable() { a = d }.a);
            ExecuteArrayParseOnExpression(e);
        }

        [TestMethod]
        public void IEnumerableOverLocalArray()
        {
            // Double check local array argument works.
            var e = GenerateExpression(d => d);
            ExecuteArrayParseOnExpression(e);
        }

        [TestMethod]
        public void IEnumerableAsQuery()
        {
            var e = GetModel(d => d.Where(t => t > 5));
            ExecuteArrayParseOnExpression(e);
        }

        [TestMethod]
        public void IEnumerableCustomObjectWithSubQuery()
        {
            var e = GetModel(d => new ObjWithEnumerable() { a = d.Where(t => t > 5) }.a);
            ExecuteArrayParseOnExpression(e);
        }

        [TestMethod]
        public void IEnumerableCustomObjectGenerated()
        {
            var e = GetModel(d => d.Select(t => new ObjWithEnumerable() { a = d }).First().a);
            ExecuteArrayParseOnExpression(e);
        }

        [TestMethod]
        public void IEnumerableCustomObjectWithComplexQueryGenerated()
        {
            var e = GetModel(d => d.Select(t => new ObjWithEnumerable() { a = d.Where(ft => ft > 5) }).Where(jt => jt.a.Count() > 15).First().a);
            var gc = ExecuteArrayParseOnExpression(e);
            Assert.IsTrue(gc.DumpCode().Where(l => l.Contains(">15")).Any(), "Missing a '>15' in the code");
        }

        /// <summary>
        /// Do the work of executing the array parse
        /// </summary>
        /// <param name="e"></param>
        private static GeneratedCode ExecuteArrayParseOnExpression(Expression e)
        {
            var gc = new GeneratedCode();
            var cc = new CodeContext();
            IQuerySource s = new DummyQueryReference() { ItemName = "q", ItemType = typeof(int) };
            var r = ArrayExpressionParser.ParseArrayExpression(s, e, gc, cc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();
            Assert.IsNotNull(cc.LoopVariable, "loop variable");

            return gc;
        }

        /// <summary>
        /// Shortcut to extract an arbitrary expression.
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private Expression GenerateExpression(Expression<Func<int[], IEnumerable<int>>> func)
        {
            var l = func as LambdaExpression;
            return l.Body;
        }

        /// <summary>
        /// Return a SQE for a given model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr"></param>
        /// <returns></returns>
        private SubQueryExpression GetModel(Expression<Func<int[], IEnumerable<int>>> expr)
        {
            var parser = QueryParser.CreateDefault();
            return new SubQueryExpression(parser.GetParsedQuery(expr.Body));
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
