using ROOTNET;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
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

namespace LINQToTTreeLib
{
    public partial class CombinedGeneratedCodeTest
    {
[TestMethod]
[PexGeneratedBy(typeof(CombinedGeneratedCodeTest))]
public void CheckAddSameVariableNamesToTransfer37()
{
    NTObject[] nTObjects = new NTObject[0];
    this.CheckAddSameVariableNamesToTransfer(nTObjects);
}
[TestMethod]
[PexGeneratedBy(typeof(CombinedGeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void CheckAddSameVariableNamesToTransferThrowsArgumentNullException48()
{
    NTObject[] nTObjects = new NTObject[1];
    this.CheckAddSameVariableNamesToTransfer(nTObjects);
}
[TestMethod]
[PexGeneratedBy(typeof(CombinedGeneratedCodeTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void CheckAddSameVariableNamesToTransferThrowsArgumentNullException475()
{
    using (PexDisposableContext disposables = PexDisposableContext.Create())
    {
      NTObject nTObject;
      nTObject = NTObjectFactory.Create();
      disposables.Add((IDisposable)nTObject);
      NTObject[] nTObjects = new NTObject[1];
      nTObjects[0] = nTObject;
      this.CheckAddSameVariableNamesToTransfer(nTObjects);
      disposables.Dispose();
    }
}
    }
}
