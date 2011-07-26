using LINQToTTreeLib.Variables;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
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
public void CodeItUp280()
{
    VarInteger varInteger;
    StatementIfOnCount statementIfOnCount;
    IEnumerable<string> iEnumerable;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, (IVariable[])null);
    iEnumerable = this.CodeItUp(statementIfOnCount);
    Assert.IsNotNull((object)iEnumerable);
    Assert.IsNotNull((object)statementIfOnCount);
    Assert.IsNotNull(statementIfOnCount.Counter);
    Assert.IsNotNull(statementIfOnCount.Limit);
    Assert.IsTrue(
                 object.ReferenceEquals(statementIfOnCount.Limit, statementIfOnCount.Counter)
                 );
    Assert.AreEqual<StatementIfOnCount.ComparisonOperator>
        (StatementIfOnCount.ComparisonOperator.GreaterThan, 
         statementIfOnCount.Comparison);
    Assert.IsNotNull(((StatementInlineBlockBase)statementIfOnCount).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementIfOnCount).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementIfOnCount).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
public void CodeItUp28001()
{
    VarInteger varInteger;
    StatementIfOnCount statementIfOnCount;
    IEnumerable<string> iEnumerable;
    varInteger = VarIntegerFactory.Create(false, 0);
    IVariable[] iVariables = new IVariable[1];
    iVariables[0] = (IVariable)varInteger;
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, iVariables);
    iEnumerable = this.CodeItUp(statementIfOnCount);
    Assert.IsNotNull((object)iEnumerable);
    Assert.IsNotNull((object)statementIfOnCount);
    Assert.IsNotNull(statementIfOnCount.Counter);
    Assert.IsNotNull(statementIfOnCount.Limit);
    Assert.IsTrue(
                 object.ReferenceEquals(statementIfOnCount.Limit, statementIfOnCount.Counter)
                 );
    Assert.AreEqual<StatementIfOnCount.ComparisonOperator>
        (StatementIfOnCount.ComparisonOperator.GreaterThan, 
         statementIfOnCount.Comparison);
    Assert.IsNotNull(((StatementInlineBlockBase)statementIfOnCount).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementIfOnCount).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementIfOnCount).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
public void CodeItUp28002()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    StatementIfOnCount statementIfOnCount;
    IEnumerable<string> iEnumerable;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementIncrementInteger;
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       iStatements, (IVariable[])null);
    iEnumerable = this.CodeItUp(statementIfOnCount);
    Assert.IsNotNull((object)iEnumerable);
    Assert.IsNotNull((object)statementIfOnCount);
    Assert.IsNotNull(statementIfOnCount.Counter);
    Assert.IsNotNull(statementIfOnCount.Limit);
    Assert.IsTrue(
                 object.ReferenceEquals(statementIfOnCount.Limit, statementIfOnCount.Counter)
                 );
    Assert.AreEqual<StatementIfOnCount.ComparisonOperator>
        (StatementIfOnCount.ComparisonOperator.GreaterThan, 
         statementIfOnCount.Comparison);
    Assert.IsNotNull(((StatementInlineBlockBase)statementIfOnCount).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementIfOnCount).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementIfOnCount).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
public void CodeItUp28003()
{
    VarInteger varInteger;
    StatementIfOnCount statementIfOnCount;
    IEnumerable<string> iEnumerable;
    varInteger = VarIntegerFactory.Create(false, 2);
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, (IVariable[])null);
    iEnumerable = this.CodeItUp(statementIfOnCount);
    Assert.IsNotNull((object)iEnumerable);
    Assert.IsNotNull((object)statementIfOnCount);
    Assert.IsNotNull(statementIfOnCount.Counter);
    Assert.IsNotNull(statementIfOnCount.Limit);
    Assert.IsTrue(
                 object.ReferenceEquals(statementIfOnCount.Limit, statementIfOnCount.Counter)
                 );
    Assert.AreEqual<StatementIfOnCount.ComparisonOperator>
        (StatementIfOnCount.ComparisonOperator.GreaterThan, 
         statementIfOnCount.Comparison);
    Assert.IsNotNull(((StatementInlineBlockBase)statementIfOnCount).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementIfOnCount).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementIfOnCount).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
public void CodeItUp28004()
{
    VarInteger varInteger;
    VarInteger varInteger1;
    StatementIfOnCount statementIfOnCount;
    IEnumerable<string> iEnumerable;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    varInteger1 = VarIntegerFactory.Create(false, 0);
    IVariable[] iVariables = new IVariable[2];
    iVariables[0] = (IVariable)varInteger;
    iVariables[1] = (IVariable)varInteger1;
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger1, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, iVariables);
    iEnumerable = this.CodeItUp(statementIfOnCount);
    Assert.IsNotNull((object)iEnumerable);
    Assert.IsNotNull((object)statementIfOnCount);
    Assert.IsNotNull(statementIfOnCount.Counter);
    Assert.IsNotNull(statementIfOnCount.Limit);
    Assert.AreEqual<StatementIfOnCount.ComparisonOperator>
        (StatementIfOnCount.ComparisonOperator.GreaterThan, 
         statementIfOnCount.Comparison);
    Assert.IsNotNull(((StatementInlineBlockBase)statementIfOnCount).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementIfOnCount).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementIfOnCount).Parent);
}
    }
}
