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
public void CastToType29()
{
    string s;
    s = this.CastToType(0, 0);
    Assert.AreEqual<string>("10", s);
}
[TestMethod]
[PexGeneratedBy(typeof(VarUtilsTest))]
[PexRaisedException(typeof(AssertFailedException))]
public void CastToTypeThrowsAssertFailedException468()
{
    string s;
    s = this.CastToType(1, 0);
}
[TestMethod]
[PexGeneratedBy(typeof(VarUtilsTest))]
public void CastToType480()
{
    string s;
    s = this.CastToType(2, 0);
    Assert.AreEqual<string>("", s);
}
[TestMethod]
[PexGeneratedBy(typeof(VarUtilsTest))]
public void CastToType281()
{
    string s;
    s = this.CastToType(0, 2);
    Assert.AreEqual<string>("", s);
}
[TestMethod]
[PexGeneratedBy(typeof(VarUtilsTest))]
public void CastToType302()
{
    string s;
    s = this.CastToType(0, 1);
    Assert.AreEqual<string>("10", s);
}
    }
}
