using System;
using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public partial class CombinedGeneratedCodeTest
    {
[TestMethod]
[PexGeneratedBy(typeof(CombinedGeneratedCodeTest))]
public void CheckAddIncludeFiles271()
{
    string[] ss = new string[0];
    this.CheckAddIncludeFiles(ss);
}
[TestMethod]
[PexGeneratedBy(typeof(CombinedGeneratedCodeTest))]
[ExpectedException(typeof(ArgumentException))]
public void CheckAddIncludeFilesThrowsArgumentException550()
{
    string[] ss = new string[1];
    this.CheckAddIncludeFiles(ss);
}
[TestMethod]
[PexGeneratedBy(typeof(CombinedGeneratedCodeTest))]
[ExpectedException(typeof(ArgumentException))]
public void CheckAddIncludeFilesThrowsArgumentException400()
{
    string[] ss = new string[1];
    ss[0] = "";
    this.CheckAddIncludeFiles(ss);
}
[TestMethod]
[PexGeneratedBy(typeof(CombinedGeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void CheckAddIncludeFilesThrowsArgumentNullException988()
{
    string[] ss = new string[1];
    ss[0] = "\0";
    this.CheckAddIncludeFiles(ss);
}
[TestMethod]
[PexGeneratedBy(typeof(CombinedGeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void CheckAddIncludeFilesThrowsArgumentNullException631()
{
    string[] ss = new string[1];
    ss[0] = "\u0100";
    this.CheckAddIncludeFiles(ss);
}
[TestMethod]
[PexGeneratedBy(typeof(CombinedGeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void CheckAddIncludeFilesThrowsArgumentNullException503()
{
    string[] ss = new string[1];
    ss[0] = "\0\0";
    this.CheckAddIncludeFiles(ss);
}
[TestMethod]
[PexGeneratedBy(typeof(CombinedGeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void CheckAddIncludeFilesThrowsArgumentNullException961()
{
    string[] ss = new string[1];
    ss[0] = "\u0100\u0100";
    this.CheckAddIncludeFiles(ss);
}
    }
}
