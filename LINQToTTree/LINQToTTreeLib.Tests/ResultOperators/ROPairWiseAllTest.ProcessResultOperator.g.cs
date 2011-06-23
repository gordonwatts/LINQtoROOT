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
using LINQToTTreeLib.Variables;
using LINQToTTreeLib;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.ResultOperators;
using Remotion.Linq.Clauses;
using Remotion.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using LINQToTTreeLib.Statements;

namespace LINQToTTreeLib.Tests
{
    public partial class ROPairWiseAllTest
    {
[TestMethod]
[PexGeneratedBy(typeof(ROPairWiseAllTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException745()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, 0);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, (string[])null);
    ROUniqueCombinations s0 = new ROUniqueCombinations();
    iVariable = this.ProcessResultOperator(s0, (ResultOperatorBase)null, 
                                           (QueryModel)null, (CodeContext)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROPairWiseAllTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException112()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, 1);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, (string[])null);
    ROUniqueCombinations s0 = new ROUniqueCombinations();
    iVariable = this.ProcessResultOperator(s0, (ResultOperatorBase)null, 
                                           (QueryModel)null, (CodeContext)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROPairWiseAllTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException568()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  ss, (string[])null, (string[])null);
    ROUniqueCombinations s0 = new ROUniqueCombinations();
    iVariable = this.ProcessResultOperator(s0, (ResultOperatorBase)null, 
                                           (QueryModel)null, (CodeContext)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROPairWiseAllTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException14()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[1];
    ss[0] = "";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  ss, (string[])null, (string[])null);
    ROUniqueCombinations s0 = new ROUniqueCombinations();
    iVariable = this.ProcessResultOperator(s0, (ResultOperatorBase)null, 
                                           (QueryModel)null, (CodeContext)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROPairWiseAllTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException625()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, ss);
    ROUniqueCombinations s0 = new ROUniqueCombinations();
    iVariable = this.ProcessResultOperator(s0, (ResultOperatorBase)null, 
                                           (QueryModel)null, (CodeContext)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROPairWiseAllTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException967()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[1];
    ss[0] = "\0";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, ss, (string[])null);
    ROUniqueCombinations s0 = new ROUniqueCombinations();
    iVariable = this.ProcessResultOperator(s0, (ResultOperatorBase)null, 
                                           (QueryModel)null, (CodeContext)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROPairWiseAllTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException795()
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
                                                (string[])null, (string[])null, (string[])null);
    ROUniqueCombinations s0 = new ROUniqueCombinations();
    iVariable = this.ProcessResultOperator(s0, (ResultOperatorBase)null, 
                                           (QueryModel)null, (CodeContext)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROPairWiseAllTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException896()
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
                                  ss, (string[])null, (string[])null);
    ROUniqueCombinations s0 = new ROUniqueCombinations();
    iVariable = this.ProcessResultOperator(s0, (ResultOperatorBase)null, 
                                           (QueryModel)null, (CodeContext)null, generatedCode);
}
    }
}