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
            TypeUtils._variableNameCounter = 0;
        }

        /// <summary>
        ///A test for StatementAssign Constructor
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public StatementAssign StatementAssignConstructorTest(IVariable dest, IValue source)
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
    }
}
