using LINQToTTreeLib.Statements;
using System;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
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

namespace LINQToTTreeLib.Tests
{
    public partial class StatementAssignTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementAssignTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TryCombineStatementTestThrowsArgumentNullException605()
{
    StatementInlineBlockTest.dummyVarName dummyVarName;
    StatementAssign statementAssign;
    bool b;
    dummyVarName =
      new StatementInlineBlockTest.dummyVarName((string)null, (Type)null);
    dummyVarName.InitialValue = (IValue)null;
    dummyVarName.Declare = false;
    dummyVarName.RawValue = (string)null;
    statementAssign = StatementAssignFactory.Create
                          ((IDeclaredParameter)dummyVarName, (IValue)dummyVarName);
    b = this.TryCombineStatementTest(statementAssign, (IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementAssignTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TryCombineStatementTestThrowsArgumentNullException328()
{
    StatementInlineBlockTest.dummyVarName dummyVarName;
    StatementAssign statementAssign;
    bool b;
    dummyVarName =
      new StatementInlineBlockTest.dummyVarName((string)null, (Type)null);
    dummyVarName.InitialValue = (IValue)null;
    dummyVarName.Declare = false;
    dummyVarName.RawValue = (string)null;
    statementAssign = StatementAssignFactory.Create
                          ((IDeclaredParameter)dummyVarName, (IValue)dummyVarName);
    b = this.TryCombineStatementTest(statementAssign, (IStatement)statementAssign);
}
    }
}
