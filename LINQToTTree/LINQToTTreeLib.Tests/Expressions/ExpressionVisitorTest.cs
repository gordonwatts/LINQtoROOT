// <copyright file="ExpressionVisitorTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.ResultOperators;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.TypeHandlers.ROOT;
using LINQToTTreeLib.TypeHandlers.TranslationTypes;
using LINQToTTreeLib.Utils;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing.Structure;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for ExpressionVisitor</summary>
    [PexClass(typeof(ExpressionToCPP))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class ExpressionVisitorTest
    {
        [TestInitialize]
        public void Setup()
        {
            MEFUtilities.MyClassInit();
            MEFUtilities.AddPart(new ArrayArrayInfoFactory());
            MEFUtilities.AddPart(new SubQueryArrayTypeFactory());
            MEFUtilities.AddPart(new TranslatedArrayInfoFactory());
            MEFUtilities.AddPart(new HandleGroupType());
            MEFUtilities.AddPart(new SubQueryExpressionArrayInfoFactory());
        }

        [TestCleanup]
        public void Cleanup()
        {
            MEFUtilities.MyClassDone();
        }

        /// <summary>Test stub for GetExpression(Expression, IGeneratedCode)</summary>
        [PexMethod]
        internal IValue GetExpression([PexAssumeNotNull]Expression expr, IGeneratedQueryCode ce)
        {
            IValue result = ExpressionToCPP.GetExpression(expr, ce, null, MEFUtilities.MEFContainer);
            return result;
        }

        public class ConstantTestTemplate
        {
            public ConstantExpression expr;
            public Type ExpectedType;
            public string ExpectedValue;
        }

        public void TestConstantExpression(ConstantTestTemplate myTest)
        {
            GeneratedCode g = new GeneratedCode();
            var r = GetExpression(myTest.expr, g);
            CheckGeneratedCodeEmpty(g);
            Assert.AreEqual(myTest.ExpectedType, r.Type, "Type incorrect");
            Regex reg = new Regex(myTest.ExpectedValue);
            var m = reg.Match(r.RawValue);
            Assert.IsTrue(m.Success, "Raw value is incorrect (expected:" + myTest.ExpectedValue + " actual:" + r.RawValue + ")");
        }

        public static List<ConstantTestTemplate> ConstantExpressionTestCases = new List<ConstantTestTemplate>()
        {
            new ConstantTestTemplate(){ ExpectedType=typeof(int), ExpectedValue="10", expr = Expression.Constant(10)},
            new ConstantTestTemplate(){ ExpectedType=typeof(float), ExpectedValue="10.0", expr = Expression.Constant((float)10)},
            new ConstantTestTemplate(){ ExpectedType=typeof(double), ExpectedValue="10.0", expr = Expression.Constant((double)10)},
            new ConstantTestTemplate(){ ExpectedType=typeof(bool), ExpectedValue="true", expr = Expression.Constant(true)},
            new ConstantTestTemplate(){ ExpectedType=typeof(bool), ExpectedValue="false", expr = Expression.Constant(false)},
            new ConstantTestTemplate(){ ExpectedType=typeof(ROOTNET.NTH1F), ExpectedValue="LoadFromInputList\\<TH1F\\*\\>\\(\"aNTH1F_.+\"\\)", expr = Expression.Constant(new ROOTNET.NTH1F("hi", "there", 10, 0.0, 20.0))},
            new ConstantTestTemplate(){ ExpectedType=typeof(string), ExpectedValue="dude", expr = Expression.Constant("dude")}
        };

        [TestMethod]
        public void TestAllConstantExpressions()
        {
            MEFUtilities.AddPart(new TypeHandlerROOT());
            var t = new TypeHandlerCache();
            MEFUtilities.Compose(t);

            foreach (var expr in ConstantExpressionTestCases)
            {
                Utils.TypeUtils._variableNameCounter = 0;
                TestConstantExpression(expr);
            }
        }

        public class BinaryExpressionTestCase
        {
            public ExpressionType BinaryType;
            public Expression LHS;
            public Expression RHS;
            public string ExpectedValue;
            public Type ExpectedType;
        }

        List<BinaryExpressionTestCase> BinaryTestCases = new List<BinaryExpressionTestCase>()
        {
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.Equal, LHS=Expression.Constant(10), RHS=Expression.Constant(10), ExpectedType=typeof(bool), ExpectedValue="10==10"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.GreaterThan, LHS=Expression.Constant(10), RHS=Expression.Constant(10), ExpectedType=typeof(bool), ExpectedValue="10>10"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.GreaterThanOrEqual, LHS=Expression.Constant(10), RHS=Expression.Constant(10), ExpectedType=typeof(bool), ExpectedValue="10>=10"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.LessThan, LHS=Expression.Constant(10), RHS=Expression.Constant(10), ExpectedType=typeof(bool), ExpectedValue="10<10"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.LessThanOrEqual, LHS=Expression.Constant(10), RHS=Expression.Constant(10), ExpectedType=typeof(bool), ExpectedValue="10<=10"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.OrElse, LHS=Expression.Constant(false), RHS=Expression.Constant(true), ExpectedType=typeof(bool), ExpectedValue="false||true"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.AndAlso, LHS=Expression.Constant(false), RHS=Expression.Constant(true), ExpectedType=typeof(bool), ExpectedValue="false&&true"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.Add, LHS=Expression.Constant(10), RHS=Expression.Constant(20), ExpectedType=typeof(int), ExpectedValue="10+20"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.Subtract, LHS=Expression.Constant(10), RHS=Expression.Constant(20), ExpectedType=typeof(int), ExpectedValue="10-20"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.Multiply, LHS=Expression.Constant(10), RHS=Expression.Constant(20), ExpectedType=typeof(int), ExpectedValue="10*20"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.Divide, LHS=Expression.Constant(10), RHS=Expression.Constant(20), ExpectedType=typeof(int), ExpectedValue="10/20"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.Divide, LHS=Expression.Constant(10.0, typeof(double)), RHS=Expression.Constant(20.0, typeof(double)), ExpectedType=typeof(double), ExpectedValue="10.0/20.0"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.Divide, LHS=Expression.MakeBinary(ExpressionType.Add, Expression.Constant(10), Expression.Constant(20)), RHS=Expression.Constant(30), ExpectedType=typeof(int), ExpectedValue="(10+20)/30"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.Modulo, LHS=Expression.Constant(10), RHS=Expression.Constant(2), ExpectedType=typeof(int), ExpectedValue="10%2"}
        };

        public void TestBinaryExpressionCase(BinaryExpressionTestCase c)
        {
            var e = Expression.MakeBinary(c.BinaryType, c.LHS, c.RHS);
            GeneratedCode g = new GeneratedCode();
            var r = ExpressionToCPP.GetExpression(e, g, null, MEFUtilities.MEFContainer);
            CheckGeneratedCodeEmpty(g);
            Assert.AreEqual(c.ExpectedType, r.Type, "Expected type is incorrect");
            Assert.AreEqual(c.ExpectedValue, r.RawValue, "value is incorrect");
        }

        [TestMethod]
        public void TestBinaryExpression()
        {
            var t = new TypeHandlerCache();
            MEFUtilities.Compose(t);
            foreach (var c in BinaryTestCases)
            {
                TestBinaryExpressionCase(c);
            }
        }

        public class UnaryTestCase
        {
            public ExpressionType UnaryType;
            public Expression UnaryTarget;
            public Type ConvertType = null;
            public Type ExpectedType;
            public string ExpectedValue;
        };

        List<UnaryTestCase> UnaryTests = new List<UnaryTestCase>()
        {
            new UnaryTestCase() { UnaryType= ExpressionType.Negate, UnaryTarget=Expression.Constant(10), ExpectedType=typeof(int), ExpectedValue = "-10"},
            new UnaryTestCase() { UnaryType= ExpressionType.Negate, UnaryTarget=Expression.MakeBinary(ExpressionType.Add, Expression.Constant(5), Expression.Constant(10)), ExpectedType=typeof(int), ExpectedValue = "-(5+10)"},
            new UnaryTestCase() { UnaryType= ExpressionType.Not, UnaryTarget=Expression.Constant(true), ExpectedType=typeof(bool), ExpectedValue="!true"},
            new UnaryTestCase() { UnaryType= ExpressionType.Convert, UnaryTarget=Expression.Constant(10), ConvertType=typeof(double), ExpectedType=typeof(double), ExpectedValue="((double)10)"},
            new UnaryTestCase() { UnaryType= ExpressionType.Convert, UnaryTarget=Expression.Parameter(typeof(int), "dude"), ConvertType=typeof(double), ExpectedType=typeof(double), ExpectedValue="((double)dude)"}
        };

        public void TestUnaryTestCase(UnaryTestCase u)
        {
            var e = Expression.MakeUnary(u.UnaryType, u.UnaryTarget, u.ConvertType);
            GeneratedCode g = new GeneratedCode();
            var r = ExpressionToCPP.GetExpression(e, g, null, MEFUtilities.MEFContainer);
            CheckGeneratedCodeEmpty(g);
            Assert.AreEqual(u.ExpectedType, r.Type, "type not correct");
            Assert.AreEqual(u.ExpectedValue, r.RawValue, "resulting value not correct");
        }

        [TestMethod]
        public void TestUnary()
        {
            var t = new TypeHandlerCache();
            MEFUtilities.Compose(t);
            foreach (var u in UnaryTests)
            {
                TestUnaryTestCase(u);
            }
        }

        public class DummyQueryReference : IQuerySource
        {
            public string ItemName { get; set; }
            public Type ItemType { get; set; }
        };

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void TestSubQueryReferenceWhenNotDefined()
        {
            QuerySourceReferenceExpression q = new QuerySourceReferenceExpression(new DummyQueryReference() { ItemName = "evt", ItemType = typeof(int) });
            GeneratedCode gc = new GeneratedCode();
            var r = ExpressionToCPP.GetExpression(q, gc, null, null);
        }

        [TestMethod]
        public void TestSubQueryReference()
        {
            QuerySourceReferenceExpression q = new QuerySourceReferenceExpression(new DummyQueryReference() { ItemName = "evt", ItemType = typeof(int) });
            GeneratedCode gc = new GeneratedCode();
            var cc = new CodeContext();
            cc.Add(q.ReferencedQuerySource, Expression.Parameter(typeof(int), "evt"));
            var r = ExpressionToCPP.GetExpression(q, gc, cc, null);
            CheckGeneratedCodeEmpty(gc);
            Assert.AreEqual(typeof(int), r.Type, "incorrect type");
            Assert.AreEqual("evt", r.RawValue, "expansion incorrect");
        }

        /// <summary>
        /// Make sure the value that come back here is dead!
        /// </summary>
        /// <param name="g"></param>
        private void CheckGeneratedCodeEmpty(GeneratedCode g)
        {
            Assert.AreEqual(0, g.CodeBody.DeclaredVariables.Count(), "There should be no declared variables");
            Assert.AreEqual(0, g.CodeBody.Statements.Count(), "There should be no statements");
        }

        class ntup
        {
#pragma warning disable 0169
            int run;
            IEnumerable<int> numbers;
#pragma warning restore 0169
        }

        [TestMethod]
        public void TestMember()
        {
            var e = Expression.Field(Expression.Variable(typeof(ntup), "d"), "run");
            GeneratedCode gc = new GeneratedCode();
            MEFUtilities.Compose(new TypeHandlerCache());
            var r = ExpressionToCPP.GetExpression(e, gc, null, MEFUtilities.MEFContainer);
            CheckGeneratedCodeEmpty(gc);
            Assert.AreEqual(typeof(int), r.Type, "incorrect type");
            Assert.AreEqual("(*d).run", r.RawValue, "incorrect reference");
        }

        [TestMethod]
        public void TestMemberAndRefereceRecorded()
        {
            var e = Expression.Field(Expression.Variable(typeof(ntup), "d"), "run");
            GeneratedCode gc = new GeneratedCode();
            MEFUtilities.Compose(new TypeHandlerCache());
            var r = ExpressionToCPP.GetExpression(e, gc, null, MEFUtilities.MEFContainer);
            var refLeaves = gc.ReferencedLeafNames.ToArray();
            Assert.AreEqual(1, refLeaves.Length, "# of referenced leaves is incorrect");
            Assert.AreEqual("run", refLeaves[0], "Referenced leaf name incorrect");
        }

        [TestMethod]
        public void TestMemberEnumerable()
        {
            var e = Expression.Field(Expression.Variable(typeof(ntup), "d"), "numbers");
            GeneratedCode gc = new GeneratedCode();
            MEFUtilities.Compose(new TypeHandlerCache());
            var r = ExpressionToCPP.GetExpression(e, gc, null, MEFUtilities.MEFContainer);
            CheckGeneratedCodeEmpty(gc);
            Assert.AreEqual(typeof(IEnumerable<int>), r.Type, "incorrect type");
            Assert.AreEqual("(*d).numbers", r.RawValue, "incorrect reference");
        }

        [TestMethod]
        public void TestParameterSimple()
        {
            var e = Expression.Parameter(typeof(int), "p");
            GeneratedCode gc = new GeneratedCode();
            var r = ExpressionToCPP.GetExpression(e, gc, null, null);
            CheckGeneratedCodeEmpty(gc);
            Assert.AreEqual(typeof(int), r.Type, "type is not correct");
            Assert.AreEqual("p", r.RawValue, "raw value is not right");
        }

        [TestMethod]
        public void TestParameterPtr()
        {
            var e = Expression.Parameter(typeof(ntup), "p");
            GeneratedCode gc = new GeneratedCode();
            var r = ExpressionToCPP.GetExpression(e, gc, null, null);
            CheckGeneratedCodeEmpty(gc);
            Assert.AreEqual(typeof(ntup), r.Type, "type is not correct");
            Assert.AreEqual("p", r.RawValue, "raw value is not right");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestParameterSubstitutionIncompatibleTypes()
        {
            var e = Expression.Parameter(typeof(ntup), "p");
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            cc.Add("p", Expression.Parameter(typeof(int), "count"));
            var r = ExpressionToCPP.GetExpression(e, gc, cc, null);
        }

        [TestMethod]
        public void TestParameterSubstitutionOk()
        {
            var e = Expression.Parameter(typeof(ntup), "p");
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            cc.Add("p", Expression.Parameter(typeof(ntup), "count"));
            var r = ExpressionToCPP.GetExpression(e, gc, cc, null);
            CheckGeneratedCodeEmpty(gc);
            Assert.AreEqual(typeof(ntup), r.Type, "type is not correct");
            Assert.AreEqual("count", r.RawValue, "raw value is not right");
        }

        [TestMethod]
        public void TestLambaBasic()
        {
            var t = new TypeHandlerCache();
            MEFUtilities.Compose(t);
            var laFunc = Expression.Lambda(Expression.MakeBinary(ExpressionType.Add,
                Expression.Constant(1),
                Expression.Constant(2)));

            GeneratedCode gc = new GeneratedCode();
            var result = ExpressionToCPP.GetExpression(laFunc, gc, null, MEFUtilities.MEFContainer);
            CheckGeneratedCodeEmpty(gc);
            Assert.AreEqual(typeof(int), result.Type, "bad type came back");
            Assert.AreEqual("1+2", result.RawValue, "raw value was not right");
        }

        [TestMethod]
        public void TestLambaWithParams()
        {
            var t = new TypeHandlerCache();
            MEFUtilities.Compose(t);
            var laFunc = Expression.Lambda(Expression.MakeBinary(ExpressionType.Add,
                Expression.Parameter(typeof(int), "p"),
                Expression.Constant(2)));
            GeneratedCode gc = new GeneratedCode();
            var result = ExpressionToCPP.GetExpression(laFunc, gc, null, MEFUtilities.MEFContainer);
            CheckGeneratedCodeEmpty(gc);
            Assert.AreEqual(typeof(int), result.Type, "bad type came back");
            Assert.AreEqual("p+2", result.RawValue, "raw value was not right");
        }

        [TranslateToClass(typeof(transToNtup))]
        public class toTransNtupe
        {
            [RenameVariable("rVal")]
            public int[] val;

        }

        public class transToNtup : IExpressionHolder
        {
            public transToNtup(Expression holder)
            {
                HeldExpression = holder;
            }
            public int[] rVal;

            public Expression HeldExpression { get; set; }
        }

        [TestMethod]
        public void TestTrivialTranslation()
        {
            var model = GetModel(() => (from q in new QueriableDummy<toTransNtupe>() select q.val.Count()));
            var expr = model.SelectClause.Selector;

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(toTransNtupe) };
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));

            cc.Add(model.MainFromClause, Expression.Parameter(typeof(toTransNtupe), "q"));

            var result = ExpressionToCPP.GetExpression(expr, gc, cc, MEFUtilities.MEFContainer);
            Assert.AreEqual(typeof(int), result.Type, "bad type for return");
        }

        public class dummyntup
        {
            public int run;
            public int[] vals;
        }

        private QueryModel GetModel<T>(Expression<Func<T>> expr)
        {
            var parser = QueryParser.CreateDefault();
            return parser.GetParsedQuery(expr.Body);
        }

        [TestMethod]
        public void TestSimpleSubQuery()
        {
            var model = GetModel(() => (from q in new QueriableDummy<dummyntup>() select q.vals.Count()).Count());
            var expr = model.SelectClause.Selector as SubQueryExpression;

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(dummyntup) };
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));

            cc.Add(model.MainFromClause, Expression.Parameter(typeof(dummyntup), "q"));

            var result = ExpressionToCPP.GetExpression(expr, gc, cc, MEFUtilities.MEFContainer);
            gc.DumpCodeToConsole();

            Assert.AreEqual(typeof(int), result.Type, "bad type for return");

            ///
            /// Make sure that the aint1 has been declared in the body of the code, and that there are some statements.
            /// The top level statement should be a loop over whatever it is we are looping over!
            /// 

            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "Expect only the loop statement");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.First(), typeof(IBookingStatementBlock), "Incorrect looping statement");
            Assert.IsFalse(gc.CodeBody.Statements.First().CodeItUp().First().Contains("<generated>"), "Contains a funny variable name: " + gc.CodeBody.Statements.First().CodeItUp().First());
            Assert.AreEqual(1, gc.CodeBody.DeclaredVariables.Count(), "Expected one declared variable");

            ///
            /// Next, make sure if we add a statement it goes where we think it should - after teh stuff that has been added,
            /// not inside it.
            /// 

            gc.Add(new Statements.StatementSimpleStatement("dude"));
            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "Scope has not been reset");

            Assert.AreEqual(0, cc.NumberOfParams, "Impromper # of parameter replacements left over");
        }

        [TestMethod]
        public void TestSimpleSubQueryWithAddon()
        {
            var model = GetModel(() => (from q in new QueriableDummy<dummyntup>() select q.vals.Where(v => v > 20).Count()).Count());
            var expr = model.SelectClause.Selector as SubQueryExpression;

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext() { BaseNtupleObjectType = typeof(dummyntup) };
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));

            cc.Add(model.MainFromClause, Expression.Parameter(typeof(dummyntup), "q"));

            var result = ExpressionToCPP.GetExpression(expr, gc, cc, MEFUtilities.MEFContainer);

            ///
            /// Next, go after the code that comes back and make sure the if statement for the > 20 actually makes sense.
            /// 

            var loop = gc.CodeBody.Statements.First() as IBookingStatementBlock;
            Assert.IsNotNull(loop, "Loop statement not found");
            Assert.AreEqual(1, loop.Statements.Count(), "Expected one sub-statement");
            Assert.IsInstanceOfType(loop.Statements.First(), typeof(Statements.StatementFilter), "bad if statement");
        }

        private static IValue RunArrayLengthOnExpression(Expression arrayLenLambda, Type expectedType)
        {
            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));

            var result = ExpressionToCPP.GetExpression(arrayLenLambda, gc, cc, MEFUtilities.MEFContainer);

            Assert.IsNotNull(result, "result");
            Assert.AreEqual(expectedType, result.Type, "result type");
            return result;
        }

        class ResultType0
        {
#pragma warning disable 0649
            public int[] val1;
            public int n;
            [ArraySizeIndex("n")]
            public int[] val2;
            [ArraySizeIndex("20", IsConstantExpression = true)]
            public int[] val3;
            [ArraySizeIndex("20", IsConstantExpression = true, Index = 0)]
            [ArraySizeIndex("30", IsConstantExpression = true, Index = 1)]
            public int[][] val4;
#pragma warning restore 0649
        }

        [TestMethod]
        public void TestClassArraySize()
        {
            Expression<Func<ResultType0, int>> arrayLenLambda = arr => arr.val1.Length;
            var result = RunArrayLengthOnExpression(arrayLenLambda, typeof(int));
            Assert.AreEqual("(*(*arr).val1).size()", result.RawValue, "actual translation incorrect");
        }

        [TestMethod]
        public void TestClassArrayCPPConstAccess()
        {
            Expression<Func<ResultType0, int>> arrayLenLambda = arr => arr.val3[5];
            var result = RunArrayLengthOnExpression(arrayLenLambda, typeof(int));
            Assert.AreEqual("(*(*arr).val3)[5]", result.RawValue, "lookup translation incorrect");
        }

        [TestMethod]
        public void TestClassArrayCPPSize()
        {
            Expression<Func<ResultType0, int>> arrayLenLambda = arr => arr.val2.Length;
            var result = RunArrayLengthOnExpression(arrayLenLambda, typeof(int));
            Assert.AreEqual("(*arr).n", result.RawValue, "actual translation incorrect");
        }

        [TestMethod]
        public void TestClassArrayCPPConstSize()
        {
            Expression<Func<ResultType0, int>> arrayLenLambda = arr => arr.val3.Length;
            var result = RunArrayLengthOnExpression(arrayLenLambda, typeof(int));
            Assert.AreEqual("20", result.RawValue, "actual translation incorrect");
        }

        [TestMethod]
        public void TestClassArrayCPPAccess()
        {
            Expression<Func<ResultType0, int>> arrayLenLambda = arr => arr.val3[5];
            var result = RunArrayLengthOnExpression(arrayLenLambda, typeof(int));
            Assert.AreEqual("(*(*arr).val3)[5]", result.RawValue, "lookup translation incorrect");
        }

        [TestMethod]
        public void TestClassArrayCPP2D1stAcces()
        {
            // Index == 0
            Expression<Func<ResultType0, int>> arrayLenLambda = arr => arr.val4[1][2];
            var result = RunArrayLengthOnExpression(arrayLenLambda, typeof(int));
            Assert.AreEqual("((*(*arr).val4)[1])[2]", result.RawValue, "2d array access");
        }

        [TestMethod]
        public void TestClassArrayCPP2D1st()
        {
            // Index == 0
            Expression<Func<ResultType0, int>> arrayLenLambda = arr => arr.val4.Length;
            var result = RunArrayLengthOnExpression(arrayLenLambda, typeof(int));
            Assert.AreEqual("20", result.RawValue, "index 0 size");
        }

        [TestMethod]
        public void TestClassArrayCPP2D2nd()
        {
            // Index == 1
            Expression<Func<ResultType0, int>> arrayLenLambda1 = arr => arr.val4[0].Length;
            var result = RunArrayLengthOnExpression(arrayLenLambda1, typeof(int));
            Assert.AreEqual("30", result.RawValue, "index 1 size");

        }

        [TestMethod]
        public void TestSimpleArrayLengthReference()
        {
            Expression<Func<ResultType0, int>> arrayLenLambda = arr => arr.val1.Length;

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));

            ExpressionToCPP.GetExpression(arrayLenLambda, gc, cc, MEFUtilities.MEFContainer);

            var refVars = gc.ReferencedLeafNames.ToArray();
            Assert.AreEqual(1, refVars.Length, "# of referenced leaves");
            Assert.AreEqual("val1", refVars[0], "Name of referenced leaf");
        }

        // THis is a vector that is burried in two sub-classes that are generated by the TTree structure.
        class ResultType2TClonesArray
        {
#pragma warning disable 0649
            [NotAPointer]
            public int[] arr;
#pragma warning restore 0649
        }

        class ResultType2TBase
        {
#pragma warning disable 0649
            [NotAPointer]
            public ResultType2TClonesArray arrholder;
#pragma warning restore 0649
        }

        class ResultType2
        {
#pragma warning disable 0649
            [NotAPointer]
            public ResultType2TBase bs;
#pragma warning restore 0649
        }

        [TestMethod]
        public void TestTCANormalObjectLength()
        {
            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            Expression<Func<ResultType2, int>> arrayLenLambda = q => q.bs.arrholder.arr.Length;

            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));
            var r = ExpressionToCPP.GetExpression(arrayLenLambda, gc, cc, MEFUtilities.MEFContainer);
            Assert.AreEqual("(*q).bs.arrholder.arr.size()", r.RawValue, "Array length fo a tclones array");
        }

        [TestMethod]
        public void TestTCANormalObjectAccess()
        {
            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            Expression<Func<ResultType2, int>> arrayLenLambda = q => q.bs.arrholder.arr[0];

            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));
            var r = ExpressionToCPP.GetExpression(arrayLenLambda, gc, cc, MEFUtilities.MEFContainer);
            Assert.AreEqual("((*q).bs.arrholder.arr).at(0)", r.RawValue, "Array length fo a tclones array");
        }

        // A true TClonesArray structure is built here, with the guys in it being arrays but collected
        // grouped objects (as one might expect).
        [TClonesArrayImpliedClass]
        class ResultType3TClonesArray
        {
#pragma warning disable 0649
            [NotAPointer]
            public int[] arr;
            [NotAPointer]
            public int[][] arr2D;
            [NotAPointer]
            [ArraySizeIndex("5", IsConstantExpression = true, Index = 1)]
            public int[][] arr2DConst;
#pragma warning restore 0649
        }

        class ResultType3TBase
        {
#pragma warning disable 0649
            [NotAPointer]
            public ResultType3TClonesArray arrholder;
#pragma warning restore 0649
        }

        class ResultType3
        {
#pragma warning disable 0649
            [NotAPointer]
            public ResultType3TBase bs;
#pragma warning restore 0649
        }

        class ResultTypeTLZ
        {
#pragma warning disable 0649
            public ROOTNET.NTLorentzVector[] val1;
#pragma warning restore 0649
        }

        [TestMethod]
        public void TestTCALength()
        {
            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());

            Expression<Func<ResultType3, int>> arrayLenLambda = q => q.bs.arrholder.arr.Length;

            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));
            var r = ExpressionToCPP.GetExpression(arrayLenLambda, gc, cc, MEFUtilities.MEFContainer);
            Assert.AreEqual("(*q).bs.arrholder.GetEntries()", r.RawValue, "Array length fo a tclones array");
        }

        [TestMethod]
        public void TeatACAAccess()
        {
            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());

            Expression<Func<ResultType3, int>> arrayLenLambda = q => q.bs.arrholder.arr[0];

            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));
            var r = ExpressionToCPP.GetExpression(arrayLenLambda, gc, cc, MEFUtilities.MEFContainer);
            Assert.AreEqual("((*q).bs.arrholder.arr)[0]", r.RawValue, "Array length fo a tclones array");
        }

        [TestMethod]
        public void TeatACA2DAccess()
        {
            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());

            Expression<Func<ResultType3, int>> arrayLenLambda = q => q.bs.arrholder.arr2D[0][1];

            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));
            var r = ExpressionToCPP.GetExpression(arrayLenLambda, gc, cc, MEFUtilities.MEFContainer);
            Assert.AreEqual("(((*q).bs.arrholder.arr2D)[0]).at(1)", r.RawValue, "Array length fo a tclones array");
        }

        [TestMethod]
        public void TeatACA2DConstLength()
        {
            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());

            Expression<Func<ResultType3, int>> arrayLenLambda = q => q.bs.arrholder.arr2DConst[0].Length;

            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));
            var r = ExpressionToCPP.GetExpression(arrayLenLambda, gc, cc, MEFUtilities.MEFContainer);
            Assert.AreEqual("5", r.RawValue, "Array length fo a tclones array");
        }

        [TestMethod]
        public void TestObjectLeafLengthReference()
        {
            Expression<Func<ResultTypeTLZ, int>> arrayLenLambda = arr => arr.val1.Length;

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));

            ExpressionToCPP.GetExpression(arrayLenLambda, gc, cc, MEFUtilities.MEFContainer);

            var refVars = gc.ReferencedLeafNames.ToArray();
            Assert.AreEqual(1, refVars.Length, "# of referenced leaves");
            Assert.AreEqual("val1", refVars[0], "Name of referenced leaf");
        }

        [TestMethod]
        public void TestObjectLeafReference()
        {
            Expression<Func<ResultTypeTLZ, int, double>> arrayLenLambda = (arr, index) => arr.val1[index].Pt();

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            MEFUtilities.AddPart(new TypeHandlerROOT());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));

            ExpressionToCPP.GetExpression(arrayLenLambda, gc, cc, MEFUtilities.MEFContainer);

            var refVars = gc.ReferencedLeafNames.ToArray();
            Assert.AreEqual(1, refVars.Length, "# of referenced leaves");
            Assert.AreEqual("val1", refVars[0], "Name of referenced leaf");
        }

        [TestMethod]
        public void TestArrayReferenceRecorded()
        {
            Expression<Func<ResultType0, int, int>> arrayLenLambda = (arr, index) => arr.val1[index];

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));

            ExpressionToCPP.GetExpression(arrayLenLambda, gc, cc, MEFUtilities.MEFContainer);

            var refVars = gc.ReferencedLeafNames.ToArray();
            Assert.AreEqual(1, refVars.Length, "# of referenced leaves");
            Assert.AreEqual("val1", refVars[0], "Name of referenced leaf");
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
        public void TestTranslationWithNestedAnonymousObject()
        {
            //var model = GetModel(() => (from q in new QueriableDummy<SourceType1>()
            //                            from j in q.jets
            //                            select new { Jet = j }.Jet.val1).Count());
            Expression<Func<SourceType1, bool>> exprL = s => new { Jet = new { Bogus = s.jets[0] }.Bogus }.Jet.val1 > 5;
            var expr = (exprL as LambdaExpression).Body;

            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));

            var result = ExpressionToCPP.GetExpression(expr, gc, cc, MEFUtilities.MEFContainer);
            Assert.AreEqual(typeof(bool), result.Type, "bad type for return");
        }

        [TestMethod]
        public void TestRenamedArrayLength()
        {
            /// There are extensive translation tests in the TranslationExpressionVisitor test - so we just need to
            /// make sure at least one case goes all the way through. Since it uses that code, all the cases covered
            /// in the TranslationExpressionVisitor object should take care of the rest. Fingers crossed! :-)

            Expression<Func<SourceType1, int>> arrayLenLambda = arr => arr.jets.Length;
            var result = RunArrayLengthOnExpression(arrayLenLambda, typeof(int));
            Assert.AreEqual("(*(*arr).val1).size()", result.RawValue, "actual translation incorrect");
        }

        [TestMethod]
        public void TestParameterReplacement()
        {
            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));

            var expr = Expression.Variable(typeof(int), "d");

            cc.Add("d", Expression.Constant(20));

            var result = ExpressionToCPP.GetExpression(expr, gc, cc, MEFUtilities.MEFContainer);

            Assert.IsNotNull(result, "result");
            Assert.AreEqual(typeof(int), result.Type, "result type");
            Assert.AreEqual("20", result.RawValue, "raw value");
        }

        [TestMethod]
        public void TestSimpleArrayAccess()
        {
            var myvar = Expression.Variable(typeof(int[]), "d");
            var myaccess = Expression.ArrayIndex(myvar, Expression.Constant(1));

            var result = RunArrayLengthOnExpression(myaccess, typeof(int));
            Assert.AreEqual("(*d).at(1)", result.RawValue, "C++ incorrectly translated");
        }

        [TestMethod]
        public void Test2DArrayAccess()
        {
            var myvar = Expression.Variable(typeof(int[][]), "d");
            var myaccess1 = Expression.ArrayIndex(myvar, Expression.Constant(1));
            var myaccess = Expression.ArrayIndex(myaccess1, Expression.Constant(2));

            var result = RunArrayLengthOnExpression(myaccess, typeof(int));
            Assert.AreEqual("((*d).at(1)).at(2)", result.RawValue, "C++ incorrectly translated");
        }

        [TestMethod]
        public void Test2DArrayLength()
        {
            var myvar = Expression.Variable(typeof(int[][]), "d");
            var myaccess1 = Expression.ArrayIndex(myvar, Expression.Constant(1));
            var myArrayLength = Expression.ArrayLength(myaccess1);

            var result = RunArrayLengthOnExpression(myArrayLength, typeof(int));
            Assert.AreEqual("(*d).at(1).size()", result.RawValue, "C++ incorrectly translated");
        }

        [TestMethod]
        public void Test2DArrayLength1DLevel()
        {
            var myvar = Expression.Variable(typeof(int[][]), "d");
            var myArrayLength = Expression.ArrayLength(myvar);

            var result = RunArrayLengthOnExpression(myArrayLength, typeof(int));
            Assert.AreEqual("(*d).size()", result.RawValue, "C++ incorrectly translated");
        }

        [TestMethod]
        public void TestComplexObjectArrayAccess()
        {
            var myarray = Expression.Variable(typeof(ROOTNET.NTH1F[]), "harr");
            var myaccess = Expression.ArrayIndex(myarray, Expression.Constant(1));

            var result = RunArrayLengthOnExpression(myaccess, typeof(ROOTNET.NTH1F));
            Assert.AreEqual("(*harr).at(1)", result.RawValue, "C++ th1f not translated correctly");
        }

        class ObjectArrayTest
        {
#pragma warning disable 0649
            public int[] arr;
#pragma warning restore 0649
        }

        [TestMethod]
        public void TestObjectArrayAccess()
        {
            var arr = Expression.Variable(typeof(ObjectArrayTest), "obj");
            var arrMember = Expression.MakeMemberAccess(arr, typeof(ObjectArrayTest).GetMember("arr").First());
            var arrayIndex = Expression.ArrayIndex(arrMember, Expression.Constant(1));

            var result = RunArrayLengthOnExpression(arrayIndex, typeof(int));

            Assert.AreEqual("(*(*obj).arr).at(1)", result.RawValue, "array text");
        }

        [TestMethod]
        public void TestLambdaCallFunction()
        {
            /// Test a call that looks like v => v+1.

            Expression<Func<int, int>> incr = v => v + 1;
            var invoke = Expression.Invoke(incr, Expression.Variable(typeof(int), "d"));

            var result = CallBasicGetExpression(invoke);

            Assert.AreEqual(typeof(int), result.Type, "type");
            Assert.AreEqual("d+1", result.RawValue, "raw value");
        }

        [TestMethod]
        public void TestNestedLambaCallFunction()
        {
            Expression<Func<int, int>> incr = v => v + 1;
            var invoke = Expression.Invoke(incr, Expression.Variable(typeof(int), "d"));

            var invoke2 = Expression.Invoke(incr, invoke);

            var result = CallBasicGetExpression(invoke2);

            Assert.AreEqual(typeof(int), result.Type, "type");
            Assert.AreEqual("(d+1)+1", result.RawValue, "raw value");
        }

        /// <summary>
        /// Does some simple work to get an expression
        /// </summary>
        /// <param name="invoke"></param>
        private static IValue CallBasicGetExpression(InvocationExpression invoke)
        {
            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.AddPart(new ROCount());
            MEFUtilities.AddPart(new TypeHandlerCache());
            MEFUtilities.AddPart(new TypeHandlerTranslationClass());
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            MEFUtilities.Compose(new QueryVisitor(gc, cc, MEFUtilities.MEFContainer));

            return ExpressionToCPP.GetExpression(invoke, gc, cc, MEFUtilities.MEFContainer);
        }

        class ResultIAT : IExpressionHolder
        {
            public ResultIAT(Expression holder)
            {
                HeldExpression = holder;
            }
#pragma warning disable 0649
            public float[] values;
#pragma warning restore 0649

            public Expression HeldExpression { get; private set; }
        }

        class SourceMuonsIAT
        {
            [TTreeVariableGrouping()]
#pragma warning disable 0649
            public float values;
#pragma warning restore 0649
        }

        [TranslateToClass(typeof(ResultIAT))]
        class SourceIAT
        {
            [TTreeVariableGrouping()]
#pragma warning disable 0649
            public SourceMuonsIAT[] muons;
#pragma warning restore 0649
        }

        [TestMethod]
        public void TestInvokeAccrossTranslation()
        {
            ///
            /// Make sure invoke works when the arguments and the resolved item ahve to be put together
            /// in order to get the full story.
            /// 

            Expression<Func<SourceMuonsIAT, float>> accessor = m => m.values;

            var sourceVar = Expression.Variable(typeof(SourceIAT), "main");
            var allmuons = Expression.MakeMemberAccess(sourceVar, typeof(SourceIAT).GetMember("muons").First());
            var oneMuon = Expression.ArrayIndex(allmuons, Expression.Constant(1));

            var invoke = Expression.Invoke(accessor, oneMuon);

            var result = CallBasicGetExpression(invoke);
            Console.WriteLine("result: " + result.RawValue);

            Assert.IsFalse(result.RawValue.Contains("muons"), "Result should not reference muons: " + result.RawValue);
        }
    }
}
