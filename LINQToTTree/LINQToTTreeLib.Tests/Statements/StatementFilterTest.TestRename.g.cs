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
    public partial class StatementFilterTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementFilterTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestRenameThrowsArgumentNullException975()
{
    VarInteger varInteger;
    StatementFilter statementFilter;
    IStatement iStatement;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    IStatement[] iStatements = new IStatement[0];
    IVariable[] iVariables = new IVariable[0];
    statementFilter =
      StatementFilterFactory.Create((IValue)varInteger, iStatements, iVariables);
    iStatement = this.TestRename(statementFilter, "", "");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementFilterTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestRenameThrowsArgumentNullException675()
{
    VarInteger varInteger;
    StatementFilter statementFilter;
    IStatement iStatement;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    IStatement[] iStatements = new IStatement[0];
    IVariable[] iVariables = new IVariable[0];
    statementFilter =
      StatementFilterFactory.Create((IValue)varInteger, iStatements, iVariables);
    iStatement = this.TestRename(statementFilter, "\t", "\t");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementFilterTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestRenameThrowsArgumentNullException262()
{
    VarInteger varInteger;
    StatementFilter statementFilter;
    IStatement iStatement;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    IStatement[] iStatements = new IStatement[0];
    IVariable[] iVariables = new IVariable[0];
    statementFilter =
      StatementFilterFactory.Create((IValue)varInteger, iStatements, iVariables);
    iStatement = this.TestRename(statementFilter, "\u0089", "\t");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementFilterTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestRenameThrowsArgumentNullException447()
{
    VarInteger varInteger;
    StatementFilter statementFilter;
    IStatement iStatement;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    IStatement[] iStatements = new IStatement[0];
    IVariable[] iVariables = new IVariable[1];
    iVariables[0] = (IVariable)varInteger;
    statementFilter =
      StatementFilterFactory.Create((IValue)varInteger, iStatements, iVariables);
    iStatement = this.TestRename(statementFilter, "", "");
}
[TestMethod]
[PexGeneratedBy(typeof(StatementFilterTest))]
public void TestRename380()
{
    VarInteger varInteger;
    StatementFilter statementFilter;
    IStatement iStatement;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    IStatement[] iStatements = new IStatement[0];
    IVariable[] iVariables = new IVariable[0];
    statementFilter =
      StatementFilterFactory.Create((IValue)varInteger, iStatements, iVariables);
    iStatement = this.TestRename(statementFilter, "\07", "\07");
    Assert.IsNotNull((object)iStatement);
    Assert.IsNotNull((object)statementFilter);
    Assert.IsTrue
        (object.ReferenceEquals((object)statementFilter, (object)iStatement));
}
    }
}
