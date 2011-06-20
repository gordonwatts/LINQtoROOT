using System;
using System.Collections.Generic;
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

        [TestInitialize]
        public void TestInit()
        {
            MEFUtilities.MyClassInit();
            DummyQueryExectuor.GlobalInitalized = false;
        }

        [TestCleanup]
        public void TestDone()
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

            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(expr, caches);

            /// Make sure nothing changed!

            Assert.IsInstanceOfType(result, typeof(MemberExpression), "type incorrect");
            var me = result as MemberExpression;
            Assert.AreEqual("val", me.Member.Name, "member access name incorrect");
            Assert.AreEqual(typeof(NoTranslateClass), me.Expression.Type, "variable type not right");
        }

        class NoTranslateArrayClass
        {
#pragma warning disable 0649
            public int[] val;
            public int[][] val2D;
#pragma warning restore 0649
        }

        [TestMethod]
        public void TestNoTranslateArrayLength()
        {
            Expression<Func<NoTranslateArrayClass, int>> lambdaExpr = arr => arr.val.Length;
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambdaExpr.Body, caches);
            Assert.AreEqual(ExpressionType.ArrayLength, result.NodeType, "expression node");
            var ue = result as UnaryExpression;
            Assert.IsNotNull(ue, "not unaryexpression");
            var me = ue.Operand as MemberExpression;
            Assert.IsNotNull(me, "unary operand type");
            Assert.AreEqual(typeof(NoTranslateArrayClass), me.Expression.Type, "type of member we are applying val to");
            Assert.AreEqual("val", me.Member.Name, "member access bad");
        }

        [TestMethod]
        public void TestNoTranslate1DArrayIndex()
        {
            Expression<Func<NoTranslateArrayClass, int>> lambdaExpr = arr => arr.val[1];
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambdaExpr.Body, caches);
            Assert.AreEqual(ExpressionType.ArrayIndex, result.NodeType, "expression node");
            Assert.AreEqual(lambdaExpr.Body.ToString(), result.ToString(), "expression doesn't match");
        }

        [TestMethod]
        public void TestTranslateNewPair1()
        {
            Expression<Func<int>> lambaExpr = () => new Tuple<int, int>(5, 10).Item1;
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambaExpr.Body, caches);
            Assert.IsInstanceOfType(result, typeof(ConstantExpression), "Expression type");
            Assert.AreEqual(typeof(int), result.Type, "result type not right");
            Assert.AreEqual(5, (result as ConstantExpression).Value, "value incorrect");
        }

        [TestMethod]
        public void TestTranslateNewPair2()
        {
            Expression<Func<int>> lambaExpr = () => new Tuple<int, int>(5, 10).Item2;
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambaExpr.Body, caches);
            Assert.IsInstanceOfType(result, typeof(ConstantExpression), "Expression type");
            Assert.AreEqual(typeof(int), result.Type, "result type not right");
            Assert.AreEqual(10, (result as ConstantExpression).Value, "value incorrect");
        }

        [TestMethod]
        public void TestNoTranslate2DArrayIndex()
        {
            Expression<Func<NoTranslateArrayClass, int>> lambdaExpr = arr => arr.val2D[1][1];
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambdaExpr.Body, caches);
            Assert.AreEqual(ExpressionType.ArrayIndex, result.NodeType, "expression node");
            Assert.AreEqual(lambdaExpr.Body.ToString(), result.ToString(), "expression doesn't match");
            Console.WriteLine(lambdaExpr.Body.ToString());
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

            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(expr, caches);

            /// Make sure nothing changed!

            Assert.IsInstanceOfType(result, typeof(MemberExpression), "type incorrect");
            var me = result as MemberExpression;
            Assert.AreEqual("Val", me.Member.Name, "member access name incorrect");
            Assert.AreEqual(typeof(ResultType1), me.Expression.Type, "variable type not right");
        }

        [TestMethod]
        public void TestRenameCookieCrumbs()
        {
            var value = Expression.Variable(typeof(SourceType1));
            var expr = Expression.MakeMemberAccess(value, typeof(SourceType1).GetMember("val").First());

            List<string> cookies = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(expr, cookies);

            /// Make sure cookies came back ok!

            Assert.AreEqual(1, cookies.Count, "# of cookies");
            Assert.IsTrue(cookies[0].Contains("val"), "cookie missing val");
            Assert.IsTrue(cookies[0].Contains("Val"), "cookie missing Val");

        }

        [TranslateToClass(typeof(ResultType6))]
        public class SourceType6
        {
            [RenameVariable("Val")]
            public int[] val;
            public int[] same;
        }

        public class ResultType6
        {
            public ResultType6(Expression holder)
            {

            }
            public int[] Val;
            public int[] same;
        }

        [TestMethod]
        public void TestArrayRenameLength()
        {
            Expression<Func<SourceType6, int>> lambdaExpr = arr => arr.val.Length;
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambdaExpr.Body, caches);
            Assert.AreEqual(ExpressionType.ArrayLength, result.NodeType, "expression node");
            var ue = result as UnaryExpression;
            Assert.IsNotNull(ue, "not unaryexpression");
            var me = ue.Operand as MemberExpression;
            Assert.IsNotNull(me, "unary operand type");
            Assert.AreEqual(typeof(ResultType6), me.Expression.Type, "type of member we are applying val to");
            Assert.AreEqual("Val", me.Member.Name, "member access bad");
        }

        [TestMethod]
        public void TestArrayLengthNoRename()
        {
            Expression<Func<SourceType6, int>> lambdaExpr = arr => arr.same.Length;
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambdaExpr.Body, caches);
            Assert.AreEqual(ExpressionType.ArrayLength, result.NodeType, "expression node");
            var ue = result as UnaryExpression;
            Assert.IsNotNull(ue, "not unaryexpression");
            var me = ue.Operand as MemberExpression;
            Assert.IsNotNull(me, "unary operand type");
            Assert.AreEqual(typeof(ResultType6), me.Expression.Type, "type of member we are applying val to");
            Assert.AreEqual("same", me.Member.Name, "member access bad");
        }

        [TestMethod]
        public void TestTranslateClassButNotMember()
        {
            var value = Expression.Variable(typeof(SourceType1));
            var expr = Expression.MakeMemberAccess(value, typeof(SourceType1).GetMember("same").First());

            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(expr, caches);

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

            [TTreeVariableGrouping]
            public int[] val2D;
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
            public int[][] val2D;
        }

        [TestMethod]
        public void TestArrayLengthGroupingChange()
        {
            Expression<Func<SourceType2, int>> lambdaExpr = arr => arr.jets.Length;
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambdaExpr.Body, caches);
            Assert.AreEqual(ExpressionType.ArrayLength, result.NodeType, "expression node");
            var ue = result as UnaryExpression;
            Assert.IsNotNull(ue, "not unaryexpression");
            var me = ue.Operand as MemberExpression;
            Assert.IsNotNull(me, "unary operand type");
            Assert.AreEqual(typeof(ResultType2), me.Expression.Type, "type of member we are applying val to");
            Assert.AreEqual("val", me.Member.Name, "member access bad");
        }

        [TestMethod]
        public void TestArrayLengthFor2D()
        {
            Expression<Func<SourceType2, int>> lambdaExpr = arr => arr.jets[0].val2D.Length;
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambdaExpr.Body, caches);
            Assert.AreEqual(ExpressionType.ArrayLength, result.NodeType, "expression node");
            var ue = result as UnaryExpression;
            Assert.AreEqual(ExpressionType.ArrayIndex, ue.Operand.NodeType, "expression array index for the insize missing");
            var be = ue.Operand as BinaryExpression;
            Assert.AreEqual(ExpressionType.MemberAccess, be.Left.NodeType, "member access incorrect");
            var me = be.Left as MemberExpression;
            Assert.AreEqual("val2D", me.Member.Name, "va2D member isn't being referenced!");
            Console.WriteLine(result.ToString());
        }

        [TestMethod]
        public void TestArrayIndex2D()
        {
            Expression<Func<SourceType2, int>> lambdaExpr = arr => arr.jets[0].val2D[1];
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambdaExpr.Body, caches);
            Assert.AreEqual(ExpressionType.ArrayIndex, result.NodeType, "expression node");
            var firstbe = result as BinaryExpression;
            Assert.AreEqual(ExpressionType.ArrayIndex, firstbe.Left.NodeType, "expression array index for the insize missing");
            var be = firstbe.Left as BinaryExpression;
            Assert.AreEqual(ExpressionType.MemberAccess, be.Left.NodeType, "member access incorrect");
            var me = be.Left as MemberExpression;
            Assert.AreEqual("val2D", me.Member.Name, "va2D member isn't being referenced!");
            Console.WriteLine(result.ToString());
        }

        [TestMethod]
        public void TestArrayGroupingNoTranslation()
        {
            Expression<Func<SourceType2, SourceType2Container>> lambdaExpr = arr => arr.jets[12];
            var notReadyYet = lambdaExpr.Body;

            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(notReadyYet, caches);
            Assert.AreEqual(notReadyYet.ToString(), result.ToString(), "translation shouldn't have happened");
        }

        [TestMethod]
        public void TestArrayGroupingNoIndexTranslation()
        {
            Expression<Func<SourceType2, SourceType2Container[]>> lambdaExpr = arr => arr.jets;
            var notReadyYet = lambdaExpr.Body;

            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(notReadyYet, caches);
            Assert.AreEqual(notReadyYet.ToString(), result.ToString(), "translation shouldn't have happened");
        }

        [TestMethod]
        public void TestArrayGroupingSubClassTranslation()
        {
            Expression<Func<SourceType2Container, int>> lambdaExpr = arr => arr.val;
            var notReadyYet = lambdaExpr.Body;

            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(notReadyYet, caches);
            Assert.AreEqual(notReadyYet.ToString(), result.ToString(), "translation shouldn't have happened");
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

            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(exprArrValue, caches);

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

            [TTreeVariableGrouping]
            [IndexToOtherObjectArray(typeof(SourceType3), "muons")]
            public SourceType3Container2[] specialIndicies;
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
            public int[][] specialIndicies;
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

            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(originalExpression, caches);

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

        [TestMethod]
        public void TestArray2DIndexLength()
        {
            Expression<Func<SourceType3, int>> lambdaExpr = arr => arr.jets[0].specialIndicies.Length;
            /// => arr.specialIndicies[0].Length
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambdaExpr.Body, caches);

            Assert.AreEqual(ExpressionType.ArrayLength, result.NodeType, "expression node");
            var ue = result as UnaryExpression;
            Assert.AreEqual(ExpressionType.ArrayIndex, ue.Operand.NodeType, "expression array index for the insize missing");
            var be = ue.Operand as BinaryExpression;
            Assert.AreEqual(ExpressionType.MemberAccess, be.Left.NodeType, "member access incorrect");
            var me = be.Left as MemberExpression;
            Assert.AreEqual("specialIndicies", me.Member.Name, "va2D member isn't being referenced!");
            Console.WriteLine(result.ToString());
        }

        [TestMethod]
        public void TestArray2DIndexLookup()
        {
            Expression<Func<SourceType3, int>> lambdaExpr = arr => arr.jets[0].specialIndicies[1].val;
            /// => arr.muons[obj.specialIndicies[0][1]].val => obj.val[obj.specialIndicies[0][1]]
            /// 
            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(lambdaExpr.Body, caches);

            Assert.AreEqual(ExpressionType.ArrayIndex, result.NodeType, "expression node");
            var topLevelBe = result as BinaryExpression;

            Assert.AreEqual(ExpressionType.MemberAccess, topLevelBe.Left.NodeType, "obj.val lookup");
            var topLevelMe = topLevelBe.Left as MemberExpression;
            Assert.AreEqual("val", topLevelMe.Member.Name, "top level member access");

            Assert.AreEqual(ExpressionType.ArrayIndex, topLevelBe.Right.NodeType, "master index lookup");
            var topArrayIndex = topLevelBe.Right as BinaryExpression;
            Assert.AreEqual(ExpressionType.Constant, topArrayIndex.Right.NodeType, "constant 1 not right");
            Assert.AreEqual(ExpressionType.ArrayIndex, topArrayIndex.Left.NodeType, "second level array lookup");
            var secondArrayIndex = topArrayIndex.Left as BinaryExpression;
            Assert.AreEqual(ExpressionType.Constant, secondArrayIndex.Right.NodeType, "2nd array constant lookup");
            Assert.AreEqual(ExpressionType.MemberAccess, secondArrayIndex.Left.NodeType, "2nd array member acess");
            var secondMemberAccess = secondArrayIndex.Left as MemberExpression;
            Assert.AreEqual("specialIndicies", secondMemberAccess.Member.Name, "member access for index");

            Console.WriteLine(result);
        }

        public class SourceType4Container1
        {
            [TTreeVariableGrouping]
            [IndexToOtherObjectArray(typeof(SourceType4), "muons")]
            public SourceType4Container3 specialIndex1;
            [TTreeVariableGrouping]
            [IndexToOtherObjectArray(typeof(SourceType4), "muons")]
            public SourceType4Container2[] specialIndex3;
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
            [TTreeVariableGrouping]
            [IndexToOtherObjectArray(typeof(SourceType4), "tracks")]
            public SourceType4Container2[] specialIndex3;
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
            public int[][] specialIndex3;
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

            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(originalExpression, caches);

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

        /// <summary>
        /// When a complex object is referenced, but the final bit is missing, make sure
        /// that no translation occurs!
        /// </summary>
        [TestMethod]
        public void TestPartialSingleArrayReference()
        {
            ///
            /// BUild up an expression to do the functional query we are interested in seeing go
            /// 

            Expression<Func<SourceType4, SourceType4Container3>> loader = s => s.jets[0].specialIndex1;

            ///
            /// Do the translation
            /// 

            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(loader.Body, caches);

            Assert.AreEqual(loader.Body.ToString(), result.ToString(), "expression should have been untouched!");
        }

        [TestMethod]
        public void TestPartialDoubleArrayReference()
        {
            ///
            /// BUild up an expression to do the functional query we are interested in seeing go
            /// 

            Expression<Func<SourceType4, SourceType4Container2>> loader = s => s.jets[0].specialIndex3[0];

            ///
            /// Do the translation
            /// 

            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(loader.Body, caches);

            Assert.AreEqual(loader.Body.ToString(), result.ToString(), "expression should have been untouched!");
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

            List<string> caches = new List<string>();
            var result = TranslatingExpressionVisitor.Translate(originalExpression, caches);

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

        /// <summary>
        /// Helper classes for the next test.
        /// </summary>
        [TranslateToClass(typeof(TestSubExpressionBuriedArgumentClassDest))]
        public class TestSubExpressionBuriedArgumentClass
        {
            [TTreeVariableGrouping]
            public TestSubExpressionBuriedArgumentPVClass[] PVs;
        }

        public class TestSubExpressionBuriedArgumentPVClass
        {
            [TTreeVariableGrouping]
            public int nTracks;
        }

        public class TestSubExpressionBuriedArgumentClassDest
        {
            public TestSubExpressionBuriedArgumentClassDest(Expression h)
            { }
            public int[] nTracks;
        }

        [TestMethod]
        public void TestNoTranslateSubQueryExpression()
        {
            ////
            /// This is a regression we found in code. Bummer. There seems to be a problem with
            /// doing parameter replacement that is a subquery expression.
            /// 

            var cc = new CodeContext();

            ///
            /// Create the sub query expression that is
            /// d.PVs => First() - so take the first of an array.
            /// To create the sub query expression we need a query expression!
            /// We then thake .nTracks, and that becomes the argubment "v".
            /// 

            var q = new QueriableDummy<TestSubExpressionBuriedArgumentClass>();
            var result = (from d in q
                          select d.PVs.First()).Count();
            var qm = DummyQueryExectuor.LastQueryModel;
            var squery = qm.SelectClause.Selector as Remotion.Linq.Clauses.Expressions.SubQueryExpression;
            var ntracks = Expression.MakeMemberAccess(squery, typeof(TestSubExpressionBuriedArgumentPVClass).GetMember("nTracks").First());
            Console.WriteLine("The internal subquery ntrack expression: {0}", ntracks.ToString());
            Console.WriteLine("And the query model for the squery expression above is {0}.", squery.QueryModel.ToString());

            string initialDesc = ntracks.ToString();

            List<string> caches = new List<string>();
            var exprTans = TranslatingExpressionVisitor.Translate(ntracks, caches);
            Assert.AreEqual(initialDesc, exprTans.ToString(), "shouldn't have touched it");
        }
    }
}
