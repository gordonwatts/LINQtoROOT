﻿using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for TestStatementCheckLoopPairwise and is intended
    ///to contain all TestStatementCheckLoopPairwise Unit Tests
    ///</summary>
    [TestClass]
    public partial class TestStatementCheckLoopPairwise
    {
        [TestInitialize]
        public void TestInit()
        {
            TestUtils.ResetLINQLibrary();
        }

#if false
        /// <summary>
        ///A test for StatementCheckLoopPairwise Constructor
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public void TestStatementCheckLoopPairwiseConstructor(IDeclaredParameter indiciesToInspect, IDeclaredParameter index1, IDeclaredParameter index2, IDeclaredParameter passedArray)
        {
            StatementCheckLoopPairwise target = new StatementCheckLoopPairwise(indiciesToInspect, index1, index2, passedArray);
        }

        /// <summary>
        ///A test for CodeItUp
        ///</summary>
        [PexMethod]
        public string[] TestCodeItUp([PexAssumeUnderTest] StatementCheckLoopPairwise target)
        {
            var actual = target.CodeItUp();
            return actual.ToArray();
        }

        /// <summary>
        ///A test for RenameVariable
        ///</summary>
        [PexMethod]
        public StatementCheckLoopPairwise TestRenameVariable([PexAssumeUnderTest] StatementCheckLoopPairwise target, string oldName, string newName)
        {
            target.RenameVariable(oldName, newName);
            return target;
        }

        /// <summary>
        ///A test for TryCombineStatement
        ///</summary>
        [PexMethod]
        public bool TestTryCombineStatement([PexAssumeUnderTest] StatementCheckLoopPairwise target, IStatement statement, ICodeOptimizationService opt)
        {
            var actual = target.TryCombineStatement(statement, opt);

            return actual;
        }
#endif

        [TestMethod]
        public void TestTryCombinedFail()
        {
            var indiciesToInspect = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(int));
            var index1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var passedArray = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(bool));
            var s1 = new StatementCheckLoopPairwise(indiciesToInspect, index1, index2, passedArray);

            var indiciesToInspect1 = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(int));
            var index3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var index4 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var passedArray1 = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(bool));
            var s2 = new StatementCheckLoopPairwise(indiciesToInspect1, index3, index4, passedArray1);

            Assert.IsFalse(s1.TryCombineStatement(s2, null), "COmbine should fail");
        }

        class DoRenames : ICodeOptimizationService
        {
            private StatementCheckLoopPairwise s2;

            public DoRenames(StatementCheckLoopPairwise s2)
            {
                // TODO: Complete member initialization
                this.s2 = s2;
            }

            public bool TryRenameVarialbeOneLevelUp(string oldName, IDeclaredParameter newVariable)
            {
                s2.RenameVariable(oldName, newVariable.ParameterName);

                return true;
            }


            public void ForceRenameVariable(string originalName, string newName)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void TestTryCombineGood()
        {
            var indiciesToInspect = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(int));
            var index1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var passedArray = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(bool));
            var s1 = new StatementCheckLoopPairwise(indiciesToInspect, index1, index2, passedArray);

            var index3 = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(int));
            var index4 = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(int));
            var passedArray1 = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(bool));
            var s2 = new StatementCheckLoopPairwise(indiciesToInspect, index3, index4, passedArray1);
            s2.Add(new StatementSimpleStatement(string.Format("{0} = dude", index3.RawValue)));

            var co = new DoRenames(s2);
            Assert.IsTrue(s1.TryCombineStatement(s2, co), "COmbine should pass");
            Assert.AreEqual(1, s1.Statements.Count(), "# of statements");
            Assert.AreEqual(string.Format("{0} = dude", index1.RawValue), (s1.Statements.First() as StatementSimpleStatement).Line, "statement not translated");
        }

        [TestMethod]
        public void TestRename()
        {
            var indiciesToInspect = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(int));
            var index1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var passedArray = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(bool));
            var s1 = new StatementCheckLoopPairwise(indiciesToInspect, index1, index2, passedArray);
            s1.Add(new StatementSimpleStatement(string.Format("{0} = fork", index2.RawValue)));

            s1.RenameVariable(indiciesToInspect.RawValue, "dude1");
            Assert.AreEqual("dude1", indiciesToInspect.RawValue, "indices 1");

            s1.RenameVariable(index1.RawValue, "dude2");
            Assert.AreEqual(index1.RawValue, "dude2", "index1 didn't get set");

            s1.RenameVariable(index2.RawValue, "dude3");
            Assert.AreEqual(index2.RawValue, "dude3", "index2 didn't get set");

            s1.RenameVariable(passedArray.RawValue, "dude4");
            Assert.AreEqual(passedArray.RawValue, "dude4", "passed array didn't get set");

            Assert.AreEqual("dude3 = fork", (s1.Statements.First() as StatementSimpleStatement).Line, "statement 1 didn't get translated");
        }
    }
}
