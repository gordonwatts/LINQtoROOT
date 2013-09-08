using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for StatementAssignTest and is intended
    ///to contain all StatementAssignTest Unit Tests
    ///</summary>
    [PexClass(typeof(StatementAssign))]
    [TestClass]
    public partial class StatementAssignTest
    {
        [TestInitialize]
        public void initTest()
        {
            TestUtils.ResetLINQLibrary();
        }

        /// <summary>
        ///A test for StatementAssign Constructor
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public StatementAssign StatementAssignConstructorTest(IDeclaredParameter dest, IValue source)
        {
            StatementAssign target = new StatementAssign(dest, source);
            return target;
        }

        /// <summary>
        ///A test for CodeItUp
        ///</summary>
        [PexMethod]
        public string CodeItUpTest([PexAssumeUnderTest] StatementAssign target)
        {
            var result = target.CodeItUp().ToArray();

            var expectedCode = 0;
            if (target.Expression.RawValue != target.ResultVariable.RawValue)
                expectedCode = 1;

            Assert.AreEqual(expectedCode, result.Length, "Too many lines for an equals!");

            if (expectedCode > 0)
            {
                Assert.IsTrue(result[0].Contains("="), "missing equal sign");
                return result[0];
            }
            return "";
        }

        /// <summary>
        ///A test for RenameVariable
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public StatementAssign RenameVariableTest([PexAssumeUnderTest] StatementAssign target, string originalName, string newName)
        {
            target.RenameVariable(originalName, newName);

            return target;
        }

        /// <summary>
        ///A test for TryCombineStatement
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public bool TryCombineStatementTest([PexAssumeUnderTest] StatementAssign target, IStatement statement)
        {
            var result = target.TryCombineStatement(statement, null);

            if (statement == null)
                Assert.Fail("Statement was null");

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

        class DummyOptService : ICodeOptimizationService
        {
            public bool TryRenameVarialbeOneLevelUp(string oldName, IDeclaredParameter newVariable)
            {
                return true;
            }

            public void ForceRenameVariable(string originalName, string newName)
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// Try to combine with a statement that has no declare set.
        /// </summary>
        [TestMethod]
        public void TryCombineWithNonDeclare()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var sv = new ValSimple("5", typeof(int));
            var s1 = new StatementAssign(i, sv, true);
            var s2 = new StatementAssign(i, sv, false);

            Assert.IsFalse(s1.TryCombineStatement(s2, new DummyOptService()), "Combine a declare with a non-declare");
            Assert.IsFalse(s2.TryCombineStatement(s1, new DummyOptService()), "Combine a non-declare with a declare");
        }

        [TestMethod]
        public void TryCombineWithDeclare()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var sv = new ValSimple("5", typeof(int));
            var s1 = new StatementAssign(i, sv, true);
            var s2 = new StatementAssign(i, sv, true);

            Assert.IsTrue(s1.TryCombineStatement(s2, new DummyOptService()), "Combine a declare with a non-declare");
        }

        [TestMethod]
        public void TestDeclare()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var sv = new ValSimple("5", typeof(int));
            var s1 = new StatementAssign(i, sv, true);

            Assert.IsTrue(s1.CodeItUp().First().Trim().StartsWith("int "), "Check for decl: " + s1.CodeItUp().First());
        }

        [TestMethod]
        public void TestNoDeclare()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var sv = new ValSimple("5", typeof(int));
            var s1 = new StatementAssign(i, sv);

            Assert.IsTrue(s1.CodeItUp().First().Trim().StartsWith("aInt32_"), "Check for decl: " + s1.CodeItUp().First());
        }
    }
}
