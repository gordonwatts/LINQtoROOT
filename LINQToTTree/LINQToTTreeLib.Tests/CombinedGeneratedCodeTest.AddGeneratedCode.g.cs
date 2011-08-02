using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
using LINQToTTreeLib.Variables;
using LINQToTTreeLib.Statements;
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
public void AddGeneratedCodeThrowsArgumentException74()
{
    CombinedGeneratedCode combinedGeneratedCode;
    string[] ss = new string[0];
    combinedGeneratedCode =
      CombinedGeneratedCodeFactory.Create((string[])null, (string[])null, 
                                          (string[])null, (IBookingStatementBlock[])null, ss);
    this.AddGeneratedCode
        (combinedGeneratedCode, (IExecutableCode)combinedGeneratedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(CombinedGeneratedCodeTest))]
[ExpectedException(typeof(ArgumentException))]
public void AddGeneratedCodeThrowsArgumentException964()
{
    CombinedGeneratedCode combinedGeneratedCode;
    string[] ss = new string[1];
    combinedGeneratedCode =
      CombinedGeneratedCodeFactory.Create((string[])null, (string[])null, 
                                          (string[])null, (IBookingStatementBlock[])null, ss);
    this.AddGeneratedCode
        (combinedGeneratedCode, (IExecutableCode)combinedGeneratedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(CombinedGeneratedCodeTest))]
[ExpectedException(typeof(ArgumentException))]
public void AddGeneratedCodeThrowsArgumentException861()
{
    CombinedGeneratedCode combinedGeneratedCode;
    string[] ss = new string[1];
    ss[0] = "";
    combinedGeneratedCode =
      CombinedGeneratedCodeFactory.Create((string[])null, (string[])null, 
                                          (string[])null, (IBookingStatementBlock[])null, ss);
    this.AddGeneratedCode
        (combinedGeneratedCode, (IExecutableCode)combinedGeneratedCode);
}
    }
}
