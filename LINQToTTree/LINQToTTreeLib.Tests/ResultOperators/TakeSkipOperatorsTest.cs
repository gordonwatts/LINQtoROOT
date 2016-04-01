// <copyright file="TakeSkipOperatorsTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Linq;
using System.Linq.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.TypeHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using LINQToTTreeLib.Expressions;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>This class contains parameterized unit tests for TakeSkipOperators</summary>
    [TestClass]
    public partial class TakeSkipOperatorsTest
    {
        /// <summary>Test stub for ProcessResultOperator(ResultOperatorBase, QueryModel, IGeneratedCode)</summary>
        internal GeneratedCode ProcessResultOperator(
            ROTakeSkipOperators target,
            ResultOperatorBase resultOperator,
            QueryModel queryModel,
            GeneratedCode codeEnv
        )
        {
            if (codeEnv.ResultValue != null)
                throw new ArgumentException("this should not be null for this test");
            if (codeEnv.CodeBody.DeclaredVariables == null)
                throw new ArgumentException("Need this declare variables to be defined");

            ///
            /// We always expect to be inside a loop - and depend on it for doing our declares, so add something...
            /// 

            var inlineBlock = new StatementInlineBlock();
            codeEnv.Add(inlineBlock);

            ///
            /// Get the environment setup and run it
            /// 

            CodeContext c = new CodeContext();
            c.SetLoopVariable(Expression.Variable(typeof(int), "d"), null);

            target.ProcessResultOperator(resultOperator, queryModel, codeEnv, c, MEFUtilities.MEFContainer);

            codeEnv.DumpCodeToConsole();

            ///
            /// First, there should be a counter now declared and ready to go in the current variable block - which will
            /// be the outer one for this test. If this is the outter most, then this is going to be burried.
            /// 

            var declBlock = inlineBlock.Parent.Parent as IBookingStatementBlock;
            Assert.IsNotNull(declBlock, "Expecting a declaration block above!");

            Assert.AreEqual(1, inlineBlock.Statements.Count(), "Expected an if block/increment!");
            Assert.IsInstanceOfType(inlineBlock.Statements.First(), typeof(StatementIfOnCount), "if statement not found!");

            var s = inlineBlock.Statements.First() as StatementIfOnCount;

            bool isTopLevel = codeEnv.DumpCode().Where(l => l.Contains("static int")).Any();
            if (!isTopLevel)
            {
                Assert.AreEqual(1, declBlock.DeclaredVariables.Count(), "Expected only 1 variable to be declared");
                Assert.IsInstanceOfType(declBlock.DeclaredVariables.First(), typeof(DeclarableParameter), "Expected it to be a counter");
            } else
            {
                Assert.AreEqual(1, (s.Parent as IBookingStatementBlock).DeclaredVariables.Count());
            }

            string count = "";
            if (resultOperator is SkipResultOperator)
            {
                count = (resultOperator as SkipResultOperator).Count.ToString();
            }
            else if (resultOperator is TakeResultOperator)
            {
                count = (resultOperator as TakeResultOperator).Count.ToString();
            }
            Assert.AreEqual(count, s.Limit.RawValue, "bad count made it through");

            ///
            /// Finally, the current loop variable should be identical, and there should be no result set.
            /// 

            Assert.IsNull(codeEnv.ResultValue, "result value");
            Assert.IsInstanceOfType(c.LoopVariable, typeof(ParameterExpression), "loop variable type");
            var lv = c.LoopVariable as ParameterExpression;
            Assert.AreEqual("d", lv.Name, "loop variable name");

            //
            // Dump everything and return. To force it out, add a dummy statement
            // (because if statements, etc., are smart enough to not print anything if they
            // are empty).
            //

            codeEnv.Add(new StatementSimpleStatement("fork = left"));
            codeEnv.DumpCodeToConsole();

            return codeEnv;
        }

        [TestMethod]
        public void TestBasicTakeSkip()
        {
            var t = new TypeHandlerCache();
            MEFUtilities.Compose(t);
            GeneratedCode gc = new GeneratedCode();
            gc.Add(new LINQToTTreeLib.Statements.StatementInlineBlock());
            gc.Add(new LINQToTTreeLib.Tests.TestUtils.SimpleLoop());
            var skipper = new SkipResultOperator(Expression.Constant(10));
            ProcessResultOperator(new ROTakeSkipOperators(), skipper, null, gc);
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
        public void TestTakeSkipAtTopLevel()
        {
            ///
            /// The below is invalid because the "Take" is at the top level - we are taking only a certain
            /// number of events. That is not legal! So we need to throw when that happens!
            /// 

            var q = new QueriableDummy<ntup>();
            var result = from d in q
                         select d;
            var c = result.Take(3).Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;

            res.DumpCodeToConsole();

            Assert.AreEqual(1, res.CodeBody.DeclaredVariables.Where(v => v.DeclareAsStatic).Count());
            Assert.AreEqual(1, res.CodeBody.DeclaredVariables.Count());
        }

        [TestMethod]
        public void TestTakeSkipAtSource()
        {
            var q = new QueriableDummy<ntup>();
            var c = q.Take(5).Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;

            res.DumpCodeToConsole();

            Assert.AreEqual(1, res.CodeBody.DeclaredVariables.Where(v => v.DeclareAsStatic).Count());
            Assert.AreEqual(1, res.CodeBody.DeclaredVariables.Count());
        }

        [TestMethod]
        public void TestTakeSkipAtTopLevelBurreid()
        {
            var q = new QueriableDummy<ntup>();
            var c = q.Where(x => x.run > 10).Take(5).Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;

            res.DumpCodeToConsole();

            Assert.AreEqual(1, res.DumpCode().Where(l => l.Contains("static int")).Count());
        }

        [TestMethod]
        public void TestTakeSkipLevelBurreid()
        {
            var q = new QueriableDummy<dummyntup>();
            var c = q.Where(x => x.run > 10).SelectMany(x => x.valC1D).Take(5).Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;

            res.DumpCodeToConsole();

            Assert.AreEqual(1, res.DumpCode().Where(l => l.Contains("static int")).Count());
        }

        [TestMethod]
        public void TestTakeSkipLevelInFromExpression()
        {
            var q = new QueriableDummy<dummyntup>();
            var c1 = from evt in q.Where(x => x.run > 10).Take(5).SelectMany(x => x.valC1D)
                     where evt > 50
                     select evt;

            var c = c1.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;
            Console.WriteLine(DummyQueryExectuor.LastQueryModel.ToString());

            res.DumpCodeToConsole();

            Assert.AreEqual(1, res.DumpCode().Where(l => l.Contains("static int")).Count());
        }

        [TestMethod]
        public void TestTakeSkipLevelBurreidTwice()
        {
            var q = new QueriableDummy<dummyntup>();
            var c = q.Where(x => x.run > 10).Select(x => Tuple.Create(x.run, x.vals)).Select(x => x.Item2).SelectMany(x => x).Select(x => x + 1).Where(x => x > 10).Where(x => x > 5).Take(5).Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;

            res.DumpCodeToConsole();

            Assert.AreEqual(1, res.DumpCode().Where(l => l.Contains("static int")).Count());
        }

        [TestMethod]
        public void TestTakeSkipAtSubLevel()
        {
            ///
            /// The below is invalid because the "Take" is at the top level - we are taking only a certain
            /// number of events. That is not legal! So we need to throw when that happens!
            /// 

            var q = new QueriableDummy<dummyntup>();
            var result = from d in q
                         where d.valC1D.Take(1).Sum() > 0
                         select d;
            var c = result.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;

            res.DumpCodeToConsole();

            Assert.AreEqual(0, res.DumpCode().Where(l => l.Contains("static")).Count());
        }
    }
}
