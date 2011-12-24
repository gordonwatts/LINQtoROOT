using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
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
    public partial class StatementInlineBlockTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
[ExpectedException(typeof(ArgumentException))]
public void TestSimpleVariableDoubleInsertThrowsArgumentException190()
{
    this.TestSimpleVariableDoubleInsert("", "");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
[ExpectedException(typeof(ArgumentException))]
public void TestSimpleVariableDoubleInsertThrowsArgumentException11()
{
    this.TestSimpleVariableDoubleInsert("\0", "\0");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementInlineBlockTest))]
[ExpectedException(typeof(ArgumentException))]
public void TestSimpleVariableDoubleInsertThrowsArgumentException439()
{
    this.TestSimpleVariableDoubleInsert("\u0100", "\u0100");
}
    }
}
