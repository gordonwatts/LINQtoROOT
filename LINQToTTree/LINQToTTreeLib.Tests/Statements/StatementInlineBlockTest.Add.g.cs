using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
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

namespace LINQToTTreeLib.Statements
{
    public partial class StatementInlineBlockTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void AddThrowsArgumentNullException985()
{
    StatementInlineBlock statementInlineBlock;
    statementInlineBlock = StatementInlineBlockFactory.Create
                               ((IStatement[])null, (IDeclaredParameter[])null);
    this.Add(statementInlineBlock, (IDeclaredParameter)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void AddThrowsArgumentNullException765()
{
    StatementInlineBlock statementInlineBlock;
    statementInlineBlock = StatementInlineBlockFactory.Create
                               ((IStatement[])null, (IDeclaredParameter[])null);
    this.Add(statementInlineBlock, (IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void Add577()
{
    StatementInlineBlock statementInlineBlock;
    statementInlineBlock = StatementInlineBlockFactory.Create
                               ((IStatement[])null, (IDeclaredParameter[])null);
    this.Add(statementInlineBlock, (IStatement)statementInlineBlock);
    Assert.IsNotNull((object)statementInlineBlock);
    Assert.IsNotNull(((StatementInlineBlockBase)statementInlineBlock).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementInlineBlock).DeclaredVariables);
    Assert.IsNotNull(((StatementInlineBlockBase)statementInlineBlock).Parent);
    Assert.IsTrue(object.ReferenceEquals
                      (((StatementInlineBlockBase)statementInlineBlock).Parent, 
                       (object)statementInlineBlock));
}
    }
}
