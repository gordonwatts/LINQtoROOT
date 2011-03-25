using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
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
            ExpressionVisitor.TypeHandlers = new TypeHandlerCache();
            MEFUtilities.AddPart(ExpressionVisitor.TypeHandlers);
            MEFUtilities.AddPart(new DealWithInt32());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            var qv = new QueryVisitor(gc, cc);
            MEFUtilities.Compose(qv);
        }

        [Export(typeof(ITypeHandler))]
        class DealWithInt32 : ITypeHandler
        {
            public bool CanHandle(System.Type t)
            {
                return t == typeof(System.Int32[]);
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

            gc.Add(new Statements.StatementSimpleStatement("d = d"));

            ///
            /// Make sure the index variable comes back correctly
            /// 

            Assert.IsInstanceOfType(indexVar, typeof(BinaryExpression), "inproper expression variable type");
            Assert.AreEqual(typeof(int), indexVar.Type, "bad value type");
            var be = indexVar as BinaryExpression;
            Assert.AreEqual(ExpressionType.ArrayIndex, be.NodeType, "not array index");
            Assert.AreEqual(typeof(int), be.Right.Type, "Indexer of array type");
            Assert.IsInstanceOfType(be.Left, typeof(ParameterExpression), "now the same paraemter, I think!");
            Assert.AreEqual(simpleArrayExpr, be.Left, "array isn't right");

            ///
            /// Next, we need to look at the statements that have come back
            /// 

            var statements = gc.CodeBody.CodeItUp().ToArray();

            Assert.AreEqual("{", statements[0], "open brace");
            Assert.IsTrue(statements[1].Contains("int"), "statement 1 - int: '" + statements[1] + "'");
            Assert.IsTrue(statements[2].Contains("size();"), "statement 2 - x = 0;: '" + statements[2] + "'");
            Assert.IsTrue(statements[3].StartsWith("  for (int "), "statement 3 - for (): '" + statements[3] + "'");
            Assert.AreEqual("  {", statements[4], "for loop brace opening");
            Assert.AreEqual("    d = d;", statements[5], "the actual statement");

            Assert.Inconclusive("Need to code up checks to make sure the 'loop' variable is set correctly so other expressions depending on the loop can access this guy.");
        }

        [TestMethod]
        public void TestTranslatedArray()
        {
            Assert.Inconclusive("not written yet");
        }
    }
}
