using LINQToTTreeLib.Variables;
using LinqToTTreeInterfacesLib;
using ROOTNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
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
[ExpectedException(typeof(ArgumentNullException))]
public void TestAddTransferThrowsArgumentNullException72()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, 0);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (NTObject[])null, (string[])null);
    this.TestAddTransfer(generatedCode, (object)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void TestAddTransfer403()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, 0);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (NTObject[])null, (string[])null);
    object s0 = new object();
    this.TestAddTransfer(generatedCode, s0);
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
public void TestAddTransferThrowsArgumentNullException371()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, 1);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (NTObject[])null, (string[])null);
    this.TestAddTransfer(generatedCode, (object)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestAddTransferThrowsArgumentNullException224()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  ss, (NTObject[])null, (string[])null);
    this.TestAddTransfer(generatedCode, (object)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestAddTransferThrowsArgumentNullException196()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (NTObject[])null, ss);
    this.TestAddTransfer(generatedCode, (object)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestAddTransferThrowsArgumentNullException234()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    GeneratedCode generatedCode;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementIncrementInteger;
    generatedCode = GeneratedCodeFactory.Create((IVariable)varInteger, iStatements, 
                                                (string[])null, (NTObject[])null, (string[])null);
    this.TestAddTransfer(generatedCode, (object)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestAddTransferThrowsArgumentNullException647()
{
    using (PexDisposableContext disposables = PexDisposableContext.Create())
    {
      VarInteger varInteger;
      NTObject nTObject;
      GeneratedCode generatedCode;
      varInteger = VarIntegerFactory.Create(false, 0);
      nTObject = NTObjectFactory.Create();
      disposables.Add((IDisposable)nTObject);
      NTObject[] nTObjects = new NTObject[1];
      nTObjects[0] = nTObject;
      generatedCode =
        GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                    (string[])null, nTObjects, (string[])null);
      this.TestAddTransfer(generatedCode, (object)null);
      disposables.Dispose();
    }
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void TestAddTransfer40301()
{
    using (PexDisposableContext disposables = PexDisposableContext.Create())
    {
      VarInteger varInteger;
      NTObject nTObject;
      GeneratedCode generatedCode;
      varInteger = VarIntegerFactory.Create(false, int.MaxValue);
      nTObject = NTObjectFactory.Create();
      disposables.Add((IDisposable)nTObject);
      NTObject[] nTObjects = new NTObject[1];
      nTObjects[0] = nTObject;
      generatedCode =
        GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                    (string[])null, nTObjects, (string[])null);
      object s0 = new object();
      this.TestAddTransfer(generatedCode, s0);
      disposables.Dispose();
      Assert.IsNotNull((object)generatedCode);
      Assert.AreEqual<int>(1, generatedCode.Depth);
      Assert.IsNotNull(generatedCode.ResultValue);
      Assert.IsNotNull(generatedCode.CodeBody);
      Assert.IsNotNull(generatedCode.VariablesToTransfer);
      Assert.IsNotNull(generatedCode.IncludeFiles);
      Assert.IsNotNull(generatedCode.ReferencedLeafNames);
    }
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void TestAddTransfer326()
{
    using (PexDisposableContext disposables = PexDisposableContext.Create())
    {
      VarInteger varInteger;
      NTObject nTObject;
      GeneratedCode generatedCode;
      varInteger = VarIntegerFactory.Create(false, int.MaxValue);
      nTObject = NTObjectFactory.Create();
      disposables.Add((IDisposable)nTObject);
      NTObject[] nTObjects = new NTObject[1];
      nTObjects[0] = nTObject;
      generatedCode =
        GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                    (string[])null, nTObjects, (string[])null);
      this.TestAddTransfer(generatedCode, (object)nTObject);
      disposables.Dispose();
      Assert.IsNotNull((object)generatedCode);
      Assert.AreEqual<int>(1, generatedCode.Depth);
      Assert.IsNotNull(generatedCode.ResultValue);
      Assert.IsNotNull(generatedCode.CodeBody);
      Assert.IsNotNull(generatedCode.VariablesToTransfer);
      Assert.IsNotNull(generatedCode.IncludeFiles);
      Assert.IsNotNull(generatedCode.ReferencedLeafNames);
    }
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestAddTransferThrowsArgumentNullException340()
{
    using (PexDisposableContext disposables = PexDisposableContext.Create())
    {
      VarInteger varInteger;
      NTObject nTObject;
      NTObject nTObject1;
      GeneratedCode generatedCode;
      varInteger = VarIntegerFactory.Create(false, int.MaxValue);
      nTObject = NTObjectFactory.Create();
      disposables.Add((IDisposable)nTObject);
      nTObject1 = NTObjectFactory.Create();
      disposables.Add((IDisposable)nTObject1);
      NTObject[] nTObjects = new NTObject[2];
      nTObjects[0] = nTObject;
      nTObjects[1] = nTObject1;
      generatedCode =
        GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                    (string[])null, nTObjects, (string[])null);
      this.TestAddTransfer(generatedCode, (object)null);
      disposables.Dispose();
    }
}
    }
}
