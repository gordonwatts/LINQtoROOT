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
using LINQToTTreeLib.Statements;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace LINQToTTreeLib.Tests.Statements
{
    public partial class StatementLoopOverGroupsTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGroupsTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ConstructorThrowsArgumentNullException999()
{
    StatementLoopOverGroups statementLoopOverGroups;
    statementLoopOverGroups = this.Constructor((IValue)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGroupsTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ConstructorThrowsArgumentNullException962()
{
    StatementInlineBlockTest.dummyVarName dummyVarName;
    StatementLoopOverGroups statementLoopOverGroups;
    dummyVarName =
      new StatementInlineBlockTest.dummyVarName((string)null, (Type)null);
    dummyVarName.InitialValue = (IValue)null;
    dummyVarName.Declare = false;
    dummyVarName.RawValue = (string)null;
    statementLoopOverGroups = this.Constructor((IValue)dummyVarName);
}
    }
}
