using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
using LINQToTTreeLib.Variables;
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
    public partial class StatementIfOnCountTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ConstructorThrowsArgumentNullException876()
{
    StatementIfOnCount statementIfOnCount;
    statementIfOnCount = this.Constructor((IValue)null, 
                                          (IValue)null, StatementIfOnCount.ComparisonOperator.GreaterThan);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ConstructorThrowsArgumentNullException325()
{
    VarInteger varInteger;
    StatementIfOnCount statementIfOnCount;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIfOnCount = this.Constructor((IValue)varInteger, 
                                          (IValue)null, StatementIfOnCount.ComparisonOperator.GreaterThan);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
public void Constructor925()
{
    VarInteger varInteger;
    StatementIfOnCount statementIfOnCount;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIfOnCount = this.Constructor((IValue)varInteger, 
                                          (IValue)varInteger, StatementIfOnCount.ComparisonOperator.GreaterThan);
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
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ConstructorThrowsArgumentNullException890()
{
    VarInteger varInteger;
    StatementIfOnCount statementIfOnCount;
    varInteger = VarIntegerFactory.Create(false, 1);
    statementIfOnCount = this.Constructor((IValue)varInteger, 
                                          (IValue)null, StatementIfOnCount.ComparisonOperator.GreaterThan);
}
    }
}
