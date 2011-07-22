using LINQToTTreeLib.Statements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using LinqToTTreeInterfacesLib;
using System.Collections.Generic;
using Microsoft.Pex.Framework;
using LINQToTTreeLib.Utils;
using Microsoft.Pex.Framework.Validation;
using System.Linq;

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
        public StatementAggregate StatementAggregateConstructorTest(IVariable dest, IValue source)
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
        ///A test for IsSameStatement
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public bool IsSameStatementTest([PexAssumeUnderTest] StatementAggregate target, IStatement statement)
        {
            var result = target.IsSameStatement(statement);

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

        class Opt : ICodeOptimizationService
        {
            public bool ReturnValue = true;

            public string OldName {get; set;}
            public string NewName {get; set;}

            public bool TryRenameVarialbeOneLevelUp(string oldName, string newName)
            {
                OldName = oldName;
                NewName = newName;
                return ReturnValue;
            }
        }

        [TestMethod]
        public void TestCombineWithRename()
        {
            // a = a + b
            // c = c + b
            // These two should combine correctly, somehow.

            var a = new Variables.VarInteger();
            var ainc = new Variables.ValSimple(string.Format("{0}+b", a.RawValue), typeof(int));
            var s1 = new StatementAggregate(a, ainc);

            var c = new Variables.VarInteger();
            var cinc = new Variables.ValSimple(string.Format("{0}+b", c.RawValue), typeof(int));
            var s2 = new StatementAggregate(c, cinc);

            var opt = new Opt();
            var result = s1.TryCombineStatement(s2, opt);
            Assert.IsTrue(result, "Expected combination would work");

            Assert.AreEqual(a.RawValue, opt.NewName, "new name not renamed to");
            Assert.AreEqual(c.RawValue, opt.OldName, "old name for rename not right");
        }

        [TestMethod]
        public void TestCombineWithRenameNoChance()
        {
            // a = a + b
            // c = c + b
            // These two should combine correctly, somehow.

            var a = new Variables.VarInteger();
            var ainc = new Variables.ValSimple(string.Format("{0}+b", a.RawValue), typeof(int));
            var s1 = new StatementAggregate(a, ainc);

            var c = new Variables.VarInteger();
            var cinc = new Variables.ValSimple(string.Format("{0}+b", c.RawValue), typeof(int));
            var s2 = new StatementAggregate(c, cinc);

            var opt = new Opt() { ReturnValue = false };
            var result = s1.TryCombineStatement(s2, opt);
            Assert.IsFalse(result, "Expected combination would work");
        }
    }
}
