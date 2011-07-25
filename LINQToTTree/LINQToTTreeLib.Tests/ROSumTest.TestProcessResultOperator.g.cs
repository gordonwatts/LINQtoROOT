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
using LINQToTTreeLib.ResultOperators;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace LINQToTTreeLib.Tests
{
    public partial class ROSumTest
    {
[TestMethod]
[PexGeneratedBy(typeof(ROSumTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestProcessResultOperatorThrowsArgumentNullException230()
{
    IVariable iVariable;
    ROSum s0 = new ROSum();
    iVariable = this.TestProcessResultOperator(s0, (SumResultOperator)null, 
                                               (QueryModel)null, (IGeneratedQueryCode)null, (ICodeContext)null);
}
[TestMethod]
[PexGeneratedBy(typeof(ROSumTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestProcessResultOperatorThrowsArgumentNullException977()
{
    IVariable iVariable;
    ROSum s0 = new ROSum();
    SumResultOperator s1 = new SumResultOperator();
    iVariable = this.TestProcessResultOperator
                    (s0, s1, (QueryModel)null, (IGeneratedQueryCode)null, (ICodeContext)null);
}
    }
}
