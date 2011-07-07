using System.Linq.Expressions;
using LINQToTTreeLib.Variables;
using LINQToTTreeLib;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
using LINQToTTreeLib.Statements;
using ROOTNET;
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

namespace LINQToTTreeLib.TypeHandlers.ROOT
{
    public partial class TypeHanlderROOTTest
    {
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessConstantReferenceThrowsArgumentException304()
{
    ConstantExpression constantExpression;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IValue iValue;
    constantExpression = ConstantExpressionFactory.Create(0);
    varInteger = VarIntegerFactory.Create(false, 0);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (NTObject[])null, (string[])null);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    iValue = this.ProcessConstantReference
                 (s0, constantExpression, (IGeneratedQueryCode)generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessConstantReferenceThrowsArgumentException319()
{
    ConstantExpression constantExpression;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IValue iValue;
    constantExpression = ConstantExpressionFactory.Create(0);
    varInteger = VarIntegerFactory.Create(false, 1);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (NTObject[])null, (string[])null);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    iValue = this.ProcessConstantReference
                 (s0, constantExpression, (IGeneratedQueryCode)generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessConstantReferenceThrowsArgumentException167()
{
    ConstantExpression constantExpression;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IValue iValue;
    constantExpression = ConstantExpressionFactory.Create(0);
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  ss, (NTObject[])null, (string[])null);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    iValue = this.ProcessConstantReference
                 (s0, constantExpression, (IGeneratedQueryCode)generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessConstantReferenceThrowsArgumentException740()
{
    ConstantExpression constantExpression;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IValue iValue;
    constantExpression = ConstantExpressionFactory.Create(0);
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (NTObject[])null, ss);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    iValue = this.ProcessConstantReference
                 (s0, constantExpression, (IGeneratedQueryCode)generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessConstantReferenceThrowsArgumentException563()
{
    ConstantExpression constantExpression;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IValue iValue;
    constantExpression = ConstantExpressionFactory.Create(0);
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[1];
    ss[0] = "";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  ss, (NTObject[])null, (string[])null);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    iValue = this.ProcessConstantReference
                 (s0, constantExpression, (IGeneratedQueryCode)generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
public void ProcessConstantReference980()
{
    ConstantExpression constantExpression;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IValue iValue;
    constantExpression = ConstantExpressionFactory.Create(1);
    varInteger = VarIntegerFactory.Create(false, 0);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (NTObject[])null, (string[])null);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    iValue = this.ProcessConstantReference
                 (s0, constantExpression, (IGeneratedQueryCode)generatedCode);
    Assert.IsNotNull((object)iValue);
    Assert.IsNotNull((object)s0);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessConstantReferenceThrowsArgumentException322()
{
    ConstantExpression constantExpression;
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    GeneratedCode generatedCode;
    IValue iValue;
    constantExpression = ConstantExpressionFactory.Create(0);
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    IStatement[] iStatements = new IStatement[1];
    string[] ss = new string[1];
    string[] ss1 = new string[1];
    iStatements[0] = (IStatement)statementIncrementInteger;
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)varInteger, iStatements, ss, (NTObject[])null, ss1);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    iValue = this.ProcessConstantReference
                 (s0, constantExpression, (IGeneratedQueryCode)generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
public void ProcessConstantReference98001()
{
    using (PexDisposableContext disposables = PexDisposableContext.Create())
    {
      ConstantExpression constantExpression;
      VarInteger varInteger;
      NTObject nTObject;
      GeneratedCode generatedCode;
      IValue iValue;
      constantExpression = ConstantExpressionFactory.Create(1);
      varInteger = VarIntegerFactory.Create(false, int.MaxValue);
      nTObject = NTObjectFactory.Create();
      disposables.Add((IDisposable)nTObject);
      NTObject[] nTObjects = new NTObject[1];
      string[] ss = new string[1];
      nTObjects[0] = nTObject;
      generatedCode =
        GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                    (string[])null, nTObjects, ss);
      TypeHandlerROOT s0 = new TypeHandlerROOT();
      iValue = this.ProcessConstantReference
                   (s0, constantExpression, (IGeneratedQueryCode)generatedCode);
      disposables.Dispose();
      Assert.IsNotNull((object)iValue);
      Assert.IsNotNull((object)s0);
    }
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
public void ProcessConstantReference98002()
{
    ConstantExpression constantExpression;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IValue iValue;
    constantExpression = ConstantExpressionFactory.Create(1);
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[2];
    ss[0] = "";
    ss[1] = "";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  ss, (NTObject[])null, (string[])null);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    iValue = this.ProcessConstantReference
                 (s0, constantExpression, (IGeneratedQueryCode)generatedCode);
    Assert.IsNotNull((object)iValue);
    Assert.IsNotNull((object)s0);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessConstantReferenceThrowsArgumentException26()
{
    using (PexDisposableContext disposables = PexDisposableContext.Create())
    {
      ConstantExpression constantExpression;
      VarInteger varInteger;
      NTObject nTObject;
      NTObject nTObject1;
      GeneratedCode generatedCode;
      IValue iValue;
      constantExpression = ConstantExpressionFactory.Create(0);
      varInteger = VarIntegerFactory.Create(false, int.MaxValue);
      nTObject = NTObjectFactory.Create();
      disposables.Add((IDisposable)nTObject);
      nTObject1 = NTObjectFactory.Create();
      disposables.Add((IDisposable)nTObject1);
      NTObject[] nTObjects = new NTObject[3];
      nTObjects[0] = nTObject;
      nTObjects[1] = nTObject1;
      nTObjects[2] = nTObject;
      generatedCode =
        GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                    (string[])null, nTObjects, (string[])null);
      TypeHandlerROOT s0 = new TypeHandlerROOT();
      iValue = this.ProcessConstantReference
                   (s0, constantExpression, (IGeneratedQueryCode)generatedCode);
      disposables.Dispose();
    }
}
    }
}
