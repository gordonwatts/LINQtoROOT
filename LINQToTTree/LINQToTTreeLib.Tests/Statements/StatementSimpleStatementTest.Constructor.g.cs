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
using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Statements
{
    public partial class StatementSimpleStatementTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentException))]
public void ConstructorThrowsArgumentException117()
{
    StatementSimpleStatement statementSimpleStatement;
    statementSimpleStatement = this.Constructor("");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ConstructorThrowsArgumentNullException777()
{
    StatementSimpleStatement statementSimpleStatement;
    statementSimpleStatement = this.Constructor((string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentException))]
public void ConstructorThrowsArgumentException97()
{
    StatementSimpleStatement statementSimpleStatement;
    statementSimpleStatement = this.Constructor(";");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentException))]
public void ConstructorThrowsArgumentException634()
{
    StatementSimpleStatement statementSimpleStatement;
    statementSimpleStatement = this.Constructor(";;");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
public void Constructor215()
{
    StatementSimpleStatement statementSimpleStatement;
    statementSimpleStatement = this.Constructor("\0;;");
    Assert.IsNotNull((object)statementSimpleStatement);
    Assert.AreEqual<string>("\0", statementSimpleStatement.Line);
    Assert.AreEqual<bool>(true, statementSimpleStatement.AddSemicolon);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementSimpleStatementTest))]
[ExpectedException(typeof(ArgumentException))]
public void ConstructorThrowsArgumentException921()
{
    StatementSimpleStatement statementSimpleStatement;
    statementSimpleStatement = this.Constructor(";\u2000");
}
    }
}
