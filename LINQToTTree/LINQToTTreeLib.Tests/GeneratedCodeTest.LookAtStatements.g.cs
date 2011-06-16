using LINQToTTreeLib.Variables;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using LINQToTTreeLib.Statements;
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

namespace LINQToTTreeLib
{
    public partial class GeneratedCodeTest
    {
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void LookAtStatements273()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, 0);
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)varInteger, (IStatement[])null, (string[])null, (string[])null);
    this.LookAtStatements(generatedCode);
    Assert.IsNotNull((object)generatedCode);
    Assert.AreEqual<int>(1, generatedCode.Depth);
    Assert.IsNotNull(generatedCode.ResultValue);
    Assert.IsNotNull(generatedCode.CodeBody);
    Assert.IsNotNull(generatedCode.VariablesToTransfer);
    Assert.IsNotNull(generatedCode.IncludeFiles);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void LookAtStatements27301()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, 1);
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)varInteger, (IStatement[])null, (string[])null, (string[])null);
    this.LookAtStatements(generatedCode);
    Assert.IsNotNull((object)generatedCode);
    Assert.AreEqual<int>(1, generatedCode.Depth);
    Assert.IsNotNull(generatedCode.ResultValue);
    Assert.IsNotNull(generatedCode.CodeBody);
    Assert.IsNotNull(generatedCode.VariablesToTransfer);
    Assert.IsNotNull(generatedCode.IncludeFiles);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void LookAtStatements27302()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[1];
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)varInteger, (IStatement[])null, ss, (string[])null);
    this.LookAtStatements(generatedCode);
    Assert.IsNotNull((object)generatedCode);
    Assert.AreEqual<int>(1, generatedCode.Depth);
    Assert.IsNotNull(generatedCode.ResultValue);
    Assert.IsNotNull(generatedCode.CodeBody);
    Assert.IsNotNull(generatedCode.VariablesToTransfer);
    Assert.IsNotNull(generatedCode.IncludeFiles);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void LookAtStatements27303()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[1];
    ss[0] = "";
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)varInteger, (IStatement[])null, ss, (string[])null);
    this.LookAtStatements(generatedCode);
    Assert.IsNotNull((object)generatedCode);
    Assert.AreEqual<int>(1, generatedCode.Depth);
    Assert.IsNotNull(generatedCode.ResultValue);
    Assert.IsNotNull(generatedCode.CodeBody);
    Assert.IsNotNull(generatedCode.VariablesToTransfer);
    Assert.IsNotNull(generatedCode.IncludeFiles);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void LookAtStatements27304()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[1];
    ss[0] = "\0";
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)varInteger, (IStatement[])null, (string[])null, ss);
    this.LookAtStatements(generatedCode);
    Assert.IsNotNull((object)generatedCode);
    Assert.AreEqual<int>(1, generatedCode.Depth);
    Assert.IsNotNull(generatedCode.ResultValue);
    Assert.IsNotNull(generatedCode.CodeBody);
    Assert.IsNotNull(generatedCode.VariablesToTransfer);
    Assert.IsNotNull(generatedCode.IncludeFiles);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void LookAtStatements27305()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementIncrementInteger;
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)varInteger, iStatements, (string[])null, (string[])null);
    this.LookAtStatements(generatedCode);
    Assert.IsNotNull((object)generatedCode);
    Assert.AreEqual<int>(1, generatedCode.Depth);
    Assert.IsNotNull(generatedCode.ResultValue);
    Assert.IsNotNull(generatedCode.CodeBody);
    Assert.IsNotNull(generatedCode.VariablesToTransfer);
    Assert.IsNotNull(generatedCode.IncludeFiles);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void LookAtStatements27306()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[2];
    ss[0] = "\u0100";
    ss[1] = "\u0100";
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)varInteger, (IStatement[])null, (string[])null, ss);
    this.LookAtStatements(generatedCode);
    Assert.IsNotNull((object)generatedCode);
    Assert.AreEqual<int>(1, generatedCode.Depth);
    Assert.IsNotNull(generatedCode.ResultValue);
    Assert.IsNotNull(generatedCode.CodeBody);
    Assert.IsNotNull(generatedCode.VariablesToTransfer);
    Assert.IsNotNull(generatedCode.IncludeFiles);
}
    }
}
