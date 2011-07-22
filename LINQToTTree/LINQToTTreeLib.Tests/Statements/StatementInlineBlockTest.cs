// <copyright file="StatementInlineBlockTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Using;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Statements
{
    /// <summary>This class contains parameterized unit tests for StatementInlineBlock</summary>
    [PexClass(typeof(StatementInlineBlock))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class StatementInlineBlockTest
    {
        /// <summary>Test stub for Add(IStatement)</summary>
        [PexMethod]
        [PexUseType(typeof(StatementIncrementInteger))]
        public void Add(
            [PexAssumeUnderTest]StatementInlineBlock target,
            IStatement statement
        )
        {
            var oldCount = target.Statements.Count();
            target.Add(statement);
            Assert.AreEqual(oldCount + 1, target.Statements.Count(), "Expected a statement to have been added");
            Assert.IsFalse(target.Statements.Any(s => s == null), "Should never add a null statement");
        }

        [PexMethod]
        [PexUseType(typeof(StatementIncrementInteger))]
        public void Add(
            [PexAssumeUnderTest]StatementInlineBlock target,
            IVariable var
        )
        {
            target.Add(var);
            Assert.AreEqual(1, target.DeclaredVariables.Count(), "Expected a statement to have been added");
            Assert.IsFalse(target.DeclaredVariables.Any(s => s == null), "Should never add a null statement");
        }

        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public StatementInlineBlock Constructor()
        {
            StatementInlineBlock target = new StatementInlineBlock();
            Assert.AreEqual(0, target.Statements.Count(), "Expected no statements after creation");
            return target;
        }

        /// <summary>Test stub for get_Statements()</summary>
        [PexMethod]
        public IEnumerable<IStatement> StatementsGet([PexAssumeUnderTest]StatementInlineBlock target)
        {
            IEnumerable<IStatement> result = target.Statements;
            return result;
        }

        [TestMethod]
        public void TestCodeItUp()
        {
            StatementInlineBlock b = new StatementInlineBlock();
            Assert.AreEqual(0, b.CodeItUp().Count(), "Expect nothing for an empty inline block");
        }

        [TestMethod]
        public void TestSimpleCodeing()
        {
            StatementInlineBlock b = new StatementInlineBlock();
            b.Add(new StatementSimpleStatement("junk;"));
            var r = b.CodeItUp().ToArray();
            Assert.AreEqual(3, r.Length, "incorrect number of lines");
            Assert.AreEqual("{", r[0], "open bracket");
            Assert.AreEqual("}", r[2], "close bracket");
            Assert.AreEqual("  junk;", r[1], "statement");
        }

        [TestMethod]
        public void TestSimpleVariableCoding()
        {
            StatementInlineBlock b = new StatementInlineBlock();
            b.Add(new VarInteger());
            var r = b.CodeItUp().ToArray();
            Assert.AreEqual(3, r.Length, "incorrect number of lines");
            Assert.AreEqual("{", r[0], "open bracket");
            Assert.AreEqual("}", r[2], "close bracket");
            Assert.IsTrue(r[1].EndsWith("=0;"));
        }

        [TestMethod]
        public void TestSimpleVariableCodingNoDecl()
        {
            StatementInlineBlock b = new StatementInlineBlock();
            b.Add(new VarInteger() { Declare = false });
            var r = b.CodeItUp().ToArray();
            Assert.AreEqual(0, r.Length, "# of statements");
        }

        [TestMethod]
        public void TestSimpleVariableCodingNoDeclAndDecl()
        {
            StatementInlineBlock b = new StatementInlineBlock();
            b.Add(new VarInteger() { Declare = false });
            b.Add(new VarInteger());
            var r = b.CodeItUp().ToArray();
            Assert.AreEqual(3, r.Length, "# of statements");
        }

        /// <summary>
        /// Help with the next test
        /// </summary>
        class dummyVarName : IVariable
        {
            public dummyVarName(string name, Type t)
            {
                VariableName = name;
                Type = t;
            }
            public string VariableName { get; set; }

            public IValue InitialValue { get; set; }

            public bool Declare { get; set; }

            public string RawValue { get; set; }

            public Type Type { get; set; }


            public void RenameRawValue(string oldname, string newname)
            {
                throw new NotImplementedException();
            }
        }

        [PexMethod, PexAllowedException(typeof(ArgumentException))]
        public void TestSimpleVariableDoubleInsert([PexAssumeNotNull] string name1, [PexAssumeNotNull] string name2)
        {
            if (string.IsNullOrWhiteSpace(name1) || string.IsNullOrWhiteSpace(name2))
                throw new ArgumentException("names must be something");

            StatementInlineBlock b = new StatementInlineBlock();
            var v1 = new dummyVarName(name1, typeof(int));
            var v2 = new dummyVarName(name2, typeof(int));

            b.Add(v1);
            b.Add(v2);

            Assert.AreEqual(2, b.DeclaredVariables.Count(), "incorrect number of variables");
        }

        [TestMethod]
        public void TestSimpleBlockCombine()
        {
            /// Combine two statements in a single block. Make sure that
            IStatement b1 = new StatementSimpleStatement("int");
            IStatement b2 = new StatementSimpleStatement("dude");

            var b = new StatementInlineBlock();
            Assert.IsTrue(b.TryCombineStatement(b1, null), "should always be able to add extra statements");
            Assert.IsTrue(b.TryCombineStatement(b2, null), "should always be able to add another extra statement");

            Assert.AreEqual(2, b.Statements.Count(), "expected both statements in there");
        }

        /// <summary>
        /// Make sure to test adding statements with inline blocks both the type that we can do and can't, and also
        /// simple single line statements.
        /// </summary>
        /// <param name="s"></param>
        [PexMethod]
        [PexUseType(typeof(StatementInlineBlock))]
        [PexUseType(typeof(StatementIncrementInteger))]
        [PexUseType(typeof(StatementIfOnCount))]
        public void TestAddSingleStatement(IStatement s)
        {
            var b = new StatementInlineBlock();
            Assert.IsTrue(b.TryCombineStatement(s, null), "Failed to add statement");

            ///
            /// Now check...
            /// 

            if (s.GetType() == typeof(StatementInlineBlock))
            {
                // This is a little tricky as we have to go pretty deep to figure out what
                // what are "good" and bad statements for counting. 

                var goodInfo = CountInterestingStatements(s as StatementInlineBlock);
                Assert.AreEqual(goodInfo.Item1, b.Statements.Count(), "# of statements");
                Assert.AreEqual(goodInfo.Item2, b.DeclaredVariables.Count(), "# of declared variables");
            }
            else
            {
                /// Just append!

                Assert.AreEqual(1, b.Statements.Count(), "# of statements");
                Assert.AreEqual(0, b.DeclaredVariables.Count(), "# of declared variables");
            }
        }

        /// <summary>
        /// Recurisvely count the # of good statements.
        /// </summary>
        /// <param name="statementInlineBlock"></param>
        /// <returns></returns>
        private Tuple<int, int> CountInterestingStatements(StatementInlineBlock statementInlineBlock)
        {
            var varCount = statementInlineBlock.DeclaredVariables.Count();

            if (varCount > 0)
            {
                // need to keep the way we declare variables here, so don't go any deeper!
                return Tuple.Create(varCount, statementInlineBlock.Statements.Count());
            }

            // Ok, we can lift the statements by one, since this wrapper is basically "empty".

            int statementCount = 0;
            foreach (var s in statementInlineBlock.Statements)
            {
                if (s.GetType() == typeof(StatementInlineBlock))
                {
                    var tr = CountInterestingStatements(s as StatementInlineBlock);
                    statementCount += tr.Item1;
                    varCount += tr.Item2;
                }
                else
                {
                    statementCount++;
                }
            }

            return Tuple.Create(statementCount, varCount);
        }
    }
}
