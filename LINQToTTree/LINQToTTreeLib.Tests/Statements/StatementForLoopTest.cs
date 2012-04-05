using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework.Validation;
using LINQToTTreeLib.Variables;

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
            TypeUtils._variableNameCounter = 0;
        }

        [TestMethod]
        public void TestCombineDifferentInitialValues()
        {
            IValue initial1 = new ValSimple("0", typeof(int));
            IValue initial2 = new ValSimple("1", typeof(int));
            IValue size = new ValSimple("10", typeof(int));

            var p1 = new StatementForLoop("freak", size, initial1);
            var p2 = new StatementForLoop("out", size, initial2);

            var r = p1.TryCombineStatement(p2, null);
            Assert.IsFalse(r, "different initial conditions, should be null");
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException)), PexAllowedException(typeof(ArgumentException))]
        public void TestCombineIdentical(string loopVarName, IValue ivalSize, IValue initialValue)
        {
            var p1 = new StatementForLoop(loopVarName, ivalSize, initialValue);
            var p2 = new StatementForLoop(loopVarName, ivalSize, initialValue);

            var r = p1.TryCombineStatement(p2, null);
            Assert.IsTrue(r, "Should always be equal");
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException)), PexAllowedException(typeof(ArgumentException))]
        public void TestCombineDifInitial(string loopVarName, IValue ivalSize, IValue initialValue1, IValue initialValue2)
        {
            var p1 = new StatementForLoop(loopVarName, ivalSize, initialValue1);
            var p2 = new StatementForLoop(loopVarName, ivalSize, initialValue2);

            var r = p1.TryCombineStatement(p2, null);
            Assert.IsFalse(r, "Should always be equal");
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException)), PexAllowedException(typeof(ArgumentException))]
        public StatementForLoop TestCTor(string loopVarName, IValue ivalSize, IValue initialValue)
        {
            var p = new StatementForLoop(loopVarName, ivalSize, initialValue);
            Assert.AreEqual(typeof(int), ivalSize.Type, "size value");
            if (initialValue != null)
                Assert.AreEqual(typeof(int), initialValue.Type, "inital value type");

            Assert.AreEqual(p.ArrayLength.RawValue, ivalSize.RawValue, "Initial value must be set");
            Assert.IsFalse(string.IsNullOrWhiteSpace(loopVarName));

            return p;
        }

    }
}
