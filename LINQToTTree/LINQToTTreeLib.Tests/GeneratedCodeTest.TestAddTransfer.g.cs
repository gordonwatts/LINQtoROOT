using LINQToTTreeLib.Variables;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
using LINQToTTreeLib.Statements;
using Microsoft.ExtendedReflection.DataAccess;
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
[ExpectedException(typeof(ArgumentNullException))]
public void TestAddTransferThrowsArgumentNullException145()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[0];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, ss);
    this.TestAddTransfer(generatedCode, (string)null, (object)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentException))]
public void TestAddTransferThrowsArgumentException300()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[0];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, ss);
    object s0 = new object();
    this.TestAddTransfer(generatedCode, (string)null, s0);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentException))]
public void TestAddTransferThrowsArgumentException532()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[0];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, ss);
    object s0 = new object();
    this.TestAddTransfer(generatedCode, "", s0);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestAddTransferThrowsArgumentNullException617()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, ss);
    this.TestAddTransfer(generatedCode, (string)null, (object)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void TestAddTransfer439()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[0];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, ss);
    object s0 = new object();
    this.TestAddTransfer(generatedCode, "\0", s0);
    Assert.IsNotNull((object)generatedCode);
    Assert.AreEqual<int>(1, generatedCode.Depth);
    Assert.IsNotNull(generatedCode.ResultValue);
    Assert.IsNotNull(generatedCode.CodeBody);
    Assert.IsNotNull(generatedCode.VariablesToTransfer);
    Assert.IsNotNull(generatedCode.IncludeFiles);
    Assert.IsNotNull(generatedCode.ReferencedLeafNames);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestAddTransferThrowsArgumentNullException227()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, 1);
    string[] ss = new string[0];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, ss);
    this.TestAddTransfer(generatedCode, (string)null, (object)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestAddTransferThrowsArgumentNullException709()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  ss, (string[])null, (string[])null);
    this.TestAddTransfer(generatedCode, (string)null, (object)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestAddTransferThrowsArgumentNullException882()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[1];
    ss[0] = "\0";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, ss, (string[])null);
    this.TestAddTransfer(generatedCode, (string)null, (object)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestAddTransferThrowsArgumentNullException745()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementIncrementInteger;
    generatedCode = GeneratedCodeFactory.Create((IVariable)varInteger, iStatements, 
                                                (string[])null, (string[])null, (string[])null);
    this.TestAddTransfer(generatedCode, (string)null, (object)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestAddTransferThrowsArgumentNullException944()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[2];
    ss[0] = "\u0100";
    ss[1] = "\u0100";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, ss, (string[])null);
    this.TestAddTransfer(generatedCode, (string)null, (object)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestAddTransferThrowsArgumentNullException600()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[2];
    ss[0] = "\u0101";
    ss[1] = "\u0100";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, ss, (string[])null);
    this.TestAddTransfer(generatedCode, (string)null, (object)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestAddTransferThrowsArgumentNullException282()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[4];
    ss[0] = "\u0100";
    ss[1] = "\u0100";
    ss[2] = "\u0100";
    ss[3] = "\u0100\0";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, ss, (string[])null);
    this.TestAddTransfer(generatedCode, (string)null, (object)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestAddTransferThrowsArgumentNullException576()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[2];
    ss[0] = "\u36ae";
    ss[1] = "\u65e7";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, ss, (string[])null);
    this.TestAddTransfer(generatedCode, (string)null, (object)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void TestAddTransfer489()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[6];
    ss[0] = "\u4ba0";
    ss[1] = "\u4ba0";
    ss[2] = "\u4ba0";
    ss[3] = "\u4ba0";
    ss[4] = "\u4ba0";
    ss[5] = "\u4ba0";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, ss, (string[])null);
    object s0 = new object();
    this.TestAddTransfer(generatedCode, "\u4ba0", s0);
    Assert.IsNotNull((object)generatedCode);
    Assert.AreEqual<int>(1, generatedCode.Depth);
    Assert.IsNotNull(generatedCode.ResultValue);
    Assert.IsNotNull(generatedCode.CodeBody);
    Assert.IsNotNull(generatedCode.VariablesToTransfer);
    Assert.IsNotNull(generatedCode.IncludeFiles);
    Assert.IsNotNull(generatedCode.ReferencedLeafNames);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void TestAddTransfer894()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[6];
    ss[0] = "\u1862";
    ss[1] = "\u1862";
    ss[2] = "\u1862";
    ss[3] = "\u1862";
    ss[4] = "\u1862";
    ss[5] = "\u1862";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, ss, (string[])null);
    object s0 = new object();
    this.TestAddTransfer(generatedCode, "\udc43", s0);
    Assert.IsNotNull((object)generatedCode);
    Assert.AreEqual<int>(1, generatedCode.Depth);
    Assert.IsNotNull(generatedCode.ResultValue);
    Assert.IsNotNull(generatedCode.CodeBody);
    Assert.IsNotNull(generatedCode.VariablesToTransfer);
    Assert.IsNotNull(generatedCode.IncludeFiles);
    Assert.IsNotNull(generatedCode.ReferencedLeafNames);
}
    }
}
