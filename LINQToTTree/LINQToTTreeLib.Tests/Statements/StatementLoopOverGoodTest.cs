using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests.Statements
{
    [TestClass]
    [PexClass(typeof(StatementLoopOverGood))]
    public partial class StatementLoopOverGoodTest
    {
        /// <summary>Test stub for CodeItUp()</summary>
        [PexMethod]
        public IEnumerable<string> CodeItUp([PexAssumeUnderTest]StatementLoopOverGood target)
        {
            IEnumerable<string> result = target.CodeItUp();
            return result;
        }

        /// <summary>Test stub for .ctor(IValue)</summary>
        [PexMethod]
        public StatementLoopOverGood Constructor(IValue indiciesToCheck, IValue indexIsGood, IValue index)
        {
            StatementLoopOverGood target = new StatementLoopOverGood(indiciesToCheck, indexIsGood, index);
            return target;
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public void TestEquiv([PexAssumeUnderTest] StatementLoopOverGood statement1, IStatement statement2)
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

        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public bool TestTryCombine([PexAssumeUnderTest] StatementLoopOverGood loop, IStatement s)
        {
            /// We should never be able to combine any filter statements currently!

            return loop.TryCombineStatement(s);
        }

    }
}
