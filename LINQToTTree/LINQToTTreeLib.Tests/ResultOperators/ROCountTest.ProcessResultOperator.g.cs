using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.ResultOperators;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
using LINQToTTreeLib.Variables;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Moles;
using LINQToTTreeLib.Variables.Moles;
// <copyright file="ROCountTest.ProcessResultOperator.g.cs" company="Microsoft">Copyright � Microsoft 2010</copyright>
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
    public partial class ROCountTest
    {
[TestMethod]
[PexGeneratedBy(typeof(ROCountTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException153()
{
    IVariable iVariable;
    ROCount s0 = new ROCount();
    iVariable = this.ProcessResultOperator
                    (s0, (CountResultOperator)null, (QueryModel)null, (GeneratedCode)null);
}
[TestMethod]
[PexGeneratedBy(typeof(ROCountTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException515()
{
    SGeneratedCode sGeneratedCode;
    IVariable iVariable;
    sGeneratedCode = new SGeneratedCode();
    ROCount s0 = new ROCount();
    iVariable = this.ProcessResultOperator(s0, (CountResultOperator)null, 
                                           (QueryModel)null, (GeneratedCode)sGeneratedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROCountTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException748()
{
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sVarInteger = new SVarInteger();
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, (string[])null);
    ROCount s0 = new ROCount();
    iVariable = this.ProcessResultOperator
                    (s0, (CountResultOperator)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROCountTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException267()
{
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sVarInteger = new SVarInteger();
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, ss);
    ROCount s0 = new ROCount();
    iVariable = this.ProcessResultOperator
                    (s0, (CountResultOperator)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROCountTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException950()
{
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sVarInteger = new SVarInteger();
    string[] ss = new string[1];
    ss[0] = "";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, ss);
    ROCount s0 = new ROCount();
    iVariable = this.ProcessResultOperator
                    (s0, (CountResultOperator)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROCountTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException844()
{
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sVarInteger = new SVarInteger();
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  ss, (string[])null, (string[])null);
    ROCount s0 = new ROCount();
    iVariable = this.ProcessResultOperator
                    (s0, (CountResultOperator)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROCountTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException355()
{
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sVarInteger = new SVarInteger();
    string[] ss = new string[1];
    ss[0] = "\0";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, ss, (string[])null);
    ROCount s0 = new ROCount();
    iVariable = this.ProcessResultOperator
                    (s0, (CountResultOperator)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROCountTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException510()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, 0);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, (string[])null);
    ROCount s0 = new ROCount();
    iVariable = this.ProcessResultOperator
                    (s0, (CountResultOperator)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROCountTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException516()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, 1);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, (string[])null);
    ROCount s0 = new ROCount();
    iVariable = this.ProcessResultOperator
                    (s0, (CountResultOperator)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROCountTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException83()
{
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sVarInteger = new SVarInteger();
    string[] ss = new string[2];
    ss[0] = "\0";
    ss[1] = "\0";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, ss, (string[])null);
    ROCount s0 = new ROCount();
    iVariable = this.ProcessResultOperator
                    (s0, (CountResultOperator)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROCountTest))]
public void ProcessResultOperator4401()
{
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sVarInteger = new SVarInteger();
    string[] ss = new string[2];
    ss[0] = "\u0089";
    ss[1] = "\u0089";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, ss, (string[])null);
    ROCount s0 = new ROCount();
    CountResultOperator s1 = new CountResultOperator();
    iVariable = this.ProcessResultOperator(s0, s1, (QueryModel)null, generatedCode);
    Assert.IsNotNull((object)iVariable);
    Assert.IsNotNull((object)s0);
}
[TestMethod]
[PexGeneratedBy(typeof(ROCountTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException719()
{
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sVarInteger = new SVarInteger();
    string[] ss = new string[2];
    string[] ss1 = new string[6];
    ss[0] = "\u0089";
    ss[1] = "\u0089";
    ss1[2] = "";
    ss1[3] = "";
    ss1[4] = "\u0089\0\0\0\0\0\0";
    ss1[5] = "\u0089";
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)sVarInteger, (IStatement[])null, (string[])null, ss, ss1);
    ROCount s0 = new ROCount();
    iVariable = this.ProcessResultOperator
                    (s0, (CountResultOperator)null, (QueryModel)null, generatedCode);
}
    }
}
