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
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests.Statements
{
    public partial class StatementLoopOverGoodTest
    {
        [TestMethod]
        [PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestTryCombineThrowsArgumentNullException904()
        {
            VarInteger varInteger;
            StatementLoopOverGood statementLoopOverGood;
            bool b;
            varInteger = VarIntegerFactory.Create(false, 0);
            statementLoopOverGood =
              StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger,
                                                  (IValue)varInteger, (IStatement[])null, (IVariable[])null);
            b = this.TestTryCombine(statementLoopOverGood, (IStatement)null);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestTryCombineThrowsArgumentNullException838()
        {
            VarInteger varInteger;
            StatementLoopOverGood statementLoopOverGood;
            bool b;
            varInteger = VarIntegerFactory.Create(false, 0);
            IVariable[] iVariables = new IVariable[1];
            iVariables[0] = (IVariable)varInteger;
            statementLoopOverGood =
              StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger,
                                                  (IValue)varInteger, (IStatement[])null, iVariables);
            b = this.TestTryCombine(statementLoopOverGood, (IStatement)null);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
        public void TestTryCombine186()
        {
            VarInteger varInteger;
            StatementLoopOverGood statementLoopOverGood;
            StatementIncrementInteger statementIncrementInteger;
            bool b;
            varInteger = VarIntegerFactory.Create(false, 0);
            statementLoopOverGood =
              StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger,
                                                  (IValue)varInteger, (IStatement[])null, (IVariable[])null);
            statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
            b = this.TestTryCombine
                    (statementLoopOverGood, (IStatement)statementIncrementInteger);
            Assert.AreEqual<bool>(false, b);
            Assert.IsNotNull((object)statementLoopOverGood);
            Assert.IsNotNull(((StatementInlineBlockBase)statementLoopOverGood).Statements);
            Assert.IsNotNull
                (((StatementInlineBlockBase)statementLoopOverGood).DeclaredVariables);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestTryCombineThrowsArgumentNullException361()
        {
            VarInteger varInteger;
            StatementIncrementInteger statementIncrementInteger;
            StatementLoopOverGood statementLoopOverGood;
            bool b;
            varInteger = VarIntegerFactory.Create(false, int.MaxValue);
            statementIncrementInteger = StatementIncrementIntegerFactory.Create(varInteger);
            IStatement[] iStatements = new IStatement[1];
            iStatements[0] = (IStatement)statementIncrementInteger;
            statementLoopOverGood =
              StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger,
                                                  (IValue)varInteger, iStatements, (IVariable[])null);
            b = this.TestTryCombine(statementLoopOverGood, (IStatement)null);
        }
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
[ExpectedException(typeof(ArgumentException))]
public void TestTryCombineThrowsArgumentException424()
{
    VarInteger varInteger;
    StatementLoopOverGood statementLoopOverGood;
    bool b;
    varInteger = VarIntegerFactory.Create(false, 0);
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IValue)varInteger, (IStatement[])null, (IVariable[])null);
    b =
      this.TestTryCombine(statementLoopOverGood, (IStatement)statementLoopOverGood);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestTryCombineThrowsArgumentNullException86()
{
    VarInteger varInteger;
    VarInteger varInteger1;
    StatementLoopOverGood statementLoopOverGood;
    bool b;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    varInteger1 = VarIntegerFactory.Create(false, 0);
    IVariable[] iVariables = new IVariable[2];
    iVariables[0] = (IVariable)varInteger1;
    iVariables[1] = (IVariable)varInteger;
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IValue)varInteger, (IStatement[])null, iVariables);
    b = this.TestTryCombine(statementLoopOverGood, (IStatement)null);
}
[TestMethod]
[PexGeneratedBy(typeof(StatementLoopOverGoodTest))]
[ExpectedException(typeof(ArgumentNullException))]
public void TestTryCombineThrowsArgumentNullException836()
{
    VarInteger varInteger;
    VarInteger varInteger1;
    StatementLoopOverGood statementLoopOverGood;
    bool b;
    varInteger = VarIntegerFactory.Create(false, int.MaxValue);
    varInteger1 = VarIntegerFactory.Create(false, 1);
    IVariable[] iVariables = new IVariable[2];
    iVariables[0] = (IVariable)varInteger1;
    iVariables[1] = (IVariable)varInteger;
    statementLoopOverGood =
      StatementLoopOverGoodFactory.Create((IValue)varInteger, (IValue)varInteger, 
                                          (IValue)varInteger, (IStatement[])null, iVariables);
    b = this.TestTryCombine(statementLoopOverGood, (IStatement)null);
}
    }
}
