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
    }
}
