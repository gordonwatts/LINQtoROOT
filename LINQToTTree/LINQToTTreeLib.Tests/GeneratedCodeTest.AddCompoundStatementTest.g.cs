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
using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib
{
    public partial class GeneratedCodeTest
    {
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void AddCompoundStatementTestThrowsArgumentNullException582()
{
    this.AddCompoundStatementTest((IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void AddCompoundStatementTest805()
{
    GeneratedCodeTest.SimpleStatement s0 = new GeneratedCodeTest.SimpleStatement();
    this.AddCompoundStatementTest((IStatement)s0);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void AddCompoundStatementTest818()
{
    GeneratedCodeTest.CompoundStatement compoundStatement;
    compoundStatement = new GeneratedCodeTest.CompoundStatement();
    this.AddCompoundStatementTest((IStatement)compoundStatement);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void AddCompoundStatementTest81801()
{
    GeneratedCodeTest.CompoundBookingStatement compoundBookingStatement;
    compoundBookingStatement = new GeneratedCodeTest.CompoundBookingStatement();
    this.AddCompoundStatementTest((IStatement)compoundBookingStatement);
}
    }
}
