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

namespace LINQToTTreeLib.Tests
{
    public partial class TestStatementRecordValue
    {
[TestMethod]
[PexGeneratedBy(typeof(TestStatementRecordValue))]
public void TestRenameVariable283()
{
    StatementInlineBlockTest.dummyVarName dummyVarName;
    StatementRecordValue statementRecordValue;
    StatementRecordValue statementRecordValue1;
    dummyVarName =
      new StatementInlineBlockTest.dummyVarName((string)null, (Type)null);
    dummyVarName.InitialValue = (IValue)null;
    dummyVarName.Declare = false;
    dummyVarName.RawValue = (string)null;
    statementRecordValue = StatementRecordValueFactory.Create
                               ((IDeclaredParameter)dummyVarName, (IValue)dummyVarName, 
                                (IDeclaredParameter)dummyVarName, false);
    statementRecordValue1 =
      this.TestRenameVariable(statementRecordValue, (string)null, (string)null);
    Assert.IsNotNull((object)statementRecordValue1);
    Assert.IsNull(statementRecordValue1.Parent);
    Assert.IsNotNull((object)statementRecordValue);
    Assert.IsTrue(object.ReferenceEquals
                      ((object)statementRecordValue, (object)statementRecordValue1));
}
    }
}