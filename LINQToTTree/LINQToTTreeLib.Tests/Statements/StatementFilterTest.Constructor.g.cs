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
using System;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace LINQToTTreeLib.Statements
{
    public partial class StatementFilterTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementFilterTest))]
public void Constructor682()
{
    StatementFilter statementFilter;
    statementFilter = this.Constructor((IValue)null);
    Assert.IsNotNull((object)statementFilter);
    Assert.IsNull(statementFilter.TestExpression);
    Assert.IsNotNull(((StatementInlineBlock)statementFilter).Statements);
    Assert.IsNotNull(((StatementInlineBlock)statementFilter).DeclaredVariables);
}
    }
}
