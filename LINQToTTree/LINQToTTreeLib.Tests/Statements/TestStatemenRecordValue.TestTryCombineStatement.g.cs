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
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using LINQToTTreeLib.Tests.Factories;

namespace LINQToTTreeLib.Tests
{
    public partial class TestStatemenRecordValue
    {
[TestMethod]
[PexGeneratedBy(typeof(TestStatemenRecordValue))]
public void TestTryCombineStatement189()
{
    VarInteger varInteger;
    StatementRecordValue statementRecordValue;
    CodeOptimizerTest codeOptimizerTest;
    bool b;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementRecordValue = StatementRecordValueFactory.Create
                               ((IVariable)varInteger, (IValue)varInteger, (IVariable)varInteger, false);
    codeOptimizerTest = CodeOptimizerTestFactory.Create(false);
    b = this.TestTryCombineStatement
            (statementRecordValue, (IStatement)statementRecordValue, 
                                   (ICodeOptimizationService)codeOptimizerTest);
    Assert.AreEqual<bool>(true, b);
    Assert.IsNotNull((object)statementRecordValue);
    Assert.IsNull(statementRecordValue.Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(TestStatemenRecordValue))]
public void TestTryCombineStatement717()
{
    VarInteger varInteger;
    StatementRecordValue statementRecordValue;
    StatementIncrementInteger statementIncrementInteger;
    bool b;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementRecordValue = StatementRecordValueFactory.Create
                               ((IVariable)varInteger, (IValue)varInteger, (IVariable)varInteger, false);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    b = this.TestTryCombineStatement(statementRecordValue, 
                                     (IStatement)statementIncrementInteger, (ICodeOptimizationService)null);
    Assert.AreEqual<bool>(false, b);
    Assert.IsNotNull((object)statementRecordValue);
    Assert.IsNull(statementRecordValue.Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(TestStatemenRecordValue))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestTryCombineStatementThrowsArgumentNullException279()
{
    VarInteger varInteger;
    StatementRecordValue statementRecordValue;
    bool b;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementRecordValue = StatementRecordValueFactory.Create
                               ((IVariable)varInteger, (IValue)varInteger, (IVariable)varInteger, false);
    b = this.TestTryCombineStatement
            (statementRecordValue, (IStatement)null, (ICodeOptimizationService)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TestStatemenRecordValue))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestTryCombineStatementThrowsArgumentNullException270()
{
    VarInteger varInteger;
    StatementRecordValue statementRecordValue;
    bool b;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementRecordValue = StatementRecordValueFactory.Create
                               ((IVariable)varInteger, (IValue)varInteger, (IVariable)varInteger, false);
    b = this.TestTryCombineStatement(statementRecordValue, 
                                     (IStatement)statementRecordValue, (ICodeOptimizationService)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TestStatemenRecordValue))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestTryCombineStatementThrowsArgumentNullException982()
{
    VarInteger varInteger;
    StatementRecordValue statementRecordValue;
    bool b;
    varInteger = VarIntegerFactory.Create(false, -8);
    statementRecordValue = StatementRecordValueFactory.Create
                               ((IVariable)varInteger, (IValue)varInteger, (IVariable)varInteger, false);
    b = this.TestTryCombineStatement
            (statementRecordValue, (IStatement)null, (ICodeOptimizationService)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TestStatemenRecordValue))]
public void TestTryCombineStatement71702()
{
    VarInteger varInteger;
    VarInteger varInteger1;
    StatementRecordValue statementRecordValue;
    StatementRecordValue statementRecordValue1;
    bool b;
    varInteger = VarIntegerFactory.Create(false, 0);
    varInteger1 = VarIntegerFactory.Create(false, 0);
    statementRecordValue = StatementRecordValueFactory.Create
                               ((IVariable)varInteger, (IValue)varInteger, (IVariable)varInteger, false);
    statementRecordValue1 = StatementRecordValueFactory.Create
                                ((IVariable)varInteger1, (IValue)varInteger1, (IVariable)varInteger, false);
    b = this.TestTryCombineStatement(statementRecordValue1, 
                                     (IStatement)statementRecordValue, (ICodeOptimizationService)null);
    Assert.AreEqual<bool>(false, b);
    Assert.IsNotNull((object)statementRecordValue1);
    Assert.IsNull(statementRecordValue1.Parent);
}
    }
}
