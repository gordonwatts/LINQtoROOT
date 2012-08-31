using System;
using System.Linq;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    ///This is a test class for StatementLoopOverSortedPairValueTest and is intended
    ///to contain all StatementLoopOverSortedPairValueTest Unit Tests
    ///</summary>
    [TestClass]
    [PexClass(typeof(StatementLoopOverSortedPairValue))]
    public partial class StatementLoopOverSortedPairValueTest
    {
        [TestInitialize]
        public void initTest()
        {
            TestUtils.ResetLINQLibrary();
        }

        /// <summary>
        ///A test for CodeItUp
        ///</summary>
        [PexMethod]
        public string[] CodeItUpTest([PexAssumeUnderTest] StatementLoopOverSortedPairValue target)
        {
            var actual = target.CodeItUp().ToArray();
            return actual;
        }

        /// <summary>
        ///A test for RenameVariable
        ///</summary>
        [PexMethod]
        public StatementLoopOverSortedPairValue RenameVariableTest([PexAssumeUnderTest] StatementLoopOverSortedPairValue target, string origin, string final)
        {
            target.RenameVariable(origin, final);
            var finder = new Regex(string.Format("\b{0}\b", origin));
            var hasit = target.CodeItUp().Where(s => finder.IsMatch(s)).Any();
            Assert.IsFalse(hasit, "found some code that contained the original guy");
            return target;
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public static void TestTryCombine([PexAssumeUnderTest] StatementLoopOverSortedPairValue target, IStatement statement)
        {
            var canComb = target.TryCombineStatement(statement, null);
            Assert.IsNotNull(statement, "Second statement null should cause a failure");
            var allSame = target.CodeItUp().Zip(statement.CodeItUp(), (f, s) => f == s).All(t => t);
            Assert.IsTrue(allSame == canComb || target.Statements.Count() == 0, "not expected combination!");
        }

        [TestMethod]
        public void TestStatementCreation()
        {
            var v = StatementLoopOverSortedPairValueFactory.Create(true);
        }
    }
}
