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
using System.IO;
using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;

namespace LINQToTTreeLib
{
    public partial class QueryResultCacheTest
    {
[TestMethod]
[PexGeneratedBy(typeof(QueryResultCacheTest))]
[ExpectedException(typeof(ArgumentException))]
public void LookupThrowsArgumentException137()
{
    QueryResultCache queryResultCache;
    Tuple<bool, int> tuple;
    queryResultCache = new QueryResultCache();
    tuple = this.Lookup<int>(queryResultCache, (FileInfo)null, (string)null, 
                             (object[])null, (QueryModel)null, (IVariableSaver)null, false);
}
    }
}
