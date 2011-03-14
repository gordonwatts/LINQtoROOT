using System.Linq;
using System.Linq.Expressions;
using LINQToTTreeLib.CodeAttributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for TestTranslatingExpressionVisitor and is intended
    ///to contain all TestTranslatingExpressionVisitor Unit Tests
    ///</summary>
    [TestClass()]
    public class TestTranslatingExpressionVisitor
    {


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


        /// <summary>
        ///A test for TranslatingExpressionVisitor Constructor
        ///</summary>
        [TestMethod()]
        public void TestTranslatingExpressionVisitorConstructor()
        {
            TranslatingExpressionVisitor target = new TranslatingExpressionVisitor();
        }

        public class NoTranslateClass
        {
            public int val;
        }

        [TestMethod]
        public void TestNoTranslateOfSimpleExpression()
        {
            var value = Expression.Variable(typeof(NoTranslateClass));
            var expr = Expression.MakeMemberAccess(value, typeof(NoTranslateClass).GetMember("val").First());

            var result = TranslatingExpressionVisitor.Translate(expr);

            /// Make sure nothing changed!

            Assert.IsInstanceOfType(result, typeof(MemberExpression), "type incorrect");
            var me = result as MemberExpression;
            Assert.AreEqual("val", me.Member.Name, "member access name incorrect");
            Assert.AreEqual(typeof(NoTranslateClass), me.Expression.Type, "variable type not right");
        }

        [TranslateToClass(typeof(ResultType1))]
        public class SourceType1
        {
            [RenameVariable("Val")]
            public int val;

            public int same;
        }

        public class ResultType1
        {
            public ResultType1(Expression holder)
            {

            }
            public int Val;
            public int same;
        }

        [TestMethod]
        public void TestTranslationTest()
        {
            Assert.IsFalse(TranslatingExpressionVisitor.NeedsTranslation(typeof(ResultType1)));
            Assert.IsTrue(TranslatingExpressionVisitor.NeedsTranslation(typeof(SourceType1)));
        }

        [TestMethod]
        public void TestSimpleRename()
        {
            var value = Expression.Variable(typeof(SourceType1));
            var expr = Expression.MakeMemberAccess(value, typeof(SourceType1).GetMember("val").First());

            var result = TranslatingExpressionVisitor.Translate(expr);

            /// Make sure nothing changed!

            Assert.IsInstanceOfType(result, typeof(MemberExpression), "type incorrect");
            var me = result as MemberExpression;
            Assert.AreEqual("Val", me.Member.Name, "member access name incorrect");
            Assert.AreEqual(typeof(ResultType1), me.Expression.Type, "variable type not right");
        }

        [TestMethod]
        public void TestTranslateClassButNotMember()
        {
            var value = Expression.Variable(typeof(SourceType1));
            var expr = Expression.MakeMemberAccess(value, typeof(SourceType1).GetMember("same").First());

            var result = TranslatingExpressionVisitor.Translate(expr);

            /// Make sure nothing changed!

            Assert.IsInstanceOfType(result, typeof(MemberExpression), "type incorrect");
            var me = result as MemberExpression;
            Assert.AreEqual("same", me.Member.Name, "member access name incorrect");
            Assert.AreEqual(typeof(ResultType1), me.Expression.Type, "variable type not right");
        }
    }
}
