using System.Collections.Generic;
// <auto-generated>
// This file contains automatically generated unit tests.
// Do NOT modify this file manually.
// 
// When Pex is invoked again,
// it might remove or update any previously generated unit tests.
// 
// If the contents of this file becomes outdated, e.g. if it does not
// compile anymore, you may delete this file and invoke Pex again.
// </auto-generated>
using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Statements
{
    public partial class StatementInlineBlockTest
    {
        [TestMethod]
        [PexGeneratedBy(typeof(StatementInlineBlockTest))]
        public void StatementsGet490()
        {
            StatementInlineBlock statementInlineBlock;
            IEnumerable<IStatement> iEnumerable;
            statementInlineBlock = StatementInlineBlockFactory.Create();
            iEnumerable = this.StatementsGet(statementInlineBlock);
            Assert.IsNotNull((object)iEnumerable);
            Assert.IsNotNull((object)statementInlineBlock);
            Assert.IsNotNull(statementInlineBlock.Statements);
            Assert.IsTrue(
                         object.ReferenceEquals(statementInlineBlock.Statements, (object)iEnumerable)
                         );
            Assert.IsNotNull(statementInlineBlock.DeclaredVariables);
        }
    }
}
