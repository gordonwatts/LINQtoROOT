using LINQToTTreeLib.Variables.Moles;
using LINQToTTreeLib.Variables;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
using LINQToTTreeLib.Statements.Moles;
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
    public partial class StatementIncrementIntegerTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementIncrementIntegerTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestEquivThrowsArgumentNullException320()
{
    SVarInteger sVarInteger;
    StatementIncrementInteger statementIncrementInteger;
    sVarInteger = new SVarInteger();
    statementIncrementInteger =
      StatementIncrementIntegerFactory.Create((VarInteger)sVarInteger);
    this.TestEquiv(statementIncrementInteger, (IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIncrementIntegerTest))]
public void TestEquiv308()
{
    SVarInteger sVarInteger;
    StatementIncrementInteger statementIncrementInteger;
    sVarInteger = new SVarInteger();
    statementIncrementInteger =
      StatementIncrementIntegerFactory.Create((VarInteger)sVarInteger);
    this.TestEquiv(statementIncrementInteger, (IStatement)statementIncrementInteger);
    Assert.IsNotNull((object)statementIncrementInteger);
    Assert.IsNotNull(statementIncrementInteger.Integer);
    Assert.AreEqual<string>("anint_1", statementIncrementInteger.Integer.RawValue);
    Assert.AreEqual<string>
        ("anint_1", statementIncrementInteger.Integer.VariableName);
    Assert.IsNotNull(statementIncrementInteger.Integer.InitialValue);
    Assert.AreEqual<bool>(true, statementIncrementInteger.Integer.Declare);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIncrementIntegerTest))]
public void TestEquiv406()
{
    SVarInteger sVarInteger;
    StatementIncrementInteger statementIncrementInteger;
    SStatementIncrementInteger sStatementIncrementInteger;
    sVarInteger = new SVarInteger();
    statementIncrementInteger =
      StatementIncrementIntegerFactory.Create((VarInteger)sVarInteger);
    sStatementIncrementInteger =
      new SStatementIncrementInteger((VarInteger)sVarInteger);
    this.TestEquiv
        (statementIncrementInteger, (IStatement)sStatementIncrementInteger);
    Assert.IsNotNull((object)statementIncrementInteger);
    Assert.IsNotNull(statementIncrementInteger.Integer);
    Assert.AreEqual<string>("anint_1", statementIncrementInteger.Integer.RawValue);
    Assert.AreEqual<string>
        ("anint_1", statementIncrementInteger.Integer.VariableName);
    Assert.IsNotNull(statementIncrementInteger.Integer.InitialValue);
    Assert.AreEqual<bool>(true, statementIncrementInteger.Integer.Declare);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIncrementIntegerTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestEquivThrowsArgumentNullException578()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    this.TestEquiv(statementIncrementInteger, (IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIncrementIntegerTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestEquivThrowsArgumentNullException674()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    varInteger = VarIntegerFactory.Create(false, 1);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    this.TestEquiv(statementIncrementInteger, (IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIncrementIntegerTest))]
public void TestEquiv40601()
{
    SVarInteger sVarInteger;
    StatementIncrementInteger statementIncrementInteger;
    SVarInteger sVarInteger1;
    SStatementIncrementInteger sStatementIncrementInteger;
    sVarInteger = new SVarInteger();
    statementIncrementInteger =
      StatementIncrementIntegerFactory.Create((VarInteger)sVarInteger);
    sVarInteger1 = new SVarInteger();
    sStatementIncrementInteger =
      new SStatementIncrementInteger((VarInteger)sVarInteger1);
    this.TestEquiv
        (statementIncrementInteger, (IStatement)sStatementIncrementInteger);
    Assert.IsNotNull((object)statementIncrementInteger);
    Assert.IsNotNull(statementIncrementInteger.Integer);
    Assert.AreEqual<string>("anint_1", statementIncrementInteger.Integer.RawValue);
    Assert.AreEqual<string>
        ("anint_1", statementIncrementInteger.Integer.VariableName);
    Assert.IsNotNull(statementIncrementInteger.Integer.InitialValue);
    Assert.AreEqual<bool>(true, statementIncrementInteger.Integer.Declare);
}
    }
}
