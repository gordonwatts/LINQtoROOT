using System;
using System.Linq;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Using;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests.Statements
{
    [TestClass]
    [PexClass(typeof(StatementLoopOverGroupItems))]
    public partial class StatementLoopOverGroupItemsTest
    {
        [TestInitialize]
        public void initTest()
        {
            TypeUtils._variableNameCounter = 0;
        }

        /// <summary>
        ///A test for CodeItUp
        ///</summary>
        [PexMethod]
        [PexUseType(typeof(StatementLoopOverGroupItems))]
        public string[] CodeItUpTest([PexAssumeUnderTest] StatementLoopOverGroupItems target)
        {
            var actual = target.CodeItUp().ToArray();
            if (target.Statements.Any())
            {
                Assert.IsTrue(actual.Length > 0, "length of the code should be some statements!");
            }
            else
            {
                Assert.AreEqual(0, actual.Length, "no statements");
            }
            return actual;
        }

        /// <summary>
        ///A test for RenameVariable
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public StatementLoopOverGroupItems RenameVariableTest([PexAssumeUnderTest] StatementLoopOverGroupItems target, string origin, string final)
        {
            target.RenameVariable(origin, final);
            var finder = new Regex(string.Format("\b{0}\b", origin));
            var hasit = target.CodeItUp().Where(s => finder.IsMatch(s)).Any();
            Assert.IsFalse(hasit, "found some code that contained the original guy");
            return target;
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public static void TestTryCombine([PexAssumeUnderTest] StatementLoopOverGroupItems target, IStatement statement)
        {
            var canComb = target.TryCombineStatement(statement, null);
            Assert.IsNotNull(statement, "Second statement null should cause a failure");
            var allSame = target.CodeItUp().Zip(statement.CodeItUp(), (f, s) => f == s).All(t => t);
            Assert.IsTrue(allSame == canComb || target.Statements.Count() == 0, "not expected combination!");
        }
    }
}
