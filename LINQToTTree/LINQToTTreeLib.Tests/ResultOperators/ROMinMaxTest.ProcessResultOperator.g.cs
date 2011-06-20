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
using Remotion.Linq.Clauses;
using Remotion.Linq;
using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using LINQToTTreeLib.Variables;
using LINQToTTreeLib;
using Remotion.Linq.Clauses.ResultOperators;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Moles;
using LINQToTTreeLib.Variables.Moles;
using LINQToTTreeLib.Statements.Moles;

namespace LINQToTTreeLib.ResultOperators
{
    public partial class ROMinMaxTest
    {
[TestMethod]
[PexGeneratedBy(typeof(ROMinMaxTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException451()
{
    IVariable iVariable;
    ROMinMax s0 = new ROMinMax();
    iVariable =
      this.ProcessResultOperator(s0, (ResultOperatorBase)null, (QueryModel)null, 
                                 (IGeneratedQueryCode)null, (ICodeContext)null, (CompositionContainer)null);
}
[TestMethod]
[PexGeneratedBy(typeof(ROMinMaxTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException695()
{
    SCodeContext sCodeContext;
    IVariable iVariable;
    sCodeContext = new SCodeContext();
    ROMinMax s0 = new ROMinMax();
    iVariable =
      this.ProcessResultOperator(s0, (ResultOperatorBase)null, (QueryModel)null, 
                                 (IGeneratedQueryCode)null, 
                                 (ICodeContext)sCodeContext, (CompositionContainer)null);
}
[TestMethod]
[PexGeneratedBy(typeof(ROMinMaxTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException134()
{
    SCodeContext sCodeContext;
    SGeneratedCode sGeneratedCode;
    IVariable iVariable;
    sCodeContext = new SCodeContext();
    sGeneratedCode = new SGeneratedCode();
    ROMinMax s0 = new ROMinMax();
    iVariable =
      this.ProcessResultOperator(s0, (ResultOperatorBase)null, (QueryModel)null, 
                                 (IGeneratedQueryCode)sGeneratedCode, 
                                 (ICodeContext)sCodeContext, (CompositionContainer)null);
}
[TestMethod]
[PexGeneratedBy(typeof(ROMinMaxTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException827()
{
    SCodeContext sCodeContext;
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sCodeContext = new SCodeContext();
    sVarInteger = new SVarInteger();
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, (string[])null);
    ROMinMax s0 = new ROMinMax();
    iVariable =
      this.ProcessResultOperator(s0, (ResultOperatorBase)null, (QueryModel)null, 
                                 (IGeneratedQueryCode)generatedCode, 
                                 (ICodeContext)sCodeContext, (CompositionContainer)null);
}
[TestMethod]
[PexGeneratedBy(typeof(ROMinMaxTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException865()
{
    SCodeContext sCodeContext;
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sCodeContext = new SCodeContext();
    sVarInteger = new SVarInteger();
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, ss);
    ROMinMax s0 = new ROMinMax();
    iVariable =
      this.ProcessResultOperator(s0, (ResultOperatorBase)null, (QueryModel)null, 
                                 (IGeneratedQueryCode)generatedCode, 
                                 (ICodeContext)sCodeContext, (CompositionContainer)null);
}
[TestMethod]
[PexGeneratedBy(typeof(ROMinMaxTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException726()
{
    SCodeContext sCodeContext;
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sCodeContext = new SCodeContext();
    sVarInteger = new SVarInteger();
    string[] ss = new string[1];
    ss[0] = "";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, ss);
    ROMinMax s0 = new ROMinMax();
    iVariable =
      this.ProcessResultOperator(s0, (ResultOperatorBase)null, (QueryModel)null, 
                                 (IGeneratedQueryCode)generatedCode, 
                                 (ICodeContext)sCodeContext, (CompositionContainer)null);
}
[TestMethod]
[PexGeneratedBy(typeof(ROMinMaxTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException222()
{
    SCodeContext sCodeContext;
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sCodeContext = new SCodeContext();
    sVarInteger = new SVarInteger();
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  ss, (string[])null, (string[])null);
    ROMinMax s0 = new ROMinMax();
    iVariable =
      this.ProcessResultOperator(s0, (ResultOperatorBase)null, (QueryModel)null, 
                                 (IGeneratedQueryCode)generatedCode, 
                                 (ICodeContext)sCodeContext, (CompositionContainer)null);
}
[TestMethod]
[PexGeneratedBy(typeof(ROMinMaxTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException354()
{
    SCodeContext sCodeContext;
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sCodeContext = new SCodeContext();
    sVarInteger = new SVarInteger();
    string[] ss = new string[1];
    ss[0] = "\0";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, ss, (string[])null);
    ROMinMax s0 = new ROMinMax();
    iVariable =
      this.ProcessResultOperator(s0, (ResultOperatorBase)null, (QueryModel)null, 
                                 (IGeneratedQueryCode)generatedCode, 
                                 (ICodeContext)sCodeContext, (CompositionContainer)null);
}
[TestMethod]
[PexGeneratedBy(typeof(ROMinMaxTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException594()
{
    SCodeContext sCodeContext;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sCodeContext = new SCodeContext();
    varInteger = VarIntegerFactory.Create(false, 0);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, (string[])null);
    ROMinMax s0 = new ROMinMax();
    iVariable =
      this.ProcessResultOperator(s0, (ResultOperatorBase)null, (QueryModel)null, 
                                 (IGeneratedQueryCode)generatedCode, 
                                 (ICodeContext)sCodeContext, (CompositionContainer)null);
}
[TestMethod]
[PexGeneratedBy(typeof(ROMinMaxTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException682()
{
    SCodeContext sCodeContext;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sCodeContext = new SCodeContext();
    varInteger = VarIntegerFactory.Create(false, 1);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, (string[])null);
    ROMinMax s0 = new ROMinMax();
    iVariable =
      this.ProcessResultOperator(s0, (ResultOperatorBase)null, (QueryModel)null, 
                                 (IGeneratedQueryCode)generatedCode, 
                                 (ICodeContext)sCodeContext, (CompositionContainer)null);
}
[TestMethod]
[PexGeneratedBy(typeof(ROMinMaxTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException559()
{
    SCodeContext sCodeContext;
    SVarInteger sVarInteger;
    SStatementIncrementInteger sStatementIncrementInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sCodeContext = new SCodeContext();
    sVarInteger = new SVarInteger();
    sStatementIncrementInteger =
      new SStatementIncrementInteger((VarInteger)sVarInteger);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)sStatementIncrementInteger;
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, iStatements, 
                                  (string[])null, (string[])null, (string[])null);
    ROMinMax s0 = new ROMinMax();
    iVariable =
      this.ProcessResultOperator(s0, (ResultOperatorBase)null, (QueryModel)null, 
                                 (IGeneratedQueryCode)generatedCode, 
                                 (ICodeContext)sCodeContext, (CompositionContainer)null);
}
[TestMethod]
[PexGeneratedBy(typeof(ROMinMaxTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException80()
{
    SCodeContext sCodeContext;
    SVarInteger sVarInteger;
    StatementIncrementInteger statementIncrementInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sCodeContext = new SCodeContext();
    sVarInteger = new SVarInteger();
    statementIncrementInteger =
      StatementIncrementIntegerFactory.Create((VarInteger)sVarInteger);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementIncrementInteger;
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, iStatements, 
                                  (string[])null, (string[])null, (string[])null);
    ROMinMax s0 = new ROMinMax();
    iVariable =
      this.ProcessResultOperator(s0, (ResultOperatorBase)null, (QueryModel)null, 
                                 (IGeneratedQueryCode)generatedCode, 
                                 (ICodeContext)sCodeContext, (CompositionContainer)null);
}
[TestMethod]
[PexGeneratedBy(typeof(ROMinMaxTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException721()
{
    SCodeContext sCodeContext;
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sCodeContext = new SCodeContext();
    sVarInteger = new SVarInteger();
    string[] ss = new string[2];
    ss[0] = "\u0100";
    ss[1] = "\u0100";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, ss, (string[])null);
    ROMinMax s0 = new ROMinMax();
    iVariable =
      this.ProcessResultOperator(s0, (ResultOperatorBase)null, (QueryModel)null, 
                                 (IGeneratedQueryCode)generatedCode, 
                                 (ICodeContext)sCodeContext, (CompositionContainer)null);
}
[TestMethod]
[PexGeneratedBy(typeof(ROMinMaxTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException117()
{
    SCodeContext sCodeContext;
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sCodeContext = new SCodeContext();
    sVarInteger = new SVarInteger();
    string[] ss = new string[4];
    ss[0] = "\u0089";
    ss[1] = "\u0089\u0089";
    ss[2] = "\u0089\u008c";
    ss[3] = "\u0001\u0089";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, ss, (string[])null);
    ROMinMax s0 = new ROMinMax();
    iVariable =
      this.ProcessResultOperator(s0, (ResultOperatorBase)null, (QueryModel)null, 
                                 (IGeneratedQueryCode)generatedCode, 
                                 (ICodeContext)sCodeContext, (CompositionContainer)null);
}
[TestMethod]
[PexGeneratedBy(typeof(ROMinMaxTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException68201()
{
    SCodeContext sCodeContext;
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    sCodeContext = new SCodeContext();
    sVarInteger = new SVarInteger();
    string[] ss = new string[4];
    ss[1] = "\0";
    ss[2] = "\u0100\0\0\0\0\0\0";
    ss[3] = "\u0100";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  ss, (string[])null, (string[])null);
    ROMinMax s0 = new ROMinMax();
    iVariable =
      this.ProcessResultOperator(s0, (ResultOperatorBase)null, (QueryModel)null, 
                                 (IGeneratedQueryCode)generatedCode, 
                                 (ICodeContext)sCodeContext, (CompositionContainer)null);
}
    }
}
