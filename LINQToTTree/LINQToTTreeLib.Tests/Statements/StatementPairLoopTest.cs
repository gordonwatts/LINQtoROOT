using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests.Statements
{
    [PexClass(typeof(StatementPairLoop))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class StatementPairLoopTest
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

        [PexMethod]
        internal StatementPairLoop StatementPairLoopCtor(VarArray varArray, IVariable index1, IVariable index2)
        {
            var target = new StatementPairLoop(varArray, index1, index2);
            return target;
        }

        [PexMethod]
        internal string[] StatementPairLoopCtor([PexAssumeUnderTest] StatementPairLoop target)
        {
            return target.CodeItUp().ToArray();
        }

        [TestMethod]
        public void TestForEmittingNoStatements()
        {
            var array = new VarArray(typeof(int));
            var index1 = new VarInteger();
            var index2 = new VarInteger();
            var t = new StatementPairLoop(array, index1, index2);
            Assert.AreEqual(0, t.CodeItUp().Count(), "# of lines incorrect");
        }

        [TestMethod]
        public void TestForEmittingSimpleStatement()
        {
            var array = new VarArray(typeof(int));
            var index1 = new VarInteger();
            var index2 = new VarInteger();
            var t = new StatementPairLoop(array, index1, index2);
            t.Add(new LINQToTTreeLib.Statements.StatementSimpleStatement("dir"));
            Assert.AreEqual(13, t.CodeItUp().Count(), "# of lines incorrect");
        }

        [TestMethod]
        public void TestForBreakPlacement()
        {
            var array = new VarArray(typeof(int));
            var index1 = new VarInteger();
            var index2 = new VarInteger();
            var t = new StatementPairLoop(array, index1, index2);
            t.Add(new LINQToTTreeLib.Statements.StatementSimpleStatement("dir"));
            var statements = t.CodeItUp().ToArray();
            Assert.AreEqual(13, statements.Count(), "# of statements");
            var postdir = statements.SkipWhile(l => !l.Contains("dir;")).Skip(2).ToArray();
            Assert.IsTrue(postdir[0].Contains("breakSeen = false"), "seen break line not reset '" + postdir[0] + "'.");
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public void TestEquiv([PexAssumeUnderTest] StatementPairLoop statement1, IStatement statement2)
        {
            var result = statement1.IsSameStatement(statement2);

            var originalLines = statement1.CodeItUp().ToArray();
            var resultinglines = statement2.CodeItUp().ToArray();

            if (resultinglines.Length != originalLines.Length)
            {
                Assert.IsFalse(result, "# of lines is different, so the compare should be too");
                return;
            }

            var pairedLines = originalLines.Zip(resultinglines, (o1, o2) => Tuple.Create(o1, o2));
            foreach (var pair in pairedLines)
            {
                if (pair.Item1 != pair.Item2)
                {
                    Assert.IsFalse(result, string.Format("Line '{0}' and '{1}' are not same!", pair.Item1, pair.Item2));
                }
                else
                {
                    Assert.IsTrue(result, string.Format("Line '{0}' and '{1}' are not same!", pair.Item1, pair.Item2));
                }
            }
        }

        class ntupArray
        {
#pragma warning disable 0649
            public int[] run;
#pragma warning restore 0649
        }
        [TestMethod]
        public void TestUnqiueCombineStatements()
        {
            var q = new QueriableDummy<ntupArray>();

            // Query #1

            var results1 = from evt in q
                           select evt.run.UniqueCombinations().Count();
            var total1 = results1.Aggregate(0, (seed, val) => seed + val);
            var gc1 = DummyQueryExectuor.FinalResult;

            // Query #2

            var results2 = from evt in q
                           select evt.run.UniqueCombinations().Count();
            var total2 = results2.Aggregate(0, (seed, val) => seed + val);
            var gc2 = DummyQueryExectuor.FinalResult;

            // Combine

            Assert.IsTrue(gc1.CodeBody.TryCombineStatement(gc2.CodeBody, null), "Combine should work!");
            gc1.DumpCodeToConsole();

            // Check that the combine actually worked well!!
            Assert.Inconclusive();
        }

    }
}
