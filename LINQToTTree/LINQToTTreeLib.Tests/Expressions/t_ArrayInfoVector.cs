using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.Utils;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for TestArrayInfoVector and is intended
    ///to contain all TestArrayInfoVector Unit Tests
    ///</summary>
    [TestClass()]
    public class TestArrayInfoVector
    {


        [TestInitialize]
        public void Setup()
        {
            MEFUtilities.MyClassInit();
            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new DealWithMyTypes());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            var qv = new QueryVisitor(gc, cc, MEFUtilities.MEFContainer);
            MEFUtilities.Compose(qv);
        }

        [Export(typeof(ITypeHandler))]
        class DealWithMyTypes : ITypeHandler
        {
            public bool CanHandle(System.Type t)
            {
                return t == typeof(System.Int32[])
                    || t == typeof(ResultType1);
            }

            public IValue ProcessConstantReference(ConstantExpression expr, IGeneratedQueryCode codeEnv, ICodeContext context, System.ComponentModel.Composition.Hosting.CompositionContainer container)
            {
                return new Variables.ValSimple("35", expr.Type);
            }

            public Expression ProcessMethodCall(MethodCallExpression expr, out IValue result, IGeneratedQueryCode gc, ICodeContext context, System.ComponentModel.Composition.Hosting.CompositionContainer container)
            {
                throw new NotImplementedException();
            }


            public Expression ProcessNew(NewExpression expression, out IValue result, IGeneratedQueryCode gc, ICodeContext context, System.ComponentModel.Composition.Hosting.CompositionContainer container)
            {
                throw new NotImplementedException();
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            MEFUtilities.MyClassDone();
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        [TestMethod]
        public void TestSimpleArray()
        {
            var simpleArrayExpr = Expression.Variable(typeof(int[]), "d");
            ArrayInfoVector vec = new ArrayInfoVector(simpleArrayExpr);

            CodeContext cc = new CodeContext();
            GeneratedCode gc = new GeneratedCode();

            var indexVar = vec.AddLoop(gc, cc, MEFUtilities.MEFContainer);

            ///
            /// Add a dumb statement to force the rendering of the loop (empty loops don't render)
            /// 

            gc.Add(new LINQToTTreeLib.Statements.StatementSimpleStatement("d = d"));

            ///
            /// Make sure the index variable comes back correctly
            /// 

            Assert.IsInstanceOfType(indexVar.Item1, typeof(BinaryExpression), "inproper expression variable type");
            Assert.AreEqual(typeof(int), indexVar.Item1.Type, "bad value type");
            var be = indexVar.Item1 as BinaryExpression;
            Assert.AreEqual(ExpressionType.ArrayIndex, be.NodeType, "not array index");
            Assert.AreEqual(typeof(int), be.Right.Type, "Indexer of array type");
            Assert.IsInstanceOfType(be.Left, typeof(ParameterExpression), "now the same paraemter, I think!");
            Assert.AreEqual(simpleArrayExpr, be.Left, "array isn't right");

            Assert.AreEqual(typeof(int), indexVar.Item2.Type, "Bad index variable");

            ///
            /// Next, we need to look at the statements that have come back
            /// 

            var statements = gc.CodeBody.CodeItUp().ToArray();

            Assert.AreEqual("{", statements[0], "open brace");
            Assert.IsTrue(statements[1].Contains("int"), "statement 1 - int: '" + statements[1] + "'");
            Assert.IsTrue(statements[1].Contains("size();"), "statement 2 - x = 0;: '" + statements[2] + "'");
            Assert.IsTrue(statements[2].StartsWith("  for (int "), "statement 3 - for (): '" + statements[3] + "'");
            Assert.AreEqual("  {", statements[3], "for loop brace opening");
            Assert.AreEqual("    d = d;", statements[4], "the actual statement");
        }

        [TranslateToClass(typeof(ResultType1))]
        class SourceType1
        {
#pragma warning disable 0649
            [TTreeVariableGrouping]
            public SourceType1SubType[] jets;
#pragma warning restore 0649
        }

        class SourceType1SubType
        {
#pragma warning disable 0649
            [TTreeVariableGrouping]
            public int val1;
#pragma warning restore 0649
        }

        class ResultType1 : IExpressionHolder
        {
            public ResultType1(Expression holder)
            { HeldExpression = holder; }

#pragma warning disable 0649
            public int[] val1;
#pragma warning restore 0649

            public Expression HeldExpression
            {
                get;
                private set;
            }
        }

        [TestMethod]
        public void TestTranslatedArray()
        {
            var baseVar = Expression.Variable(typeof(SourceType1), "d");
            var jetRef = Expression.MakeMemberAccess(baseVar, typeof(SourceType1).GetMember("jets").First());

            ArrayInfoVector vec = new ArrayInfoVector(jetRef);

            CodeContext cc = new CodeContext();
            GeneratedCode gc = new GeneratedCode();

            var indexVar = vec.AddLoop(gc, cc, MEFUtilities.MEFContainer);
            gc.Add(new LINQToTTreeLib.Statements.StatementSimpleStatement("dude"));

            ///
            /// Make sure the indexvar is working correctly
            /// 

            Assert.IsInstanceOfType(indexVar.Item1, typeof(BinaryExpression), "inproper expression variable type");
            Assert.AreEqual(typeof(SourceType1SubType), indexVar.Item1.Type, "index var type");
            var be = indexVar.Item1 as BinaryExpression;
            Assert.AreEqual(ExpressionType.ArrayIndex, be.NodeType, "not array index");
            Assert.AreEqual(typeof(int), be.Right.Type, "Indexer of array type");
            Assert.IsInstanceOfType(be.Left, typeof(MemberExpression), "now the same paraemter, I think!");
            Assert.AreEqual(jetRef, be.Left, "array isn't right");

            ///
            /// Now, make sure we got as far as a proper size variable
            /// 

            var statements = gc.CodeBody.CodeItUp().ToArray();
            Assert.IsTrue(statements[1].Contains(".val1).size()"), "size statement incorrect: '" + statements[1] + "'");
        }
    }
}
