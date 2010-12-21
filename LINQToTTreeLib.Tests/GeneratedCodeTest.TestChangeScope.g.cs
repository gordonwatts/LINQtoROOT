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
using LINQToTTreeLib.Statements.Moles;
using LINQToTTreeLib.Variables;
using LINQToTTreeLib.Statements;

namespace LINQToTTreeLib
{
    public partial class GeneratedCodeTest
    {
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestChangeScopeThrowsArgumentNullException854()
{
    GeneratedCode generatedCode;
    generatedCode = GeneratedCodeFactory.Create();
    this.TestChangeScope(generatedCode, (IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void TestChangeScope877()
{
    GeneratedCode generatedCode;
    generatedCode = GeneratedCodeFactory.Create();
    StatementInlineBlock statementInlineBlock = new StatementInlineBlock();
    this.TestChangeScope(generatedCode, (IStatement)statementInlineBlock);
    Assert.IsNotNull((object)generatedCode);
    Assert.IsNull(generatedCode.ResultValue);
    Assert.IsNotNull(generatedCode.CodeBody);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void TestChangeScope149()
{
    GeneratedCode generatedCode;
    SStatementIncrementInteger sStatementIncrementInteger;
    generatedCode = GeneratedCodeFactory.Create();
    VarInteger s0 = new VarInteger();
    sStatementIncrementInteger = new SStatementIncrementInteger(s0);
    this.TestChangeScope(generatedCode, (IStatement)sStatementIncrementInteger);
    Assert.IsNotNull((object)generatedCode);
    Assert.IsNull(generatedCode.ResultValue);
    Assert.IsNotNull(generatedCode.CodeBody);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void TestChangeScope703()
{
    GeneratedCode generatedCode;
    StatementIncrementInteger statementIncrementInteger;
    generatedCode = GeneratedCodeFactory.Create();
    VarInteger s0 = new VarInteger();
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(s0);
    this.TestChangeScope(generatedCode, (IStatement)statementIncrementInteger);
    Assert.IsNotNull((object)generatedCode);
    Assert.IsNull(generatedCode.ResultValue);
    Assert.IsNotNull(generatedCode.CodeBody);
}
    }
}
