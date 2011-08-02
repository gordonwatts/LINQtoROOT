using System;
using LinqToTTreeInterfacesLib;
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
[ExpectedException(typeof(ArgumentException))]
public void AddIncludeFileThrowsArgumentException112()
{
    CombinedGeneratedCode combinedGeneratedCode;
    string[] ss = new string[0];
    combinedGeneratedCode =
      CombinedGeneratedCodeFactory.Create((string[])null, (string[])null, 
                                          (string[])null, (IBookingStatementBlock[])null, ss);
    this.AddIncludeFile(combinedGeneratedCode, (string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(CombinedGeneratedCodeTest))]
[ExpectedException(typeof(ArgumentException))]
public void AddIncludeFileThrowsArgumentException509()
{
    CombinedGeneratedCode combinedGeneratedCode;
    string[] ss = new string[1];
    combinedGeneratedCode =
      CombinedGeneratedCodeFactory.Create((string[])null, (string[])null, 
                                          (string[])null, (IBookingStatementBlock[])null, ss);
    this.AddIncludeFile(combinedGeneratedCode, (string)null);
}
[TestMethod]
[PexGeneratedBy(typeof(CombinedGeneratedCodeTest))]
[ExpectedException(typeof(ArgumentException))]
public void AddIncludeFileThrowsArgumentException422()
{
    CombinedGeneratedCode combinedGeneratedCode;
    string[] ss = new string[0];
    combinedGeneratedCode =
      CombinedGeneratedCodeFactory.Create((string[])null, (string[])null, 
                                          (string[])null, (IBookingStatementBlock[])null, ss);
    this.AddIncludeFile(combinedGeneratedCode, "");
}
[TestMethod]
[PexGeneratedBy(typeof(CombinedGeneratedCodeTest))]
public void AddIncludeFile554()
{
    CombinedGeneratedCode combinedGeneratedCode;
    string[] ss = new string[0];
    combinedGeneratedCode =
      CombinedGeneratedCodeFactory.Create((string[])null, (string[])null, 
                                          (string[])null, (IBookingStatementBlock[])null, ss);
    this.AddIncludeFile(combinedGeneratedCode, "\0");
    Assert.IsNotNull((object)combinedGeneratedCode);
    Assert.IsNotNull(combinedGeneratedCode.VariablesToTransfer);
    Assert.IsNotNull(combinedGeneratedCode.ResultValues);
    Assert.IsNotNull(combinedGeneratedCode.IncludeFiles);
    Assert.IsNotNull(combinedGeneratedCode.ReferencedLeafNames);
}
    }
}
