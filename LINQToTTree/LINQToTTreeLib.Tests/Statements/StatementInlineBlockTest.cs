using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Variables;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Using;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// <copyright file="StatementInlineBlockTest.cs" company="Microsoft">Copyright � Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;

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
            Assert.AreEqual(target, statement.Parent, "Parent not set correctly");
        }

        [PexMethod]
        [PexUseType(typeof(StatementIncrementInteger))]
        public void Add(
            [PexAssumeUnderTest]StatementInlineBlock target,
            IDeclaredParameter var
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
        [ExpectedException(typeof(ArgumentException))]
        public void TestBadRemove()
        {
            var s = new StatementInlineBlock();
            var tmp = new StatementSimpleStatement("fork");
            s.Remove(tmp);
        }

        [TestMethod]
        public void TestRemoveSingleStatement()
        {
            var s = new StatementInlineBlock();
            var s1 = new StatementSimpleStatement("one");
            var s2 = new StatementSimpleStatement("two");
            s.Add(s1);
            s.Add(s2);

            s.Remove(s1);
            Assert.AreEqual(1, s.Statements.Count(), "# of statements after remove");
            Assert.AreEqual(s2, s.Statements.First(), "First statement");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddBeforeBad()
        {
            var s = new StatementInlineBlock();
            var s1 = new StatementSimpleStatement("one");
            var s2 = new StatementSimpleStatement("two");
            s.AddBefore(s1, s2);
        }

        [TestMethod]
        public void TestAddBefore()
        {
            var s = new StatementInlineBlock();
            var s1 = new StatementSimpleStatement("one");
            var s2 = new StatementSimpleStatement("two");
            s.Add(s2);
            s.AddBefore(s1, s2);

            Assert.AreEqual(2, s.Statements.Count(), "# of statements");
            Assert.AreEqual(s1, s.Statements.First(), "first statement");
            Assert.AreEqual(s2, s.Statements.Skip(1).First(), "second statement");
        }

        [TestMethod]
        public void TestAddBeforeWithAnother()
        {
            var s = new StatementInlineBlock();
            var s1 = new StatementSimpleStatement("one");
            var s2 = new StatementSimpleStatement("two");
            var s3 = new StatementSimpleStatement("three");
            s.Add(s2);
            s.Add(s3);
            s.AddBefore(s1, s3);

            Assert.AreEqual(3, s.Statements.Count(), "# of statements");
            Assert.AreEqual(s2, s.Statements.First(), "first statement");
            Assert.AreEqual(s1, s.Statements.Skip(1).First(), "second statement");
            Assert.AreEqual(s3, s.Statements.Skip(2).First(), "third statement");
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
            b.Add(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));
            b.Add(new Statements.StatementSimpleStatement("bork"));
            var r = b.CodeItUp().ToArray();
            Assert.AreEqual(4, r.Length, "incorrect number of lines");
            Assert.AreEqual("{", r[0], "open bracket");
            Assert.AreEqual("}", r[3], "close bracket");
            Assert.IsTrue(r[1].EndsWith("=0;"));
        }

        [TestMethod]
        public void TestSimpleVariableCodingNoDecl()
        {
            // No statements - so there should be no declares.
            StatementInlineBlock b = new StatementInlineBlock();
            b.Add(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));
            var r = b.CodeItUp().ToArray();
            Assert.AreEqual(0, r.Length, "# of statements");
        }

        [TestMethod]
        public void TestSimpleVariableCodingNoDeclAndDecl()
        {
            StatementInlineBlock b = new StatementInlineBlock();
            b.Add(DeclarableParameter.CreateDeclarableParameterExpression(typeof(int)));
            b.Add(new Statements.StatementSimpleStatement("bork"));
            var r = b.CodeItUp().ToArray();
            Assert.AreEqual(4, r.Length, "# of statements");
        }

        /// <summary>
        /// Help with the next test
        /// </summary>
        public class dummyVarName : IDeclaredParameter
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
            }

            public string ParameterName
            {
                get { return RawValue; }
            }

            IValue IDeclaredParameter.InitialValue
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public void RenameParameter(string oldname, string newname)
            {
                return;
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

        class DummyOptimizer : ICodeOptimizationService
        {
            public bool TryRenameVarialbeOneLevelUp(string oldName, IDeclaredParameter newVariable)
            {
                throw new NotImplementedException();
            }


            public void ForceRenameVariable(string originalName, string newName)
            {
                throw new NotImplementedException();
            }
        }


        [TestMethod]
        public void TestSimpleBlockCombine()
        {
            /// Combine two statements in a single block. Make sure that
            IStatement b1 = new StatementSimpleStatement("int");
            IStatement b2 = new StatementSimpleStatement("dude");

            var b = new StatementInlineBlock();
            Assert.IsTrue(b.TryCombineStatement(b1, new DummyOptimizer()), "should always be able to add extra statements");
            Assert.IsTrue(b.TryCombineStatement(b2, new DummyOptimizer()), "should always be able to add another extra statement");

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

                var inlineblock = s as StatementInlineBlock;
                var goodInfo = CountDownlevelStatements(inlineblock);

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

        /// <summary>
        /// Recurisvely count the # of good statements.
        /// </summary>
        /// <param name="statementInlineBlock"></param>
        /// <returns></returns>
        private Tuple<int, int> CountDownlevelStatements(StatementInlineBlock statementInlineBlock)
        {
            var varCount = statementInlineBlock.DeclaredVariables.Count();
            return Tuple.Create(statementInlineBlock.Statements.Count(), varCount);
        }

        /// <summary>
        /// Helper class to force a rename
        /// </summary>
        class CombineTestStatement : IStatement
        {
            private IDeclaredParameter vdecl2;

            public CombineTestStatement(IDeclaredParameter vdecl2)
            {
                // TODO: Complete member initialization
                this.vdecl2 = vdecl2;
            }
            public IEnumerable<string> CodeItUp()
            {
                throw new NotImplementedException();
            }

            public void RenameVariable(string originalName, string newName)
            {
                return;
            }

            public bool TryCombineStatement(IStatement statement, ICodeOptimizationService optimize)
            {
                var other = statement as CombineTestStatement;
                if (other == null)
                    return false;
                return optimize.TryRenameVarialbeOneLevelUp(other.vdecl2.ParameterName, vdecl2);
            }


            public IStatement Parent { get; set; }
        }

        [TestMethod]
        public void TestCombineWithRenameSimple()
        {
            // Try to combine two statements that will combine, but require
            // a rename first.

            var inline1 = new StatementInlineBlock();
            var inline2 = new StatementInlineBlock();

            var vdecl1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var vdecl2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            inline1.Add(vdecl1);
            inline2.Add(vdecl2);

            var s1 = new CombineTestStatement(vdecl1);
            inline1.Add(s1);
            var s2 = new CombineTestStatement(vdecl2);
            inline2.Add(s2);

            var result = inline1.TryCombineStatement(inline2, null);
            Assert.IsTrue(result, "try combine didn't work");
            Assert.AreEqual(1, inline1.Statements.Count(), "bad # of combined statements");
        }

        [TestMethod]
        public void TestCombineWithRenameDownstream()
        {
            // When doing a good rename, make sure downstream statements get the rename too.

            var inline1 = new StatementInlineBlock();
            var inline2 = new StatementInlineBlock();

            var vdecl1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var vdecl2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            inline1.Add(vdecl1);
            inline2.Add(vdecl2);

            var s1 = new CombineTestStatement(vdecl1);
            inline1.Add(s1);
            var s2 = new CombineTestStatement(vdecl2);
            inline2.Add(s2);
            inline2.Add(new Statements.StatementSimpleStatement(string.Format("dude = {0}", vdecl2.ParameterName)));

            var result = inline1.TryCombineStatement(inline2, null);
            Assert.IsTrue(result, "try combine didn't work");
            Assert.AreEqual(2, inline1.Statements.Count(), "bad # of combined statements");
            Assert.AreEqual(string.Format("dude = {0};", vdecl1.ParameterName), inline1.Statements.Skip(1).First().CodeItUp().First(), "Line wasn't renamed");
        }

        [TestMethod]
        public void TestCombineWithRenameAtDifferentLevels()
        {
            // Try to combine two statements that look like they should combine,
            // but one variable is declared at a different "level" than the other
            // up the hierarchy.

            var inline1 = new StatementInlineBlock();
            var inline11 = new StatementInlineBlock();
            inline1.Add(inline11);
            var inline2 = new StatementInlineBlock();
            var inline22 = new StatementInlineBlock();
            inline2.Add(inline22);

            var vdecl1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var vdecl2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            inline1.Add(vdecl1);
            inline22.Add(vdecl2);

            var s1 = new CombineTestStatement(vdecl1);
            inline11.Add(s1);
            var s2 = new CombineTestStatement(vdecl2);
            inline22.Add(s2);

            var result = inline1.TryCombineStatement(inline2, null);
            Assert.IsTrue(result, "try combine should go ok");
            Assert.AreEqual(1, inline1.Statements.Count(), "# statements inside inline 1");
            var innerBlock = inline1.Statements.First() as IBookingStatementBlock;
            Assert.IsNotNull(innerBlock, "inner block a booking statement");
            Assert.AreEqual(2, innerBlock.Statements.Count(), "Statements in inner block should not have combined");
        }

        [TestMethod]
        public void TestCombineWithRenameVarsDifferent()
        {
            // If the varialbes are initialized differently, then we can't combine them!

            var inline1 = new StatementInlineBlock();
            var inline2 = new StatementInlineBlock();

            var vdecl1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            vdecl1.SetInitialValue("0");
            var vdecl2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            vdecl2.SetInitialValue("1");

            inline1.Add(vdecl1);
            inline2.Add(vdecl2);

            var s1 = new CombineTestStatement(vdecl1);
            inline1.Add(s1);
            var s2 = new CombineTestStatement(vdecl2);
            inline2.Add(s2);
            inline2.Add(new Statements.StatementSimpleStatement(string.Format("dude = {0}", vdecl2.RawValue)));

            var result = inline1.TryCombineStatement(inline2, null);
            Assert.IsTrue(result, "try combine didn't work");
            Assert.AreEqual(3, inline1.Statements.Count(), "bad # of combined statements");
        }

        [TestMethod]
        public void TestCombineWithRenameVarsNotDecl()
        {
            // If one of the variables isn't declared, then this is a "result" and it shouldn't
            // be combined (or similar - whatever, it is outside the block). So we can't
            // do the combine for now!

            var inline1 = new StatementInlineBlock();
            var inline2 = new StatementInlineBlock();

            var vdecl1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var vdecl2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            inline1.Add(vdecl1);

            var s1 = new CombineTestStatement(vdecl1);
            inline1.Add(s1);
            var s2 = new CombineTestStatement(vdecl2);
            inline2.Add(s2);
            inline2.Add(new Statements.StatementSimpleStatement(string.Format("dude = {0}", vdecl2.RawValue)));

            var result = inline1.TryCombineStatement(inline2, null);
            Assert.IsTrue(result, "try combine didn't work");
            Assert.AreEqual(3, inline1.Statements.Count(), "bad # of combined statements");
        }

        [TestMethod]
        public void TestCombineWithRenameVarsNotDeclR()
        {
            // If one of the variables isn't declared, then this is a "result" and it shouldn't
            // be combined (or similar - whatever, it is outside the block). So we can't
            // do the combine for now!
            // This is the same guy as above - but in reverse order. This is important because
            // this test needs to be symetric.

            var inline1 = new StatementInlineBlock();
            var inline2 = new StatementInlineBlock();

            var vdecl1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var vdecl2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));

            inline1.Add(vdecl1);

            var s1 = new CombineTestStatement(vdecl1);
            inline1.Add(s1);
            var s2 = new CombineTestStatement(vdecl2);
            inline2.Add(s2);
            inline2.Add(new Statements.StatementSimpleStatement(string.Format("dude = {0}", vdecl2.RawValue)));

            var result = inline2.TryCombineStatement(inline1, null);
            Assert.IsTrue(result, "try combine didn't work");
            Assert.AreEqual(3, inline2.Statements.Count(), "bad # of combined statements");
        }

        /// <summary>
        /// Found out in the wild. Make sure when we do the combination that we don't accidentally
        /// move a block ahead of another block that has altered some dependent variable!
        /// </summary>
        [TestMethod]
        public void TestCombineWithAlteredValue()
        {
            // This variable will be modified in an assignment statement.
            var varToBeModified = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var statementModifier = new StatementAssign(varToBeModified, new ValSimple("1", typeof(int)), new IDeclaredParameter[] { });

            // Next, we access this variable in an if statement.
            var finalVar = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var assignment = new StatementAssign(finalVar, varToBeModified, new IDeclaredParameter[] { varToBeModified });
            var checkVar = DeclarableParameter.CreateDeclarableParameterExpression(typeof(bool));
            var ifUsesModifiedValue = new StatementFilter(new ValSimple(checkVar.RawValue, typeof(bool)));
            ifUsesModifiedValue.Add(assignment);

            var ifNoUsesModifiedValue = new StatementFilter(new ValSimple(checkVar.RawValue, typeof(bool)));

            // Ok, now create the two sets of top level statements.

            var blockWithModified = new StatementInlineBlock();
            var blockWithoutModified = new StatementInlineBlock();

            blockWithModified.Add(varToBeModified);
            blockWithModified.Add(finalVar);
            blockWithModified.Add(statementModifier);

            blockWithModified.Add(checkVar);
            blockWithoutModified.Add(checkVar);

            blockWithModified.Add(ifUsesModifiedValue);
            blockWithoutModified.Add(ifNoUsesModifiedValue);

            // Combine

            var r = blockWithoutModified.TryCombineStatement(blockWithModified, null);
            Assert.IsTrue(r, "try combine result");

            foreach (var s in blockWithoutModified.CodeItUp())
            {
                System.Diagnostics.Trace.WriteLine(s);
            }

            // Make sure the checkVar guy comes after the modified statement.

            var topLevelStatementForAssign = findStatementThatContains(blockWithoutModified, assignment);
            var posOfUse = findStatementIndex(blockWithoutModified, topLevelStatementForAssign);

            var posOfMod = findStatementIndex(blockWithoutModified, statementModifier);

            Assert.IsTrue(posOfMod < posOfUse, string.Format("Modification happens after use. modification: {0} use {1}", posOfMod, posOfUse));
        }

        /// <summary>
        /// Given a statement in an compound block, return the order it is. Or throw if we can't find it.
        /// </summary>
        /// <param name="blockWithoutModified"></param>
        /// <param name="statement"></param>
        /// <returns></returns>
        private int findStatementIndex(IStatementCompound block, IStatement statement)
        {
            var r = from s in block.Statements.Zip(Enumerable.Range(1, 5000), (st, cnt) => Tuple.Create(st, cnt))
                    where s.Item1 == statement
                    select s;
            var match = r.FirstOrDefault();
            if (match == null)
                throw new ArgumentException("Unable to find the statement in the sequence");

            return match.Item2;
        }

        /// <summary>
        /// Find a statement that may be inside a compound statement - return the top level statement that
        /// contains it. Throw if we can't find it.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="statement"></param>
        /// <returns></returns>
        private IStatement findStatementThatContains(IStatementCompound block, IStatement statement)
        {
            var m = block.Statements.Where(s => ContainsStatement(s, statement)).FirstOrDefault();
            if (m == null)
                throw new ArgumentException("Unable to find the requested statement in the block");

            return m;
        }

        /// <summary>
        /// Returns true if the statement is inside the block.
        /// </summary>
        /// <param name="statementCompound"></param>
        /// <param name="assignment"></param>
        /// <returns></returns>
        private bool ContainsStatement(IStatement sblock, IStatement statement)
        {
            if (sblock == statement)
                return true;

            var block = sblock as IStatementCompound;
            if (block == null)
                return false;

            return block.Statements.Where(s => ContainsStatement(s, statement)).Any();
        }

        [TestMethod]
        public void TestIsBefore()
        {
            var s = new StatementInlineBlock();

            var vdecl1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new Statements.StatementAssign(vdecl1, new ValSimple("fork", typeof(int)), null);
            var s2 = new Statements.StatementAssign(vdecl1, new ValSimple("fork", typeof(int)), null);
            s.Add(s1);
            s.Add(s2);

            Assert.IsTrue(s.IsBefore(s1, s2), "s1 before s2");
            Assert.IsFalse(s.IsBefore(s2, s1), "s2 is before s1");
        }
    }
}
