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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace LINQToTTreeLib.Variables
{
    public partial class VarUtilsTest
    {
[TestMethod]
[PexGeneratedBy(typeof(VarUtilsTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void AsCastStringThrowsArgumentNullException82()
{
    string s;
    s = this.AsCastString((IValue)null);
}
[TestMethod]
[PexGeneratedBy(typeof(VarUtilsTest))]
public void AsCastString984()
{
    VarInteger varInteger;
    string s;
    varInteger = new VarInteger();
    s = this.AsCastString((IValue)varInteger);
    Assert.AreEqual<string>("((int)anint_38)", s);
}
    }
}
