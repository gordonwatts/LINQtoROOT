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
using LINQToTTreeLib.Variables;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace LINQToTTreeLib.Statements
{
    public partial class StatementIfOnCountTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
public void CodeItUp280()
{
    VarInteger varInteger;
    StatementIfOnCount statementIfOnCount;
    IEnumerable<string> iEnumerable;
    varInteger = new VarInteger();
    statementIfOnCount = new StatementIfOnCount((IValue)varInteger, 
                                                (IValue)varInteger, StatementIfOnCount.ComparisonOperator.GreaterThan);
    iEnumerable = this.CodeItUp(statementIfOnCount);
    Assert.IsNotNull((object)iEnumerable);
    Assert.IsNotNull((object)statementIfOnCount);
    Assert.IsNotNull(statementIfOnCount.ValLeft);
    Assert.IsNotNull(statementIfOnCount.ValRight);
    Assert.IsTrue(object.ReferenceEquals
                      (statementIfOnCount.ValRight, statementIfOnCount.ValLeft));
    Assert.AreEqual<StatementIfOnCount.ComparisonOperator>
        (StatementIfOnCount.ComparisonOperator.GreaterThan, 
         statementIfOnCount.Comparison);
    Assert.IsNotNull(((StatementInlineBlock)statementIfOnCount).Statements);
    Assert.IsNotNull(((StatementInlineBlock)statementIfOnCount).DeclaredVariables);
}
    }
}