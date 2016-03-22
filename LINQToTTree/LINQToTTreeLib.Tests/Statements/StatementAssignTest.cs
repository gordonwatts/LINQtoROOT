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
            var i1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var i2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var sv = new ValSimple("5", typeof(int));
            var s1 = new StatementAssign(i1, sv, null, false);
            var s2 = new StatementAssign(i2, sv, null, false);

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

        [TestMethod]
        public void AssignEquivalentSame()
        {
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var s1 = new StatementAssign(p1, new ValSimple($"{d1.RawValue}", typeof(int)), new IDeclaredParameter[] { d1 });
            var s2 = new StatementAssign(p1, new ValSimple($"{d1.RawValue}", typeof(int)), new IDeclaredParameter[] { d1 });

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsTrue(r.Item1, "can do the combination");
            Assert.AreEqual(0, r.Item2.Count(), "# of variables to rename");
        }

        [TestMethod]
        public void AssignEquivalentSameAfterReplacement()
        {
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var s1 = new StatementAssign(p1, new ValSimple($"{d1.RawValue}", typeof(int)), new IDeclaredParameter[] { d1 });
            var s2 = new StatementAssign(p1, new ValSimple($"{d2.RawValue}", typeof(int)), new IDeclaredParameter[] { d2 });

            var r = s1.RequiredForEquivalence(s2, new Tuple<string, string>[] { new Tuple<string, string>(d2.RawValue, d1.RawValue) });
            Assert.IsTrue(r.Item1, "can do the combination");
            Assert.AreEqual(0, r.Item2.Count(), "# of variables to rename");
        }

        [TestMethod]
        public void AssignEquivalentDiffFunc()
        {
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var s1 = new StatementAssign(p1, new ValSimple($"{d1.RawValue}", typeof(int)), new IDeclaredParameter[] { d1 });
            var s2 = new StatementAssign(p1, new ValSimple($"sin({d1.RawValue})", typeof(int)), new IDeclaredParameter[] { d1 });

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsFalse(r.Item1, "should not be able to do the combination");
        }

        [TestMethod]
        public void AssignEquivalentDifferentDependents()
        {
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var s1 = new StatementAssign(p1, new ValSimple($"{d1.RawValue}", typeof(int)), new IDeclaredParameter[] { d1 });
            var s2 = new StatementAssign(p1, new ValSimple($"{d2.RawValue}", typeof(int)), new IDeclaredParameter[] { d2 });

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsTrue(r.Item1, "can do the combination");
            Assert.AreEqual(1, r.Item2.Count(), "# of variables to rename");
            Assert.AreEqual(d2.RawValue, r.Item2.First().Item1, "first time of rename");
            Assert.AreEqual(d1.RawValue, r.Item2.First().Item2, "first time of rename");
        }

        [TestMethod]
        public void AssignEquivalentTwoDifferentDependents()
        {
            // Replacing two dependents gets to be more than we can do becasue of ordering and the roll they might
            // be playing.
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d4 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var s1 = new StatementAssign(p1, new ValSimple($"{d1.RawValue}+{d2.RawValue}", typeof(int)), new IDeclaredParameter[] { d1, d2 });
            var s2 = new StatementAssign(p1, new ValSimple($"{d3.RawValue}+{d4.RawValue}", typeof(int)), new IDeclaredParameter[] { d3, d4 });

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsTrue(r.Item1, "can do the combination");
        }

        [TestMethod]
        public void AssignEquivalentDifferentResults()
        {
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var s1 = new StatementAssign(p1, new ValSimple($"{d1.RawValue}", typeof(int)), new IDeclaredParameter[] { d1 });
            var s2 = new StatementAssign(p2, new ValSimple($"{d1.RawValue}", typeof(int)), new IDeclaredParameter[] { d1 });

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsTrue(r.Item1, "can do the combination");
            Assert.AreEqual(1, r.Item2.Count(), "# of variables to rename");
            Assert.AreEqual(p2.RawValue, r.Item2.First().Item1, "first time of rename");
            Assert.AreEqual(p1.RawValue, r.Item2.First().Item2, "first time of rename");
        }

        [TestMethod]
        public void AssignEquivalentDifferentResultsAndDependents()
        {
            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var d2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            var s1 = new StatementAssign(p1, new ValSimple($"{d1.RawValue}", typeof(int)), new IDeclaredParameter[] { d1 });
            var s2 = new StatementAssign(p2, new ValSimple($"{d2.RawValue}", typeof(int)), new IDeclaredParameter[] { d2 });

            var r = s1.RequiredForEquivalence(s2);
            Assert.IsTrue(r.Item1, "can do the combination");
            Assert.AreEqual(2, r.Item2.Count(), "# of variables to rename");
        }
    }
}
