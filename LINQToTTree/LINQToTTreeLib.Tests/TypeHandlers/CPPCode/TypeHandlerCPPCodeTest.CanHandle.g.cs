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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace LINQToTTreeLib.TypeHandlers.CPPCode
{
    public partial class TypeHandlerCPPCodeTest
    {
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerCPPCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void CanHandleThrowsArgumentNullException991()
{
    TypeHandlerCPPCode typeHandlerCPPCode;
    bool b;
    typeHandlerCPPCode = new TypeHandlerCPPCode();
    b = this.CanHandle(typeHandlerCPPCode, (Type)null);
}
    }
}
