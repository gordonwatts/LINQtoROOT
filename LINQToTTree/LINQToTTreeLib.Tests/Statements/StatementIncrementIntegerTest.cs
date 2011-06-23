// <copyright file="StatementIncrementIntegerTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Statements
{
    /// <summary>This class contains parameterized unit tests for StatementIncrementInteger</summary>
    [PexClass(typeof(StatementIncrementInteger))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class StatementIncrementIntegerTest
    {
        [PexMethod]
        public StatementIncrementInteger Constructor(VarInteger i)
        {
            StatementIncrementInteger target = new StatementIncrementInteger(i);
            Assert.AreEqual(i, target.Integer, "initial value not set correctly");
            return null;
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public void TestEquiv([PexAssumeUnderTest] StatementIncrementInteger statement1, IStatement statement2)
        {
            LINQToTTreeLib.Tests.Statements.Utils.TestForEquiv(statement1, statement2);
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public IStatement TestRename([PexAssumeUnderTest] StatementIncrementInteger statement, string oldname, string newname)
        {
            return LINQToTTreeLib.Tests.Statements.Utils.TestRenameOfStatement(statement, oldname, newname);
        }
    }
}
