using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    ///This is a test class for StatementAssignTest and is intended
    ///to contain all StatementAssignTest Unit Tests
    ///</summary>
    [TestClass]
    public partial class StatementAssignTest
    {
        [TestInitialize]
        public void initTest()
        {
            TestUtils.ResetLINQLibrary();
        }

#if false
        /// <summary>
        ///A test for StatementAssign Constructor
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public StatementAssign StatementAssignConstructorTest(IDeclaredParameter dest, IValue source)
        {
            StatementAssign target = new StatementAssign(dest, source, null);
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
#endif

        class DummyOptService : ICodeOptimizationService
        {
            private bool ReturnWhenTry;
            public DummyOptService(bool whatToReturnWhenTry = true)
            {
                this.ReturnWhenTry = whatToReturnWhenTry;
            }
            public bool TryRenameVarialbeOneLevelUp(string oldName, IDeclaredParameter newVariable)
            {
                return ReturnWhenTry;
            }

            public void ForceRenameVariable(string originalName, string newName)
            {
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
            var s1 = new StatementAssign(i, sv, null, true);
            var s2 = new StatementAssign(i, sv, null, false);

            Assert.IsFalse(s1.TryCombineStatement(s2, new DummyOptService()), "Combine a declare with a non-declare");
            Assert.IsFalse(s2.TryCombineStatement(s1, new DummyOptService()), "Combine a non-declare with a declare");
        }

        /// <summary>
        /// Try to combine two identical statements, when not doing the decl, but when we can't find where we
        /// have declared the variable.
        /// </summary>
        [TestMethod]
        public void TryCombineTwoNoneDeclaresNoDeclFound()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var sv = new ValSimple("5", typeof(int));
            var s1 = new StatementAssign(i, sv, null, false);
            var s2 = new StatementAssign(i, sv, null, false);

            Assert.IsFalse(s1.TryCombineStatement(s2, new DummyOptService(false)), "Combine when no decl found");
        }

        /// <summary>
        /// Try to combine two identical statements, when not doing the decl, but when we can't find where we
        /// have declared the variable.
        /// </summary>
        [TestMethod]
        public void TryCombineTwoNoneDeclaresDeclFound()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var sv = new ValSimple("5", typeof(int));
            var s1 = new StatementAssign(i, sv, null, false);
            var s2 = new StatementAssign(i, sv, null, false);

            Assert.IsTrue(s1.TryCombineStatement(s2, new DummyOptService(true)), "Combine when no decl found");
        }

        /// <summary>
        /// This is a little tricky as teh combination service will return false here - becuase it can't find where
        /// the variable is declared. But that is ok, since we are declaring this guy here. So it should go anyway.
        /// </summary>
        [TestMethod]
        public void TryCombineWithDeclare()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var sv = new ValSimple("5", typeof(int));
            var s1 = new StatementAssign(i, sv, null, true);
            var s2 = new StatementAssign(i, sv, null, true);

            Assert.IsTrue(s1.TryCombineStatement(s2, new DummyOptService(false)), "Combine a declare with a non-declare");
        }

        [TestMethod]
        public void TestDeclare()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var sv = new ValSimple("5", typeof(int));
            var s1 = new StatementAssign(i, sv, null, true);

            Assert.IsTrue(s1.CodeItUp().First().Trim().StartsWith("int "), "Check for decl: " + s1.CodeItUp().First());
        }

        [TestMethod]
        public void TestNoDeclare()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var sv = new ValSimple("5", typeof(int));
            var s1 = new StatementAssign(i, sv, null);

            Assert.IsTrue(s1.CodeItUp().First().Trim().StartsWith("aInt32_"), "Check for decl: " + s1.CodeItUp().First());
        }

        [TestMethod]
        public void TestBasicCMValues()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var sv = new ValSimple("5", typeof(int));
            var s1 = new StatementAssign(i, sv, null);

            Assert.AreEqual(1, s1.ResultVariables.Count, "# result variables");
            Assert.AreEqual(i.RawValue, s1.ResultVariables.First(), "the name");
            Assert.AreEqual(0, s1.DependentVariables.Count, "no dependent variables");
        }

        [TestMethod]
        public void TestCMValuesForSimpleExpression()
        {
            var i = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var sv = new ValSimple("5", typeof(int));
            var di = DeclarableParameter.CreateDeclarableParameterExpression(typeof(double));
            var s1 = new StatementAssign(i, sv, dependentVariables: new IDeclaredParameter[] { di });
            Assert.AreEqual(1, s1.ResultVariables.Count, "# result variables");
            Assert.AreEqual(i.RawValue, s1.ResultVariables.First(), "the name");
            Assert.AreEqual(1, s1.DependentVariables.Count, "no dependent variables");
            Assert.AreEqual(di.RawValue, s1.DependentVariables.First(), "a dependent variable");

        }
    }
}
