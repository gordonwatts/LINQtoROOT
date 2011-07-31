using System;
using System.Linq;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Using;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests.Statements
{
    /// <summary>
    /// Some generic tests for equivalence and also for variable renaming
    /// </summary>
    [TestClass, PexClass]
    public partial class IStatementCombineTest
    {
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        [PexUseType(typeof(StatementAssign))]
        [PexUseType(typeof(StatementBreak))]
        [PexUseType(typeof(StatementCheckLoopPairwise))]
        [PexUseType(typeof(StatementFilter))]
        [PexUseType(typeof(StatementIfOnCount))]
        [PexUseType(typeof(StatementIncrementInteger))]
        [PexUseType(typeof(StatementInlineBlock))]
        [PexUseType(typeof(StatementLoopOverGood))]
        [PexUseType(typeof(StatementMinMaxTest))]
        [PexUseType(typeof(StatementPairLoop))]
        [PexUseType(typeof(StatementRecordIndicies))]
        [PexUseType(typeof(StatementSimpleStatement))]
        public IStatement TestRename([PexAssumeUnderTest] IStatement statement, [PexAssumeNotNull] string oldname, [PexAssumeNotNull]string newname)
        {
            var origianllines = statement.CodeItUp().ToArray();
            statement.RenameVariable(oldname, newname);
            var finallines = statement.CodeItUp().ToArray();

            Assert.AreEqual(origianllines.Length, finallines.Length, "# of lines change during variable rename");

            var varReplacer = new Regex(string.Format(@"\b{0}\b", oldname));

            var sharedlines = origianllines.Zip(finallines, (o, n) => Tuple.Create(o, n));
            foreach (var pair in sharedlines)
            {
                var orig = pair.Item1;
                var origReplafce = varReplacer.Replace(orig, newname);
                Assert.AreEqual(origReplafce, pair.Item2, "expected the renaming to be pretty simple.");
            }

            return statement;
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        [PexUseType(typeof(StatementAssign))]
        [PexUseType(typeof(StatementBreak))]
        [PexUseType(typeof(StatementCheckLoopPairwise))]
        [PexUseType(typeof(StatementFilter))]
        [PexUseType(typeof(StatementIfOnCount))]
        [PexUseType(typeof(StatementIncrementInteger))]
        [PexUseType(typeof(StatementInlineBlock))]
        [PexUseType(typeof(StatementLoopOverGood))]
        [PexUseType(typeof(StatementMinMaxTest))]
        [PexUseType(typeof(StatementPairLoop))]
        [PexUseType(typeof(StatementRecordIndicies))]
        [PexUseType(typeof(StatementSimpleStatement))]
        public void TestEquiv([PexAssumeUnderTest] IStatement statement1, IStatement statement2)
        {
            var result = statement1.IsSameStatement(statement2);

            var originalLines = statement1.CodeItUp().ToArray();
            var resultinglines = statement2.CodeItUp().ToArray();

            if (resultinglines.Length != originalLines.Length)
            {
                Assert.IsFalse(result, "# of lines is different, so the compare should be too");
                return;
            }

            var pairedLines = originalLines.Zip(resultinglines, (o1, o2) => Tuple.Create(o1, o2));
            foreach (var pair in pairedLines)
            {
                if (pair.Item1 != pair.Item2)
                {
                    Assert.IsFalse(result, string.Format("Line '{0}' and '{1}' are not same!", pair.Item1, pair.Item2));
                }
                else
                {
                    Assert.IsTrue(result, string.Format("Line '{0}' and '{1}' are not same!", pair.Item1, pair.Item2));
                }
            }
        }
    }
}
