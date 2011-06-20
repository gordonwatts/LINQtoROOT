using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
using LINQToTTreeLib.Variables;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Moles;
using LINQToTTreeLib.Variables.Moles;
using LINQToTTreeLib.Statements.Moles;
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
public void TranslateGeneratedCodeThrowsArgumentNullException74()
{
    CPPTranslator cPPTranslator;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    dictionary = this.TranslateGeneratedCode(cPPTranslator, (GeneratedCode)null);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode912()
{
    CPPTranslator cPPTranslator;
    SGeneratedCode sGeneratedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    sGeneratedCode = new SGeneratedCode();
    dictionary =
      this.TranslateGeneratedCode(cPPTranslator, (GeneratedCode)sGeneratedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(4, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode550()
{
    CPPTranslator cPPTranslator;
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    sVarInteger = new SVarInteger();
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, (string[])null);
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
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    sVarInteger = new SVarInteger();
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, ss);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(4, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode55002()
{
    CPPTranslator cPPTranslator;
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    sVarInteger = new SVarInteger();
    string[] ss = new string[1];
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  ss, (string[])null, (string[])null);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(4, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode55003()
{
    CPPTranslator cPPTranslator;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    varInteger = VarIntegerFactory.Create(false, 0);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, (string[])null);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(4, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode55004()
{
    CPPTranslator cPPTranslator;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    varInteger = VarIntegerFactory.Create(false, 1);
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)varInteger, (IStatement[])null, 
                                  (string[])null, (string[])null, (string[])null);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(4, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode55005()
{
    CPPTranslator cPPTranslator;
    SVarInteger sVarInteger;
    SStatementIncrementInteger sStatementIncrementInteger;
    GeneratedCode generatedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    sVarInteger = new SVarInteger();
    sStatementIncrementInteger =
      new SStatementIncrementInteger((VarInteger)sVarInteger);
    IStatement[] iStatements = new IStatement[1];
    string[] ss = new string[1];
    iStatements[0] = (IStatement)sStatementIncrementInteger;
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)sVarInteger, iStatements, (string[])null, (string[])null, ss);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(4, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode55006()
{
    CPPTranslator cPPTranslator;
    SVarInteger sVarInteger;
    StatementIncrementInteger statementIncrementInteger;
    GeneratedCode generatedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    sVarInteger = new SVarInteger();
    statementIncrementInteger =
      StatementIncrementIntegerFactory.Create((VarInteger)sVarInteger);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementIncrementInteger;
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, iStatements, 
                                  (string[])null, (string[])null, (string[])null);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(4, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode55007()
{
    CPPTranslator cPPTranslator;
    SVarInteger sVarInteger;
    StatementInlineBlock statementInlineBlock;
    GeneratedCode generatedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    sVarInteger = new SVarInteger();
    IStatement[] iStatements = new IStatement[0];
    IVariable[] iVariables = new IVariable[1];
    statementInlineBlock =
      StatementInlineBlockFactory.Create(iStatements, iVariables);
    IStatement[] iStatements1 = new IStatement[1];
    string[] ss = new string[1];
    iStatements1[0] = (IStatement)statementInlineBlock;
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)sVarInteger, iStatements1, (string[])null, (string[])null, ss);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(4, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode55008()
{
    CPPTranslator cPPTranslator;
    SVarInteger sVarInteger;
    SStatementInlineBlock sStatementInlineBlock;
    GeneratedCode generatedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    sVarInteger = new SVarInteger();
    sStatementInlineBlock = new SStatementInlineBlock();
    IStatement[] iStatements = new IStatement[1];
    string[] ss = new string[1];
    string[] ss1 = new string[1];
    iStatements[0] = (IStatement)sStatementInlineBlock;
    ss[0] = "\0";
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)sVarInteger, iStatements, (string[])null, ss, ss1);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(4, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode55012()
{
    CPPTranslator cPPTranslator;
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    sVarInteger = new SVarInteger();
    string[] ss = new string[1];
    ss[0] = "\0";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, ss, (string[])null);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(4, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode55017()
{
    CPPTranslator cPPTranslator;
    SVarInteger sVarInteger;
    StatementInlineBlock statementInlineBlock;
    GeneratedCode generatedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    sVarInteger = new SVarInteger();
    statementInlineBlock =
      StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
    IStatement[] iStatements = new IStatement[1];
    iStatements[0] = (IStatement)statementInlineBlock;
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, iStatements, 
                                  (string[])null, (string[])null, (string[])null);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(4, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
    }
}
