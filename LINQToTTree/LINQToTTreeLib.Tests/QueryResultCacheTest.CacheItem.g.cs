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
using Remotion.Linq;
using ROOTNET.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace LINQToTTreeLib
{
    public partial class QueryResultCacheTest
    {
[TestMethod]
[PexGeneratedBy(typeof(QueryResultCacheTest))]
[ExpectedException(typeof(ArgumentException))]
public void CacheItemThrowsArgumentException170()
{
    QueryResultCache queryResultCache;
    queryResultCache = new QueryResultCache();
    this.CacheItem(queryResultCache, (object[])null, (FileInfo)null, 
                   (string[])null, (QueryModel)null, (NTObject)null);
}
    }
}
