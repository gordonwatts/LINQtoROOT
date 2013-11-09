using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LINQToTTreeLib.Tests.Statements
{
    /// <summary>
    /// Some tests for the for loop
    /// </summary>
    [TestClass]
    [PexClass(typeof(StatementForLoop))]
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

        [PexMethod, PexAllowedException(typeof(ArgumentNullException)), PexAllowedException(typeof(ArgumentException))]
        public void TestCombineIdentical(IDeclaredParameter loopVarName, IValue ivalSize, IValue initialValue)
        {
            var p1 = new StatementForLoop(loopVarName, ivalSize, initialValue);
            var p2 = new StatementForLoop(loopVarName, ivalSize, initialValue);

            var r = p1.TryCombineStatement(p2, new dummyOpt());
            Assert.IsTrue(r, "Should always be equal");
        }

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

    }
}
