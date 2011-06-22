using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
using LINQToTTreeLib.Variables;
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

namespace LINQToTTreeLib.TypeHandlers.ROOT
{
    public partial class TypeHanlderROOTTest
    {
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessNewThrowsArgumentException375()
{
    Expression expression;
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    IValue iValue = (IValue)null;
    expression = this.ProcessNew(s0, (NewExpression)null, 
                                 out iValue, (GeneratedCode)null, (CodeContext)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessNewThrowsArgumentException741()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    Expression expression;
    varInteger = VarIntegerFactory.Create(false, 0);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, (string[])null);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    IValue iValue = (IValue)null;
    expression = this.ProcessNew
                     (s0, (NewExpression)null, out iValue, generatedCode, (CodeContext)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessNewThrowsArgumentException981()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    Expression expression;
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  ss, (string[])null, (string[])null);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    IValue iValue = (IValue)null;
    expression = this.ProcessNew
                     (s0, (NewExpression)null, out iValue, generatedCode, (CodeContext)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessNewThrowsArgumentException395()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    Expression expression;
    varInteger = VarIntegerFactory.Create(false, 1);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, (string[])null);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    IValue iValue = (IValue)null;
    expression = this.ProcessNew
                     (s0, (NewExpression)null, out iValue, generatedCode, (CodeContext)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessNewThrowsArgumentException111()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    Expression expression;
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[1];
    ss[0] = "";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  ss, (string[])null, (string[])null);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    IValue iValue = (IValue)null;
    expression = this.ProcessNew
                     (s0, (NewExpression)null, out iValue, generatedCode, (CodeContext)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessNewThrowsArgumentException890()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    Expression expression;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, ss);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    IValue iValue = (IValue)null;
    expression = this.ProcessNew
                     (s0, (NewExpression)null, out iValue, generatedCode, (CodeContext)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessNewThrowsArgumentException739()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    Expression expression;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[1];
    ss[0] = "\0";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, ss, (string[])null);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    IValue iValue = (IValue)null;
    expression = this.ProcessNew
                     (s0, (NewExpression)null, out iValue, generatedCode, (CodeContext)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessNewThrowsArgumentException187()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    GeneratedCode generatedCode;
    Expression expression;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementIncrementInteger;
    generatedCode = GeneratedCodeFactory.Create((IVariable)varInteger, iStatements, 
                                                (string[])null, (string[])null, (string[])null);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    IValue iValue = (IValue)null;
    expression = this.ProcessNew
                     (s0, (NewExpression)null, out iValue, generatedCode, (CodeContext)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessNewThrowsArgumentException406()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    Expression expression;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[2];
    ss[0] = "";
    ss[1] = "";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  ss, (string[])null, (string[])null);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    IValue iValue = (IValue)null;
    expression = this.ProcessNew
                     (s0, (NewExpression)null, out iValue, generatedCode, (CodeContext)null);
}
    }
}
