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
using LINQToTTreeLib.Variables;
using LinqToTTreeInterfacesLib.Moles;
using LINQToTTreeLib.Variables.Moles;
using LINQToTTreeLib.Statements.Moles;
using Microsoft.Pex.Framework.Moles;
using Microsoft.Pex.Framework;

namespace LINQToTTreeLib.Statements
{
    public partial class StatementInlineBlockTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestAddSingleStatementThrowsArgumentNullException484()
{
    this.TestAddSingleStatement((IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void TestAddSingleStatement460()
{
    SIStatement sIStatement;
    sIStatement = new SIStatement();
    this.TestAddSingleStatement((IStatement)sIStatement);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void TestAddSingleStatement818()
{
    StatementInlineBlock statementInlineBlock;
    statementInlineBlock =
      StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
    this.TestAddSingleStatement((IStatement)statementInlineBlock);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void TestAddSingleStatement622()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    this.TestAddSingleStatement((IStatement)statementIncrementInteger);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void TestAddSingleStatement62201()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    varInteger = VarIntegerFactory.Create(false, 1);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    this.TestAddSingleStatement((IStatement)statementIncrementInteger);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void TestAddSingleStatement282()
{
    VarInteger varInteger;
    SStatementIncrementInteger sStatementIncrementInteger;
    varInteger = VarIntegerFactory.Create(false, 0);
    sStatementIncrementInteger = new SStatementIncrementInteger(varInteger);
    this.TestAddSingleStatement((IStatement)sStatementIncrementInteger);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void TestAddSingleStatement23101()
{
    SIValue sIValue;
    SStatementIfOnCount sStatementIfOnCount;
    sIValue = new SIValue();
    sStatementIfOnCount = new SStatementIfOnCount((IValue)sIValue, 
                                                  (IValue)sIValue, StatementIfOnCount.ComparisonOperator.GreaterThan);
    this.TestAddSingleStatement((IStatement)sStatementIfOnCount);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void TestAddSingleStatement81806()
{
    SIValue sIValue;
    StatementIfOnCount statementIfOnCount;
    StatementInlineBlock statementInlineBlock;
    sIValue = new SIValue();
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IValue)sIValue, (IValue)sIValue, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, (IVariable[])null);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementIfOnCount;
    statementInlineBlock =
      StatementInlineBlockFactory.Create(iStatements, (IVariable[])null);
    this.TestAddSingleStatement((IStatement)statementInlineBlock);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void TestAddSingleStatement81807()
{
    SIValue sIValue;
    SVarInteger sVarInteger;
    StatementIfOnCount statementIfOnCount;
    StatementInlineBlock statementInlineBlock;
    sIValue = new SIValue();
    sVarInteger = new SVarInteger();
    IVariable[] iVariables = new IVariable[1];
    iVariables[0] = (IVariable)sVarInteger;
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IValue)sIValue, (IValue)sIValue, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, iVariables);
    IStatement[] iStatements = new IStatement[1];
    IVariable[] iVariables1 = new IVariable[1];
    iStatements[0] = (IStatement)statementIfOnCount;
    statementInlineBlock =
      StatementInlineBlockFactory.Create(iStatements, iVariables1);
    this.TestAddSingleStatement((IStatement)statementInlineBlock);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
public void TestAddSingleStatement81808()
{
    using (PexChooseBehavedBehavior.Setup())
    {
      SIValue sIValue;
      SIVariable sIVariable;
      StatementIfOnCount statementIfOnCount;
      StatementInlineBlock statementInlineBlock;
      sIValue = new SIValue();
      sIVariable = new SIVariable();
      IPexChoiceRecorder choices = PexChoose.Replay.Setup();
      choices.NextSegment(2).DefaultSession
          .At(1, "sIVariable.LinqToTTreeInterfacesLib.IVariable.get_VariableName", "");
      IVariable[] iVariables = new IVariable[2];
      iVariables[0] = (IVariable)sIVariable;
      iVariables[1] = (IVariable)sIVariable;
      statementIfOnCount =
        StatementIfOnCountFactory.Create((IValue)sIValue, (IValue)sIValue, 
                                         StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                         (IStatement[])null, iVariables);
      IStatement[] iStatements = new IStatement[1];
      iStatements[0] = (IStatement)statementIfOnCount;
      statementInlineBlock =
        StatementInlineBlockFactory.Create(iStatements, (IVariable[])null);
      this.TestAddSingleStatement((IStatement)statementInlineBlock);
    }
}
    }
}
