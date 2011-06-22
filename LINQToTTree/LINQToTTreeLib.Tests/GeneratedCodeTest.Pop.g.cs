using LINQToTTreeLib.Variables;
using LINQToTTreeLib.Statements;
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

namespace LINQToTTreeLib
{
    public partial class GeneratedCodeTest
    {
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void Pop622()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    this.Pop((IStatement)statementIncrementInteger);
}
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void Pop62201()
{
    VarInteger varInteger;
    StatementIncrementInteger statementIncrementInteger;
    varInteger = VarIntegerFactory.Create(false, 1);
    statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
    this.Pop((IStatement)statementIncrementInteger);
}
    }
}
