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
public void TestTryCombineThrowsArgumentNullException744()
{
    this.TestTryCombine((IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
public void TestTryCombine818()
{
    StatementInlineBlock statementInlineBlock;
    statementInlineBlock =
      StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
    this.TestTryCombine((IStatement)statementInlineBlock);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
public void TestTryCombine81801()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    this.TestTryCombine((IStatement)statementIncrementInteger);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
public void TestTryCombine81802()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    varInteger = VarIntegerFactory.Create(false, 1);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    this.TestTryCombine((IStatement)statementIncrementInteger);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
public void TestTryCombine81803()
{
    VarInteger varInteger;
    StatementIfOnCount statementIfOnCount;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, (IVariable[])null);
    this.TestTryCombine((IStatement)statementIfOnCount);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
public void TestTryCombine81804()
{
    VarInteger varInteger;
    StatementIfOnCount statementIfOnCount;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger, 
                                       StatementIfOnCount.ComparisonOperator.EqualTo, 
                                       (IStatement[])null, (IVariable[])null);
    this.TestTryCombine((IStatement)statementIfOnCount);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
public void TestTryCombine81805()
{
    VarInteger varInteger;
    StatementIfOnCount statementIfOnCount;
    varInteger = VarIntegerFactory.Create(false, 0);
    IVariable[] iVariables = new IVariable[1];
    iVariables[0] = (IVariable)varInteger;
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, iVariables);
    this.TestTryCombine((IStatement)statementIfOnCount);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
public void TestTryCombine81806()
{
    StatementInlineBlock statementInlineBlock;
    StatementInlineBlock statementInlineBlock1;
    StatementInlineBlock statementInlineBlock2;
    statementInlineBlock =
      StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementInlineBlock;
    statementInlineBlock1 =
      StatementInlineBlockFactory.Create(iStatements, (IVariable[])null);
    IStatement[] iStatements1 = new IStatement[1];
    iStatements1[0] = (IStatement)statementInlineBlock1;
    statementInlineBlock2 =
      StatementInlineBlockFactory.Create(iStatements1, (IVariable[])null);
    this.TestTryCombine((IStatement)statementInlineBlock2);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
public void TestTryCombine81807()
{
    VarInteger varInteger;
    VarInteger varInteger1;
    VarInteger varInteger2;
    StatementIfOnCount statementIfOnCount;
    StatementInlineBlock statementInlineBlock;
    StatementInlineBlock statementInlineBlock1;
    varInteger = VarIntegerFactory.Create(false, 0);
    varInteger1 = VarIntegerFactory.Create(false, 0);
    varInteger2 = VarIntegerFactory.Create(false, 0);
    IVariable[] iVariables = new IVariable[2];
    iVariables[0] = (IVariable)varInteger1;
    iVariables[1] = (IVariable)varInteger2;
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger2, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, iVariables);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementIfOnCount;
    statementInlineBlock =
      StatementInlineBlockFactory.Create(iStatements, (IVariable[])null);
    IStatement[] iStatements1 = new IStatement[1];
    iStatements1[0] = (IStatement)statementInlineBlock;
    statementInlineBlock1 =
      StatementInlineBlockFactory.Create(iStatements1, (IVariable[])null);
    this.TestTryCombine((IStatement)statementInlineBlock1);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIfOnCountTest))]
public void TestTryCombine81808()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    StatementInlineBlock statementInlineBlock;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementIncrementInteger;
    statementInlineBlock =
      StatementInlineBlockFactory.Create(iStatements, (IVariable[])null);
    this.TestTryCombine((IStatement)statementInlineBlock);
}
    }
}
