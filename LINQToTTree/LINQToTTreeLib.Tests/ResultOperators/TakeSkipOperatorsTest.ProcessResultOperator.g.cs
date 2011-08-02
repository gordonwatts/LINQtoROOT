using LINQToTTreeLib.Variables;
using LINQToTTreeLib;
using LinqToTTreeInterfacesLib;
using ROOTNET;
using Remotion.Linq.Clauses;
using Remotion.Linq;
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

namespace LINQToTTreeLib.ResultOperators
{
    public partial class TakeSkipOperatorsTest
    {
[TestMethod]
[PexGeneratedBy(typeof(TakeSkipOperatorsTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException653()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, 0);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (NTObject[])null, (string[])null);
    ROTakeSkipOperators s0 = new ROTakeSkipOperators();
    iVariable = this.ProcessResultOperator
                    (s0, (ResultOperatorBase)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TakeSkipOperatorsTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException192()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, 1);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (NTObject[])null, (string[])null);
    ROTakeSkipOperators s0 = new ROTakeSkipOperators();
    iVariable = this.ProcessResultOperator
                    (s0, (ResultOperatorBase)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TakeSkipOperatorsTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException794()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  ss, (NTObject[])null, (string[])null);
    ROTakeSkipOperators s0 = new ROTakeSkipOperators();
    iVariable = this.ProcessResultOperator
                    (s0, (ResultOperatorBase)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TakeSkipOperatorsTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException87()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[1];
    ss[0] = "";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  ss, (NTObject[])null, (string[])null);
    ROTakeSkipOperators s0 = new ROTakeSkipOperators();
    iVariable = this.ProcessResultOperator
                    (s0, (ResultOperatorBase)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TakeSkipOperatorsTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException847()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (NTObject[])null, ss);
    ROTakeSkipOperators s0 = new ROTakeSkipOperators();
    iVariable = this.ProcessResultOperator
                    (s0, (ResultOperatorBase)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TakeSkipOperatorsTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException566()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementIncrementInteger;
    generatedCode = GeneratedCodeFactory.Create((IVariable)varInteger, iStatements, 
                                                (string[])null, (NTObject[])null, (string[])null);
    ROTakeSkipOperators s0 = new ROTakeSkipOperators();
    iVariable = this.ProcessResultOperator
                    (s0, (ResultOperatorBase)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TakeSkipOperatorsTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException959()
{
    using (PexDisposableContext disposables = PexDisposableContext.Create())
    {
      VarInteger varInteger;
      NTObject nTObject;
      GeneratedCode generatedCode;
      IVariable iVariable;
      varInteger = VarIntegerFactory.Create(false, 0);
      nTObject = NTObjectFactory.Create();
      disposables.Add((IDisposable)nTObject);
      NTObject[] nTObjects = new NTObject[1];
      nTObjects[0] = nTObject;
      generatedCode =
        GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                    (string[])null, nTObjects, (string[])null);
      ROTakeSkipOperators s0 = new ROTakeSkipOperators();
      iVariable = this.ProcessResultOperator
                      (s0, (ResultOperatorBase)null, (QueryModel)null, generatedCode);
      disposables.Dispose();
    }
}
[TestMethod]
[PexGeneratedBy(typeof(TakeSkipOperatorsTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException677()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[2];
    ss[0] = "";
    ss[1] = "";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  ss, (NTObject[])null, (string[])null);
    ROTakeSkipOperators s0 = new ROTakeSkipOperators();
    iVariable = this.ProcessResultOperator
                    (s0, (ResultOperatorBase)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TakeSkipOperatorsTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException660()
{
    using (PexDisposableContext disposables = PexDisposableContext.Create())
    {
      VarInteger varInteger;
      NTObject nTObject;
      GeneratedCode generatedCode;
      IVariable iVariable;
      varInteger = VarIntegerFactory.Create(false, int.MaxValue);
      nTObject = NTObjectFactory.Create();
      disposables.Add((IDisposable)nTObject);
      string[] ss = new string[3];
      NTObject[] nTObjects = new NTObject[2];
      ss[0] = "";
      ss[1] = "";
      nTObjects[0] = nTObject;
      nTObjects[1] = nTObject;
      generatedCode =
        GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                    ss, nTObjects, (string[])null);
      ROTakeSkipOperators s0 = new ROTakeSkipOperators();
      iVariable = this.ProcessResultOperator
                      (s0, (ResultOperatorBase)null, (QueryModel)null, generatedCode);
      disposables.Dispose();
    }
}
[TestMethod]
[PexGeneratedBy(typeof(TakeSkipOperatorsTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException651()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    StatementLoopOverGood statementLoopOverGood;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IVariable)varInteger, (IStatement[])null, (IVariable[])null);
    IStatement[] iStatements = new IStatement[2];
    iStatements[0] = (IStatement)statementIncrementInteger;
    iStatements[1] = (IStatement)statementLoopOverGood;
    generatedCode = GeneratedCodeFactory.Create((IVariable)varInteger, iStatements, 
                                                (string[])null, (NTObject[])null, (string[])null);
    ROTakeSkipOperators s0 = new ROTakeSkipOperators();
    iVariable = this.ProcessResultOperator
                    (s0, (ResultOperatorBase)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TakeSkipOperatorsTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException413()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    StatementLoopOverGood statementLoopOverGood;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    IVariable[] iVariables = new IVariable[1];
    iVariables[0] = (IVariable)varInteger;
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IVariable)varInteger, (IStatement[])null, iVariables);
    IStatement[] iStatements = new IStatement[2];
    iStatements[0] = (IStatement)statementIncrementInteger;
    iStatements[1] = (IStatement)statementLoopOverGood;
    generatedCode = GeneratedCodeFactory.Create((IVariable)varInteger, iStatements, 
                                                (string[])null, (NTObject[])null, (string[])null);
    ROTakeSkipOperators s0 = new ROTakeSkipOperators();
    iVariable = this.ProcessResultOperator
                    (s0, (ResultOperatorBase)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TakeSkipOperatorsTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException285()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    StatementIfOnCount statementIfOnCount;
    StatementLoopOverGood statementLoopOverGood;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       (IStatement[])null, (IVariable[])null);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementIfOnCount;
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IVariable)varInteger, iStatements, (IVariable[])null);
    IStatement[] iStatements1 = new IStatement[2];
    iStatements1[0] = (IStatement)statementIncrementInteger;
    iStatements1[1] = (IStatement)statementLoopOverGood;
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, iStatements1, 
                                  (string[])null, (NTObject[])null, (string[])null);
    ROTakeSkipOperators s0 = new ROTakeSkipOperators();
    iVariable = this.ProcessResultOperator
                    (s0, (ResultOperatorBase)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TakeSkipOperatorsTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException666()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    StatementFilter statementFilter;
    StatementIfOnCount statementIfOnCount;
    StatementLoopOverGood statementLoopOverGood;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    IStatement[] iStatements = new IStatement[0];
    IVariable[] iVariables = new IVariable[0];
    statementFilter =
      StatementFilterFactory.Create((IValue)varInteger, iStatements, iVariables);
    IStatement[] iStatements1 = new IStatement[1];
    iStatements1[0] = (IStatement)statementFilter;
    statementIfOnCount =
      StatementIfOnCountFactory.Create((IVariable)varInteger, (IValue)varInteger, 
                                       StatementIfOnCount.ComparisonOperator.GreaterThan, 
                                       iStatements1, (IVariable[])null);
    IStatement[] iStatements2 = new IStatement[1];
    iStatements2[0] = (IStatement)statementIfOnCount;
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IVariable)varInteger, iStatements2, (IVariable[])null);
    IStatement[] iStatements3 = new IStatement[2];
    iStatements3[0] = (IStatement)statementIncrementInteger;
    iStatements3[1] = (IStatement)statementLoopOverGood;
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, iStatements3, 
                                  (string[])null, (NTObject[])null, (string[])null);
    ROTakeSkipOperators s0 = new ROTakeSkipOperators();
    iVariable = this.ProcessResultOperator
                    (s0, (ResultOperatorBase)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TakeSkipOperatorsTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException267()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    StatementLoopOverGood statementLoopOverGood;
    GeneratedCodeTest.CompoundStatement compoundStatement;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IVariable)varInteger, (IStatement[])null, (IVariable[])null);
    compoundStatement = new GeneratedCodeTest.CompoundStatement();
    compoundStatement.Parent = (IStatement)null;
    IStatement[] iStatements = new IStatement[3];
    iStatements[0] = (IStatement)statementIncrementInteger;
    iStatements[1] = (IStatement)statementLoopOverGood;
    iStatements[2] = (IStatement)compoundStatement;
    generatedCode = GeneratedCodeFactory.Create((IVariable)varInteger, iStatements, 
                                                (string[])null, (NTObject[])null, (string[])null);
    ROTakeSkipOperators s0 = new ROTakeSkipOperators();
    iVariable = this.ProcessResultOperator
                    (s0, (ResultOperatorBase)null, (QueryModel)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TakeSkipOperatorsTest))]
[ExpectedException(typeof(ArgumentException))]
public void ProcessResultOperatorThrowsArgumentException444()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    StatementLoopOverGood statementLoopOverGood;
    StatementInlineBlock statementInlineBlock;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IVariable)varInteger, (IStatement[])null, (IVariable[])null);
    statementInlineBlock =
      StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
    IStatement[] iStatements = new IStatement[3];
    iStatements[0] = (IStatement)statementIncrementInteger;
    iStatements[1] = (IStatement)statementLoopOverGood;
    iStatements[2] = (IStatement)statementInlineBlock;
    generatedCode = GeneratedCodeFactory.Create((IVariable)varInteger, iStatements, 
                                                (string[])null, (NTObject[])null, (string[])null);
    ROTakeSkipOperators s0 = new ROTakeSkipOperators();
    iVariable = this.ProcessResultOperator
                    (s0, (ResultOperatorBase)null, (QueryModel)null, generatedCode);
}
    }
}
