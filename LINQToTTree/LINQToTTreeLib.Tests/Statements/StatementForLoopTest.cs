using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System;

namespace LINQToTTreeLib.Tests.Statements
{
    /// <summary>
    /// Some tests for the for loop
    /// </summary>
    [TestClass]
    public partial class StatementForLoopTest
    {
        [TestInitialize]
        public void TestInit()
        {
            TestUtils.ResetLINQLibrary();
        }

        class dummyOpt : ICodeOptimizationService
        {

            public bool TryRenameVarialbeOneLevelUp(string oldName, IDeclaredParameter newVariable)
            {
                return true;
            }

            public void ForceRenameVariable(string originalName, string newName)
            {
            }
        }

        [TestMethod]
        public void TestCombineDifferentInitialValues()
        {
            IValue initial1 = new ValSimple("0", typeof(int));
            IValue initial2 = new ValSimple("1", typeof(int));
            IValue size = new ValSimple("10", typeof(int));

            var lv1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var lv2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var p1 = new StatementForLoop(lv1, size, initial1);
            var p2 = new StatementForLoop(lv2, size, initial2);

            var r = p1.TryCombineStatement(p2, new dummyOpt());
            Assert.IsFalse(r, "different initial conditions, should be null");
        }

#if false
        [PexMethod, PexAllowedException(typeof(ArgumentNullException)), PexAllowedException(typeof(ArgumentException))]
        public void TestCombineIdentical(IDeclaredParameter loopVarName, IValue ivalSize, IValue initialValue)
        {
            var p1 = new StatementForLoop(loopVarName, ivalSize, initialValue);
            var p2 = new StatementForLoop(loopVarName, ivalSize, initialValue);

            var r = p1.TryCombineStatement(p2, new dummyOpt());
            Assert.IsTrue(r, "Should always be equal");
        }
        [PexMethod, PexAllowedException(typeof(ArgumentNullException)), PexAllowedException(typeof(ArgumentException))]
        public void TestCombineDifInitial(IDeclaredParameter loopVarName, IValue ivalSize, IValue initialValue1, IValue initialValue2)
        {
            var p1 = new StatementForLoop(loopVarName, ivalSize, initialValue1);
            var p2 = new StatementForLoop(loopVarName, ivalSize, initialValue2);

            var r = p1.TryCombineStatement(p2, new dummyOpt());
            Assert.IsFalse(r, "Should always be equal");
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException)), PexAllowedException(typeof(ArgumentException))]
        public StatementForLoop TestCTor(IDeclaredParameter loopVarName, IValue ivalSize, IValue initialValue)
        {
            var p = new StatementForLoop(loopVarName, ivalSize, initialValue);
            Assert.AreEqual(typeof(int), ivalSize.Type, "size value");
            if (initialValue != null)
                Assert.AreEqual(typeof(int), initialValue.Type, "inital value type");

            Assert.AreEqual(p.ArrayLength.RawValue, ivalSize.RawValue, "Initial value must be set");

            return p;
        }

#endif

        [TestMethod]
        public void TestCombineNestedForLoop()
        {
            var limit = new LINQToTTreeLib.Variables.ValSimple("5", typeof(int));
            var loopP1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop1 = new StatementForLoop(loopP1, limit);
            var loopP2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop2 = new StatementForLoop(loopP2, limit);
            loop1.Add(loop2);

            var limit2 = new LINQToTTreeLib.Variables.ValSimple("5", typeof(int));
            var loopP12 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop12 = new StatementForLoop(loopP12, limit);
            var loopP22 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop22 = new StatementForLoop(loopP22, limit);
            loop12.Add(loop22);

            var r = loop1.TryCombineStatement(loop12, new dummyOpt());
            Assert.IsTrue(r, "combination should work");
            Assert.IsNull(loop1.Parent, "loop 1 parent");
            Assert.AreEqual(loop1, loop2.Parent, "Loop 2 parent");
        }

        [TestMethod]
        public void TestCombineNestedForLoopOnOneSide()
        {
            var base1 = new StatementInlineBlock();
            var limit = new LINQToTTreeLib.Variables.ValSimple("5", typeof(int));
            var loopP1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop1 = new StatementForLoop(loopP1, limit);
            base1.Add(loop1);

            var base2 = new StatementInlineBlock();
            var limit2 = new LINQToTTreeLib.Variables.ValSimple("5", typeof(int));
            var loopP12 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop12 = new StatementForLoop(loopP12, limit);
            base2.Add(loop12);
            var loopP22 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var loop22 = new StatementForLoop(loopP22, limit);
            loop12.Add(loop22);

            var r = base1.TryCombineStatement(base2, new dummyOpt());
            Assert.IsTrue(r, "combination should work");
            Assert.AreEqual(base1, loop1.Parent, "loop 1 parent");
            Assert.AreEqual(loop1, loop22.Parent, "Loop 2 parent");
        }

        [TestMethod]
        public void ZeroDependentVariables()
        {
            var counter = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s = new StatementForLoop(counter, new ValSimple("5", typeof(int)));

            Assert.AreEqual(0, s.DependentVariables.Count, "# of dependent variables");
            Assert.AreEqual(0, s.ResultVariables.Count, "# of result variables");
        }

        [TestMethod]
        public void DependentAndResultVariables()
        {
            var counter = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s = new StatementForLoop(counter, new ValSimple("5", typeof(int)));

            var result = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var assign = new StatementAssign(result, new ValSimple($"{result.RawValue}+{counter.RawValue}", typeof(int), new IDeclaredParameter[] { counter, result }));
            s.Add(assign);

            Assert.AreEqual(1, s.DependentVariables.Count, "# of dependent variables");
            Assert.AreEqual(result.RawValue, s.DependentVariables.First());
            Assert.AreEqual(result.RawValue, s.ResultVariables.First());
            Assert.AreEqual(1, s.ResultVariables.Count, "# of result variables");
        }

        [TestMethod]
        public void DependentAndResultVariablesWithDecl()
        {
            var counter = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s = new StatementForLoop(counter, new ValSimple("5", typeof(int)));

            var result = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s.Add(result);

            var assign = new StatementAssign(result, new ValSimple($"{result.RawValue}+{counter.RawValue}", typeof(int), new IDeclaredParameter[] { counter, result }));
            s.Add(assign);

            Assert.AreEqual(0, s.DependentVariables.Count, "# of dependent variables");
            Assert.AreEqual(0, s.ResultVariables.Count, "# of result variables");
        }

        [TestMethod]
        public void DeclaredVariablesLocalOnly()
        {
            var outter = new StatementInlineBlock();
            outter.Add(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));

            var s = new StatementForLoop(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)), new ValSimple("5", typeof(int)));
            s.Add(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));

            outter.Add(s);

            Assert.AreEqual(2, s.DeclaredVariables.Count());
            Assert.AreEqual(3, s.AllDeclaredVariables.Count());
        }

        [TestMethod]
        public void ForLoopEmptySameLimit()
        {
            var c1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementForLoop(c1, new ValSimple("5", typeof(int)));

            var c2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new StatementForLoop(c1, new ValSimple("5", typeof(int)));

            var r = s1.RequiredForEquivalence(s2);

            // Since only loop variable matters here, we don't care, and we can do the rename.
            Assert.IsTrue(r.Item1, "Are Equivalent");
            Assert.AreEqual(0, r.Item2.Count(), "# of renames");
        }

        [TestMethod]
        public void ForLoopEmptyDifferentLimit()
        {
            var c1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementForLoop(c1, new ValSimple("5", typeof(int)));

            var c2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new StatementForLoop(c1, new ValSimple("6", typeof(int)));

            var r = s1.RequiredForEquivalence(s2);

            // Since only loop variable matters here, we don't care, and we can do the rename.
            Assert.IsFalse(r.Item1, "Can't be equivalent");
        }

        [TestMethod]
        public void ForLoopDifferentNumberOfStatements()
        {
            var c1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementForLoop(c1, new ValSimple("5", typeof(int)));
            var a1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s1.Add(new StatementAssign(a1, new ValSimple("10", typeof(int))));

            var c2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new StatementForLoop(c1, new ValSimple("6", typeof(int)));

            var r = s1.RequiredForEquivalence(s2);

            // Since only loop variable matters here, we don't care, and we can do the rename.
            Assert.IsFalse(r.Item1, "Can't be equivalent");
        }

        [TestMethod]
        public void ForLoopSimilarStatements()
        {
            var c1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementForLoop(c1, new ValSimple("5", typeof(int)));
            var a1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s1.Add(new StatementAssign(a1, new ValSimple("10", typeof(int))));

            var c2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new StatementForLoop(c2, new ValSimple("5", typeof(int)));
            var a2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s2.Add(new StatementAssign(a2, new ValSimple("10", typeof(int))));

            var r = s1.RequiredForEquivalence(s2);

            // Since only loop variable matters here, we don't care, and we can do the rename.
            Assert.IsTrue(r.Item1, "Should be equivalent");
            var renames = r.Item2.ToArray();
            Assert.AreEqual(1, renames.Length, "# of renames");
            Assert.AreEqual(a2.RawValue, renames[0].Item1, "from rename");
            Assert.AreEqual(a1.RawValue, renames[0].Item2, "to rename");
        }

        [TestMethod]
        public void ForLoopGivenRenameRequestStatements()
        {
            var c1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementForLoop(c1, new ValSimple("5", typeof(int)));
            var a1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s1.Add(new StatementAssign(a1, new ValSimple("10", typeof(int))));

            var c2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new StatementForLoop(c2, new ValSimple("5", typeof(int)));
            var a2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s2.Add(new StatementAssign(a2, new ValSimple("10", typeof(int))));

            var r = s1.RequiredForEquivalence(s2, new Tuple<string, string>[] { new Tuple<string, string>(a2.RawValue, a1.RawValue) });

            // Since only loop variable matters here, we don't care, and we can do the rename.
            Assert.IsTrue(r.Item1, "Should be equivalent");
            var renames = r.Item2.ToArray();
            Assert.AreEqual(0, renames.Length, "# of renames");
        }

        [TestMethod]
        public void ForLoopGivenRenameInLoopLimit()
        {
            var c1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var l1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementForLoop(c1, l1);
            var a1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var c2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var l2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new StatementForLoop(c2, l2);

            var r = s1.RequiredForEquivalence(s2, new Tuple<string, string>[] { new Tuple<string, string>(l2.RawValue, l1.RawValue) });

            // Since only loop variable matters here, we don't care, and we can do the rename.
            Assert.IsTrue(r.Item1, "Should be equivalent");
            var renames = r.Item2.ToArray();
            Assert.AreEqual(0, renames.Length, "# of renames");
        }

        [TestMethod]
        public void ForLoopPropagateRenameToSecondStatement()
        {
            var c1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementForLoop(c1, new ValSimple("5", typeof(int)));
            var a1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s1.Add(new StatementAssign(a1, new ValSimple("10", typeof(int))));
            s1.Add(new StatementAssign(a1, new ValSimple($"{a1.RawValue}+1", typeof(int), new IDeclaredParameter[] { a1 })));

            var c2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new StatementForLoop(c2, new ValSimple("5", typeof(int)));
            var a2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s2.Add(new StatementAssign(a2, new ValSimple("10", typeof(int))));
            s2.Add(new StatementAssign(a2, new ValSimple($"{a2.RawValue}+1", typeof(int), new IDeclaredParameter[] { a2 })));

            var r = s1.RequiredForEquivalence(s2);

            // Since only loop variable matters here, we don't care, and we can do the rename.
            Assert.IsTrue(r.Item1, "Should be equivalent");
            var renames = r.Item2.ToArray();
            Assert.AreEqual(1, renames.Length, "# of renames");
            Assert.AreEqual(a2.RawValue, renames[0].Item1, "from rename");
            Assert.AreEqual(a1.RawValue, renames[0].Item2, "to rename");
        }

        [TestMethod]
        public void ForLoopRenameTwoVariables()
        {
            var c1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementForLoop(c1, new ValSimple("5", typeof(int)));
            var a1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var v1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s1.Add(new StatementAssign(a1, new ValSimple("10", typeof(int))));
            s1.Add(new StatementAssign(a1, new ValSimple($"{a1.RawValue}+{v1.RawValue}", typeof(int), new IDeclaredParameter[] { a1, v1 })));

            var c2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new StatementForLoop(c2, new ValSimple("5", typeof(int)));
            var a2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var v2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s2.Add(new StatementAssign(a2, new ValSimple("10", typeof(int))));
            s2.Add(new StatementAssign(a2, new ValSimple($"{a2.RawValue}+{v2.RawValue}", typeof(int), new IDeclaredParameter[] { a2, v2 })));

            var r = s1.RequiredForEquivalence(s2);

            // Since only loop variable matters here, we don't care, and we can do the rename.
            Assert.IsTrue(r.Item1, "Should be equivalent");
            var renames = r.Item2.ToArray();
            Assert.AreEqual(2, renames.Length, "# of renames");
        }

        [TestMethod]
        public void ForLoopRenameIgnoreDeclaredVariables()
        {
            var c1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementForLoop(c1, new ValSimple("5", typeof(int)));
            var a1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var v1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s1.Add(new StatementAssign(a1, new ValSimple("10", typeof(int))));
            s1.Add(new StatementAssign(a1, new ValSimple($"{a1.RawValue}+{v1.RawValue}", typeof(int), new IDeclaredParameter[] { a1, v1 })));
            s1.Add(v1);

            var c2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new StatementForLoop(c2, new ValSimple("5", typeof(int)));
            var a2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var v2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s2.Add(new StatementAssign(a2, new ValSimple("10", typeof(int))));
            s2.Add(new StatementAssign(a2, new ValSimple($"{a2.RawValue}+{v2.RawValue}", typeof(int), new IDeclaredParameter[] { a2, v2 })));
            s2.Add(v2);

            var r = s1.RequiredForEquivalence(s2);

            // Since only loop variable matters here, we don't care, and we can do the rename.
            Assert.IsTrue(r.Item1, "Should be equivalent");
            var renames = r.Item2.ToArray();
            Assert.AreEqual(1, renames.Length, "# of renames");
        }

        [TestMethod]
        public void ForLoopRenameTwiceAsTarget()
        {
            var c1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementForLoop(c1, new ValSimple("5", typeof(int)));
            var a1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var v1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s1.Add(new StatementAssign(a1, new ValSimple("10", typeof(int))));
            s1.Add(new StatementAssign(a1, new ValSimple($"{a1.RawValue}+{v1.RawValue}", typeof(int), new IDeclaredParameter[] { a1, v1 })));

            var c2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new StatementForLoop(c1, new ValSimple("6", typeof(int)));
            var a2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var v2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s2.Add(new StatementAssign(a2, new ValSimple("10", typeof(int))));
            s2.Add(new StatementAssign(a2, new ValSimple($"{a2.RawValue}+{a2.RawValue}", typeof(int), new IDeclaredParameter[] { a2 })));

            var r = s1.RequiredForEquivalence(s2);

            // Since only loop variable matters here, we don't care, and we can do the rename.
            Assert.IsFalse(r.Item1, "Should be equivalent");
        }

        [TestMethod]
        public void ForLoopRenameTwiceAsSource()
        {
            var c1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementForLoop(c1, new ValSimple("5", typeof(int)));
            var a1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var v1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s1.Add(new StatementAssign(a1, new ValSimple("10", typeof(int))));
            s1.Add(new StatementAssign(a1, new ValSimple($"{a1.RawValue}+{a1.RawValue}", typeof(int), new IDeclaredParameter[] { a1 })));

            var c2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new StatementForLoop(c1, new ValSimple("6", typeof(int)));
            var a2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var v2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            s2.Add(new StatementAssign(a2, new ValSimple("10", typeof(int))));
            s2.Add(new StatementAssign(a2, new ValSimple($"{a2.RawValue}+{v2.RawValue}", typeof(int), new IDeclaredParameter[] { a2, v2 })));

            var r = s1.RequiredForEquivalence(s2);

            // Since only loop variable matters here, we don't care, and we can do the rename.
            Assert.IsFalse(r.Item1, "Should be equivalent");
        }
    }
}
