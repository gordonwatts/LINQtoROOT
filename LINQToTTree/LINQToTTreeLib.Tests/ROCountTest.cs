// <copyright file="ROCountTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>

using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.ResultOperators;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Using;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses.ResultOperators;

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
            GeneratedCode codeEnv
        )
        {
            int origCount = 0;
            if (codeEnv != null)
                origCount = codeEnv.CodeBody.Statements.Count();
            CodeContext c = new CodeContext();
            IVariable result = target.ProcessResultOperator(resultOperator, queryModel, codeEnv, c, null);
            Assert.AreEqual(origCount + 1, codeEnv.CodeBody.Statements.Count(), "Expected an added statement!");
            Assert.IsInstanceOfType(codeEnv.CodeBody.Statements.Last(), typeof(StatementIncrementInteger), "Statement to inc the integer must have been done!");
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
    }
}
