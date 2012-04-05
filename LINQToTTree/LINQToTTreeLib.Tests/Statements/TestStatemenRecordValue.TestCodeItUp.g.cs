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
    public partial class TestStatemenRecordValue
    {
[TestMethod]
[PexGeneratedBy(typeof(TestStatemenRecordValue))]
public void TestCodeItUp518()
{
    StatementInlineBlockTest.dummyVarName dummyVarName;
    StatementRecordValue statementRecordValue;
    string[] ss;
    dummyVarName =
      new StatementInlineBlockTest.dummyVarName((string)null, (Type)null);
    dummyVarName.InitialValue = (IValue)null;
    dummyVarName.Declare = false;
    dummyVarName.RawValue = (string)null;
    statementRecordValue = StatementRecordValueFactory.Create
                               ((IDeclaredParameter)dummyVarName, (IValue)dummyVarName, 
                                (IDeclaredParameter)dummyVarName, false);
    ss = this.TestCodeItUp(statementRecordValue);
    Assert.IsNotNull((object)ss);
    Assert.AreEqual<int>(2, ss.Length);
    Assert.AreEqual<string>(" = ;", ss[0]);
    Assert.AreEqual<string>(" = true;", ss[1]);
    Assert.IsNotNull((object)statementRecordValue);
    Assert.IsNull(statementRecordValue.Parent);
}
[TestMethod]
[PexGeneratedBy(typeof(TestStatemenRecordValue))]
public void TestCodeItUp181()
{
    StatementInlineBlockTest.dummyVarName dummyVarName;
    StatementRecordValue statementRecordValue;
    string[] ss;
    dummyVarName =
      new StatementInlineBlockTest.dummyVarName((string)null, (Type)null);
    dummyVarName.InitialValue = (IValue)null;
    dummyVarName.Declare = false;
    dummyVarName.RawValue = (string)null;
    statementRecordValue = StatementRecordValueFactory.Create
                               ((IDeclaredParameter)dummyVarName, (IValue)dummyVarName, 
                                (IDeclaredParameter)dummyVarName, true);
    ss = this.TestCodeItUp(statementRecordValue);
    Assert.IsNotNull((object)ss);
    Assert.AreEqual<int>(3, ss.Length);
    Assert.AreEqual<string>(" = ;", ss[0]);
    Assert.AreEqual<string>(" = true;", ss[1]);
    Assert.AreEqual<string>("break;", ss[2]);
    Assert.IsNotNull((object)statementRecordValue);
    Assert.IsNull(statementRecordValue.Parent);
}
    }
}
