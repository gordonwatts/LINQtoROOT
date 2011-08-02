// <copyright file="TypeHandlerHelpersTest.ProcessMethodCall.g.cs" company="Microsoft">Copyright � Microsoft 2010</copyright>
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
    public partial class TypeHandlerHelpersTest
    {
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerHelpersTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessMethodCallThrowsArgumentNullException566()
{
    Expression expression;
    TypeHandlerHelpers s0 = new TypeHandlerHelpers();
    IValue iValue = (IValue)null;
    expression = this.ProcessMethodCall(s0, (MethodCallExpression)null, 
                                        out iValue, (IGeneratedQueryCode)null, (ICodeContext)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerHelpersTest))]
[ExpectedException(typeof(NotImplementedException))]
public void ProcessMethodCallThrowsNotImplementedException447()
{
    MethodCallExpression methodCallExpression;
    Expression expression;
    methodCallExpression = MethodCallExpressionFactory.Create();
    TypeHandlerHelpers s0 = new TypeHandlerHelpers();
    IValue iValue = (IValue)null;
    expression = this.ProcessMethodCall(s0, methodCallExpression, 
                                        out iValue, (IGeneratedQueryCode)null, (ICodeContext)null);
}
    }
}
