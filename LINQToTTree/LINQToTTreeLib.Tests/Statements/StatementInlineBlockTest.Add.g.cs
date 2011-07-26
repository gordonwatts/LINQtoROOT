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
    public partial class StatementInlineBlockTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void AddThrowsArgumentNullException373()
{
    StatementInlineBlock statementInlineBlock;
    statementInlineBlock =
      StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
    this.Add(statementInlineBlock, (IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void Add577()
{
    StatementInlineBlock statementInlineBlock;
    statementInlineBlock =
      StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
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
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void Add57701()
{
    StatementInlineBlock statementInlineBlock;
    VarInteger varInteger;
    StatementIfOnCount statementIfOnCount;
    statementInlineBlock =
      StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, (IVariable[])null);
    this.Add(statementInlineBlock, (IStatement)statementIfOnCount);
    Assert.IsNotNull((object)statementInlineBlock);
    Assert.IsNotNull(((StatementInlineBlockBase)statementInlineBlock).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementInlineBlock).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementInlineBlock).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void Add57702()
{
    StatementInlineBlock statementInlineBlock;
    VarInteger varInteger;
    StatementIfOnCount statementIfOnCount;
    statementInlineBlock =
      StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
    varInteger = VarIntegerFactory.Create(false, 0);
    IVariable[] iVariables = new IVariable[1];
    iVariables[0] = (IVariable)varInteger;
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, iVariables);
    this.Add(statementInlineBlock, (IStatement)statementIfOnCount);
    Assert.IsNotNull((object)statementInlineBlock);
    Assert.IsNotNull(((StatementInlineBlockBase)statementInlineBlock).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementInlineBlock).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementInlineBlock).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void Add57703()
{
    StatementInlineBlock statementInlineBlock;
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    statementInlineBlock =
      StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    this.Add(statementInlineBlock, (IStatement)statementIncrementInteger);
    Assert.IsNotNull((object)statementInlineBlock);
    Assert.IsNotNull(((StatementInlineBlockBase)statementInlineBlock).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementInlineBlock).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementInlineBlock).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void Add57704()
{
    StatementInlineBlock statementInlineBlock;
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    statementInlineBlock =
      StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
    varInteger = VarIntegerFactory.Create(false, 1);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    this.Add(statementInlineBlock, (IStatement)statementIncrementInteger);
    Assert.IsNotNull((object)statementInlineBlock);
    Assert.IsNotNull(((StatementInlineBlockBase)statementInlineBlock).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementInlineBlock).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementInlineBlock).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void Add57705()
{
    StatementInlineBlock statementInlineBlock;
    VarInteger varInteger;
    StatementIfOnCount statementIfOnCount;
    StatementFilter statementFilter;
    statementInlineBlock =
      StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, (IVariable[])null);
    IStatement[] iStatements = new IStatement[0];
    IVariable[] iVariables = new IVariable[0];
    statementFilter =
      StatementFilterFactory.Create((IValue)varInteger, iStatements, iVariables);
    this.Add(statementInlineBlock, (IStatement)statementFilter);
    Assert.IsNotNull((object)statementInlineBlock);
    Assert.IsNotNull(((StatementInlineBlockBase)statementInlineBlock).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementInlineBlock).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementInlineBlock).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void Add57706()
{
    StatementInlineBlock statementInlineBlock;
    VarInteger varInteger;
    StatementIfOnCount statementIfOnCount;
    StatementLoopOverGood statementLoopOverGood;
    StatementFilter statementFilter;
    statementInlineBlock =
      StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, (IVariable[])null);
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IVariable)varInteger, (IStatement[])null, (IVariable[])null);
    IStatement[] iStatements = new IStatement[1];
    IVariable[] iVariables = new IVariable[1];
    iStatements[0] = (IStatement)statementLoopOverGood;
    iVariables[0] = (IVariable)varInteger;
    statementFilter =
      StatementFilterFactory.Create((IValue)varInteger, iStatements, iVariables);
    this.Add(statementInlineBlock, (IStatement)statementFilter);
    Assert.IsNotNull((object)statementInlineBlock);
    Assert.IsNotNull(((StatementInlineBlockBase)statementInlineBlock).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementInlineBlock).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementInlineBlock).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void Add57707()
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
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void Add57708()
{
    StatementInlineBlock statementInlineBlock;
    VarInteger varInteger;
    VarInteger varInteger1;
    VarInteger varInteger2;
    VarInteger varInteger3;
    StatementIfOnCount statementIfOnCount;
    statementInlineBlock =
      StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    varInteger1 = VarIntegerFactory.Create(false, 0);
    varInteger2 = VarIntegerFactory.Create(false, 0);
    varInteger3 = VarIntegerFactory.Create(false, 0);
    IVariable[] iVariables = new IVariable[2];
    iVariables[0] = (IVariable)varInteger2;
    iVariables[1] = (IVariable)varInteger3;
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger1, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, iVariables);
    this.Add(statementInlineBlock, (IStatement)statementIfOnCount);
    Assert.IsNotNull((object)statementInlineBlock);
    Assert.IsNotNull(((StatementInlineBlockBase)statementInlineBlock).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementInlineBlock).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementInlineBlock).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void AddThrowsArgumentNullException617()
{
    StatementInlineBlock statementInlineBlock;
    statementInlineBlock =
      StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
    this.Add(statementInlineBlock, (IVariable)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void Add319()
{
    StatementInlineBlock statementInlineBlock;
    VarInteger varInteger;
    statementInlineBlock =
      StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
    varInteger = VarIntegerFactory.Create(false, 0);
    this.Add(statementInlineBlock, (IVariable)varInteger);
    Assert.IsNotNull((object)statementInlineBlock);
    Assert.IsNotNull(((StatementInlineBlockBase)statementInlineBlock).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementInlineBlock).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementInlineBlock).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void Add31901()
{
    StatementInlineBlock statementInlineBlock;
    VarInteger varInteger;
    statementInlineBlock =
      StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
    varInteger = VarIntegerFactory.Create(false, 1);
    this.Add(statementInlineBlock, (IVariable)varInteger);
    Assert.IsNotNull((object)statementInlineBlock);
    Assert.IsNotNull(((StatementInlineBlockBase)statementInlineBlock).Statements);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementInlineBlock).DeclaredVariables);
    Assert.IsNull(((StatementInlineBlockBase)statementInlineBlock).Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void AddThrowsArgumentNullException656()
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
    this.Add(statementInlineBlock, (IVariable)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void AddThrowsArgumentNullException513()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    StatementIfOnCount statementIfOnCount;
    StatementInlineBlock statementInlineBlock;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, (IVariable[])null);
    IStatement[] iStatements = new IStatement[2];
    iStatements[0] = (IStatement)statementIncrementInteger;
    iStatements[1] = (IStatement)statementIfOnCount;
    statementInlineBlock =
      StatementInlineBlockFactory.Create(iStatements, (IVariable[])null);
    this.Add(statementInlineBlock, (IVariable)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void AddThrowsArgumentNullException()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    VarInteger varInteger1;
    VarInteger varInteger2;
    StatementIfOnCount statementIfOnCount;
    StatementInlineBlock statementInlineBlock;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    varInteger1 = VarIntegerFactory.Create(false, 0);
    varInteger2 = VarIntegerFactory.Create(false, 0);
    IVariable[] iVariables = new IVariable[2];
    iVariables[0] = (IVariable)varInteger2;
    iVariables[1] = (IVariable)varInteger1;
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger2, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, iVariables);
    IStatement[] iStatements = new IStatement[2];
    iStatements[0] = (IStatement)statementIncrementInteger;
    iStatements[1] = (IStatement)statementIfOnCount;
    statementInlineBlock =
      StatementInlineBlockFactory.Create(iStatements, (IVariable[])null);
    this.Add(statementInlineBlock, (IVariable)null);
}
    }
}
