using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using System;
using LINQToTTreeLib.Variables;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Moles;
using Microsoft.Pex.Framework.Moles;
using LinqToTTreeInterfacesLib.Moles;
using LINQToTTreeLib.Variables.Moles;
using LINQToTTreeLib.Statements.Moles;
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
public void TranslateGeneratedCodeThrowsArgumentNullException50()
{
    CPPTranslator cPPTranslator;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    dictionary = this.TranslateGeneratedCode(cPPTranslator, (GeneratedCode)null);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode990()
{
    CPPTranslator cPPTranslator;
    SVarInteger sVarInteger;
    GeneratedCode generatedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    sVarInteger = new SVarInteger();
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, (IStatement[])null, 
                                  (string[])null, (string[])null);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(3, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode99001()
{
    CPPTranslator cPPTranslator;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    varInteger = VarIntegerFactory.Create(false, 0);
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)varInteger, (IStatement[])null, (string[])null, (string[])null);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(3, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode99002()
{
    CPPTranslator cPPTranslator;
    VarInteger varInteger;
    GeneratedCode generatedCode;
    Dictionary<string, object> dictionary;
    cPPTranslator = CPPTranslatorFactory.Create();
    varInteger = VarIntegerFactory.Create(false, 1);
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)varInteger, (IStatement[])null, (string[])null, (string[])null);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(3, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode360()
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
    Assert.AreEqual<int>(3, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TranslateGeneratedCodeThrowsArgumentNullException168()
{
    using (PexChooseBehavedBehavior.Setup())
    {
      CPPTranslator cPPTranslator;
      SIVariable sIVariable;
      GeneratedCode generatedCode;
      Dictionary<string, object> dictionary;
      cPPTranslator = CPPTranslatorFactory.Create();
      sIVariable = new SIVariable();
      generatedCode =
        GeneratedCodeFactory.Create((IVariable)sIVariable, (IStatement[])null, 
                                    (string[])null, (string[])null);
      dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    }
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TranslateGeneratedCodeThrowsArgumentNullException312()
{
    using (PexChooseBehavedBehavior.Setup())
    {
      CPPTranslator cPPTranslator;
      SIVariable sIVariable;
      GeneratedCode generatedCode;
      Dictionary<string, object> dictionary;
      cPPTranslator = CPPTranslatorFactory.Create();
      sIVariable = new SIVariable();
      string[] ss = new string[1];
      generatedCode = GeneratedCodeFactory.Create
                          ((IVariable)sIVariable, (IStatement[])null, ss, (string[])null);
      dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    }
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TranslateGeneratedCodeThrowsArgumentNullException651()
{
    using (PexChooseBehavedBehavior.Setup())
    {
      CPPTranslator cPPTranslator;
      SIVariable sIVariable;
      GeneratedCode generatedCode;
      Dictionary<string, object> dictionary;
      cPPTranslator = CPPTranslatorFactory.Create();
      sIVariable = new SIVariable();
      string[] ss = new string[1];
      ss[0] = "\0";
      generatedCode = GeneratedCodeFactory.Create
                          ((IVariable)sIVariable, (IStatement[])null, (string[])null, ss);
      dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    }
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TranslateGeneratedCodeThrowsArgumentNullException329()
{
    using (PexChooseBehavedBehavior.Setup())
    {
      CPPTranslator cPPTranslator;
      SIVariable sIVariable;
      SIStatement sIStatement;
      GeneratedCode generatedCode;
      Dictionary<string, object> dictionary;
      cPPTranslator = CPPTranslatorFactory.Create();
      sIVariable = new SIVariable();
      sIStatement = new SIStatement();
      IStatement[] iStatements = new IStatement[1];
      iStatements[0] = (IStatement)sIStatement;
      generatedCode = GeneratedCodeFactory.Create
                          ((IVariable)sIVariable, iStatements, (string[])null, (string[])null);
      dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    }
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode99007()
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
    iStatements[0] = (IStatement)sStatementInlineBlock;
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)sVarInteger, iStatements, (string[])null, (string[])null);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(3, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode99008()
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
    generatedCode = GeneratedCodeFactory.Create
                        ((IVariable)sVarInteger, iStatements, (string[])null, (string[])null);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(3, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TranslateGeneratedCodeThrowsArgumentNullException854()
{
    using (PexChooseBehavedBehavior.Setup())
    {
      CPPTranslator cPPTranslator;
      SIVariable sIVariable;
      StatementInlineBlock statementInlineBlock;
      GeneratedCode generatedCode;
      Dictionary<string, object> dictionary;
      cPPTranslator = CPPTranslatorFactory.Create();
      sIVariable = new SIVariable();
      statementInlineBlock =
        StatementInlineBlockFactory.Create((IStatement[])null, (IVariable[])null);
      IStatement[] iStatements = new IStatement[1];
      iStatements[0] = (IStatement)statementInlineBlock;
      generatedCode = GeneratedCodeFactory.Create
                          ((IVariable)sIVariable, iStatements, (string[])null, (string[])null);
      dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    }
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TranslateGeneratedCodeThrowsArgumentNullException76()
{
    using (PexChooseBehavedBehavior.Setup())
    {
      CPPTranslator cPPTranslator;
      SIVariable sIVariable;
      SVarInteger sVarInteger;
      SStatementIncrementInteger sStatementIncrementInteger;
      GeneratedCode generatedCode;
      Dictionary<string, object> dictionary;
      cPPTranslator = CPPTranslatorFactory.Create();
      sIVariable = new SIVariable();
      sVarInteger = new SVarInteger();
      sStatementIncrementInteger =
        new SStatementIncrementInteger((VarInteger)sVarInteger);
      IStatement[] iStatements = new IStatement[1];
      iStatements[0] = (IStatement)sStatementIncrementInteger;
      generatedCode = GeneratedCodeFactory.Create
                          ((IVariable)sIVariable, iStatements, (string[])null, (string[])null);
      dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    }
}
[TestMethod]
[PexGeneratedBy(typeof(CPPTranslatorTest))]
public void TranslateGeneratedCode99011()
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
    string[] ss = new string[7];
    string[] ss1 = new string[4];
    iStatements[0] = (IStatement)sStatementIncrementInteger;
    ss[0] = "";
    ss[1] = "";
    ss[2] = "";
    ss[3] = "";
    ss[4] = "";
    ss[5] = "";
    ss[6] = "";
    ss1[0] = "\u0089\u0089";
    ss1[1] = "\0\0";
    ss1[2] = "\0\0";
    ss1[3] = "\0\0";
    generatedCode =
      GeneratedCodeFactory.Create((IVariable)sVarInteger, iStatements, ss, ss1);
    dictionary = this.TranslateGeneratedCode(cPPTranslator, generatedCode);
    Assert.IsNotNull((object)dictionary);
    Assert.IsNotNull(dictionary.Comparer);
    Assert.AreEqual<int>(3, dictionary.Count);
    Assert.IsNotNull((object)cPPTranslator);
}
    }
}
