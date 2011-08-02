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
    ///This is a test class for StatementRecordIndiciesTest and is intended
    ///to contain all StatementRecordIndiciesTest Unit Tests
    ///</summary>
    [TestClass]
    [PexClass(typeof(StatementRecordIndicies))]
    public partial class StatementRecordIndiciesTest
    {
        [TestInitialize]
        public void initTest()
        {
            TypeUtils._variableNameCounter = 0;
        }

        /// <summary>
        ///A test for StatementRecordIndicies Constructor
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public StatementRecordIndicies StatementRecordIndiciesConstructorTest(IValue intToRecord, IVariable storageArray)
        {
            StatementRecordIndicies target = new StatementRecordIndicies(intToRecord, storageArray);
            return target;
        }

        /// <summary>
        ///A test for CodeItUp
        ///</summary>
        [PexMethod]
        public string CodeItUpTest([PexAssumeUnderTest] StatementRecordIndicies target)
        {
            var actual = target.CodeItUp().ToArray();
            Assert.AreEqual(1, actual.Length, "only xpected one line");
            Assert.IsTrue(actual[0].Contains("push_back"), "push_back missing");
            return actual[0];
        }

        /// <summary>
        ///A test for RenameVariable
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public StatementRecordIndicies RenameVariableTest([PexAssumeUnderTest] StatementRecordIndicies target, string origin, string final)
        {
            target.RenameVariable(origin, final);
            return target;
        }

        /// <summary>
        ///A test for TryCombineStatement
        ///</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentException)), PexAllowedException(typeof(ArgumentNullException))]
        public bool TryCombineStatementTest([PexAssumeUnderTest] StatementRecordIndicies target, IStatement statement)
        {
            var result = target.TryCombineStatement(statement, null);

            if (statement == null)
                Assert.Fail("Null statement should have caused an exception");

            var allSame = target.CodeItUp().Zip(statement.CodeItUp(), (f, s) => f == s).All(t => t);
            Assert.AreEqual(allSame, result, "not expected combination!");

            return result;
        }
    }
}
