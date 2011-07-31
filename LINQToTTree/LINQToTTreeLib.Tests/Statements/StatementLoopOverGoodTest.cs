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
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public StatementLoopOverGood Constructor(IValue indiciesToCheck, IValue indexIsGood, IVariable index)
        {
            StatementLoopOverGood target = new StatementLoopOverGood(indiciesToCheck, indexIsGood, index);
            return target;
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException)), PexAllowedException(typeof(ArgumentException))]
        public bool TestTryCombine([PexAssumeUnderTest] StatementLoopOverGood loop, IStatement s)
        {
            /// We should never be able to combine any filter statements currently!

            return loop.TryCombineStatement(s, null);
        }

    }
}
