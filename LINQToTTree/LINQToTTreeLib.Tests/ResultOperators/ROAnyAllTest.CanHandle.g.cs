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
using Remotion.Linq.Clauses.ResultOperators;

namespace LINQToTTreeLib.ResultOperators
{
    public partial class ROAnyAllTest
    {
[TestMethod]
[PexGeneratedBy(typeof(ROAnyAllTest))]
public void CanHandle1()
{
    bool b;
    ROAnyAll s0 = new ROAnyAll();
    b = this.CanHandle(s0, (Type)null);
    Assert.AreEqual<bool>(false, b);
    Assert.IsNotNull((object)s0);
}
[TestMethod]
[PexGeneratedBy(typeof(ROAnyAllTest))]
public void CanHandle96()
{
    bool b;
    ROAnyAll s0 = new ROAnyAll();
    b = this.CanHandle(s0, typeof(AllResultOperator));
    Assert.AreEqual<bool>(true, b);
    Assert.IsNotNull((object)s0);
}
    }
}