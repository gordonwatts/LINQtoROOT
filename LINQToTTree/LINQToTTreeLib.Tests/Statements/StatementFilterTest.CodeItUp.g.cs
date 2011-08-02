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
    public partial class StatementFilterTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementFilterTest))]
public void CodeItUp280()
{
    VarInteger varInteger;
    StatementFilter statementFilter;
    IEnumerable<string> iEnumerable;
    varInteger = VarIntegerFactory.Create(false, 113);
    IStatement[] iStatements = new IStatement[0];
    IVariable[] iVariables = new IVariable[0];
    statementFilter =
      StatementFilterFactory.Create((IValue)varInteger, iStatements, iVariables);
    iEnumerable = this.CodeItUp(statementFilter);
    Assert.IsNotNull((object)iEnumerable);
    Assert.IsNotNull((object)statementFilter);
    Assert.IsNotNull(statementFilter.TestExpression);
    Assert.IsNotNull(((StatementInlineBlockBase)statementFilter).Statements);
    Assert.IsNotNull(((StatementInlineBlockBase)statementFilter).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementFilter).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementFilterTest))]
public void CodeItUp28001()
{
    VarInteger varInteger;
    StatementFilter statementFilter;
    IEnumerable<string> iEnumerable;
    varInteger = VarIntegerFactory.Create(false, 640);
    IStatement[] iStatements = new IStatement[0];
    IVariable[] iVariables = new IVariable[1];
    iVariables[0] = (IVariable)varInteger;
    statementFilter =
      StatementFilterFactory.Create((IValue)varInteger, iStatements, iVariables);
    iEnumerable = this.CodeItUp(statementFilter);
    Assert.IsNotNull((object)iEnumerable);
    Assert.IsNotNull((object)statementFilter);
    Assert.IsNotNull(statementFilter.TestExpression);
    Assert.IsNotNull(((StatementInlineBlockBase)statementFilter).Statements);
    Assert.IsNotNull(((StatementInlineBlockBase)statementFilter).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementFilter).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementFilterTest))]
public void CodeItUp28002()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    StatementFilter statementFilter;
    IEnumerable<string> iEnumerable;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    IStatement[] iStatements = new IStatement[1];
    IVariable[] iVariables = new IVariable[1];
    iStatements[0] = (IStatement)statementIncrementInteger;
    iVariables[0] = (IVariable)varInteger;
    statementFilter =
      StatementFilterFactory.Create((IValue)varInteger, iStatements, iVariables);
    iEnumerable = this.CodeItUp(statementFilter);
    Assert.IsNotNull((object)iEnumerable);
    Assert.IsNotNull((object)statementFilter);
    Assert.IsNotNull(statementFilter.TestExpression);
    Assert.IsNotNull(((StatementInlineBlockBase)statementFilter).Statements);
    Assert.IsNotNull(((StatementInlineBlockBase)statementFilter).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementFilter).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementFilterTest))]
public void CodeItUp28007()
{
    VarInteger varInteger;
    VarInteger varInteger1;
    VarInteger varInteger2;
    VarInteger varInteger3;
    StatementFilter statementFilter;
    IEnumerable<string> iEnumerable;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    varInteger1 = VarIntegerFactory.Create(false, 0);
    varInteger2 = VarIntegerFactory.Create(false, 987);
    varInteger3 = VarIntegerFactory.Create(false, int.MinValue);
    IStatement[] iStatements = new IStatement[0];
    IVariable[] iVariables = new IVariable[3];
    iVariables[0] = (IVariable)varInteger1;
    iVariables[1] = (IVariable)varInteger3;
    iVariables[2] = (IVariable)varInteger2;
    statementFilter =
      StatementFilterFactory.Create((IValue)varInteger, iStatements, iVariables);
    iEnumerable = this.CodeItUp(statementFilter);
    Assert.IsNotNull((object)iEnumerable);
    Assert.IsNotNull((object)statementFilter);
    Assert.IsNotNull(statementFilter.TestExpression);
    Assert.IsNotNull(((StatementInlineBlockBase)statementFilter).Statements);
    Assert.IsNotNull(((StatementInlineBlockBase)statementFilter).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementFilter).Parent);
}
    }
}
