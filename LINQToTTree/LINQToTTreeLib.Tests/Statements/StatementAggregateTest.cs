using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for StatementAggregateTest and is intended
    ///to contain all StatementAggregateTest Unit Tests
    ///</summary>
    [TestClass]
    public partial class StatementAggregateTest
    {
        [TestInitialize]
        public void initTest()
        {
            TestUtils.ResetLINQLibrary();
        }

        [TestMethod]
        public void AggregateCodeItUp()
        {
            var result = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var target = new StatementAggregate(result, new ValSimple("5", typeof(int)));
            Assert.AreEqual(0, target.DependentVariables.Count);
            Assert.AreEqual(result.RawValue, target.ResultVariable.RawValue);
            Assert.AreEqual(result.RawValue, target.ResultVariables.First());

            target.CodeItUp().DumpToConsole();

            Assert.AreEqual(1, target.CodeItUp().Count());
            Assert.IsTrue(target.CodeItUp().First().Contains("="), "the equal sign");
        }

        [TestMethod]
        public void AggregateCheckDependents()
        {
            var result = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var dep = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var target = new StatementAggregate(result, dep);
            Assert.AreEqual(1, target.DependentVariables.Count);
            Assert.AreEqual(dep.RawValue, target.DependentVariables.First());
        }

        [TestMethod]
        public void AggregateRenameDependent()
        {
            var result = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var dep = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var depname = dep.RawValue;
            var target = new StatementAggregate(result, dep);
            target.CodeItUp().DumpToConsole();

            target.RenameVariable(dep.RawValue, "foot");

            target.CodeItUp().DumpToConsole();

            var r = target.CodeItUp().ToArray();
            Assert.AreEqual(-1, r[0].IndexOf(depname));
            Assert.AreNotEqual(-1, r[0].IndexOf("foot"));

            Assert.AreEqual(result.RawValue, target.ResultVariable.RawValue);
            Assert.AreEqual(1, target.DependentVariables.Count);
            Assert.AreEqual("foot", target.DependentVariables.First());
        }

        [TestMethod]
        public void AggregateCombineWithRename()
        {
            // a = a + b
            // c = c + b
            // These two should combine correctly, somehow.

            var a = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var ainc = new ValSimple(string.Format("{0}+b", a.ParameterName), typeof(int));
            var s1 = new StatementAggregate(a, ainc);

            var c = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var cinc = new ValSimple(string.Format("{0}+b", c.ParameterName), typeof(int));
            var s2 = new StatementAggregate(c, cinc);

            var opt = new MyCodeOptimizer(true);
            var result = s1.TryCombineStatement(s2, opt);
            Assert.IsTrue(result, "Expected combination would work");

            Assert.AreEqual(a.ParameterName, opt.NewVariable.ParameterName, "new name not renamed to");
            Assert.AreEqual(c.ParameterName, opt.OldName, "old name for rename not right");
        }

        [TestMethod]
        public void AggregateCombineWithRenameNoChance()
        {
            // a = a + b
            // c = c + b
            // These two should combine correctly, somehow.

            var a = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var ainc = new ValSimple(string.Format("{0}+b", a.ParameterName), typeof(int));
            var s1 = new StatementAggregate(a, ainc);

            var c = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var cinc = new ValSimple(string.Format("{0}+b", c.ParameterName), typeof(int));
            var s2 = new StatementAggregate(c, cinc);

            var opt = new MyCodeOptimizer(false);
            var result = s1.TryCombineStatement(s2, opt);
            Assert.IsFalse(result, "Expected combination would work");
        }

        [TestMethod]
        public void AggregateEquivalentSame()
        {
            var r1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var t1 = new StatementAggregate(r1, d1);

            var r2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var t2 = new StatementAggregate(r2, d2);

            var r = t1.RequiredForEquivalence(t2);
            Assert.IsTrue(r.Item1);
            var renames = r.Item2.ToArray();
            Assert.AreEqual(2, renames.Length);
            Assert.AreEqual(r1.RawValue, renames.Where(p => p.Item1 == r2.RawValue).First().Item2);
            Assert.AreEqual(d1.RawValue, renames.Where(p => p.Item1 == d2.RawValue).First().Item2);
        }

        [TestMethod]
        public void AggregateEquivalentSameWithTwoSums()
        {
            var r1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var t1 = new StatementAggregate(r1, new ValSimple($"{d1.RawValue}+{d2.RawValue}", typeof(int), new IDeclaredParameter[] { d1, d2 }));

            var r2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d4 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var t2 = new StatementAggregate(r2, new ValSimple($"{d3.RawValue}+{d4.RawValue}", typeof(int), new IDeclaredParameter[] { d4, d3 }));

            var r = t1.RequiredForEquivalence(t2);
            Assert.IsTrue(r.Item1);
            var renames = r.Item2.ToArray();
            Assert.AreEqual(3, renames.Length);
        }

        [TestMethod]
        public void AggregateEquivalentNotSame()
        {
            var r1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var t1 = new StatementAggregate(r1, d1);

            var r2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var t2 = new StatementAggregate(r2, new ValSimple($"{d2.RawValue}+b", typeof(int), new IDeclaredParameter[] { d1 }));

            var r = t1.RequiredForEquivalence(t2);
            Assert.IsFalse(r.Item1);
        }

        [TestMethod]
        public void AggregateEquivalentSameWithPreRenames()
        {
            var r1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var t1 = new StatementAggregate(r1, d1);

            var r2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var t2 = new StatementAggregate(r2, d2);

            var r = t1.RequiredForEquivalence(t2, new Tuple<string, string>[] { new Tuple<string, string>(d2.RawValue, d1.RawValue) });
            Assert.IsTrue(r.Item1);
            var renames = r.Item2.ToArray();
            Assert.AreEqual(1, renames.Length);
            Assert.AreEqual(r1.RawValue, renames.Where(p => p.Item1 == r2.RawValue).First().Item2);
        }

        /// <summary>
        /// Test object
        /// </summary>
        private class MyCodeOptimizer : ICodeOptimizationService
        {
            private bool _allowRename;

            public MyCodeOptimizer(bool allowTryRename)
            {
                this._allowRename = allowTryRename;
            }

            public IDeclaredParameter NewVariable { get; private set; }
            public string OldName { get; private set; }

            public void ForceRenameVariable(string originalName, string newName)
            {
            }

            public bool TryRenameVarialbeOneLevelUp(string oldName, IDeclaredParameter newVariable)
            {
                NewVariable = newVariable;
                OldName = oldName;
                return _allowRename;
            }
        }
    }
}
