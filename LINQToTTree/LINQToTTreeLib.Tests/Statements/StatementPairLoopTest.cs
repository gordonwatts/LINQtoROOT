using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
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
            TestUtils.ResetLINQLibrary();
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        [PexMethod]
        internal StatementPairLoop StatementPairLoopCtor(IDeclaredParameter varArray, IDeclaredParameter index1, IDeclaredParameter index2)
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
            var array = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(int));
            var index1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var t = new StatementPairLoop(array, index1, index2);
            Assert.AreEqual(0, t.CodeItUp().Count(), "# of lines incorrect");
        }

        [TestMethod]
        public void TestForEmittingSimpleStatement()
        {
            var array = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(int));
            var index1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var t = new StatementPairLoop(array, index1, index2);
            t.Add(new LINQToTTreeLib.Statements.StatementSimpleStatement("dir"));
            Assert.AreEqual(13, t.CodeItUp().Count(), "# of lines incorrect");
        }

        [TestMethod]
        public void TestForBreakPlacement()
        {
            var array = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(int));
            var index1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var t = new StatementPairLoop(array, index1, index2);
            t.Add(new LINQToTTreeLib.Statements.StatementSimpleStatement("dir"));
            var statements = t.CodeItUp().ToArray();
            Assert.AreEqual(13, statements.Count(), "# of statements");
            var postdir = statements.SkipWhile(l => !l.Contains("dir;")).Skip(2).ToArray();
            Assert.IsTrue(postdir[0].Contains("breakSeen = false"), "seen break line not reset '" + postdir[0] + "'.");
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public bool TestTryCombine([PexAssumeUnderTest] StatementPairLoop pairloop, IStatement statement, ICodeOptimizationService codeOpt)
        {
            var result = pairloop.TryCombineStatement(statement, codeOpt);
            return result;
        }

        [PexMethod]
        public StatementPairLoop TestRename([PexAssumeUnderTest] StatementPairLoop pairLoop, string oldName, string newName)
        {
            pairLoop.RenameVariable(oldName, newName);
            return pairLoop;
        }

        [TestMethod]
        public void TestSimpleRename()
        {
            var array = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(int));
            var index1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var st = new StatementPairLoop(array, index1, index2);
            var vr = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            vr.RenameRawValue(vr.RawValue, index1.RawValue);
            st.Add(new StatementAssign(vr, new ValSimple("ops", typeof(int))));

            st.RenameVariable(index1.RawValue, "dude1");
            Assert.AreEqual("dude1", index1.RawValue, "index1 after index1 rename");
            Assert.AreEqual("dude1", (st.Statements.First() as StatementAssign).ResultVariable.RawValue, "sub statement not renamed correctly");

            st.RenameVariable(index2.RawValue, "dude2");
            Assert.AreEqual("dude1", index1.RawValue, "index1 after index2 rename");
            Assert.AreEqual("dude2", index2.RawValue, "index1 after index1 rename");

            st.RenameVariable(array.RawValue, "fork");
            Assert.AreEqual("fork", array.RawValue, "array after array rename");
            Assert.AreEqual("dude1", index1.RawValue, "index1 after array rename");
            Assert.AreEqual("dude2", index2.RawValue, "index1 after array rename");
        }

        [TestMethod]
        public void TestCombineShouldFail()
        {
            var index1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var arrayRecord = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(int));
            var stp1 = new StatementPairLoop(arrayRecord, index1, index2);

            var index3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var index4 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var arrayRecord2 = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(int));
            var stp2 = new StatementPairLoop(arrayRecord2, index3, index4);

            Assert.IsFalse(stp1.TryCombineStatement(stp2, null), "should not have combined");
        }

        [TestMethod]
        public void TestCombineWithSameArray()
        {

            var index1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var arrayRecord = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(int));
            var stp1 = new StatementPairLoop(arrayRecord, index1, index2);

            var index3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var index4 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var stp2 = new StatementPairLoop(arrayRecord, index3, index4);
            var statAss = new StatementAssign(index3, new ValSimple("dude", typeof(int)));
            stp2.Add(statAss);

            var opt = new Factories.CodeOptimizerTest(true);
            Assert.IsTrue(stp1.TryCombineStatement(stp2, opt), "Combine should have been ok");
            Assert.AreEqual(1, stp1.Statements.Count(), "Improper number of combined sub-statements");
            var s1 = stp1.Statements.First();
            Assert.IsInstanceOfType(s1, typeof(StatementAssign), "Statement is not right type");
            var sa = s1 as StatementAssign;
            Assert.AreEqual(index1.RawValue, sa.ResultVariable.RawValue, "rename of variables didn't occur correctly");
        }

        class ntupArray
        {
#pragma warning disable 0649
            public int[] run;
#pragma warning restore 0649
        }
    }
}
