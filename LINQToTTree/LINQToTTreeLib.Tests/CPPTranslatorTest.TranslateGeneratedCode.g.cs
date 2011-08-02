using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
using LINQToTTreeLib.Variables;
using LinqToTTreeInterfacesLib;
using ROOTNET;
using LINQToTTreeLib.Statements;
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
    public partial class CPPTranslatorTest
    {
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TranslateGeneratedCodeThrowsArgumentNullException991()
{
    CPPTranslator cPPTranslator;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    dictionary = this.TranslateGeneratedCode(cPPTranslator, (GeneratedCode)null);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode550()
{
    CPPTranslator cPPTranslator;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    varInteger = VarIntegerFactory.Create(false, 0);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (NTObject[])null, (string[])null);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(4, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode55001()
{
    CPPTranslator cPPTranslator;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  ss, (NTObject[])null, (string[])null);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(4, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode55010()
{
    CPPTranslator cPPTranslator;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    varInteger = VarIntegerFactory.Create(false, 0);
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (NTObject[])null, ss);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(4, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
    }
}
