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
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace LINQToTTreeLib.Tests
{
    public partial class StatementRecordIndiciesTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementRecordIndiciesTest))]
public void TryCombineStatementTest785()
{
    VarInteger varInteger;
    StatementRecordIndicies statementRecordIndicies;
    bool b;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementRecordIndicies =
      StatementRecordIndiciesFactory.Create((IValue)varInteger, (IValue)varInteger);
    b = this.TryCombineStatementTest
            (statementRecordIndicies, (IStatement)statementRecordIndicies);
    Assert.AreEqual<bool>(true, b);
    Assert.IsNotNull((object)statementRecordIndicies);
    Assert.IsNull(statementRecordIndicies.HolderArray);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementRecordIndiciesTest))]
public void TryCombineStatementTest854()
{
    VarInteger varInteger;
    StatementRecordIndicies statementRecordIndicies;
    StatementIncrementInteger statementIncrementInteger;
    bool b;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementRecordIndicies =
      StatementRecordIndiciesFactory.Create((IValue)varInteger, (IValue)varInteger);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    b = this.TryCombineStatementTest
            (statementRecordIndicies, (IStatement)statementIncrementInteger);
    Assert.AreEqual<bool>(false, b);
    Assert.IsNotNull((object)statementRecordIndicies);
    Assert.IsNull(statementRecordIndicies.HolderArray);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementRecordIndiciesTest))]
[ExpectedException(typeof(ArgumentException))]
public void TryCombineStatementTestThrowsArgumentException18()
{
    VarInteger varInteger;
    StatementRecordIndicies statementRecordIndicies;
    bool b;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementRecordIndicies =
      StatementRecordIndiciesFactory.Create((IValue)varInteger, (IValue)varInteger);
    b = this.TryCombineStatementTest(statementRecordIndicies, (IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementRecordIndiciesTest))]
[ExpectedException(typeof(ArgumentException))]
public void TryCombineStatementTestThrowsArgumentException338()
{
    VarInteger varInteger;
    StatementRecordIndicies statementRecordIndicies;
    bool b;
    varInteger = VarIntegerFactory.Create(false, 1);
    statementRecordIndicies =
      StatementRecordIndiciesFactory.Create((IValue)varInteger, (IValue)varInteger);
    b = this.TryCombineStatementTest(statementRecordIndicies, (IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementRecordIndiciesTest))]
public void TryCombineStatementTest72701()
{
    VarInteger varInteger;
    VarInteger varInteger1;
    StatementRecordIndicies statementRecordIndicies;
    StatementRecordIndicies statementRecordIndicies1;
    bool b;
    varInteger = VarIntegerFactory.Create(false, 0);
    varInteger1 = VarIntegerFactory.Create(false, 0);
    statementRecordIndicies =
      StatementRecordIndiciesFactory.Create((IValue)varInteger, (IValue)varInteger);
    statementRecordIndicies1 =
      StatementRecordIndiciesFactory.Create((IValue)varInteger1, (IValue)varInteger);
    b = this.TryCombineStatementTest
            (statementRecordIndicies, (IStatement)statementRecordIndicies1);
    Assert.AreEqual<bool>(false, b);
    Assert.IsNotNull((object)statementRecordIndicies);
    Assert.IsNull(statementRecordIndicies.HolderArray);
}
    }
}
