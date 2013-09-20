// <copyright file="ROFirstLastTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Tests;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>This class contains parameterized unit tests for ROFirstLast</summary>
    [PexClass(typeof(ROFirstLast))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class ROFirstLastTest
    {
        [TestInitialize]
        public void TestInit()
        {
            TestUtils.ResetLINQLibrary();

            MEFUtilities.AddPart(new TypeHandlers.TypeHandlerConvert());
            MEFUtilities.AddPart(new ROFirstLast());
            MEFUtilities.AddPart(new ROAggregate());
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        /// <summary>Test stub for CanHandle(Type)</summary>
        [PexMethod]
        public bool CanHandle([PexAssumeUnderTest]ROFirstLast target, Type resultOperatorType)
        {
            bool result = target.CanHandle(resultOperatorType);
            return result;
            // TODO: add assertions to method ROFirstLastTest.CanHandle(ROFirstLast, Type)
        }

        /// <summary>Test stub for ProcessResultOperator(ResultOperatorBase, QueryModel, IGeneratedCode, ICodeContext, CompositionContainer)</summary>
        [PexMethod, PexAllowedException(typeof(NotImplementedException)), PexAllowedException(typeof(NullReferenceException))]
        public Expression ProcessResultOperator(
            [PexAssumeUnderTest]ROFirstLast target,
            ResultOperatorBase resultOperator,
            QueryModel queryModel,
            IGeneratedQueryCode _codeEnv,
            ICodeContext _codeContext,
            CompositionContainer container
        )
        {
            Expression result = target.ProcessResultOperator
                                   (resultOperator, queryModel, _codeEnv, _codeContext, container);
            return result;
            // TODO: add assertions to method ROFirstLastTest.ProcessResultOperator(ROFirstLast, ResultOperatorBase, QueryModel, IGeneratedCode, ICodeContext, CompositionContainer)
        }

        public class ntup2
        {
            public int[] run;
        }

        public class ntup3
        {
            public int[] run1;
            public int[] run2;
        }

        [TestMethod]
        public void TestSimpleFirst()
        {
            var q = new QueriableDummy<ntup2>();

            var result = from evt in q
                         where evt.run.First() > 10
                         select evt;
            var c = result.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();

            Assert.AreEqual(3, query.CodeBody.Statements.Count(), "# of statements in the code body");
            Assert.IsInstanceOfType(query.CodeBody.Statements.First(), typeof(Statements.StatementForLoop), "Expecting a for loop as the first statement");
            Assert.IsInstanceOfType(query.CodeBody.Statements.Skip(1).First(), typeof(Statements.StatementThrowIfTrue), "Expecting a filter statement next from the First statement");
        }

        [TestMethod]
        public void TestFirstInLetStatement()
        {
            var q = new QueriableDummy<ntup2>();

            var result = from evt in q
                         let rf = evt.run.First()
                         where rf > 10
                         select evt;
            var c = result.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();

            Assert.AreEqual(3, query.CodeBody.Statements.Count(), "# of statements in the code body");
            Assert.IsInstanceOfType(query.CodeBody.Statements.First(), typeof(Statements.StatementForLoop), "Expecting a for loop as the first statement");
            Assert.IsInstanceOfType(query.CodeBody.Statements.Skip(1).First(), typeof(Statements.StatementThrowIfTrue), "Expecting a filter statement next from the First statement");
        }

        [CPPHelperClass]
        static class DoItClass
        {
            [CPPCode(Code = new string[] { "DoIt = arg*2;" }, IncludeFiles = new string[] { "TLorentzVector.h" })]
            public static int DoIt(int arg)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void TestDualFirstWithCPPCode()
        {
            // This test produces somethign caught in the wild (caused a compile error).
            // The bug has to do with a combination of the First predicate and the CPPCode statement conspiring
            // to cause the problem, unfortunately. So, the test is here.
            var q = new QueriableDummy<ntup3>();

            var resultA = from evt in q
                          select new
                          {
                              r1 = evt.run1.Where(r => r > 3).Select(r => DoItClass.DoIt(r)),
                              r2 = evt.run2.Where(r => r > 4).Select(r => DoItClass.DoIt(r))
                          };
            var resultB = from e in resultA
                          select new
                          {
                              joinedR = from r1 in e.r1
                                        select (from r2 in e.r2
                                                orderby r1 - r2 ascending
                                                select r2).First()
                          };
            var result = from e in resultB
                         from r in e.joinedR
                         select r;
            var c = result.Sum();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.CodeBody.Statements.Count(), "# of statements in the code body");
            var lm = query.DumpCode().Where(l => l.Contains("aInt32_14 = ((*(*this).run2).at(aInt32_13))*2;")).FirstOrDefault();
            Assert.IsNotNull(lm, "Unable to find proper addition line");
        }

        [TestMethod]
        public void TestDualFirstWithTestAtEnd()
        {
            // This test produces somethign caught in the wild (caused a compile error).
            // The bug has to do with a combination of the First predicate and the CPPCode statement conspiring
            // to cause the problem, unfortunately. So, the test is here.
            var q = new QueriableDummy<ntup3>();

            var resultA = from evt in q
                          select new
                          {
                              r1 = evt.run1.Where(r => r > 3),
                              r2 = evt.run2.Where(r => r > 4)
                          };
            var resultB = from e in resultA
                          select new
                          {
                              joinedR = from r1 in e.r1
                                        select (from r2 in e.r2
                                                orderby r1 - r2 ascending
                                                select new
                                                {
                                                    R1 = r1,
                                                    R2 = r2
                                                }).First()
                          };
            var resultC = from e in resultB
                          select new
                          {
                              jR = from r in e.joinedR
                                   where r.R1 - r.R2 < 0.3
                                   select r
                          };

            var result = from e in resultC
                         from r in e.jR
                         select r.R2 - r.R1;

            var c = result.Sum();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.CodeBody.Statements.Count(), "# of statements in the code body");
            var firstloop = query.CodeBody.Statements.First() as IBookingStatementBlock;
            Assert.AreEqual(1, firstloop.Statements.Count(), "first loop should have only an if statement");
            var ifstatement = firstloop.Statements.First() as IBookingStatementBlock;
            Assert.AreEqual(3, ifstatement.AllDeclaredVariables.Count(), "# of declared variables");
        }

        [TestMethod]
        public void TestDualFirstWithTestAtEndButNoneAtStart()
        {
            // This is a counter test to the one TestDualFirstWithTestAtEnd - it worked, and this is to make
            // sure that the result doesn't get messed up with the bug fix (future or this one).
            var q = new QueriableDummy<ntup3>();

            var resultA = from evt in q
                          select new
                          {
                              r1 = evt.run1,
                              r2 = evt.run2
                          };
            var resultB = from e in resultA
                          select new
                          {
                              joinedR = from r1 in e.r1
                                        select (from r2 in e.r2
                                                orderby r1 - r2 ascending
                                                select new
                                                {
                                                    R1 = r1,
                                                    R2 = r2
                                                }).First()
                          };
            var resultC = from e in resultB
                          select new
                          {
                              jR = from r in e.joinedR
                                   where r.R1 - r.R2 < 0.3
                                   select r
                          };

            var result = from e in resultC
                         from r in e.jR
                         select r.R2 - r.R1;

            var c = result.Sum();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();

            Assert.AreEqual(1, query.CodeBody.Statements.Count(), "# of statements in the code body");
            Assert.AreEqual(3, (query.CodeBody.Statements.First() as IBookingStatementBlock).AllDeclaredVariables.Count(), "# of declared variables");
        }

        [TestMethod]
        public void TestFirstDownOne()
        {
            var q = new QueriableDummy<ntup2>();

            // Make sure that the "if" statement we use to do the after-check has been popped up the right number of levels.
            var result = from evt in q
                         where evt.run.Where(r => r > 5).First() > 10
                         select evt;
            var c = result.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var query = DummyQueryExectuor.FinalResult;
            query.DumpCodeToConsole();
            Assert.AreEqual(3, query.CodeBody.Statements.Count(), "# of statements in the code body");
            Assert.IsInstanceOfType(query.CodeBody.Statements.First(), typeof(Statements.StatementForLoop), "Expecting a for loop as the first statement");
            Assert.IsInstanceOfType(query.CodeBody.Statements.Skip(1).First(), typeof(Statements.StatementThrowIfTrue), "Expecting a filter statement next from the First statement");
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
        public void TestTranslatedObjectFirst()
        {
            var q = new QueriableDummy<SourceType1>();

            var result = from evt in q
                         where (evt.jets.First().val1 > 5)
                         select evt;
            var c = result.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;
            res.DumpCodeToConsole();
            Assert.IsFalse(res.CodeBody.CodeItUp().Any(l => l.Contains("SourceType1")), "C++ code contains one of our SOurceType1 references - see results dump for complete C++ code");
        }

        [TestMethod]
        public void TestTranslatedObjectFirstOrDefault()
        {
            var q = new QueriableDummy<SourceType1>();

            var result = from evt in q
                         where evt.jets.First() != null
                         select evt;
            var c = result.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;
            res.DumpCodeToConsole();
            var cnt = res.CodeBody.CodeItUp().Where(l => l.Contains("-1")).Count();
            Assert.AreEqual(2, cnt, "Improer # of -1's in the code");
        }

        [TestMethod]
        public void TestTranslatedObjectFirstCarriedOver()
        {
            var q = new QueriableDummy<SourceType1>();

            var result = from evt in q
                         select evt.jets.First();
            var c = (from evt in result
                     where evt.val1 > 5
                     select evt).Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;
            res.DumpCodeToConsole();

            Assert.IsFalse(res.CodeBody.CodeItUp().Any(l => l.Contains("SourceType1")), "C++ code contains one of our SOurceType1 references - see results dump for complete C++ code");
        }

        [TestMethod]
        public void TestAnonymousObjectFirst()
        {
            var q = new QueriableDummy<SourceType1>();

            var objs = from evt in q
                       select from j in evt.jets
                              select new
                              {
                                  Value = j.val1
                              };

            var testVeachOne = from evt in objs
                               select evt.First();

            var testV = from evt in testVeachOne
                        where evt.Value > 10
                        select evt;

            var cnt = testV.Count();
            var res = DummyQueryExectuor.FinalResult;
            res.DumpCodeToConsole();

        }
    }
}
