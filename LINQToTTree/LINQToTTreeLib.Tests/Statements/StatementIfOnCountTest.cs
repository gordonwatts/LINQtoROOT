// <copyright file="StatementIfOnCountTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Using;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Statements
{
    /// <summary>This class contains parameterized unit tests for StatementIfOnCount</summary>
    [PexClass(typeof(StatementIfOnCount))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class StatementIfOnCountTest
    {
        [TestInitialize]
        public void TestSetup()
        {
            TypeUtils._variableNameCounter = 0;
        }

        /// <summary>Test stub for CodeItUp()</summary>
        [PexMethod]
        public IEnumerable<string> CodeItUp([PexAssumeUnderTest]StatementIfOnCount target)
        {
            IEnumerable<string> result = target.CodeItUp();
            return result;
            // TODO: add assertions to method StatementIfOnCountTest.CodeItUp(StatementIfOnCount)
        }

        /// <summary>Test stub for .ctor(IValue, IValue, ComparisonOperator)</summary>
        [PexMethod]
        public StatementIfOnCount Constructor(
            IDeclaredParameter valueLeft,
            IValue valueRight,
            StatementIfOnCount.ComparisonOperator comp
        )
        {
            StatementIfOnCount target = new StatementIfOnCount(valueLeft, valueRight, comp);
            return target;
            // TODO: add assertions to method StatementIfOnCountTest.Constructor(IValue, IValue, ComparisonOperator)
        }

        [TestMethod]
        public void TestEmptyStatements()
        {
            var val = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var statement = new StatementIfOnCount(DeclarableParameter.CreateDeclarableParameterExpression(typeof(string)), new Variables.ValSimple("two", typeof(string)), StatementIfOnCount.ComparisonOperator.EqualTo);

            var result = statement.CodeItUp().ToArray();
            Assert.AreEqual(0, result.Length, "no statements, so wasn't expecting any sort of output at all");
        }

        [TestMethod]
        public void TestWithStatement()
        {
            var val = new Variables.ValSimple("true", typeof(bool));
            var statement = new StatementIfOnCount(DeclarableParameter.CreateDeclarableParameterExpression(typeof(string)), new Variables.ValSimple("two", typeof(string)), StatementIfOnCount.ComparisonOperator.EqualTo);
            statement.Add(new StatementSimpleStatement("dude"));

            var result = statement.CodeItUp().ToArray();
            Assert.AreEqual(5, result.Length, "no statements, so wasn't expecting any sort of output at all");
            Assert.AreEqual("if (aString_1 == two)", result[1], "if statement is not correct");
        }

        [TestMethod]
        public void TestAllOperators()
        {
            var matchedValues = new Tuple<StatementIfOnCount.ComparisonOperator, string>[] {
                Tuple.Create(StatementIfOnCount.ComparisonOperator.EqualTo, "=="),
                Tuple.Create(StatementIfOnCount.ComparisonOperator.GreaterThan, ">"),
                Tuple.Create(StatementIfOnCount.ComparisonOperator.GreaterThanEqual, ">="),
                Tuple.Create(StatementIfOnCount.ComparisonOperator.LessThan, "<"),
                Tuple.Create(StatementIfOnCount.ComparisonOperator.LessThanEqual, "<=")
            };

            foreach (var op in matchedValues)
            {
                var val = new Variables.ValSimple("true", typeof(bool));
                var statement = new StatementIfOnCount(DeclarableParameter.CreateDeclarableParameterExpression(typeof(string)), new Variables.ValSimple("two", typeof(string)), op.Item1);
                statement.Add(new StatementSimpleStatement("dude"));

                var result = statement.CodeItUp().ToArray();
                Assert.AreEqual(5, result.Length, "no statements, so wasn't expecting any sort of output at all");
                Assert.IsTrue(result[1].EndsWith(op.Item2 + " two)"), "if statement is not correct");
            }
        }

        [PexMethod]
        [PexUseType(typeof(StatementInlineBlock))]
        [PexUseType(typeof(StatementIncrementInteger))]
        [PexUseType(typeof(StatementIfOnCount))]
        public void TestTryCombine(IStatement s)
        {
            /// We should never be able to combine any filter statements currently!

            var val = new Variables.ValSimple("true", typeof(bool));
            var statement = new StatementIfOnCount(DeclarableParameter.CreateDeclarableParameterExpression(typeof(string)), new Variables.ValSimple("two", typeof(string)), StatementIfOnCount.ComparisonOperator.EqualTo);

            Assert.IsFalse(statement.TryCombineStatement(s, null), "unable to do any combines for Filter");
        }
    }
}
