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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using LINQToTTreeLib.Variables;
using LINQToTTreeLib;

namespace LINQToTTreeLib.TypeHandlers.ROOT
{
    public partial class TypeHanlderROOTTest
    {
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
[PexRaisedException(typeof(NullReferenceException))]
public void ProcessConstantReferenceThrowsNullReferenceException395()
{
    ConstantExpression constantExpression;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IValue iValue;
    constantExpression = ConstantExpressionFactory.Create(0);
    varInteger = new VarInteger();
    generatedCode = GeneratedCodeFactory.Create((IVariable)varInteger);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    iValue = this.ProcessConstantReference
                 (s0, constantExpression, (IGeneratedCode)generatedCode);
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHanlderROOTTest))]
public void ProcessConstantReference980()
{
    ConstantExpression constantExpression;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    IValue iValue;
    constantExpression = ConstantExpressionFactory.Create(1);
    varInteger = new VarInteger();
    generatedCode = GeneratedCodeFactory.Create((IVariable)varInteger);
    TypeHandlerROOT s0 = new TypeHandlerROOT();
    iValue = this.ProcessConstantReference
                 (s0, constantExpression, (IGeneratedCode)generatedCode);
    Assert.IsNotNull((object)iValue);
    Assert.IsNotNull((object)s0);
}
    }
}
