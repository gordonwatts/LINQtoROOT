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
    public partial class StatementLoopOverSortedPairValueTest
    {
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverSortedPairValueTest))]
public void CodeItUpTest216()
{
    StatementLoopOverSortedPairValue statementLoopOverSortedPairValue;
    string[] ss;
    statementLoopOverSortedPairValue =
      StatementLoopOverSortedPairValueFactory.Create(false);
    ss = this.CodeItUpTest(statementLoopOverSortedPairValue);
    Assert.IsNotNull((object)ss);
    Assert.AreEqual<int>(0, ss.Length);
    Assert.IsNotNull((object)statementLoopOverSortedPairValue);
    Assert.IsNotNull(statementLoopOverSortedPairValue.IndexVariable);
    Assert.AreEqual<string>
        ("aInt32_2", statementLoopOverSortedPairValue.IndexVariable.ParameterName);
    Assert.IsNull(statementLoopOverSortedPairValue.IndexVariable.InitialValue);
    Assert.IsNotNull
        (((StatementInlineBlockBase)statementLoopOverSortedPairValue).Statements);
    Assert.IsNotNull(((StatementInlineBlockBase)statementLoopOverSortedPairValue)
                       .DeclaredVariables);
    Assert.IsNull
        (((StatementInlineBlockBase)statementLoopOverSortedPairValue).Parent);
}
    }
}
