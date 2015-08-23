using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LINQToTTreeLib.Tests;

namespace LINQToTTreeLib.Statements
{
    /// <summary>This class contains parameterized unit tests for StatementIncrementInteger</summary>
    [TestClass]
    public partial class StatementIncrementIntegerTest
    {
        [TestInitialize]
        public void initTest()
        {
            TestUtils.ResetLINQLibrary();
        }

#if false
        [PexMethod]
        public StatementIncrementInteger Constructor(IDeclaredParameter i)
        {
            StatementIncrementInteger target = new StatementIncrementInteger(i);
            Assert.AreEqual(i, target.Integer, "initial value not set correctly");
            return null;
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public IStatement TestRename([PexAssumeUnderTest] StatementIncrementInteger statement, string oldname, string newname)
        {
            return LINQToTTreeLib.Tests.Statements.Utils.TestRenameOfStatement(statement, oldname, newname);
        }

        [PexMethod]
        public bool TestTryCombine([PexAssumeUnderTest] StatementIncrementInteger statement, IStatement toCombineWith)
        {
            var result = statement.TryCombineStatement(toCombineWith, null);
            if (toCombineWith != null)
            {
                var identical = statement.CodeItUp().Zip(toCombineWith.CodeItUp(), (f, s) => f == s).All(v => v == true);
                Assert.AreEqual(identical, result, "Combined but not the same");
            }
            return result;
        }
#endif
    }
}
