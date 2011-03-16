// <copyright file="ROCountTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>

using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Using;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Clauses.ResultOperators;
using LINQToTTreeLib.ResultOperators;

namespace LINQToTTreeLib
{
    [TestClass]
    [PexClass(typeof(ROCount))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class ROCountTest
    {
        [PexMethod]
        [PexUseType(typeof(GeneratedCode))]
        internal IVariable ProcessResultOperator(
            [PexAssumeUnderTest]ROCount target,
            CountResultOperator resultOperator,
            QueryModel queryModel,
            IGeneratedCode codeEnv
        )
        {
            CodeContext c = new CodeContext();
            IVariable result = target.ProcessResultOperator(resultOperator, queryModel, codeEnv, c, null);
            Assert.AreEqual(1, codeEnv.CodeBody.Statements.Count(), "Expected an added statement!");
            Assert.IsInstanceOfType(codeEnv.CodeBody.Statements.First(), typeof(StatementIncrementInteger), "Statement to inc the integer must have been done!");
            Assert.IsInstanceOfType(result, typeof(VarInteger), "Expected to be calculating an integer");
            return result;
        }
        [PexMethod]
        internal bool CanHandle([PexAssumeUnderTest]ROCount target, Type resultOperatorType)
        {
            bool result = target.CanHandle(resultOperatorType);
            Assert.IsTrue(result || resultOperatorType != typeof(CountResultOperator), "Bad response!");
            return result;
        }
        [TestMethod]
        public void CanHandle106()
        {
            bool b;
            ROCount s0 = new ROCount();
            b = this.CanHandle(s0, typeof(CountResultOperator));
            Assert.AreEqual<bool>(true, b);
            Assert.IsNotNull((object)s0);
        }
        [TestMethod]
        public void CanHandle665()
        {
            bool b;
            ROCount s0 = new ROCount();
            b = this.CanHandle(s0, (Type)null);
            Assert.AreEqual<bool>(false, b);
            Assert.IsNotNull((object)s0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessResultOperatorThrowsArgumentNullException625()
        {
            GeneratedCode generatedCode;
            IVariable iVariable;
            generatedCode = GeneratedCodeFactory.Create(new VarInteger());
            ROCount s0 = new ROCount();
            iVariable = this.ProcessResultOperator(s0, (CountResultOperator)null,
                                                   (QueryModel)null, (IGeneratedCode)generatedCode);
        }
        [TestMethod]
        public void ProcessResultOperator133()
        {
            GeneratedCode sGeneratedCode;
            IVariable iVariable;
            sGeneratedCode = new GeneratedCode();
            ROCount s0 = new ROCount();
            CountResultOperator s1 = new CountResultOperator();
            iVariable = this.ProcessResultOperator
                            (s0, s1, (QueryModel)null, (IGeneratedCode)sGeneratedCode);
            Assert.IsNotNull((object)iVariable);
            Assert.IsNotNull((object)s0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessResultOperatorThrowsArgumentNullException395()
        {
            GeneratedCode sGeneratedCode;
            IVariable iVariable;
            sGeneratedCode = new GeneratedCode();
            ROCount s0 = new ROCount();
            iVariable = this.ProcessResultOperator(s0, (CountResultOperator)null,
                                                   (QueryModel)null, (IGeneratedCode)sGeneratedCode);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ProcessResultOperatorThrowsArgumentNullException73()
        {
            IVariable iVariable;
            ROCount s0 = new ROCount();
            iVariable = this.ProcessResultOperator
                            (s0, (CountResultOperator)null, (QueryModel)null, (IGeneratedCode)null);
        }
    }
}
