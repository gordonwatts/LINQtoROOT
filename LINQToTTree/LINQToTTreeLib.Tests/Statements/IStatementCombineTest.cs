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
    }
}
