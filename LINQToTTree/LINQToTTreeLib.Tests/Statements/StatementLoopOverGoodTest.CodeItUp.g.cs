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
using LINQToTTreeLib.Statements;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace LINQToTTreeLib.Tests.Statements
{
    public partial class StatementLoopOverGoodTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
public void CodeItUp280()
{
    VarInteger varInteger;
    StatementLoopOverGood statementLoopOverGood;
    IEnumerable<string> iEnumerable;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IValue)varInteger, (IStatement[])null, (IVariable[])null);
    iEnumerable = this.CodeItUp(statementLoopOverGood);
    Assert.IsNotNull((object)iEnumerable);
    Assert.IsNotNull((object)statementLoopOverGood);
    Assert.IsNotNull(((StatementInlineBlock)statementLoopOverGood).Statements);
    Assert.IsNotNull
        (((StatementInlineBlock)statementLoopOverGood).DeclaredVariables);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
public void CodeItUp28001()
{
    VarInteger varInteger;
    StatementLoopOverGood statementLoopOverGood;
    IEnumerable<string> iEnumerable;
    varInteger = VarIntegerFactory.Create(false, 0);
    IVariable[] iVariables = new IVariable[1];
    iVariables[0] = (IVariable)varInteger;
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IValue)varInteger, (IStatement[])null, iVariables);
    iEnumerable = this.CodeItUp(statementLoopOverGood);
    Assert.IsNotNull((object)iEnumerable);
    Assert.IsNotNull((object)statementLoopOverGood);
    Assert.IsNotNull(((StatementInlineBlock)statementLoopOverGood).Statements);
    Assert.IsNotNull
        (((StatementInlineBlock)statementLoopOverGood).DeclaredVariables);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
public void CodeItUp28002()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    StatementLoopOverGood statementLoopOverGood;
    IEnumerable<string> iEnumerable;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementIncrementInteger;
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IValue)varInteger, iStatements, (IVariable[])null);
    iEnumerable = this.CodeItUp(statementLoopOverGood);
    Assert.IsNotNull((object)iEnumerable);
    Assert.IsNotNull((object)statementLoopOverGood);
    Assert.IsNotNull(((StatementInlineBlock)statementLoopOverGood).Statements);
    Assert.IsNotNull
        (((StatementInlineBlock)statementLoopOverGood).DeclaredVariables);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
public void CodeItUp28003()
{
    VarInteger varInteger;
    StatementLoopOverGood statementLoopOverGood;
    IEnumerable<string> iEnumerable;
    varInteger = VarIntegerFactory.Create(false, 835);
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IValue)varInteger, (IStatement[])null, (IVariable[])null);
    iEnumerable = this.CodeItUp(statementLoopOverGood);
    Assert.IsNotNull((object)iEnumerable);
    Assert.IsNotNull((object)statementLoopOverGood);
    Assert.IsNotNull(((StatementInlineBlock)statementLoopOverGood).Statements);
    Assert.IsNotNull
        (((StatementInlineBlock)statementLoopOverGood).DeclaredVariables);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
public void CodeItUp28004()
{
    VarInteger varInteger;
    VarInteger varInteger1;
    VarInteger varInteger2;
    StatementLoopOverGood statementLoopOverGood;
    IEnumerable<string> iEnumerable;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    varInteger1 = VarIntegerFactory.Create(false, int.MaxValue);
    varInteger2 = VarIntegerFactory.Create(false, 0);
    IVariable[] iVariables = new IVariable[3];
    iVariables[0] = (IVariable)varInteger2;
    iVariables[1] = (IVariable)varInteger1;
    iVariables[2] = (IVariable)varInteger;
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IValue)varInteger, (IStatement[])null, iVariables);
    iEnumerable = this.CodeItUp(statementLoopOverGood);
    Assert.IsNotNull((object)iEnumerable);
    Assert.IsNotNull((object)statementLoopOverGood);
    Assert.IsNotNull(((StatementInlineBlock)statementLoopOverGood).Statements);
    Assert.IsNotNull
        (((StatementInlineBlock)statementLoopOverGood).DeclaredVariables);
}
    }
}
