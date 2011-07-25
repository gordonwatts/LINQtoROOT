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
    ///This is a test class for TestStatementAnyAllDetector and is intended
    ///to contain all TestStatementAnyAllDetector Unit Tests
    ///</summary>
    [TestClass]
    [PexClass(typeof(StatementAnyAllDetector))]
    public partial class TestStatementAnyAllDetector
    {
        [TestInitialize]
        public void TestSetup()
        {
            TypeUtils._variableNameCounter = 0;
        }

        /// <summary>
        ///A test for CodeItUp
        ///</summary>
        [PexMethod]
        public string[] TestCodeItUp([PexAssumeUnderTest] StatementAnyAllDetector target)
        {
            var actual = target.CodeItUp().ToArray();
            return actual;
        }

        /// <summary>
        ///A test for RenameVariable
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public StatementAnyAllDetector TestRenameVariable([PexAssumeUnderTest] StatementAnyAllDetector target, string originalName, string newName)
        {
            target.RenameVariable(originalName, newName);
            return target;
        }

        /// <summary>
        ///A test for TryCombineStatement
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public bool TestTryCombineStatement([PexAssumeUnderTest] StatementAnyAllDetector target, IStatement statement, ICodeOptimizationService optimize)
        {
            var result = target.TryCombineStatement(statement, optimize);
            return result;
        }
    }
}
