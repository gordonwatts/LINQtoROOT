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
using ROOTNET;

namespace LINQToTTreeLib.Tests
{
    public partial class ROPairWiseAllTest
    {
[TestMethod]
[PexGeneratedBy(typeof(ROPairWiseAllTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException29()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, 0);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (NTObject[])null, (string[])null);
    ROUniqueCombinations s0 = new ROUniqueCombinations();
    iVariable = this.ProcessResultOperator(s0, (ResultOperatorBase)null, 
                                           (QueryModel)null, (CodeContext)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROPairWiseAllTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException389()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, 1);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (NTObject[])null, (string[])null);
    ROUniqueCombinations s0 = new ROUniqueCombinations();
    iVariable = this.ProcessResultOperator(s0, (ResultOperatorBase)null, 
                                           (QueryModel)null, (CodeContext)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROPairWiseAllTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException569()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  ss, (NTObject[])null, (string[])null);
    ROUniqueCombinations s0 = new ROUniqueCombinations();
    iVariable = this.ProcessResultOperator(s0, (ResultOperatorBase)null, 
                                           (QueryModel)null, (CodeContext)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROPairWiseAllTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException516()
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
    ROUniqueCombinations s0 = new ROUniqueCombinations();
    iVariable = this.ProcessResultOperator(s0, (ResultOperatorBase)null, 
                                           (QueryModel)null, (CodeContext)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROPairWiseAllTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException309()
{
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IVariable iVariable;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (NTObject[])null, ss);
    ROUniqueCombinations s0 = new ROUniqueCombinations();
    iVariable = this.ProcessResultOperator(s0, (ResultOperatorBase)null, 
                                           (QueryModel)null, (CodeContext)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROPairWiseAllTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException627()
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
      NTObject[] nTObjects = new NTObject[1];
      nTObjects[0] = nTObject;
      generatedCode =
        GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                    (string[])null, nTObjects, (string[])null);
      ROUniqueCombinations s0 = new ROUniqueCombinations();
      iVariable = this.ProcessResultOperator(s0, (ResultOperatorBase)null, 
                                             (QueryModel)null, (CodeContext)null, generatedCode);
      disposables.Dispose();
    }
}
[TestMethod]
[PexGeneratedBy(typeof(ROPairWiseAllTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException970()
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
    ROUniqueCombinations s0 = new ROUniqueCombinations();
    iVariable = this.ProcessResultOperator(s0, (ResultOperatorBase)null, 
                                           (QueryModel)null, (CodeContext)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROPairWiseAllTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException468()
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
    ROUniqueCombinations s0 = new ROUniqueCombinations();
    iVariable = this.ProcessResultOperator(s0, (ResultOperatorBase)null, 
                                           (QueryModel)null, (CodeContext)null, generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(ROPairWiseAllTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessResultOperatorThrowsArgumentNullException876()
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
      ROUniqueCombinations s0 = new ROUniqueCombinations();
      iVariable = this.ProcessResultOperator(s0, (ResultOperatorBase)null, 
                                             (QueryModel)null, (CodeContext)null, generatedCode);
      disposables.Dispose();
    }
}
    }
}
