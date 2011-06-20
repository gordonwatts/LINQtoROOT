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
            Assert.AreEqual(5, t.CodeItUp().Count(), "# of lines incorrect");
        }
    }
}
