using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;

namespace LINQToTTreeLib.ResultOperators
{
    /// <summary>This class contains parameterized unit tests for ROFirstLast</summary>
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

            Assert.AreEqual(1, query.CodeBody.Statements.Count(), "# of statements in the code body");
            Assert.AreEqual(1, query.Functions.Count(), "# of functions");
            var code = query.Functions.First().StatementBlock;
            Assert.IsInstanceOfType(code.Statements.Skip(2).First(), typeof(Statements.StatementForLoop), "Expecting a for loop as the first statement");
            Assert.IsInstanceOfType(code.Statements.Skip(3).First(), typeof(Statements.StatementThrowIfTrue), "Expecting a filter statement next from the First statement");
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

            Assert.AreEqual(1, query.CodeBody.Statements.Count(), "# of statements in the code body");
            Assert.AreEqual(1, query.Functions.Count(), "# of functions");
            var code = query.Functions.First().StatementBlock;
            Assert.IsInstanceOfType(code.Statements.Skip(2).First(), typeof(Statements.StatementForLoop), "Expecting a for loop as the first statement");
            Assert.IsInstanceOfType(code.Statements.Skip(3).First(), typeof(Statements.StatementThrowIfTrue), "Expecting a filter statement next from the First statement");
        }

        [CPPHelperClass]
        public static class DoItClass
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
            // This test produces something caught in the wild (caused a compile error).
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

            Assert.AreEqual(2, query.CodeBody.Statements.Count(), "# of statements in the code body");
            var lm = query.DumpCode().Where(l => l.Contains(" = ((*(*this).run2).at(aInt32_16))*2;")).FirstOrDefault();
            Assert.IsNotNull(lm, "Unable to find proper addition line");
        }


        public class subNtupleObjects1
        {
            [TTreeVariableGrouping]
            public int var1;
            [TTreeVariableGrouping]
            public double var2;
            [TTreeVariableGrouping]
            [RenameVariable("var3")]
            public double v3;
        }

        public class subNtupleObjects2
        {
            [TTreeVariableGrouping]
            public int var4;
            [TTreeVariableGrouping]
            public double var5;
            [TTreeVariableGrouping]
            [RenameVariable("var6")]
            public double v6;
        }

        [TranslateToClass(typeof(ntupWithObjectsDest))]
        public class ntupWithObjects
        {
            [TTreeVariableGrouping]
            public subNtupleObjects1[] jets;
            [TTreeVariableGrouping]
            public subNtupleObjects2[] tracks;
        }

        public class ntupWithObjectsDest : IExpressionHolder
        {
            public ntupWithObjectsDest(Expression expr)
            {
                HeldExpression = expr;
            }
            public int[] var1;
            public double[] var2;
            public double[] var3;

            public int[] var4;
            public double[] var5;
            public double[] var6;

            public Expression HeldExpression { get; private set; }
        }

        public class TestTranslatedNestedCompareAndSortHolder
        {
            public subNtupleObjects1 jet { get; set; }
            public subNtupleObjects2 track { get; set; }
            public double delta { get; set; }
        }

        public class TestTranslatedNestedCompareAndSortHolderEvent
        {
#pragma warning disable 0649
            public IEnumerable<TestTranslatedNestedCompareAndSortHolder> matches;
#pragma warning restore 0649
        }

        /// <summary>
        /// Derivative bug found while trying to understand a bug in the wild. THe current implementation will cause the "jets"
        /// reference to fail during translation. No reason for that.
        /// </summary>
        [TestMethod]
        public void TestFirstAndTranslationWithObjects()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            // Create a dual object. Avoid anonymous objects just for the sake of it.
            var matched = from evt in q
                          select new TestTranslatedNestedCompareAndSortHolderEvent()
                          {
                              matches = from j in evt.jets
                                        orderby j.v3 ascending
                                        select new TestTranslatedNestedCompareAndSortHolder()
                                        {
                                            jet = j
                                        }
                          };

            // Filter on the first jet in the sequence.
            var goodmatched = from evt in matched
                              where evt.matches.First().jet.v3 > 0
                              select evt;

            var r = goodmatched.Count();

            var code = DummyQueryExectuor.FinalResult;
            code.DumpCodeToConsole();

            // The current test is: the fact that this didn't crash, means it worked. But
            // we will do a simple test here.
        }

        /// <summary>
        /// This test produces somethign caught in the wild (caused a compile error).
        /// The bug has to do with a combination of the First predicate and the CPPCode statement conspiring
        /// to cause the problem, unfortunately. So, the test is here.
        /// </summary>
        [TestMethod]
        public void TestDualFirstWithTestAtEnd()
        {
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

#if false
            // Now that we do function calls, this code doesn't really makes sense any longer.
            Assert.AreEqual(2, query.CodeBody.Statements.Count(), "# of statements in the code body");
            var firstloop = query.CodeBody.Statements.First() as IBookingStatementBlock;
            Assert.AreEqual(1, firstloop.Statements.Count(), "first loop should have only an if statement");
            var ifstatement = firstloop.Statements.First() as IBookingStatementBlock;
            Assert.AreEqual(3, ifstatement.AllDeclaredVariables.Count(), "# of declared variables");
#endif
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
            Assert.AreEqual(4, (query.CodeBody.Statements.First() as IBookingStatementBlock).AllDeclaredVariables.Count(), "# of declared variables");
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
            Assert.AreEqual(1, query.CodeBody.Statements.Count(), "# of statements in the code body");
            Assert.AreEqual(1, query.Functions.Count(), "# of functions");
            var code = query.Functions.First().StatementBlock.Statements.Skip(2).First() as IStatementCompound;
            Assert.IsInstanceOfType(code, typeof(Statements.StatementForLoop), "Expecting a for loop as the first statement");
            Assert.IsInstanceOfType(query.Functions.First().StatementBlock.Statements.Skip(3).First(), typeof(Statements.StatementThrowIfTrue), "Expecting a filter statement next from the First statement");
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
                         where evt.jets.FirstOrDefault() != null
                         select evt;
            var c = result.Count();

            Assert.IsNotNull(DummyQueryExectuor.FinalResult, "Expecting some code to have been generated!");
            var res = DummyQueryExectuor.FinalResult;
            res.DumpCodeToConsole();
            var cnt = res.DumpCode().Where(l => l.Contains("-1")).Count();
            Assert.AreEqual(3, cnt, "Improper # of -1's in the code");
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

        /// <summary>
        /// Just make sure there isn't a crash.
        /// </summary>
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

        [TestMethod]
        public void TestSaveOnlyOneIndex()
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

            var l = (res.CodeBody.Statements.First() as IBookingStatementBlock).Statements.First();
            var sr = l as StatementRecordValue;
            Assert.IsNotNull(l, "record value should not be null");
            Assert.AreEqual(4, sr.CodeItUp().Count(), "Expecting 3 lines of code.");
        }

        /// <summary>
        /// THis comes from a bug in the wild. Two objects that were "close" to each other, look for the second one to do something with it,
        /// and it produced some bad code.
        /// </summary>
        [TestMethod]
        public void TestTranslatedNestedCompareAndSort()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            // Create a dual object. Avoid anonymous objects just for the sake of it.
            var matched = from evt in q
                          select new TestTranslatedNestedCompareAndSortHolderEvent()
                          {
                              matches = from j in evt.jets
                                        let mt = (from t in evt.tracks
                                                  where (t.v6 - j.v3) < 10
                                                  select t).First()
                                        select new TestTranslatedNestedCompareAndSortHolder()
                                        {
                                            jet = j,
                                            track = mt
                                        }
                          };

            // Filter on the first jet in the sequence.
            var goodmatched = from evt in matched
                              select new TestTranslatedNestedCompareAndSortHolderEvent()
                              {
                                  matches = evt.matches.Where(e => e.track.v6 < 1.3)
                              };

            // Do something with the second one now
            var otherTrack = from evt in goodmatched
                             select evt.matches.Skip(1).First().track.v6;

            //var r = matched.Where(evt => evt.matches.Where(m => m.track.v6 > 2.0).Count() > 5).Count();
            var r = otherTrack.Sum();

            var code = DummyQueryExectuor.FinalResult;
            code.DumpCodeToConsole();

            // This was crashing, but does need to be fixed up.

            var allcode = code.DumpCode().ToArray();
            var iflines = allcode.Where(l => l.Contains("<10.0")).ToArray();
            Assert.AreEqual(2, iflines.Length, "# of if 10 lines");
            var v1 = iflines[0].NextIdentifier("var3).at(");
            var v2 = iflines[1].NextIdentifier("var3).at(");
            Assert.IsFalse(v1 == v2, string.Format("v1='{0}' should not be the same as v2='{1}' due to scoping", v1, v2));
        }

        /// <summary>
        /// A bug in the wild had an index var being "forgotten" and then reused. Which caused a scoping error.
        /// Turns out the code in TestTranslatedNestedCompareAndSort had the same error. So this test looks for
        /// that specific failure.
        /// </summary>
        [TestMethod]
        public void TranslatedNestedForgetsIndexVarSub()
        {
            var q = new QueriableDummy<ntupWithObjects>();

            // Create a dual object. Avoid anonymous objects just for the sake of it.
            var matched = from evt in q
                          select new TestTranslatedNestedCompareAndSortHolderEvent()
                          {
                              matches = from j in evt.jets
                                        let mt = (from t in evt.tracks
                                                  where (t.v6 - j.v3) < 10
                                                  select t).First()
                                        select new TestTranslatedNestedCompareAndSortHolder()
                                        {
                                            jet = j,
                                            track = mt
                                        }
                          };

            // Filter on the first jet in the sequence.
            var goodmatched = from evt in matched
                              select new TestTranslatedNestedCompareAndSortHolderEvent()
                              {
                                  matches = evt.matches.Where(e => e.track.v6 < 1.3)
                              };

            // Do something with the second one now
            var otherTrack = from evt in goodmatched
                             select evt.matches.Skip(1).First().track.v6;

            //var r = matched.Where(evt => evt.matches.Where(m => m.track.v6 > 2.0).Count() > 5).Count();
            var r = otherTrack.Sum();

            var code = DummyQueryExectuor.FinalResult;
            code.DumpCodeToConsole();

            // Look for where aInt32_4 goes out of scope, and then see if it ever gets reused.

            var afterScope = code
                .DumpCode()
                .WhereScopeCloses("int aInt32_4", true)
                .Where(l => l.Contains("aInt32_4"))
                .ToArray();

            foreach (var baduse in afterScope)
            {
                Console.WriteLine("Used after it goes out of scope: {0}", baduse);
            }

            Assert.IsFalse(afterScope.Any(), "use of loop var after out of scope.");
        }

    }
}
