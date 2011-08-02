using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
using LINQToTTreeLib.Variables;
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

namespace LINQToTTreeLib.Statements
{
    public partial class StatementFilterTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementFilterTest))]
[ExpectedException(typeof(ArgumentException))]
public void TestTryCombineThrowsArgumentException636()
{
    this.TestTryCombine((IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementFilterTest))]
public void TestTryCombine818()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    this.TestTryCombine((IStatement)statementIncrementInteger);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementFilterTest))]
public void TestTryCombine81801()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    varInteger = VarIntegerFactory.Create(false, 1);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    this.TestTryCombine((IStatement)statementIncrementInteger);
}
    }
}
