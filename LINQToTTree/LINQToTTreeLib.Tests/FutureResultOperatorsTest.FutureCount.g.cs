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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace LINQToTTreeLib
{
    public partial class FutureResultOperatorsTest
    {
[TestMethod]
[PexGeneratedBy(typeof(FutureResultOperatorsTest))]
[ExpectedException(typeof(ArgumentException))]
public void FutureCountThrowsArgumentException342()
{
    IFutureValue<int> iFutureValue;
    iFutureValue = this.FutureCount<int>((IQueryable<int>)null);
}
    }
}
