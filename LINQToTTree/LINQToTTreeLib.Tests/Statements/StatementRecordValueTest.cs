using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQToTTreeLib.Tests.Statements
{
    /// <summary>
    ///This is a test class for TestStatementRecordValue and is intended
    ///to contain all TestStatementRecordValue Unit Tests
    ///</summary>
    [TestClass]
    public partial class TestStatementRecordValue
    {
        [TestInitialize]
        public void TestInit()
        {
            TestUtils.ResetLINQLibrary();
        }

        [TestMethod]
        public void RecordValue1ResultVariables()
        {
            var index = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var s1 = new StatementRecordValue(index, new ValSimple("i", typeof(int)), seen, true);

            Assert.AreEqual(2, s1.ResultVariables.Count());
            Assert.AreEqual(seen.RawValue, s1.ResultVariables.First());
            Assert.AreEqual(index.RawValue, s1.ResultVariables.Skip(1).First());

            Assert.AreEqual(1, s1.DependentVariables.Count());
            Assert.AreEqual(seen.RawValue, s1.DependentVariables.First());
        }

        [TestMethod]
        public void RecordValue1ResultVariablesNoSeen()
        {
            var index = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var s1 = new StatementRecordValue(index, new ValSimple("i", typeof(int)), seen, false);

            Assert.AreEqual(2, s1.ResultVariables.Count());
            Assert.AreEqual(seen.RawValue, s1.ResultVariables.First());
            Assert.AreEqual(index.RawValue, s1.ResultVariables.Skip(1).First());

            Assert.AreEqual(0, s1.DependentVariables.Count());
        }

        [TestMethod]
        public void RecordValue2ResultVariables()
        {
            var index = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var s1 = new StatementRecordValue(index, new ValSimple("i", typeof(int)), seen, true);

            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s1.AddNewSaver(index2, new ValSimple("j", typeof(int)));

            Assert.AreEqual(3, s1.ResultVariables.Count());
            Assert.AreEqual(1, s1.DependentVariables.Count());
        }

        [TestMethod]
        public void RecordValue2ResultVariablesWith1Dependents()
        {
            var index = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var dep = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementRecordValue(index, dep, seen, true);

            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s1.AddNewSaver(index2, new ValSimple("j", typeof(int)));

            Assert.AreEqual(3, s1.ResultVariables.Count());
            Assert.AreEqual(2, s1.DependentVariables.Count());
        }

        [TestMethod]
        public void RecordValue2ResultVariablesWith2Dependents()
        {
            var index = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var dep1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementRecordValue(index, dep1, seen, true);

            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var dep2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s1.AddNewSaver(index2, dep2);

            Assert.AreEqual(3, s1.ResultVariables.Count());
            Assert.AreEqual(3, s1.DependentVariables.Count());
        }

        [TestMethod]
        public void TestTCIdenticalButForFirstLast()
        {
            var index = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var s1 = new StatementRecordValue(index, new ValSimple("i", typeof(int)), seen, true);
            var s2 = new StatementRecordValue(index, new ValSimple("i", typeof(int)), seen, false);

            Assert.IsFalse(s1.TryCombineStatement(s2, new DummyTrackingOptimizationService()), "combine with different recording");
        }

        [TestMethod]
        public void TestTCIdentical()
        {
            var index1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var s1 = new StatementRecordValue(index1, new ValSimple("i", typeof(int)), seen1, true);
            var s2 = new StatementRecordValue(index2, new ValSimple("i", typeof(int)), seen2, true);

            Assert.IsTrue(s1.TryCombineStatement(s2, new DummyTrackingOptimizationService()), "combine with different recording");
        }

        [TestMethod]
        public void TestMultipleSavers()
        {
            var index1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var index3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var index4 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementRecordValue(index1, new ValSimple("i", typeof(int)), seen1, true);
            s1.AddNewSaver(index3, new ValSimple("j", typeof(int)));
            var s2 = new StatementRecordValue(index2, new ValSimple("i", typeof(int)), seen2, true);
            s2.AddNewSaver(index4, new ValSimple("j", typeof(int)));

            var dop = new DummyTrackingOptimizationService();
            Assert.IsTrue(s1.TryCombineStatement(s2, dop), "Combined 2 multi-saver guys");

            Assert.AreEqual(3, dop._renameRequests.Count, "# of rename requests");
            Assert.AreEqual(Tuple.Create(index2.RawValue, index1.RawValue), dop._renameRequests[0], "first rename");
            Assert.AreEqual(Tuple.Create(index4.RawValue, index3.RawValue), dop._renameRequests[1], "second rename");
        }

        [TestMethod]
        public void RecordValueEquivSame()
        {
            var index = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var s1 = new StatementRecordValue(index, new ValSimple("i", typeof(int)), seen, true);
            var s2 = new StatementRecordValue(index, new ValSimple("i", typeof(int)), seen, true);

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsTrue(r.Item1);
            Assert.AreEqual(0, r.Item2.Count());
        }

        [TestMethod]
        public void RecordValueEquivDifferentSeens()
        {
            var index = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var s1 = new StatementRecordValue(index, new ValSimple("i", typeof(int)), seen1, true);
            var seen2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var s2 = new StatementRecordValue(index, new ValSimple("i", typeof(int)), seen2, true);

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsTrue(r.Item1);
            Assert.AreEqual(1, r.Item2.Count());
            Assert.AreEqual(seen1.RawValue, r.Item2.Where(l => l.Item1 == seen2.RawValue).First().Item2);
        }

        [TestMethod]
        public void RecordValueEquivDiffRecord()
        {
            var index = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var s1 = new StatementRecordValue(index, new ValSimple("i", typeof(int)), seen, true);
            var s2 = new StatementRecordValue(index, new ValSimple("i", typeof(int)), seen, false);

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsFalse(r.Item1);
        }

#if false
        [TestMethod]
        public void RecordValueEquivDiffCalc()
        {
            var index = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var s1 = new StatementRecordValue(index, new ValSimple("i", typeof(int)), seen, true);
            var s2 = new StatementRecordValue(index, new ValSimple("j", typeof(int)), seen, true);

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsFalse(r.Item1);
        }

        [TestMethod]
        public void RecordValueEquiv1DependentVariable()
        {
            var index = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementRecordValue(index, d1, seen, true);
            var d2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new StatementRecordValue(index, d2, seen, true);

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsTrue(r.Item1);
            Assert.AreEqual(1, r.Item2.Count());
            Assert.AreEqual(d1.RawValue, r.Item2.First().Item2);
        }

        [TestMethod]
        public void RecordValueEquiv4DependentVariable()
        {
            var index = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementRecordValue(index, new ValSimple($"{d1.RawValue}+{d2.RawValue}", typeof(int), new IDeclaredParameter[] { d1, d2 }), seen, true);
            var d3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d4 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new StatementRecordValue(index, new ValSimple($"{d4.RawValue}+{d3.RawValue}", typeof(int), new IDeclaredParameter[] { d3, d4 }), seen, true);

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsTrue(r.Item1);
            Assert.AreEqual(2, r.Item2.Count());
            Assert.AreEqual(d2.RawValue, r.Item2.Where(l => l.Item1 == d3.RawValue).First().Item2);
            Assert.AreEqual(d1.RawValue, r.Item2.Where(l => l.Item1 == d4.RawValue).First().Item2);
        }

        [TestMethod]
        public void RecordValueEquiv3DependentVariable()
        {
            var index = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementRecordValue(index, new ValSimple($"{d1.RawValue}+{d2.RawValue}", typeof(int), new IDeclaredParameter[] { d1, d2 }), seen, true);
            var d3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new StatementRecordValue(index, new ValSimple($"{d3.RawValue}+{d3.RawValue}", typeof(int), new IDeclaredParameter[] { d3 }), seen, true);

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsFalse(r.Item1);
        }
#endif
    }
}
