using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for StatementAggregateTest and is intended
    ///to contain all StatementAggregateTest Unit Tests
    ///</summary>
    [PexClass(typeof(StatementAggregate))]
    [TestClass]
    public partial class StatementAggregateTest
    {
        [TestInitialize]
        public void initTest()
        {
            TypeUtils._variableNameCounter = 0;
        }

        /// <summary>
        ///A test for StatementAggregate Constructor
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public StatementAggregate StatementAggregateConstructorTest(DeclarableParameter dest, IValue source)
        {
            StatementAggregate target = new StatementAggregate(dest, source);
            return target;
        }

        /// <summary>
        ///A test for CodeItUp
        ///</summary>
        [PexMethod]
        public string CodeItUpTest([PexAssumeUnderTest] StatementAggregate target)
        {
            var result = target.CodeItUp().ToArray();

            Assert.AreEqual(1, result.Length, "Too many lines for an equals!");

            Assert.IsTrue(result[0].Contains("="), "missing equal sign");
            return result[0];
        }

        /// <summary>
        ///A test for RenameVariable
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public StatementAggregate RenameVariableTest([PexAssumeUnderTest] StatementAggregate target, string originalName, string newName)
        {
            target.RenameVariable(originalName, newName);

            return target;
        }

        /// <summary>
        ///A test for TryCombineStatement
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public bool TryCombineStatementTest([PexAssumeUnderTest] StatementAggregate target, IStatement statement)
        {
            var result = target.TryCombineStatement(statement, null);

            if (statement == null)
                Assert.Fail("Statement was null");

            if (statement.GetType() != typeof(StatementAggregate))
                Assert.IsFalse(result, "statements aren't the rigth type");

            if (statement.CodeItUp().Count() != target.CodeItUp().Count())
            {
                Assert.IsFalse(result, "Different number of items");
            }
            else
            {
                var allsame = target.CodeItUp().Zip(statement.CodeItUp(), (f, s) => f == s).All(t => t);
                Assert.AreEqual(allsame, result, "incorrect result");
            }

            return result;
        }

        [TestMethod]
        public void TestCombineWithRename()
        {
            // a = a + b
            // c = c + b
            // These two should combine correctly, somehow.

            var a = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var ainc = new Variables.ValSimple(string.Format("{0}+b", a.ParameterName), typeof(int));
            var s1 = new StatementAggregate(a, ainc);

            var c = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var cinc = new Variables.ValSimple(string.Format("{0}+b", c.ParameterName), typeof(int));
            var s2 = new StatementAggregate(c, cinc);

            var opt = new Tests.Factories.CodeOptimizerTest(true);
            var result = s1.TryCombineStatement(s2, opt);
            Assert.IsTrue(result, "Expected combination would work");

            Assert.AreEqual(a.ParameterName, opt.NewVariable.ParameterName, "new name not renamed to");
            Assert.AreEqual(c.ParameterName, opt.OldName, "old name for rename not right");
        }

        [TestMethod]
        public void TestCombineWithRenameNoChance()
        {
            // a = a + b
            // c = c + b
            // These two should combine correctly, somehow.

            var a = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var ainc = new Variables.ValSimple(string.Format("{0}+b", a.ParameterName), typeof(int));
            var s1 = new StatementAggregate(a, ainc);

            var c = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var cinc = new Variables.ValSimple(string.Format("{0}+b", c.ParameterName), typeof(int));
            var s2 = new StatementAggregate(c, cinc);

            var opt = new Factories.CodeOptimizerTest(false);
            var result = s1.TryCombineStatement(s2, opt);
            Assert.IsFalse(result, "Expected combination would work");
        }

        [TestMethod]
        public void TestAggregateWithInt()
        {
            var a = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var b = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementAggregate(a, b);

            Assert.AreEqual(string.Format("{0}={1};", a.RawValue, b.RawValue), s1.CodeItUp().First(), "Plain assignment");
        }

        [TestMethod]
        public void TestAggregateWithTLZ()
        {
            var a = DeclarableParameter.CreateDeclarableParameterExpression(typeof(ROOTNET.NTLorentzVector));
            var b = DeclarableParameter.CreateDeclarableParameterExpression(typeof(ROOTNET.NTLorentzVector));
            var s1 = new StatementAggregate(a, b);

            Assert.AreEqual(string.Format("*{0}=*{1};", a.RawValue, b.RawValue), s1.CodeItUp().First(), "Plain assignment");
        }
    }
}
