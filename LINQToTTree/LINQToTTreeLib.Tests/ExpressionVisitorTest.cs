// <copyright file="ExpressionVisitorTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.TypeHandlers.ROOT;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Data.Linq.Clauses;
using Remotion.Data.Linq.Clauses.Expressions;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for ExpressionVisitor</summary>
    [PexClass(typeof(ExpressionVisitor))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class ExpressionVisitorTest
    {
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

        /// <summary>Test stub for GetExpression(Expression, IGeneratedCode)</summary>
        [PexMethod]
        internal IValue GetExpression([PexAssumeNotNull]Expression expr, IGeneratedCode ce)
        {
            IValue result = ExpressionVisitor.GetExpression(expr, ce);
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
            new ConstantTestTemplate(){ ExpectedType=typeof(float), ExpectedValue="10", expr = Expression.Constant((float)10)},
            new ConstantTestTemplate(){ ExpectedType=typeof(double), ExpectedValue="10", expr = Expression.Constant((double)10)},
            new ConstantTestTemplate(){ ExpectedType=typeof(bool), ExpectedValue="true", expr = Expression.Constant(true)},
            new ConstantTestTemplate(){ ExpectedType=typeof(bool), ExpectedValue="false", expr = Expression.Constant(false)},
            new ConstantTestTemplate(){ ExpectedType=typeof(ROOTNET.NTH1F), ExpectedValue="LoadFromInputList\\<TH1F\\*\\>\\(\"NTH1F_.+\"\\)", expr = Expression.Constant(new ROOTNET.NTH1F("hi", "there", 10, 0.0, 20.0))}
        };

        [TestMethod]
        public void TestAllConstantExpressions()
        {
            MEFUtilities.AddPart(new TypeHandlerROOT());
            var t = new TypeHandlerCache();
            MEFUtilities.Compose(t);
            ExpressionVisitor.TypeHandlers = t;

            foreach (var expr in ConstantExpressionTestCases)
            {
                Variables.VarUtils._variableNameCounter = 0;
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
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.Equal, LHS=Expression.Constant(10), RHS=Expression.Constant(10), ExpectedType=typeof(bool), ExpectedValue="((int)10)==((int)10)"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.GreaterThan, LHS=Expression.Constant(10), RHS=Expression.Constant(10), ExpectedType=typeof(bool), ExpectedValue="((int)10)>((int)10)"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.GreaterThanOrEqual, LHS=Expression.Constant(10), RHS=Expression.Constant(10), ExpectedType=typeof(bool), ExpectedValue="((int)10)>=((int)10)"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.LessThan, LHS=Expression.Constant(10), RHS=Expression.Constant(10), ExpectedType=typeof(bool), ExpectedValue="((int)10)<((int)10)"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.LessThanOrEqual, LHS=Expression.Constant(10), RHS=Expression.Constant(10), ExpectedType=typeof(bool), ExpectedValue="((int)10)<=((int)10)"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.OrElse, LHS=Expression.Constant(false), RHS=Expression.Constant(true), ExpectedType=typeof(bool), ExpectedValue="((bool)false)||((bool)true)"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.AndAlso, LHS=Expression.Constant(false), RHS=Expression.Constant(true), ExpectedType=typeof(bool), ExpectedValue="((bool)false)&&((bool)true)"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.Add, LHS=Expression.Constant(10), RHS=Expression.Constant(20), ExpectedType=typeof(int), ExpectedValue="((int)10)+((int)20)"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.Subtract, LHS=Expression.Constant(10), RHS=Expression.Constant(20), ExpectedType=typeof(int), ExpectedValue="((int)10)-((int)20)"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.Multiply, LHS=Expression.Constant(10), RHS=Expression.Constant(20), ExpectedType=typeof(int), ExpectedValue="((int)10)*((int)20)"},
            new BinaryExpressionTestCase() { BinaryType= ExpressionType.Divide, LHS=Expression.Constant(10), RHS=Expression.Constant(20), ExpectedType=typeof(int), ExpectedValue="((int)10)/((int)20)"}
        };

        public void TestBinaryExpressionCase(BinaryExpressionTestCase c)
        {
            var e = Expression.MakeBinary(c.BinaryType, c.LHS, c.RHS);
            GeneratedCode g = new GeneratedCode();
            var r = ExpressionVisitor.GetExpression(e, g);
            CheckGeneratedCodeEmpty(g);
            Assert.AreEqual(c.ExpectedType, r.Type, "Expected type is incorrect");
            Assert.AreEqual(c.ExpectedValue, r.RawValue, "value is incorrect");
        }

        [TestMethod]
        public void TestBinaryExpression()
        {
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
            new UnaryTestCase() { UnaryType= ExpressionType.Negate, UnaryTarget=Expression.Constant(10), ExpectedType=typeof(int), ExpectedValue = "-((int)10)"},
            new UnaryTestCase() { UnaryType= ExpressionType.Not, UnaryTarget=Expression.Constant(true), ExpectedType=typeof(bool), ExpectedValue="!((bool)true)"},
            new UnaryTestCase() { UnaryType= ExpressionType.Convert, UnaryTarget=Expression.Constant(10), ConvertType=typeof(double), ExpectedType=typeof(double), ExpectedValue="((double)((int)10))"}
        };

        public void TestUnaryTestCase(UnaryTestCase u)
        {
            var e = Expression.MakeUnary(u.UnaryType, u.UnaryTarget, u.ConvertType);
            GeneratedCode g = new GeneratedCode();
            var r = ExpressionVisitor.GetExpression(e, g);
            CheckGeneratedCodeEmpty(g);
            Assert.AreEqual(u.ExpectedType, r.Type, "type not correct");
            Assert.AreEqual(u.ExpectedValue, r.RawValue, "resulting value not correct");
        }

        [TestMethod]
        public void TestUnary()
        {
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
        public void TestSubQueryReference()
        {
            QuerySourceReferenceExpression q = new QuerySourceReferenceExpression(new DummyQueryReference() { ItemName = "evt", ItemType = typeof(int) });
            GeneratedCode gc = new GeneratedCode();
            var r = ExpressionVisitor.GetExpression(q, gc);
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
#pragma warning restore 0169
        }

        [TestMethod]
        public void TestMember()
        {
            var e = Expression.Field(Expression.Variable(typeof(ntup), "d"), "run");
            GeneratedCode gc = new GeneratedCode();
            var r = ExpressionVisitor.GetExpression(e, gc);
            CheckGeneratedCodeEmpty(gc);
            Assert.AreEqual(typeof(int), r.Type, "incorrect type");
            Assert.AreEqual("(*d).run", r.RawValue, "incorrect reference");
        }

        [TestMethod]
        public void TestParameterSimple()
        {
            var e = Expression.Parameter(typeof(int), "p");
            GeneratedCode gc = new GeneratedCode();
            var r = ExpressionVisitor.GetExpression(e, gc);
            CheckGeneratedCodeEmpty(gc);
            Assert.AreEqual(typeof(int), r.Type, "type is not correct");
            Assert.AreEqual("p", r.RawValue, "raw value is not right");
        }

        [TestMethod]
        public void TestParameterPtr()
        {
            var e = Expression.Parameter(typeof(ntup), "p");
            GeneratedCode gc = new GeneratedCode();
            var r = ExpressionVisitor.GetExpression(e, gc);
            CheckGeneratedCodeEmpty(gc);
            Assert.AreEqual(typeof(ntup), r.Type, "type is not correct");
            Assert.AreEqual("p", r.RawValue, "raw value is not right");
        }

        [TestMethod]
        public void TestParameterSubstitution()
        {
            var e = Expression.Parameter(typeof(ntup), "p");
            GeneratedCode gc = new GeneratedCode();
            CodeContext cc = new CodeContext();
            cc.Add("p", new ValSimple("count", typeof(int)));
            var r = ExpressionVisitor.GetExpression(e, gc, cc);
            CheckGeneratedCodeEmpty(gc);
            Assert.AreEqual(typeof(ntup), r.Type, "type is not correct");
            Assert.AreEqual("count", r.RawValue, "raw value is not right");
        }

        [TestMethod]
        public void TestLambaBasic()
        {
            var laFunc = Expression.Lambda(Expression.MakeBinary(ExpressionType.Add,
                Expression.Constant(1),
                Expression.Constant(2)));

            GeneratedCode gc = new GeneratedCode();
            var result = ExpressionVisitor.GetExpression(laFunc, gc);
            CheckGeneratedCodeEmpty(gc);
            Assert.AreEqual(typeof(int), result.Type, "bad type came back");
            Assert.AreEqual("((int)1)+((int)2)", result.RawValue, "raw value was not right");
        }

        [TestMethod]
        public void TestLambaWithParams()
        {
            var laFunc = Expression.Lambda(Expression.MakeBinary(ExpressionType.Add,
                Expression.Parameter(typeof(int), "p"),
                Expression.Constant(2)));
            GeneratedCode gc = new GeneratedCode();
            var result = ExpressionVisitor.GetExpression(laFunc, gc);
            CheckGeneratedCodeEmpty(gc);
            Assert.AreEqual(typeof(int), result.Type, "bad type came back");
            Assert.AreEqual("((int)p)+((int)2)", result.RawValue, "raw value was not right");
        }
    }
}
