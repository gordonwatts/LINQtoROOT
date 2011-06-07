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
using LINQToTTreeLib.Statements;
using LinqToTTreeInterfacesLib.Moles;
using LINQToTTreeLib.Variables.Moles;
using LINQToTTreeLib.Statements.Moles;

namespace LINQToTTreeLib
{
    public partial class GeneratedCodeTest
    {
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void SetResultThrowsArgumentNullException401()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, 0);
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)varInteger, (IStatement[])null, (string[])null, (string[])null);
    this.SetResult(generatedCode, (IVariable)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void SetResultThrowsArgumentNullException315()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, 1);
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)varInteger, (IStatement[])null, (string[])null, (string[])null);
    this.SetResult(generatedCode, (IVariable)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void SetResultThrowsArgumentNullException623()
{
    SVarInteger sVarInteger;
    SStatementIncrementInteger sStatementIncrementInteger;
    GeneratedCode generatedCode;
    sVarInteger = new SVarInteger();
    sStatementIncrementInteger =
      new SStatementIncrementInteger((VarInteger)sVarInteger);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)sStatementIncrementInteger;
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)sVarInteger, iStatements, (string[])null, (string[])null);
    this.SetResult(generatedCode, (IVariable)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void SetResultThrowsArgumentNullException962()
{
    SVarInteger sVarInteger;
    StatementIncrementInteger statementIncrementInteger;
    GeneratedCode generatedCode;
    sVarInteger = new SVarInteger();
    statementIncrementInteger =
      StatementIncrementIntegerFactory.Create((VarInteger)sVarInteger);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementIncrementInteger;
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)sVarInteger, iStatements, (string[])null, (string[])null);
    this.SetResult(generatedCode, (IVariable)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void SetResultThrowsArgumentNullException643()
{
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    sVarInteger = new SVarInteger();
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, (string[])null);
    this.SetResult(generatedCode, (IVariable)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void SetResultThrowsArgumentNullException33()
{
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    sVarInteger = new SVarInteger();
    string[] ss = new string[1];
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)sVarInteger, (IStatement[])null, ss, (string[])null);
    this.SetResult(generatedCode, (IVariable)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void SetResultThrowsArgumentNullException245()
{
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    sVarInteger = new SVarInteger();
    string[] ss = new string[1];
    ss[0] = "";
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)sVarInteger, (IStatement[])null, ss, (string[])null);
    this.SetResult(generatedCode, (IVariable)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void SetResultThrowsArgumentNullException990()
{
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    sVarInteger = new SVarInteger();
    string[] ss = new string[1];
    ss[0] = "\0";
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)sVarInteger, (IStatement[])null, (string[])null, ss);
    this.SetResult(generatedCode, (IVariable)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void SetResult384()
{
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    sVarInteger = new SVarInteger();
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, (string[])null);
    this.SetResult(generatedCode, (IVariable)sVarInteger);
    Assert.IsNotNull((object)generatedCode);
    Assert.AreEqual<int>(1, generatedCode.Depth);
    Assert.IsNotNull(generatedCode.ResultValue);
    Assert.IsNotNull(generatedCode.CodeBody);
    Assert.IsNotNull(generatedCode.VariablesToTransfer);
    Assert.IsNotNull(generatedCode.IncludeFiles);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void SetResultThrowsArgumentNullException484()
{
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    sVarInteger = new SVarInteger();
    string[] ss = new string[2];
    ss[0] = "";
    ss[1] = "";
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)sVarInteger, (IStatement[])null, ss, (string[])null);
    this.SetResult(generatedCode, (IVariable)null);
}
    }
}
