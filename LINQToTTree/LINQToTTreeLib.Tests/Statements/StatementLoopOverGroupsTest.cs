using System;
using System.Linq;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests.Statements
{
    [TestClass]
    [PexClass(typeof(StatementLoopOverGroups))]
    public partial class StatementLoopOverGroupsTest
    {
        [TestInitialize]
        public void initTest()
        {
            TestUtils.ResetLINQLibrary();
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public StatementLoopOverGroups Constructor(IValue va)
        {
            var v = new StatementLoopOverGroups(va);
            Assert.IsNotNull(v.IndexVariable, "index variable");
            return v;
        }

        /// <summary>
        ///A test for CodeItUp
        ///</summary>
        [PexMethod]
        public string[] CodeItUpTest([PexAssumeUnderTest] StatementLoopOverGroups target)
        {
            var actual = target.CodeItUp().ToArray();
            return actual;
        }

        /// <summary>
        ///A test for RenameVariable
        ///</summary>
        [PexMethod]
        public StatementLoopOverGroups RenameVariableTest([PexAssumeUnderTest] StatementLoopOverGroups target, string origin, string final)
        {
            target.RenameVariable(origin, final);
            var finder = new Regex(string.Format("\b{0}\b", origin));
            var hasit = target.CodeItUp().Where(s => finder.IsMatch(s)).Any();
            Assert.IsFalse(hasit, "found some code that contained the original guy");
            return target;
        }

        [PexMethod]
        public static void TestTryCombine([PexAssumeUnderTest] StatementLoopOverGroups target, IStatement statement)
        {
            var canComb = target.TryCombineStatement(statement, null);
            Assert.IsNotNull(statement, "Second statement null should cause a failure");
            var allSame = target.CodeItUp().Zip(statement.CodeItUp(), (f, s) => f == s).All(t => t);
            Assert.AreEqual(allSame, canComb, "not expected combination!");
        }
    }
}
