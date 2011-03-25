using System.Collections.Generic;
using System.Linq;
using System.Text;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{
    public class ntup
    {
        public int run;
    }
    /// <summary>
    /// Test top level queires of various sorts!
    /// </summary>
    [TestClass, PexClass]
    public partial class TestQueriesTopLevel
    {
        [TestInitialize]
        public void TestInit()
        {
            MEFUtilities.MyClassInit();
            DummyQueryExectuor.GlobalInitalized = false;
            QueryResultCacheTest.SetupCacheDir();
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        [TestMethod]
        public void TestSingleTopLevelQuery()
        {
            var q = new QueriableDummy<ntup>();
            var result = from d in q
                         select d;
            var c = result.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");

            /// Return type is correct
            Assert.IsNotNull(DummyQueryExectuor.FinalResult.ResultValue, "Expected a result from the count!");
            Assert.IsInstanceOfType(DummyQueryExectuor.FinalResult.ResultValue, typeof(VarInteger), "integer return type expected");

            var res = DummyQueryExectuor.FinalResult;

            ///
            /// We should be booking "d" as a variable that hangs out for a while
            /// 

            Assert.AreEqual(1, res.CodeBody.DeclaredVariables.Count(), "expected one variable declared");
            Assert.AreEqual("this", res.CodeBody.DeclaredVariables.First().RawValue, "expected it to maintain the name!");

            ///
            /// Now, take a lok at the statements and make sure that we see them all correctly.
            ///

            Assert.AreEqual(1, res.CodeBody.Statements.Count(), "incorrect # of statements");
            var statement = res.CodeBody.Statements.First();
            Assert.IsInstanceOfType(statement, typeof(StatementIncrementInteger), "count should be incrementing an integer!");
            Assert.AreEqual(DummyQueryExectuor.FinalResult.ResultValue, (statement as StatementIncrementInteger).Integer, "The variable should match");
        }

        [TestMethod]
        public void TestWhere()
        {
            var q = new QueriableDummy<ntup>();
            var r = from d in q
                    where d.run > 10
                    select d;
            var c = r.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;

            res.DumpCodeToConsole();

            Assert.AreEqual(1, res.CodeBody.DeclaredVariables.Count(), "Expect a single declared variable");

            ///
            /// We expect a single top level statement
            /// 

            Assert.AreEqual(1, res.CodeBody.Statements.Count(), "only single statement expected");
            var ifStatement = res.CodeBody.Statements.First() as StatementFilter;
            Assert.IsNotNull(ifStatement, "if statement isn't an if statement!");
            Assert.AreEqual("((int)(*this).run)>((int)10)", ifStatement.TestExpression.RawValue, "incorrect if statement");

            ///
            /// Finally, the count statement should be down here!
            /// 

            Assert.AreEqual(1, ifStatement.Statements.Count(), "expected a single statement inside the if statemenet!");
            Assert.IsInstanceOfType(ifStatement.Statements.First(), typeof(StatementIncrementInteger), "incorrect if statement");
        }

        [TestMethod]
        public void TestWhereTwoClauses()
        {
            var q = new QueriableDummy<ntup>();
            var r = from d in q
                    where d.run > 10 && d.run < 100
                    select d;
            var c = r.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;

            Assert.AreEqual(1, res.CodeBody.DeclaredVariables.Count(), "Expect a single declared variable");

            ///
            /// We expect a single top level statement
            /// 

            var ifStatement = res.CodeBody.Statements.First() as StatementFilter;
            Assert.IsNotNull(ifStatement, "if statement isn't an if statement!");
            Assert.AreEqual("((bool)((int)(*this).run)>((int)10))&&((bool)((int)(*this).run)<((int)100))", ifStatement.TestExpression.RawValue, "incorrect if statement");
        }

        [TestMethod]
        public void TestSimpleAggregate()
        {
            var q = new QueriableDummy<ntup>();
            var r = from d in q
                    select d;
            var c = r.Aggregate(1, (count, n) => count + 1);

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;

            /// For this there should be one statement - the addition statement for the variable.

            Assert.AreEqual(1, res.CodeBody.Statements.Count(), "not right number of statements coming back!");
            Assert.IsInstanceOfType(res.CodeBody.Statements.First(), typeof(StatementAssign), "Assignment doesn't seem to be there");

            var assignment = res.CodeBody.Statements.First() as StatementAssign;
            StringBuilder bld = new StringBuilder();
            bld.AppendFormat("((int){0})+((int)1)", assignment.ResultVariable.RawValue, "bad assignment!");
            Assert.AreEqual(bld.ToString(), assignment.Expression.RawValue, "expression is incorrect");
        }

        [TestMethod]
        public void TestAggregateWithHistogram()
        {
            var q = new QueriableDummy<ntup>();
            var r = from d in q
                    select d;
            var c = r.ApplyToObject(new ROOTNET.NTH1F("dude", "put a fork in it", 10, 0.0, 20.0), (h1, n1) => h1.Fill(n1.run));

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;

            Assert.AreEqual(res.ResultValue.Type, typeof(ROOTNET.NTH1F), "incorrect result type came back!");
            var varToTrans = res.VariablesToTransfer.ToArray();
            Assert.AreEqual(1, varToTrans.Length, "variables to transfer incorrect");
            Assert.IsInstanceOfType(varToTrans[0], typeof(KeyValuePair<string, object>), "bad object type to transfer");
            var ro = (KeyValuePair<string, object>)varToTrans[0];
            Assert.IsTrue(res.ResultValue.InitialValue.RawValue.Contains(ro.Key), "variable name ('" + ro.Key + ") is not in the lookup ('" + res.ResultValue.InitialValue.RawValue + ")");
        }
    }
}
