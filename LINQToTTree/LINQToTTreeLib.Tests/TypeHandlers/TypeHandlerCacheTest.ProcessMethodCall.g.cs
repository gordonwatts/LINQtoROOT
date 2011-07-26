// <copyright file="TypeHandlerCacheTest.ProcessMethodCall.g.cs" company="Microsoft">Copyright � Microsoft 2010</copyright>
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
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace LINQToTTreeLib.TypeHandlers
{
    public partial class TypeHandlerCacheTest
    {
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerCacheTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessMethodCallThrowsArgumentNullException649()
{
    TypeHandlerCache typeHandlerCache;
    Expression expression;
    typeHandlerCache = new TypeHandlerCache();
    IValue iValue = (IValue)null;
    expression =
      this.ProcessMethodCall(typeHandlerCache, (MethodCallExpression)null, 
                             out iValue, (IGeneratedQueryCode)null, (ICodeContext)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerCacheTest))]
[ExpectedException(typeof(InvalidOperationException))]
public void ProcessMethodCallThrowsInvalidOperationException733()
{
    TypeHandlerCache typeHandlerCache;
    MethodCallExpression methodCallExpression;
    Expression expression;
    typeHandlerCache = new TypeHandlerCache();
    methodCallExpression = MethodCallExpressionFactory.Create();
    IValue iValue = (IValue)null;
    expression = this.ProcessMethodCall(typeHandlerCache, methodCallExpression, 
                                        out iValue, (IGeneratedQueryCode)null, (ICodeContext)null);
}
    }
}
