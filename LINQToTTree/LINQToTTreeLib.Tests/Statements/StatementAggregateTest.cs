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
            var target = new StatementAggregate(result, new ValSimple("5", typeof(int)), new string[0]);
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
            var target = new StatementAggregate(result, dep, new string[] { dep.RawValue });
            Assert.AreEqual(1, target.DependentVariables.Count);
            Assert.AreEqual(dep.RawValue, target.DependentVariables.First());
        }

        [TestMethod]
        public void AggregateRenameDependent()
        {
            var result = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var dep = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var depname = dep.RawValue;
            var target = new StatementAggregate(result, dep, new string[] { dep.RawValue });
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
        public void TestCombineWithRename()
        {
            // a = a + b
            // c = c + b
            // These two should combine correctly, somehow.

            var a = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var ainc = new ValSimple(string.Format("{0}+b", a.ParameterName), typeof(int));
            var s1 = new StatementAggregate(a, ainc, new string[0]);

            var c = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var cinc = new ValSimple(string.Format("{0}+b", c.ParameterName), typeof(int));
            var s2 = new StatementAggregate(c, cinc, new string[0]);

            var opt = new MyCodeOptimizer(true);
            var result = s1.TryCombineStatement(s2, opt);
            Assert.IsTrue(result, "Expected combination would work");

            Assert.AreEqual(a.ParameterName, opt.NewVariable.ParameterName, "new name not renamed to");
            Assert.AreEqual(c.ParameterName, opt.OldName, "old name for rename not right");
        }

        [TestMethod]
        public void TestCombineWithRenameNoChance()
        {
            // a = a + b
            // c = c + b
            // These two should combine correctly, somehow.

            var a = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var ainc = new ValSimple(string.Format("{0}+b", a.ParameterName), typeof(int));
            var s1 = new StatementAggregate(a, ainc, new string[0]);

            var c = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var cinc = new ValSimple(string.Format("{0}+b", c.ParameterName), typeof(int));
            var s2 = new StatementAggregate(c, cinc, new string[0]);

            var opt = new MyCodeOptimizer(false);
            var result = s1.TryCombineStatement(s2, opt);
            Assert.IsFalse(result, "Expected combination would work");
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
