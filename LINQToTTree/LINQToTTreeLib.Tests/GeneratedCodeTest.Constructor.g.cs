using Microsoft.Pex.Framework.Generated;
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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib
{
    public partial class GeneratedCodeTest
    {
[TestMethod]
[PexGeneratedBy(typeof(GeneratedCodeTest))]
public void Constructor808()
{
    GeneratedCode generatedCode;
    generatedCode = this.Constructor();
    Assert.IsNotNull((object)generatedCode);
    Assert.AreEqual<int>(1, generatedCode.Depth);
    Assert.IsNull(generatedCode.ResultValue);
    Assert.IsNotNull(generatedCode.CodeBody);
    Assert.IsNotNull(generatedCode.VariablesToTransfer);
    Assert.IsNotNull(generatedCode.IncludeFiles);
    Assert.IsNotNull(generatedCode.ReferencedLeafNames);
}
    }
}
