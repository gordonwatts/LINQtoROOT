using LinqToTTreeInterfacesLib;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
// <copyright file="TypeHandlerHelpersTest.ProcessConstantReference.g.cs" company="Microsoft">Copyright � Microsoft 2010</copyright>
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

namespace LINQToTTreeLib.TypeHandlers
{
    public partial class TypeHandlerHelpersTest
    {
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerHelpersTest))]
[ExpectedException(typeof(NotImplementedException))]
public void ProcessConstantReferenceThrowsNotImplementedException24()
{
    IValue iValue;
    TypeHandlerHelpers s0 = new TypeHandlerHelpers();
    iValue = this.ProcessConstantReference
                 (s0, (ConstantExpression)null, (IGeneratedQueryCode)null);
}
    }
}
