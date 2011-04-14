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
using Microsoft.Pex.Framework.Generated;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.TypeHandlers.ReplacementMethodCalls
{
    public partial class TypeHandlerReplacementCallTest
    {
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerReplacementCallTest))]
[Ignore]
[PexDescription("the test state was: path bounds exceeded")]
public void CanHandle274()
{
    using (PexDisposableContext disposables = PexDisposableContext.Create())
    {
      StringReader stringReader;
      TypeHandlerReplacementCall typeHandlerReplacementCall;
      bool b;
      stringReader = new StringReader("");
      disposables.Add((IDisposable)stringReader);
      typeHandlerReplacementCall = new TypeHandlerReplacementCall();
      typeHandlerReplacementCall.Parse((TextReader)stringReader);
      b = this.CanHandle(typeHandlerReplacementCall, (Type)null);
      disposables.Dispose();
    }
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerReplacementCallTest))]
[Ignore]
[PexDescription("the test state was: path bounds exceeded")]
public void CanHandle27401()
{
    using (PexDisposableContext disposables = PexDisposableContext.Create())
    {
      StringReader stringReader;
      TypeHandlerReplacementCall typeHandlerReplacementCall;
      bool b;
      stringReader = new StringReader("\r");
      disposables.Add((IDisposable)stringReader);
      typeHandlerReplacementCall = new TypeHandlerReplacementCall();
      typeHandlerReplacementCall.Parse((TextReader)stringReader);
      b = this.CanHandle(typeHandlerReplacementCall, (Type)null);
      disposables.Dispose();
    }
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerReplacementCallTest))]
[Ignore]
[PexDescription("the test state was: path bounds exceeded")]
public void CanHandle27402()
{
    using (PexDisposableContext disposables = PexDisposableContext.Create())
    {
      StringReader stringReader;
      TypeHandlerReplacementCall typeHandlerReplacementCall;
      bool b;
      stringReader = new StringReader("\t");
      disposables.Add((IDisposable)stringReader);
      typeHandlerReplacementCall = new TypeHandlerReplacementCall();
      typeHandlerReplacementCall.Parse((TextReader)stringReader);
      b = this.CanHandle(typeHandlerReplacementCall, (Type)null);
      disposables.Dispose();
    }
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerReplacementCallTest))]
[Ignore]
[PexDescription("the test state was: path bounds exceeded")]
public void CanHandle27404()
{
    using (PexDisposableContext disposables = PexDisposableContext.Create())
    {
      StringReader stringReader;
      TypeHandlerReplacementCall typeHandlerReplacementCall;
      bool b;
      stringReader = new StringReader("include:");
      disposables.Add((IDisposable)stringReader);
      typeHandlerReplacementCall = new TypeHandlerReplacementCall();
      typeHandlerReplacementCall.Parse((TextReader)stringReader);
      b = this.CanHandle(typeHandlerReplacementCall, (Type)null);
      disposables.Dispose();
    }
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerReplacementCallTest))]
[Ignore]
[PexDescription("the test state was: path bounds exceeded")]
public void CanHandle27405()
{
    using (PexDisposableContext disposables = PexDisposableContext.Create())
    {
      StringReader stringReader;
      TypeHandlerReplacementCall typeHandlerReplacementCall;
      bool b;
      stringReader = new StringReader("include:\u0001");
      disposables.Add((IDisposable)stringReader);
      typeHandlerReplacementCall = new TypeHandlerReplacementCall();
      typeHandlerReplacementCall.Parse((TextReader)stringReader);
      b = this.CanHandle(typeHandlerReplacementCall, (Type)null);
      disposables.Dispose();
    }
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerReplacementCallTest))]
[Ignore]
[PexDescription("the test state was: path bounds exceeded")]
public void CanHandle27408()
{
    using (PexDisposableContext disposables = PexDisposableContext.Create())
    {
      StringReader stringReader;
      TypeHandlerReplacementCall typeHandlerReplacementCall;
      bool b;
      stringReader = new StringReader("#\0\0\0\0\0");
      disposables.Add((IDisposable)stringReader);
      typeHandlerReplacementCall = new TypeHandlerReplacementCall();
      typeHandlerReplacementCall.Parse((TextReader)stringReader);
      b = this.CanHandle(typeHandlerReplacementCall, (Type)null);
      disposables.Dispose();
    }
}
[TestMethod]
[PexGeneratedBy(typeof(TypeHandlerReplacementCallTest))]
[Ignore]
[PexDescription("the test state was: path bounds exceeded")]
public void CanHandle27412()
{
    using (PexDisposableContext disposables = PexDisposableContext.Create())
    {
      StringReader stringReader;
      TypeHandlerReplacementCall typeHandlerReplacementCall;
      bool b;
      stringReader = new StringReader("include: \0\0\0");
      disposables.Add((IDisposable)stringReader);
      typeHandlerReplacementCall = new TypeHandlerReplacementCall();
      typeHandlerReplacementCall.Parse((TextReader)stringReader);
      b = this.CanHandle(typeHandlerReplacementCall, (Type)null);
      disposables.Dispose();
    }
}
    }
}
