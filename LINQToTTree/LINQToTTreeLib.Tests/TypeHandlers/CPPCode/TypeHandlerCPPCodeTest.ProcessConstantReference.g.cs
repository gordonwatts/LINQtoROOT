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
using System.Linq.Expressions;
using LINQToTTreeLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace LINQToTTreeLib.TypeHandlers.CPPCode
{
    public partial class TypeHandlerCPPCodeTest
    {
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerCPPCodeTest))]
[ExpectedException(typeof(NotImplementedException))]
public void ProcessConstantReferenceThrowsNotImplementedException199()
{
    IValue iValue;
    TypeHandlerCPPCode s0 = new TypeHandlerCPPCode();
    iValue = this.ProcessConstantReference
                 (s0, (ConstantExpression)null, (GeneratedCode)null, (CodeContext)null);
}
    }
}
