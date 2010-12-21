// <copyright file="StatementIncrementIntegerTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;
using Microsoft.Pex.Framework.Exceptions;

namespace LINQToTTreeLib.Statements
{
    /// <summary>This class contains parameterized unit tests for StatementIncrementInteger</summary>
    [PexClass(typeof(StatementIncrementInteger))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class StatementIncrementIntegerTest
    {
        [PexMethod]
        public StatementIncrementInteger Constructor(VarInteger i)
        {
            StatementIncrementInteger target = new StatementIncrementInteger(i);
            Assert.AreEqual(i, target.Integer, "initial value not set correctly");
            return target;
        }
        [TestMethod]
        public void Constructor341()
        {
            StatementIncrementInteger statementIncrementInteger;
            VarInteger s0 = new VarInteger();
            statementIncrementInteger = this.Constructor(s0);
            Assert.IsNotNull((object)statementIncrementInteger);
            Assert.IsNotNull(statementIncrementInteger.Integer);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsArgumentNullException49()
        {
            StatementIncrementInteger statementIncrementInteger;
            statementIncrementInteger = this.Constructor((VarInteger)null);
        }
    }
}
