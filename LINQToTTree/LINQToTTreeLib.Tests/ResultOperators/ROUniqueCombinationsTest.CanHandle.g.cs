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
using LINQToTTreeLib.ResultOperators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using LINQToTTreeLib.relinq;

namespace LINQToTTreeLib.Tests.ResultOperators
{
    public partial class ROUniqueCombinationsTest
    {
[TestMethod]
[PexGeneratedBy(typeof(ROUniqueCombinationsTest))]
public void CanHandle657()
{
    bool b;
    ROUniqueCombinations s0 = new ROUniqueCombinations();
    b = this.CanHandle(s0, (Type)null);
    Assert.AreEqual<bool>(false, b);
    Assert.IsNotNull((object)s0);
}
[TestMethod]
[PexGeneratedBy(typeof(ROUniqueCombinationsTest))]
public void CanHandle508()
{
    bool b;
    ROUniqueCombinations s0 = new ROUniqueCombinations();
    b = this.CanHandle(s0, typeof(UniqueCombinationsResultOperator));
    Assert.AreEqual<bool>(true, b);
    Assert.IsNotNull((object)s0);
}
    }
}
