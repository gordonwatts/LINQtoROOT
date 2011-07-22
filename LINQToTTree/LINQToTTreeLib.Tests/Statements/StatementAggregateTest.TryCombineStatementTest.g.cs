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
using LINQToTTreeLib.Variables.Moles;
using LINQToTTreeLib.Statements;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using LINQToTTreeLib.Statements.Moles;
using LINQToTTreeLib.Variables;

namespace LINQToTTreeLib.Tests
{
    public partial class StatementAggregateTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementAggregateTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TryCombineStatementTestThrowsArgumentNullException558()
{
    SVarInteger sVarInteger;
    StatementAggregate statementAggregate;
    bool b;
    sVarInteger = new SVarInteger();
    statementAggregate =
      new StatementAggregate((IVariable)sVarInteger, (IValue)sVarInteger);
    b = this.TryCombineStatementTest(statementAggregate, (IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementAggregateTest))]
public void TryCombineStatementTest363()
{
    SVarInteger sVarInteger;
    StatementAggregate statementAggregate;
    SStatementIncrementInteger sStatementIncrementInteger;
    bool b;
    sVarInteger = new SVarInteger();
    statementAggregate =
      new StatementAggregate((IVariable)sVarInteger, (IValue)sVarInteger);
    sStatementIncrementInteger =
      new SStatementIncrementInteger((VarInteger)sVarInteger);
    b = this.TryCombineStatementTest
            (statementAggregate, (IStatement)sStatementIncrementInteger);
    Assert.AreEqual<bool>(false, b);
    Assert.IsNotNull((object)statementAggregate);
    Assert.IsNotNull(statementAggregate.ResultVariable);
    Assert.IsNotNull(statementAggregate.Expression);
    Assert.IsTrue(object.ReferenceEquals
                      (statementAggregate.Expression, statementAggregate.ResultVariable));
}
[TestMethod]
[PexGeneratedBy(typeof(StatementAggregateTest))]
public void TryCombineStatementTest785()
{
    SVarInteger sVarInteger;
    StatementAggregate statementAggregate;
    bool b;
    sVarInteger = new SVarInteger();
    statementAggregate =
      new StatementAggregate((IVariable)sVarInteger, (IValue)sVarInteger);
    b = this.TryCombineStatementTest
            (statementAggregate, (IStatement)statementAggregate);
    Assert.AreEqual<bool>(true, b);
    Assert.IsNotNull((object)statementAggregate);
    Assert.IsNotNull(statementAggregate.ResultVariable);
    Assert.IsNotNull(statementAggregate.Expression);
    Assert.IsTrue(object.ReferenceEquals
                      (statementAggregate.Expression, statementAggregate.ResultVariable));
}
[TestMethod]
[PexGeneratedBy(typeof(StatementAggregateTest))]
public void TryCombineStatementTest36302()
{
    SVarInteger sVarInteger;
    VarInteger varInteger;
    StatementAggregate statementAggregate;
    VarInteger varInteger1;
    SStatementIncrementInteger sStatementIncrementInteger;
    bool b;
    sVarInteger = new SVarInteger();
    varInteger = VarIntegerFactory.Create(false, 0);
    statementAggregate =
      new StatementAggregate((IVariable)sVarInteger, (IValue)varInteger);
    varInteger1 = VarIntegerFactory.Create(false, 0);
    sStatementIncrementInteger = new SStatementIncrementInteger(varInteger1);
    b = this.TryCombineStatementTest
            (statementAggregate, (IStatement)sStatementIncrementInteger);
    Assert.AreEqual<bool>(false, b);
    Assert.IsNotNull((object)statementAggregate);
    Assert.IsNotNull(statementAggregate.ResultVariable);
    Assert.IsNotNull(statementAggregate.Expression);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementAggregateTest))]
public void TryCombineStatementTest36303()
{
    SVarInteger sVarInteger;
    VarInteger varInteger;
    StatementAggregate statementAggregate;
    VarInteger varInteger1;
    SStatementIncrementInteger sStatementIncrementInteger;
    bool b;
    sVarInteger = new SVarInteger();
    varInteger = VarIntegerFactory.Create(false, 1);
    statementAggregate =
      new StatementAggregate((IVariable)sVarInteger, (IValue)varInteger);
    varInteger1 = VarIntegerFactory.Create(false, 0);
    sStatementIncrementInteger = new SStatementIncrementInteger(varInteger1);
    b = this.TryCombineStatementTest
            (statementAggregate, (IStatement)sStatementIncrementInteger);
    Assert.AreEqual<bool>(false, b);
    Assert.IsNotNull((object)statementAggregate);
    Assert.IsNotNull(statementAggregate.ResultVariable);
    Assert.IsNotNull(statementAggregate.Expression);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementAggregateTest))]
public void TryCombineStatementTest727()
{
    SVarInteger sVarInteger;
    StatementAggregate statementAggregate;
    SVarInteger sVarInteger1;
    StatementAggregate statementAggregate1;
    bool b;
    sVarInteger = new SVarInteger();
    statementAggregate =
      new StatementAggregate((IVariable)sVarInteger, (IValue)sVarInteger);
    sVarInteger1 = new SVarInteger();
    statementAggregate1 =
      new StatementAggregate((IVariable)sVarInteger1, (IValue)sVarInteger);
    b = this.TryCombineStatementTest
            (statementAggregate, (IStatement)statementAggregate1);
    Assert.AreEqual<bool>(false, b);
    Assert.IsNotNull((object)statementAggregate);
    Assert.IsNotNull(statementAggregate.ResultVariable);
    Assert.IsNotNull(statementAggregate.Expression);
    Assert.IsTrue(object.ReferenceEquals
                      (statementAggregate.Expression, statementAggregate.ResultVariable));
}
    }
}
