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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using LINQToTTreeLib.Statements;

namespace LINQToTTreeLib.Tests.Statements
{
    public partial class StatementForLoopTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementForLoopTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestCombineDifInitialThrowsArgumentNullException89()
{
    this.TestCombineDifInitial
        ((IDeclaredParameter)null, (IValue)null, (IValue)null, (IValue)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementForLoopTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestCombineDifInitialThrowsArgumentNullException110()
{
    StatementInlineBlockTest.dummyVarName dummyVarName;
    dummyVarName =
      new StatementInlineBlockTest.dummyVarName((string)null, (Type)null);
    dummyVarName.InitialValue = (IValue)null;
    dummyVarName.Declare = false;
    dummyVarName.RawValue = (string)null;
    this.TestCombineDifInitial
        ((IDeclaredParameter)dummyVarName, (IValue)null, (IValue)null, (IValue)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementForLoopTest))]
[ExpectedException(typeof(ArgumentException))]
public void TestCombineDifInitialThrowsArgumentException42()
{
    StatementInlineBlockTest.dummyVarName dummyVarName;
    dummyVarName =
      new StatementInlineBlockTest.dummyVarName((string)null, (Type)null);
    dummyVarName.InitialValue = (IValue)null;
    dummyVarName.Declare = false;
    dummyVarName.RawValue = (string)null;
    this.TestCombineDifInitial
        ((IDeclaredParameter)dummyVarName, (IValue)dummyVarName, 
         (IValue)null, (IValue)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementForLoopTest))]
[ExpectedException(typeof(ArgumentException))]
public void TestCombineDifInitialThrowsArgumentException834()
{
    StatementInlineBlockTest.dummyVarName dummyVarName;
    dummyVarName =
      new StatementInlineBlockTest.dummyVarName((string)null, (Type)null);
    dummyVarName.InitialValue = (IValue)null;
    dummyVarName.Declare = false;
    dummyVarName.RawValue = (string)null;
    this.TestCombineDifInitial
        ((IDeclaredParameter)dummyVarName, (IValue)dummyVarName, 
         (IValue)dummyVarName, (IValue)null);
}
    }
}
