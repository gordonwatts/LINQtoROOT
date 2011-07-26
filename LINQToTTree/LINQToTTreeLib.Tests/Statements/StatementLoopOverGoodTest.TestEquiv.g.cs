using LINQToTTreeLib.Variables;
using LINQToTTreeLib.Statements;
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

namespace LINQToTTreeLib.Tests.Statements
{
    public partial class StatementLoopOverGoodTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestEquivThrowsArgumentNullException700()
{
    VarInteger varInteger;
    StatementLoopOverGood statementLoopOverGood;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IVariable)varInteger, (IStatement[])null, (IVariable[])null);
    this.TestEquiv(statementLoopOverGood, (IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestEquivThrowsArgumentNullException440()
{
    VarInteger varInteger;
    StatementLoopOverGood statementLoopOverGood;
    varInteger = VarIntegerFactory.Create(false, 0);
    IVariable[] iVariables = new IVariable[1];
    iVariables[0] = (IVariable)varInteger;
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IVariable)varInteger, (IStatement[])null, iVariables);
    this.TestEquiv(statementLoopOverGood, (IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
public void TestEquiv33()
{
    VarInteger varInteger;
    StatementLoopOverGood statementLoopOverGood;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IVariable)varInteger, (IStatement[])null, (IVariable[])null);
    this.TestEquiv(statementLoopOverGood, (IStatement)statementLoopOverGood);
    Assert.IsNotNull((object)statementLoopOverGood);
    Assert.IsNotNull(((StatementInlineBlockBase)statementLoopOverGood).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementLoopOverGood).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementLoopOverGood).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
public void TestEquiv3301()
{
    VarInteger varInteger;
    StatementLoopOverGood statementLoopOverGood;
    StatementIncrementInteger statementIncrementInteger;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IVariable)varInteger, (IStatement[])null, (IVariable[])null);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    this.TestEquiv(statementLoopOverGood, (IStatement)statementIncrementInteger);
    Assert.IsNotNull((object)statementLoopOverGood);
    Assert.IsNotNull(((StatementInlineBlockBase)statementLoopOverGood).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementLoopOverGood).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementLoopOverGood).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestEquivThrowsArgumentNullException661()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    StatementLoopOverGood statementLoopOverGood;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementIncrementInteger;
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IVariable)varInteger, iStatements, (IVariable[])null);
    this.TestEquiv(statementLoopOverGood, (IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
public void TestEquiv3302()
{
    VarInteger varInteger;
    StatementLoopOverGood statementLoopOverGood;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    IVariable[] iVariables = new IVariable[1];
    iVariables[0] = (IVariable)varInteger;
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IVariable)varInteger, (IStatement[])null, iVariables);
    this.TestEquiv(statementLoopOverGood, (IStatement)statementLoopOverGood);
    Assert.IsNotNull((object)statementLoopOverGood);
    Assert.IsNotNull(((StatementInlineBlockBase)statementLoopOverGood).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementLoopOverGood).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementLoopOverGood).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
public void TestEquiv3303()
{
    VarInteger varInteger;
    StatementLoopOverGood statementLoopOverGood;
    varInteger = VarIntegerFactory.Create(true, int.MaxValue);
    IVariable[] iVariables = new IVariable[1];
    iVariables[0] = (IVariable)varInteger;
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IVariable)varInteger, (IStatement[])null, iVariables);
    this.TestEquiv(statementLoopOverGood, (IStatement)statementLoopOverGood);
    Assert.IsNotNull((object)statementLoopOverGood);
    Assert.IsNotNull(((StatementInlineBlockBase)statementLoopOverGood).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementLoopOverGood).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementLoopOverGood).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestEquivThrowsArgumentNullException427()
{
    VarInteger varInteger;
    StatementLoopOverGood statementLoopOverGood;
    varInteger = VarIntegerFactory.Create(false, 1);
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IVariable)varInteger, (IStatement[])null, (IVariable[])null);
    this.TestEquiv(statementLoopOverGood, (IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
public void TestEquiv3304()
{
    VarInteger varInteger;
    VarInteger varInteger1;
    StatementLoopOverGood statementLoopOverGood;
    StatementLoopOverGood statementLoopOverGood1;
    varInteger = VarIntegerFactory.Create(false, 4);
    varInteger1 = VarIntegerFactory.Create(false, 0);
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger1, 
                                          (IVariable)varInteger, (IStatement[])null, (IVariable[])null);
    statementLoopOverGood1 =
      StatementLoopOverGoodFactory.Create((IValue)varInteger1, (IValue)varInteger, 
                                          (IVariable)varInteger1, (IStatement[])null, (IVariable[])null);
    this.TestEquiv(statementLoopOverGood, (IStatement)statementLoopOverGood1);
    Assert.IsNotNull((object)statementLoopOverGood);
    Assert.IsNotNull(((StatementInlineBlockBase)statementLoopOverGood).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementLoopOverGood).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementLoopOverGood).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
public void TestEquiv3305()
{
    VarInteger varInteger;
    VarInteger varInteger1;
    StatementLoopOverGood statementLoopOverGood;
    StatementLoopOverGood statementLoopOverGood1;
    varInteger = VarIntegerFactory.Create(false, 3);
    varInteger1 = VarIntegerFactory.Create(false, 0);
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger1, 
                                          (IVariable)varInteger1, (IStatement[])null, (IVariable[])null);
    statementLoopOverGood1 =
      StatementLoopOverGoodFactory.Create((IValue)varInteger1, (IValue)varInteger, 
                                          (IVariable)varInteger1, (IStatement[])null, (IVariable[])null);
    this.TestEquiv(statementLoopOverGood, (IStatement)statementLoopOverGood1);
    Assert.IsNotNull((object)statementLoopOverGood);
    Assert.IsNotNull(((StatementInlineBlockBase)statementLoopOverGood).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementLoopOverGood).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementLoopOverGood).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
public void TestEquiv3306()
{
    VarInteger varInteger;
    VarInteger varInteger1;
    StatementLoopOverGood statementLoopOverGood;
    StatementLoopOverGood statementLoopOverGood1;
    varInteger = VarIntegerFactory.Create(false, 8);
    varInteger1 = VarIntegerFactory.Create(false, 0);
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger1, 
                                          (IVariable)varInteger1, (IStatement[])null, (IVariable[])null);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementLoopOverGood;
    statementLoopOverGood1 =
      StatementLoopOverGoodFactory.Create((IValue)varInteger1, (IValue)varInteger, 
                                          (IVariable)varInteger1, iStatements, (IVariable[])null);
    this.TestEquiv(statementLoopOverGood, (IStatement)statementLoopOverGood1);
    Assert.IsNotNull((object)statementLoopOverGood);
    Assert.IsNotNull(((StatementInlineBlockBase)statementLoopOverGood).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementLoopOverGood).DeclaredVariables);
    Assert.IsNotNull(((StatementInlineBlockBase)statementLoopOverGood).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestEquivThrowsArgumentNullException569()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    StatementIfOnCount statementIfOnCount;
    StatementLoopOverGood statementLoopOverGood;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, (IVariable[])null);
    IStatement[] iStatements = new IStatement[2];
    iStatements[0] = (IStatement)statementIncrementInteger;
    iStatements[1] = (IStatement)statementIfOnCount;
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IVariable)varInteger, iStatements, (IVariable[])null);
    this.TestEquiv(statementLoopOverGood, (IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
public void TestEquiv3307()
{
    VarInteger varInteger;
    VarInteger varInteger1;
    StatementLoopOverGood statementLoopOverGood;
    StatementIncrementInteger statementIncrementInteger;
    StatementLoopOverGood statementLoopOverGood1;
    varInteger = VarIntegerFactory.Create(false, 4);
    varInteger1 = VarIntegerFactory.Create(false, 0);
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger1, 
                                          (IVariable)varInteger1, (IStatement[])null, (IVariable[])null);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger1);
    IStatement[] iStatements = new IStatement[2];
    iStatements[0] = (IStatement)statementIncrementInteger;
    iStatements[1] = (IStatement)statementLoopOverGood;
    statementLoopOverGood1 =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IVariable)varInteger1, iStatements, (IVariable[])null);
    this.TestEquiv(statementLoopOverGood, (IStatement)statementLoopOverGood1);
    Assert.IsNotNull((object)statementLoopOverGood);
    Assert.IsNotNull(((StatementInlineBlockBase)statementLoopOverGood).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementLoopOverGood).DeclaredVariables);
    Assert.IsNotNull(((StatementInlineBlockBase)statementLoopOverGood).Parent);
}
    }
}
