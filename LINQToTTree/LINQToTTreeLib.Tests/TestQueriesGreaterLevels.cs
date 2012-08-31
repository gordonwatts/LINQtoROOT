using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{
    [TestClass]
    public class TestQueriesGreaterLevels
    {
        public class subsubclass
        {
            public double radical;
        }

        public class subclass
        {
            public int val;
            public subsubclass[] deeper;
        }

        public class ntup
        {
            public int run;
            public subclass[] other;
        }

        [TestInitialize]
        public void TestInit()
        {
            TestUtils.ResetLINQLibrary();
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        [TestMethod]
        public void TestNestedQuery()
        {
            var q = new QueriableDummy<ntup>();
            var result = from d in q
                         from m in d.other
                         select d;
            var c = result.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");

            /// Return type is correct
            Assert.IsNotNull(DummyQueryExectuor.FinalResult.ResultValue, "Expected a result from the count!");
            Assert.AreEqual(DummyQueryExectuor.FinalResult.ResultValue.Type, typeof(int), "integer return type expected");

            var res = DummyQueryExectuor.FinalResult;

            ///
            /// We should be booking "d" as a variable that hangs out for a while at the top level
            /// 

            Assert.AreEqual(0, res.CodeBody.DeclaredVariables.Count(), "Don't need to book any variables for this");

            ///
            /// Now, take a lok at the statements and make sure that we see them all correctly. This first guy should be the
            /// loop statement over the d.other guys.
            ///

            Assert.AreEqual(1, res.CodeBody.Statements.Count(), "Expected a single statement");
            Assert.IsInstanceOfType(res.CodeBody.Statements.First(), typeof(IBookingStatementBlock), "loop missing!");

            var loop = res.CodeBody.Statements.First() as IBookingStatementBlock;
            var firstLine = loop.CodeItUp().First();

            ///
            /// And below that should be one statement that does the incrementing
            /// 

            Assert.AreEqual(1, loop.Statements.Count(), "incorrect # of statements");
            var statement = loop.Statements.First();

            Assert.IsInstanceOfType(statement, typeof(StatementAggregate), "count should be incrementing an integer!");
        }

        [TestMethod]
        public void TestDoubleNestedQuery()
        {
            var q = new QueriableDummy<ntup>();
            var result = from d in q
                         from m in d.other
                         from r in m.deeper
                         select d;
            var c = result.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");

            /// Return type is correct
            Assert.IsNotNull(DummyQueryExectuor.FinalResult.ResultValue, "Expected a result from the count!");
            Assert.AreEqual(DummyQueryExectuor.FinalResult.ResultValue.Type, typeof(int), "integer return type expected");

            var res = DummyQueryExectuor.FinalResult;

            ///
            /// We should be booking "d" as a variable that hangs out for a while at the top level
            /// 

            Assert.AreEqual(0, res.CodeBody.DeclaredVariables.Count(), "expected one variable declared");

            ///
            /// Now, take a lok at the statements and make sure that we see them all correctly. This first guy should be the
            /// loop statement over the d.other guys.
            ///

            Assert.AreEqual(1, res.CodeBody.Statements.Count(), "Expected a single statement");
            Assert.IsInstanceOfType(res.CodeBody.Statements.First(), typeof(IBookingStatementBlock), "loop missing!");

            var loop = res.CodeBody.Statements.First() as IBookingStatementBlock;

            ///
            /// Second level down...
            /// 

            Assert.AreEqual(1, loop.Statements.Count(), "expected second level down one loop statement");
            Assert.IsInstanceOfType(loop.Statements.First(), typeof(IBookingStatementBlock), "Expected 2nd level loop");

            var loop2 = loop.Statements.First() as IBookingStatementBlock;

            ///
            /// And below that should be one statement that does the incrementing
            /// 

            Assert.AreEqual(1, loop2.Statements.Count(), "incorrect # of statements");
            var statement = loop2.Statements.First();

            Assert.IsInstanceOfType(statement, typeof(StatementAggregate), "count should be incrementing an integer!");
        }
    }
}
