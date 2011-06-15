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
using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace LINQToTTreeLib.TypeHandlers.CPPCode
{
    public partial class TypeHandlerCPPCodeTest
    {
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerCPPCodeTest))]
[ExpectedException(typeof(InvalidOperationException))]
public void ProcessMethodCallThrowsInvalidOperationException779()
{
    MethodCallExpression methodCallExpression;
    Expression expression;
    methodCallExpression = MethodCallExpressionFactory.Create();
    TypeHandlerCPPCode s0 = new TypeHandlerCPPCode();
    IValue iValue = (IValue)null;
    expression = this.ProcessMethodCall(s0, methodCallExpression, out iValue, 
                                        (IGeneratedQueryCode)null, (ICodeContext)null, (CompositionContainer)null);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerCPPCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void ProcessMethodCallThrowsArgumentNullException933()
{
    Expression expression;
    TypeHandlerCPPCode s0 = new TypeHandlerCPPCode();
    IValue iValue = (IValue)null;
    expression = this.ProcessMethodCall(s0, (MethodCallExpression)null, out iValue, 
                                        (IGeneratedQueryCode)null, (ICodeContext)null, (CompositionContainer)null);
}
    }
}