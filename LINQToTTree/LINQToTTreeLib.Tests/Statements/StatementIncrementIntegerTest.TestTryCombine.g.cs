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
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace LINQToTTreeLib.Statements
{
    public partial class StatementIncrementIntegerTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementIncrementIntegerTest))]
[PexRaisedException(typeof(NotImplementedException))]
public void TestTryCombineThrowsNotImplementedException336()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    bool b;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    b = this.TestTryCombine(statementIncrementInteger, (IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIncrementIntegerTest))]
[PexRaisedException(typeof(NotImplementedException))]
public void TestTryCombineThrowsNotImplementedException549()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    bool b;
    varInteger = VarIntegerFactory.Create(false, 1);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    b = this.TestTryCombine(statementIncrementInteger, (IStatement)null);
}
    }
}
