using System;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Utils;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{
    /// <summary>
    ///This is a test class for TestStatemenRecordValue and is intended
    ///to contain all TestStatemenRecordValue Unit Tests
    ///</summary>
    [TestClass]
    [PexClass(typeof(StatementRecordValue))]
    public partial class TestStatemenRecordValue
    {
        [TestInitialize]
        public void TestInit()
        {
            TypeUtils._variableNameCounter = 0;
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
    }
}
