using LinqToTTreeInterfacesLib;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
// <copyright file="TypeHandlerCacheTest.ProcessConstantReference.g.cs" company="Microsoft">Copyright � Microsoft 2010</copyright>
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
    public partial class TypeHandlerCacheTest
    {
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerCacheTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessConstantReferenceThrowsArgumentNullException171()
{
    TypeHandlerCache typeHandlerCache;
    IValue iValue;
    typeHandlerCache = new TypeHandlerCache();
    iValue = this.ProcessConstantReference
                 (typeHandlerCache, (ConstantExpression)null, (IGeneratedQueryCode)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerCacheTest))]
[ExpectedException(typeof(InvalidOperationException))]
public void ProcessConstantReferenceThrowsInvalidOperationException837()
{
    TypeHandlerCache typeHandlerCache;
    ConstantExpression constantExpression;
    IValue iValue;
    typeHandlerCache = new TypeHandlerCache();
    constantExpression = ConstantExpressionFactory.Create(0);
    iValue = this.ProcessConstantReference
                 (typeHandlerCache, constantExpression, (IGeneratedQueryCode)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerCacheTest))]
[ExpectedException(typeof(InvalidOperationException))]
public void ProcessConstantReferenceThrowsInvalidOperationException500()
{
    TypeHandlerCache typeHandlerCache;
    ConstantExpression constantExpression;
    IValue iValue;
    typeHandlerCache = new TypeHandlerCache();
    constantExpression = ConstantExpressionFactory.Create(1);
    iValue = this.ProcessConstantReference
                 (typeHandlerCache, constantExpression, (IGeneratedQueryCode)null);
}
    }
}
