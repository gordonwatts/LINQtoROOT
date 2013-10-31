using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    ///This is a test class for TestStatementRecordValue and is intended
    ///to contain all TestStatementRecordValue Unit Tests
    ///</summary>
    [TestClass]
    [PexClass(typeof(StatementRecordValue))]
    public partial class TestStatementRecordValue
    {
        [TestInitialize]
        public void TestInit()
        {
            TestUtils.ResetLINQLibrary();
        }

        /// <summary>
        ///A test for CodeItUp
        ///</summary>
        [PexMethod]
        public string[] TestCodeItUp([PexAssumeUnderTest] StatementRecordValue target)
        {
            var actual = target.CodeItUp().ToArray();
            return actual;
        }

        /// <summary>
        ///A test for RenameVariable
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public StatementRecordValue TestRenameVariable([PexAssumeUnderTest] StatementRecordValue target, string originalName, string newName)
        {
            target.RenameVariable(originalName, newName);
            return target;
        }

        /// <summary>
        ///A test for TryCombineStatement
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public bool TestTryCombineStatement([PexAssumeUnderTest] StatementRecordValue target, IStatement statement, ICodeOptimizationService optimize)
        {
            var actual = target.TryCombineStatement(statement, optimize);
            return actual;
        }

        [TestMethod]
        public void TestTCIdenticalButForFirstLast()
        {
            var index = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var s1 = new StatementRecordValue(index, new ValSimple("i", typeof(int)), seen, true);
            var s2 = new StatementRecordValue(index, new ValSimple("i", typeof(int)), seen, false);

            Assert.IsFalse(s1.TryCombineStatement(s2, new dummyOpt()), "combine with different recording");
        }

        [TestMethod]
        public void TestTCIdentical()
        {
            var index1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var s1 = new StatementRecordValue(index1, new ValSimple("i", typeof(int)), seen1, true);
            var s2 = new StatementRecordValue(index2, new ValSimple("i", typeof(int)), seen2, true);

            Assert.IsTrue(s1.TryCombineStatement(s2, new dummyOpt()), "combine with different recording");
        }

        [TestMethod]
        public void TestMultipleSavers()
        {
            var index1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var index2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var seen2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var index3 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var index4 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new StatementRecordValue(index1, new ValSimple("i", typeof(int)), seen1, true);
            s1.AddNewSaver(index3, new ValSimple("j", typeof(int)));
            var s2 = new StatementRecordValue(index2, new ValSimple("i", typeof(int)), seen2, true);
            s2.AddNewSaver(index4, new ValSimple("j", typeof(int)));

            var dop = new dummyOpt();
            Assert.IsTrue(s1.TryCombineStatement(s2, dop), "Combined 2 multi-saver guys");

            Assert.AreEqual(3, dop._renameRequests.Count, "# of rename requests");
            Assert.AreEqual(Tuple.Create(index2.RawValue, index1.RawValue), dop._renameRequests[0], "first rename");
            Assert.AreEqual(Tuple.Create(index4.RawValue, index3.RawValue), dop._renameRequests[1], "second rename");
        }

        class dummyOpt : ICodeOptimizationService
        {
            public List<Tuple<string, string>> _renameRequests = new List<Tuple<string, string>>();
            public bool TryRenameVarialbeOneLevelUp(string oldName, IDeclaredParameter newVariable)
            {
                _renameRequests.Add(Tuple.Create(oldName, newVariable.RawValue));
                return true;
            }

            public void ForceRenameVariable(string originalName, string newName)
            {
                throw new NotImplementedException();
            }
        }

    }
}
