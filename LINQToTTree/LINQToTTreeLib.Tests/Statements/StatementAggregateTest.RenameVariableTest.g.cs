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

namespace LINQToTTreeLib.Tests
{
    public partial class StatementAggregateTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementAggregateTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void RenameVariableTestThrowsArgumentNullException329()
{
    VarInteger varInteger;
    StatementAggregate statementAggregate;
    StatementAggregate statementAggregate1;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementAggregate =
      new StatementAggregate((IVariable)varInteger, (IValue)varInteger);
    statementAggregate.Parent = (IStatement)null;
    statementAggregate1 =
      this.RenameVariableTest(statementAggregate, (string)null, (string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementAggregateTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void RenameVariableTestThrowsArgumentNullException908()
{
    VarInteger varInteger;
    StatementAggregate statementAggregate;
    StatementAggregate statementAggregate1;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementAggregate =
      new StatementAggregate((IVariable)varInteger, (IValue)varInteger);
    statementAggregate.Parent = (IStatement)null;
    statementAggregate1 =
      this.RenameVariableTest(statementAggregate, "", (string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementAggregateTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void RenameVariableTestThrowsArgumentNullException706()
{
    VarInteger varInteger;
    StatementAggregate statementAggregate;
    StatementAggregate statementAggregate1;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementAggregate =
      new StatementAggregate((IVariable)varInteger, (IValue)varInteger);
    statementAggregate.Parent = (IStatement)null;
    statementAggregate1 =
      this.RenameVariableTest(statementAggregate, "an", (string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementAggregateTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void RenameVariableTestThrowsArgumentNullException964()
{
    VarInteger varInteger;
    StatementAggregate statementAggregate;
    StatementAggregate statementAggregate1;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementAggregate =
      new StatementAggregate((IVariable)varInteger, (IValue)varInteger);
    statementAggregate.Parent = (IStatement)null;
    statementAggregate1 =
      this.RenameVariableTest(statementAggregate, "\u0100", (string)null);
}
    }
}
