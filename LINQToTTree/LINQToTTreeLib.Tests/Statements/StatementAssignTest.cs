﻿using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests.Statements
{
    /// <summary>
    ///This is a test class for StatementAssignTest and is intended
    ///to contain all StatementAssignTest Unit Tests
    ///</summary>
    [TestClass]
    public partial class StatementAssignTest
    {
        [TestInitialize]
        public void initTest()
        {
            TestUtils.ResetLINQLibrary();
        }

        /// <summary>
        /// Try to combine two identical statements, when not doing the decl, but when we can't find where we
        /// have declared the variable.
        /// </summary>
        [TestMethod]
        public void TryCombineTwoNoneDeclaresNoDeclFound()
        {
            var i1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var i2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var sv = new ValSimple("5", typeof(int));
            var s1 = new StatementAssign(i1, sv);
            var s2 = new StatementAssign(i2, sv);

            Assert.IsFalse(s1.TryCombineStatement(s2, new DummyTrackingOptimizationService(false)), "Combine when no decl found");
        }

        /// <summary>
        /// Try to combine two identical statements, when not doing the decl, but when we can't find where we
        /// have declared the variable.
        /// </summary>
        [TestMethod]
        public void TryCombineTwoNoneDeclaresDeclFound()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var sv = new ValSimple("5", typeof(int));
            var s1 = new StatementAssign(i, sv);
            var s2 = new StatementAssign(i, sv);

            Assert.IsTrue(s1.TryCombineStatement(s2, new DummyTrackingOptimizationService(true)), "Combine when no decl found");
        }

        [TestMethod]
        public void TestNoDeclare()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var sv = new ValSimple("5", typeof(int));
            var s1 = new StatementAssign(i, sv);

            Assert.IsTrue(s1.CodeItUp().First().Trim().StartsWith("aInt32_"), "Check for decl: " + s1.CodeItUp().First());
        }

        [TestMethod]
        public void TestBasicCMValues()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var sv = new ValSimple("5", typeof(int));
            var s1 = new StatementAssign(i, sv);

            Assert.AreEqual(1, s1.ResultVariables.Count(), "# result variables");
            Assert.AreEqual(i.RawValue, s1.ResultVariables.First(), "the name");
            Assert.AreEqual(0, s1.DependentVariables.Count(), "no dependent variables");
        }

        [TestMethod]
        public void TestCMValuesForSimpleExpression()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var di = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var sv = new ValSimple("5", typeof(int), new IDeclaredParameter[] { di });
            var s1 = new StatementAssign(i, sv);
            Assert.AreEqual(1, s1.ResultVariables.Count(), "# result variables");
            Assert.AreEqual(i.RawValue, s1.ResultVariables.First(), "the name");
            Assert.AreEqual(1, s1.DependentVariables.Count(), "no dependent variables");
            Assert.AreEqual(di.RawValue, s1.DependentVariables.First(), "a dependent variable");
        }

        [TestMethod]
        public void AssignEquivalentSame()
        {
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var s1 = new StatementAssign(p1, new ValSimple($"{d1.RawValue}", typeof(int), new IDeclaredParameter[] { d1 }));
            var s2 = new StatementAssign(p1, new ValSimple($"{d1.RawValue}", typeof(int), new IDeclaredParameter[] { d1 }));

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsTrue(r.Item1, "can do the combination");
            Assert.AreEqual(0, r.Item2.Count(), "# of variables to rename");
        }

#if false
        [TestMethod]
        public void AssignEquivalentSameAfterReplacement()
        {
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var s1 = new StatementAssign(p1, new ValSimple($"{d1.RawValue}", typeof(int), new IDeclaredParameter[] { d1 }));
            var s2 = new StatementAssign(p1, new ValSimple($"{d2.RawValue}", typeof(int), new IDeclaredParameter[] { d2 }));

            var r = s1.RequiredForEquivalence(s2, new Tuple<string, string>[] { new Tuple<string, string>(d2.RawValue, d1.RawValue) });
            Assert.IsTrue(r.Item1, "can do the combination");
            Assert.AreEqual(0, r.Item2.Count(), "# of variables to rename");
        }

        [TestMethod]
        public void AssignEquivalentSameAfterReplacementWith2Vars()
        {
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d4 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var s1 = new StatementAssign(p1, new ValSimple($"{d1.RawValue}+{d2.RawValue}", typeof(int), new IDeclaredParameter[] { d1, d2 }));
            var s2 = new StatementAssign(p1, new ValSimple($"{d3.RawValue}+{d4.RawValue}", typeof(int), new IDeclaredParameter[] { d3, d4 }));

            var r = s1.RequiredForEquivalence(s2, new Tuple<string, string>[] { new Tuple<string, string>(d3.RawValue, d1.RawValue) });
            Assert.IsTrue(r.Item1, "can do the combination");
            Assert.AreEqual(1, r.Item2.Count(), "# of variables to rename");
            Assert.AreEqual(d4.RawValue, r.Item2.First().Item1);
        }
#endif

        [TestMethod]
        public void AssignEquivalentDiffFunc()
        {
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var s1 = new StatementAssign(p1, new ValSimple($"{d1.RawValue}", typeof(int), new IDeclaredParameter[] { d1 }));
            var s2 = new StatementAssign(p1, new ValSimple($"sin({d1.RawValue})", typeof(int), new IDeclaredParameter[] { d1 }));

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsFalse(r.Item1, "should not be able to do the combination");
        }

#if false
        [TestMethod]
        public void AssignEquivalentDifferentDependents()
        {
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var s1 = new StatementAssign(p1, new ValSimple($"{d1.RawValue}", typeof(int), new IDeclaredParameter[] { d1 }));
            var s2 = new StatementAssign(p1, new ValSimple($"{d2.RawValue}", typeof(int), new IDeclaredParameter[] { d2 }));

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsTrue(r.Item1, "can do the combination");
            Assert.AreEqual(1, r.Item2.Count(), "# of variables to rename");
            Assert.AreEqual(d2.RawValue, r.Item2.First().Item1, "first time of rename");
            Assert.AreEqual(d1.RawValue, r.Item2.First().Item2, "first time of rename");
        }

        [TestMethod]
        public void AssignEquivalentTwoDifferentDependents()
        {
            // Replacing two dependents gets to be more than we can do becasue of ordering and the roll they might
            // be playing.
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d4 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var s1 = new StatementAssign(p1, new ValSimple($"{d1.RawValue}+{d2.RawValue}", typeof(int), new IDeclaredParameter[] { d1, d2 }));
            var s2 = new StatementAssign(p1, new ValSimple($"{d3.RawValue}+{d4.RawValue}", typeof(int), new IDeclaredParameter[] { d3, d4 }));

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsTrue(r.Item1, "can do the combination");
        }
#endif

        [TestMethod]
        public void AssignEquivalentDifferentResults()
        {
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var s1 = new StatementAssign(p1, new ValSimple($"{d1.RawValue}", typeof(int), new IDeclaredParameter[] { d1 }));
            var s2 = new StatementAssign(p2, new ValSimple($"{d1.RawValue}", typeof(int), new IDeclaredParameter[] { d1 }));

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsTrue(r.Item1, "can do the combination");
            Assert.AreEqual(1, r.Item2.Count(), "# of variables to rename");
            Assert.AreEqual(p2.RawValue, r.Item2.First().Item1, "first time of rename");
            Assert.AreEqual(p1.RawValue, r.Item2.First().Item2, "first time of rename");
        }

        [TestMethod]
        public void AssignDependents()
        {
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var v = new ValSimple($"{p2.RawValue}+10", typeof(int), new IDeclaredParameter[] { p2 });
            var a = new StatementAssign(p1, v);

            Assert.AreEqual(1, a.DependentVariables.Count());
            Assert.AreEqual(p2.RawValue, a.DependentVariables.First());          
        }

        [TestMethod]
        public void AssignRenameDependents()
        {
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var v = new ValSimple($"{p2.RawValue}+10", typeof(int), new IDeclaredParameter[] { p2 });
            var a = new StatementAssign(p1, v);
            a.RenameVariable(p2.RawValue, "aInt_1234");

            Assert.AreEqual(1, a.DependentVariables.Count());
            Assert.AreEqual("aInt_1234", a.DependentVariables.First());
        }

#if false
        [TestMethod]
        public void AssignEquivalentDifferentResultsAndDependents()
        {
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var s1 = new StatementAssign(p1, new ValSimple($"{d1.RawValue}", typeof(int), new IDeclaredParameter[] { d1 }));
            var s2 = new StatementAssign(p2, new ValSimple($"{d2.RawValue}", typeof(int), new IDeclaredParameter[] { d2 }));

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsTrue(r.Item1, "can do the combination");
            Assert.AreEqual(2, r.Item2.Count(), "# of variables to rename");
        }
#endif
    }
}
