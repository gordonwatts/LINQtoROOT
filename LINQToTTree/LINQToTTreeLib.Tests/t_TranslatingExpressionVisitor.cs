using System;
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

        public class SourceType2Container
        {
            [TTreeVariableGrouping]
            public int val;
        }

        [TranslateToClass(typeof(ResultType2))]
        public class SourceType2
        {
            [TTreeVariableGrouping]
            public SourceType2Container[] jets;
        }

        public class ResultType2
        {
            public ResultType2(Expression holder)
            {

            }
            public int[] val;
        }

        [TestMethod]
        public void TestArrayGroupingChange()
        {
            ///
            /// First, build up an object
            /// we can build expresisons against
            /// 


            var actual = new SourceType2() { jets = new SourceType2Container[] { new SourceType2Container() { val = 2 } } };
            var value = Expression.Constant(actual);
            var exprBase = Expression.MakeMemberAccess(value, typeof(SourceType2).GetMember("jets").First());
            var exprArr = Expression.MakeBinary(ExpressionType.ArrayIndex, exprBase, Expression.Constant(0));
            var exprArrValue = Expression.MakeMemberAccess(exprArr, typeof(SourceType2Container).GetMember("val").First());

            var result = TranslatingExpressionVisitor.Translate(exprArrValue);

            /// Make sure we are now doing an array access on the second object....            /// 

            Assert.IsInstanceOfType(result, typeof(BinaryExpression), "type incorrect");
            var translatedArAccess = result as BinaryExpression;
            Assert.AreEqual(ExpressionType.ArrayIndex, translatedArAccess.NodeType, "expected array index");
            var constExpr = translatedArAccess.Right as ConstantExpression;
            Assert.IsNotNull(constExpr, "const not there for index expression");
            Assert.AreEqual(0, constExpr.Value, "Index is incorrect");

            var translatedMemberAccess = translatedArAccess.Left as MemberExpression;
            Assert.IsNotNull(translatedMemberAccess, "the member access isn't right");
            Assert.AreEqual("val", translatedMemberAccess.Member.Name, "The actual member name didn't get mapped over correctly");
            Assert.AreEqual(typeof(ResultType2), translatedMemberAccess.Expression.Type, "bad return type - not translated");
        }

        public class SourceType3Container1
        {
            [TTreeVariableGrouping]
            [IndexToOtherObjectArray(typeof(SourceType3), "muons")]
            public SourceType3Container2 specialIndex;
        }

        public class SourceType3Container2
        {
            [TTreeVariableGrouping]
            public int val;
        }

        [TranslateToClass(typeof(ResultType3))]
        public class SourceType3
        {
            [TTreeVariableGrouping]
            public SourceType3Container1[] jets;

            [TTreeVariableGrouping]
            public SourceType3Container2[] muons;
        }

        public class ResultType3
        {
            public ResultType3(Expression holder)
            {
            }
            public int[] val;
            public int[] specialIndex;
        }

        [TestMethod]
        public void TestArrayIndex()
        {
            ///
            /// BUild up an expression to do the functional query we are interested in seeing go
            /// 

            SourceType3 actual = null;
            {
                actual = new SourceType3()
                {
                    muons = new SourceType3Container2[] { new SourceType3Container2() { val = 1 }, new SourceType3Container2() { val = 3 } }
                };
                actual.jets = new SourceType3Container1[]
                {
                    new SourceType3Container1() { specialIndex = actual.muons[0]},
                    new SourceType3Container1() { specialIndex = actual.muons[1]}
                };
            }

            ///
            /// Now, code up an expression that looks like the following:
            ///   actual.jets[0].specialIndex.val

            var origValueAsConst = Expression.Constant(actual);
            Expression originalExpression = null;
            {
                Expression<Func<int>> a = () => actual.jets[0].specialIndex.val;
                originalExpression = a.Body as MemberExpression;
            }

            ///
            /// Do the translation
            /// 

            var result = TranslatingExpressionVisitor.Translate(originalExpression);

            ///
            /// Ok, now that translation is done, we expect to see
            /// result.val[result.specialIndex[0]]. To do the compare, lets create a destiantion expression
            /// 

            Expression finalExpectedValue = null;
            {
                var expected = new ResultType3(null) { specialIndex = new int[] { 0, 1 }, val = new int[] { 1, 3 } };
                Expression<Func<int>> fullExpression = () => expected.val[expected.specialIndex[0]];
                finalExpectedValue = fullExpression.Body;
            }

            Assert.IsTrue(result.ToString().Contains(".specialIndex[0]]"), "missign the special index reference");
            Assert.IsTrue(result.ToString().Contains(".val[value"), "missign the .val reference");
        }

        public class SourceType4Container1
        {
            [TTreeVariableGrouping]
            [IndexToOtherObjectArray(typeof(SourceType4), "muons")]
            public SourceType4Container3 specialIndex1;
        }

        public class SourceType4Container2
        {
            [TTreeVariableGrouping]
            public int val;
        }

        public class SourceType4Container3
        {
            [TTreeVariableGrouping]
            [IndexToOtherObjectArray(typeof(SourceType4), "tracks")]
            public SourceType4Container2 specialIndex2;
        }

        [TranslateToClass(typeof(ResultType4))]
        public class SourceType4
        {
            [TTreeVariableGrouping]
            public SourceType4Container1[] jets;

            [TTreeVariableGrouping]
            public SourceType4Container3[] muons;

            [TTreeVariableGrouping]
            public SourceType4Container2[] tracks;
        }

        public class ResultType4
        {
            public ResultType4(Expression holder)
            {
            }
            public int[] val;
            public int[] specialIndex1;
            public int[] specialIndex2;
        }

        [TestMethod]
        public void TestDoubleArrayIndirectIndex()
        {
            ///
            /// BUild up an expression to do the functional query we are interested in seeing go
            /// 

            SourceType4 actual = null;
            {
                actual = new SourceType4()
                {
                    tracks = new SourceType4Container2[] { new SourceType4Container2() { val = 1 }, new SourceType4Container2() { val = 3 } }
                };
                actual.muons = new SourceType4Container3[] {
                    new SourceType4Container3() { specialIndex2 = actual.tracks[0]},
                    new SourceType4Container3() { specialIndex2 = actual.tracks[1]}
                };
                actual.jets = new SourceType4Container1[]
                {
                    new SourceType4Container1() { specialIndex1 = actual.muons[0]},
                    new SourceType4Container1() { specialIndex1 = actual.muons[1]}
                };
            }

            ///
            /// Now, code up an expression that looks like the following:
            ///   actual.jets[0].specialIndex.val

            var origValueAsConst = Expression.Constant(actual);
            Expression originalExpression = null;
            {
                Expression<Func<int>> a = () => actual.jets[0].specialIndex1.specialIndex2.val;
                originalExpression = a.Body as MemberExpression;
            }

            ///
            /// Do the translation
            /// 

            var result = TranslatingExpressionVisitor.Translate(originalExpression);

            ///
            /// Ok, now that translation is done, we expect to see
            /// result.val[result.specialIndex[0]]. To do the compare, lets create a destiantion expression
            /// 

            Expression finalExpectedValue = null;
            {
                var expected = new ResultType3(null) { specialIndex = new int[] { 0, 1 }, val = new int[] { 1, 3 } };
                Expression<Func<int>> fullExpression = () => expected.val[expected.specialIndex[0]];
                finalExpectedValue = fullExpression.Body;
            }

            Assert.IsTrue(result.ToString().Contains(".specialIndex1[0]]]"), "missign the special index 1 reference");
            Assert.IsTrue(result.ToString().Contains(".specialIndex2[value"), "missign the special index 2 reference");
            Assert.IsTrue(result.ToString().Contains(".val[value"), "missign the .val reference");
        }

        public class SourceType5Container1
        {
            [TTreeVariableGrouping]
            [IndexToOtherObjectArray(typeof(SourceType5), "muons")]
            [RenameVariable("specialIndex")]
            public SourceType5Container2 theMuon;
        }

        public class SourceType5Container2
        {
            [TTreeVariableGrouping]
            public int val;
        }

        [TranslateToClass(typeof(ResultType5))]
        public class SourceType5
        {
            [TTreeVariableGrouping]
            public SourceType5Container1[] jets;

            [TTreeVariableGrouping]
            public SourceType5Container2[] muons;
        }

        public class ResultType5
        {
            public ResultType5(Expression holder)
            {
            }
            public int[] val;
            public int[] specialIndex;
        }

        [TestMethod]
        public void TestArrayIndirectRename()
        {
            ///
            /// BUild up an expression to do the functional query we are interested in seeing go, and make
            /// sure the indirect index rename actually gets done correctly.
            /// 

            SourceType5 actual = null;
            {
                actual = new SourceType5()
                {
                    muons = new SourceType5Container2[] { new SourceType5Container2() { val = 1 }, new SourceType5Container2() { val = 3 } }
                };
                actual.jets = new SourceType5Container1[]
                {
                    new SourceType5Container1() { theMuon = actual.muons[0]},
                    new SourceType5Container1() { theMuon = actual.muons[1]}
                };
            }

            ///
            /// Now, code up an expression that looks like the following:
            ///   actual.jets[0].specialIndex.val

            var origValueAsConst = Expression.Constant(actual);
            Expression originalExpression = null;
            {
                Expression<Func<int>> a = () => actual.jets[0].theMuon.val;
                originalExpression = a.Body as MemberExpression;
            }

            ///
            /// Do the translation
            /// 

            var result = TranslatingExpressionVisitor.Translate(originalExpression);

            ///
            /// Ok, now that translation is done, we expect to see
            /// result.val[result.specialIndex[0]]. To do the compare, lets create a destiantion expression
            /// 

            Expression finalExpectedValue = null;
            {
                var expected = new ResultType3(null) { specialIndex = new int[] { 0, 1 }, val = new int[] { 1, 3 } };
                Expression<Func<int>> fullExpression = () => expected.val[expected.specialIndex[0]];
                finalExpectedValue = fullExpression.Body;
            }

            Assert.IsTrue(result.ToString().Contains(".specialIndex[0]]"), "missign the special index reference");
            Assert.IsTrue(result.ToString().Contains(".val[value"), "missign the .val reference");
        }
    }
}
