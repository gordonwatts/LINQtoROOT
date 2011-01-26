// <copyright file="ROAggregateTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.TypeHandlers.ROOT;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses.ResultOperators;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>This class contains parameterized unit tests for ROAggregate</summary>
    [PexClass(typeof(ROAggregate))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class ROAggregateTest
    {
        /// <summary>Test stub for CanHandle(Type)</summary>
        [PexMethod]
        public bool CanHandle([PexAssumeUnderTest]ROAggregate target, Type resultOperatorType)
        {
            bool result = target.CanHandle(resultOperatorType);
            return result;
            // TODO: add assertions to method ROAggregateTest.CanHandle(ROAggregate, Type)
        }

        /// <summary>Test stub for ProcessResultOperator(ResultOperatorBase, QueryModel, IGeneratedCode)</summary>
#if false
        /// TODO: Get working around pex!
        [PexMethod]
#endif
        public IVariable ProcessResultOperator(
            [PexAssumeUnderTest]ROAggregate target,
            AggregateFromSeedResultOperator resultOperator,
            QueryModel queryModel,
            IGeneratedCode _codeEnv
        )
        {
            IVariable result
               = target.ProcessResultOperator(resultOperator, queryModel, _codeEnv);
            return result;
            // TODO: add assertions to method ROAggregateTest.ProcessResultOperator(ROAggregate, ResultOperatorBase, QueryModel, IGeneratedCode)
        }

        public class ntup
        {
            public int run;
            public IEnumerable<int> vals;
        }

        [TestMethod]
        public void TestSimpleAddition()
        {
            AggregateFromSeedResultOperator agg = new AggregateFromSeedResultOperator(Expression.Constant(1),
                Expression.Lambda(Expression.MakeBinary(ExpressionType.Add, Expression.Parameter(typeof(int), "count"), Expression.Constant(1)),
                Expression.Parameter(typeof(int), "count")),
                null);

            ROAggregate processor = new ROAggregate();
            GeneratedCode gc = new GeneratedCode();
            var result = ProcessResultOperator(processor, agg, null, gc);

            Assert.AreEqual(typeof(int), result.Type, "Expected the type to be an integer!");
            Assert.IsTrue(result.RawValue.IndexOf("(") < 0, "Expected no typing in the statement '" + result.RawValue + "'");
            Assert.IsTrue(result.RawValue.IndexOf("count") < 0, "Expected not to see 'count' in the translated expression '" + result.RawValue + "'");

            Assert.IsInstanceOfType(result, typeof(Variables.VarSimple), "Expected a var simple!");
            var vs = result as Variables.VarSimple;
            Assert.AreEqual("1", result.InitialValue.RawValue, "Incorrect seed value");
            Assert.AreEqual(typeof(int), result.InitialValue.Type, "Incorrect seed value");

            ///
            /// Now make sure the statements came back ok!
            /// 

            Assert.AreEqual(0, gc.CodeBody.DeclaredVariables.Count(), "Expected no bookings");
            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "expected a statement!");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.First(), typeof(Statements.StatementAssign), "expected an assignment statement!");

            var ass = gc.CodeBody.Statements.First() as Statements.StatementAssign;
            StringBuilder bld = new StringBuilder();
            bld.AppendFormat("((int){0})+((int)1)", ass.ResultVariable.RawValue);
            Assert.AreEqual(bld.ToString(), ass.Expression.RawValue, "the raw value of hte expression is not right");
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void TestWithAgPostSelector()
        {
            Expression<Func<int, int>> selector = cnt => cnt * 2;
            AggregateFromSeedResultOperator agg = new AggregateFromSeedResultOperator(Expression.Constant(1),
                Expression.Lambda(Expression.MakeBinary(ExpressionType.Add, Expression.Parameter(typeof(int), "count"), Expression.Constant(1)),
                Expression.Parameter(typeof(int), "count")),
                selector);

            ROAggregate processor = new ROAggregate();
            GeneratedCode gc = new GeneratedCode();
            var result = ProcessResultOperator(processor, agg, null, gc);
        }

        /// <summary>
        /// Test to see if we can pick up a ang with some ROOT object as an initial value. Make sure that
        /// the value is going to come back, make sure the initial value is set right. Everything else is, I hope,
        /// set in other place.
        /// 
        /// We use the relinq infrastructure to build the test here. Otherwise this is just oo hard!
        /// </summary>
        [TestMethod]
        public void TestWithInitialValueObject()
        {
            var q = new QueriableDummy<ntup>();
            var r = from d in q
                    select d;
            var c = r.AggregateNoReturn(new ROOTNET.NTH1F("dude", "put a fork in it", 10, 0.0, 20.0), (h1, n1) => h1.Fill(n1.run));

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;

            Assert.AreEqual(res.ResultValue.Type, typeof(ROOTNET.NTH1F), "incorrect result type came back!");

            var varToTrans = res.VariablesToTransfer.ToArray();
            Assert.AreEqual(1, varToTrans.Length, "variables to transfer incorrect");
            Assert.IsInstanceOfType(varToTrans[0], typeof(ROOTObjectVariable), "bad object type to transfer");
            var ro = varToTrans[0] as ROOTObjectVariable;
            Assert.AreEqual(res.ResultValue.InitialValue.RawValue, ro.VariableName, "variable name for initial values doen't match");
        }
    }
}
