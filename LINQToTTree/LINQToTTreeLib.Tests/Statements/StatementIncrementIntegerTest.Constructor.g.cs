using LINQToTTreeLib.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
// <copyright file="StatementIncrementIntegerTest.Constructor.g.cs" company="Microsoft">Copyright � Microsoft 2010</copyright>
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
    public partial class StatementIncrementIntegerTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementIncrementIntegerTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ConstructorThrowsArgumentNullException80()
{
    StatementIncrementInteger statementIncrementInteger;
    statementIncrementInteger = this.Constructor((VarInteger)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIncrementIntegerTest))]
public void Constructor193()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIncrementInteger = this.Constructor(varInteger);
    Assert.IsNull((object)statementIncrementInteger);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementIncrementIntegerTest))]
public void Constructor19301()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    varInteger = VarIntegerFactory.Create(false, 1);
    statementIncrementInteger = this.Constructor(varInteger);
    Assert.IsNull((object)statementIncrementInteger);
}
    }
}
