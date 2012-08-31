using System.Linq;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for StatementRecordPairValuesTest and is intended
    ///to contain all StatementRecordPairValuesTest Unit Tests
    ///</summary>
    [TestClass]
    [PexClass(typeof(StatementRecordPairValues))]
    public partial class StatementRecordPairValuesTest
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
        public string CodeItUpTest([PexAssumeUnderTest] StatementRecordPairValues target)
        {
            var actual = target.CodeItUp().ToArray();
            Assert.AreEqual(1, actual.Length, "only xpected one line");
            return actual[0];
        }

        /// <summary>
        ///A test for RenameVariable
        ///</summary>
        [PexMethod]
        public StatementRecordPairValues RenameVariableTest([PexAssumeUnderTest] StatementRecordPairValues target, string origin, string final)
        {
            target.RenameVariable(origin, final);
            var finder = new Regex(string.Format("\b{0}\b", origin));
            var hasit = target.CodeItUp().Where(s => finder.IsMatch(s)).Any();
            Assert.IsFalse(hasit, "found some code that contained the original guy");
            return target;
        }

        [PexMethod]
        public static void TestTryCombine([PexAssumeUnderTest] StatementRecordPairValues target, IStatement statement)
        {
            var canComb = target.TryCombineStatement(statement, null);
            Assert.IsNotNull(statement, "Second statement null should cause a failure");
            var allSame = target.CodeItUp().Zip(statement.CodeItUp(), (f, s) => f == s).All(t => t);
            Assert.AreEqual(allSame, canComb, "not expected combination!");
        }
    }
}
