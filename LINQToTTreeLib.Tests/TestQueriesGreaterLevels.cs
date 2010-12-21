using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LINQToTTreeLib.Variables;
using LINQToTTreeLib.Statements;

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
            public IEnumerable<subsubclass> deeper;
        }

        public class ntup
        {
            public int run;
            public IEnumerable<subclass> other;
        }

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
            Assert.IsInstanceOfType(DummyQueryExectuor.FinalResult.ResultValue, typeof(VarInteger), "integer return type expected");

            var res = DummyQueryExectuor.FinalResult;

            ///
            /// We should be booking "d" as a variable that hangs out for a while at the top level
            /// 

            Assert.AreEqual(1, res.CodeBody.DeclaredVariables.Count(), "expected one variable declared");
            Assert.AreEqual("(*d)", res.CodeBody.DeclaredVariables.First().RawValue, "expected it to maintain the name!");

            ///
            /// Now, take a lok at the statements and make sure that we see them all correctly. This first guy should be the
            /// loop statement over the d.other guys.
            ///

            Assert.AreEqual(1, res.CodeBody.Statements.Count(), "Expected a single statement");
            Assert.IsInstanceOfType(res.CodeBody.Statements.First(), typeof(StatementLoopOnVector), "loop missing!");

            var loop = res.CodeBody.Statements.First() as StatementLoopOnVector;
            Assert.AreEqual("(*d).other", loop.VectorToLoopOver.RawValue, "vector that will be looped over is not specified correctly");

            ///
            /// And below that should be one statement that does the incrementing
            /// 

            Assert.AreEqual(1, loop.Statements.Count(), "incorrect # of statements");
            var statement = loop.Statements.First();

            Assert.IsInstanceOfType(statement, typeof(StatementIncrementInteger), "count should be incrementing an integer!");
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
            Assert.IsInstanceOfType(DummyQueryExectuor.FinalResult.ResultValue, typeof(VarInteger), "integer return type expected");

            var res = DummyQueryExectuor.FinalResult;

            ///
            /// We should be booking "d" as a variable that hangs out for a while at the top level
            /// 

            Assert.AreEqual(1, res.CodeBody.DeclaredVariables.Count(), "expected one variable declared");
            Assert.AreEqual("(*d)", res.CodeBody.DeclaredVariables.First().RawValue, "expected it to maintain the name!");

            ///
            /// Now, take a lok at the statements and make sure that we see them all correctly. This first guy should be the
            /// loop statement over the d.other guys.
            ///

            Assert.AreEqual(1, res.CodeBody.Statements.Count(), "Expected a single statement");
            Assert.IsInstanceOfType(res.CodeBody.Statements.First(), typeof(StatementLoopOnVector), "loop missing!");

            var loop = res.CodeBody.Statements.First() as StatementLoopOnVector;
            Assert.AreEqual("(*d).other", loop.VectorToLoopOver.RawValue, "vector that will be looped over is not specified correctly");

            ///
            /// Second level down...
            /// 

            Assert.AreEqual(1, loop.Statements.Count(), "expected second level down one loop statement");
            Assert.IsInstanceOfType(loop.Statements.First(), typeof(StatementLoopOnVector), "Expected 2nd level loop");

            var loop2 = loop.Statements.First() as StatementLoopOnVector;
            Assert.AreEqual("(*m).deeper", loop2.VectorToLoopOver.RawValue, "2nd vector loop variable is not correct");

            ///
            /// And below that should be one statement that does the incrementing
            /// 

            Assert.AreEqual(1, loop2.Statements.Count(), "incorrect # of statements");
            var statement = loop2.Statements.First();

            Assert.IsInstanceOfType(statement, typeof(StatementIncrementInteger), "count should be incrementing an integer!");
        }
    }
}
