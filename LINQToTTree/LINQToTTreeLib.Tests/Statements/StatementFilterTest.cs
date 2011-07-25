// <copyright file="StatementFilterTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Statements
{
    /// <summary>This class contains parameterized unit tests for StatementFilter</summary>
    [PexClass(typeof(StatementFilter))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class StatementFilterTest
    {
        /// <summary>Test stub for CodeItUp()</summary>
        [PexMethod]
        public IEnumerable<string> CodeItUp([PexAssumeUnderTest]StatementFilter target)
        {
            IEnumerable<string> result = target.CodeItUp();
            return result;
            // TODO: add assertions to method StatementFilterTest.CodeItUp(StatementFilter)
        }

        /// <summary>Test stub for .ctor(IValue)</summary>
        [PexMethod]
        public StatementFilter Constructor(IValue testExpression)
        {
            StatementFilter target = new StatementFilter(testExpression);
            return target;
            // TODO: add assertions to method StatementFilterTest.Constructor(IValue)
        }

        [TestMethod]
        public void TestExprNoStatements()
        {
            var val = new Variables.ValSimple("true", typeof(bool));
            var statement = new StatementFilter(val);

            var result = statement.CodeItUp().ToArray();
            Assert.AreEqual(0, result.Length, "no statements, so wasn't expecting any sort of output at all");
        }

        [TestMethod]
        public void TestExprWithStatement()
        {
            var val = new Variables.ValSimple("true", typeof(bool));
            var statement = new StatementFilter(val);
            statement.Add(new StatementSimpleStatement("dude"));

            var result = statement.CodeItUp().ToArray();
            Assert.AreEqual(4, result.Length, "no statements, so wasn't expecting any sort of output at all");
            Assert.AreEqual("if (true)", result[0], "if statement isn't correct");
            Assert.AreEqual("{", result[1], "open braket");
            Assert.AreEqual("  dude;", result[2], "statement isn't in the proper spot");
            Assert.AreEqual("}", result[3], "end of block not right");
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public void TestEquiv([PexAssumeUnderTest] StatementFilter statement1, IStatement statement2)
        {
            var result = statement1.IsSameStatement(statement2);

            var originalLines = statement1.CodeItUp().ToArray();
            var resultinglines = statement2.CodeItUp().ToArray();

            if (resultinglines.Length != originalLines.Length)
            {
                Assert.IsFalse(result, "# of lines is different, so the compare should be too");
                return;
            }

            var pairedLines = originalLines.Zip(resultinglines, (o1, o2) => Tuple.Create(o1, o2));
            foreach (var pair in pairedLines)
            {
                if (pair.Item1 != pair.Item2)
                {
                    Assert.IsFalse(result, string.Format("Line '{0}' and '{1}' are not same!", pair.Item1, pair.Item2));
                }
                else
                {
                    Assert.IsTrue(result, string.Format("Line '{0}' and '{1}' are not same!", pair.Item1, pair.Item2));
                }
            }
        }

        [PexMethod]
        public void TestTryCombine(IStatement s)
        {
            /// We should never be able to combine any filter statements currently!

            var val = new Variables.ValSimple("true", typeof(bool));
            var statement = new StatementFilter(val);

            Assert.IsFalse(statement.TryCombineStatement(s, null), "unable to do any combines for Filter");
        }

        [TestMethod]
        public void TestSimpleCombine()
        {
            var val1 = new Variables.ValSimple("true", typeof(bool));
            var s1 = new StatementFilter(val1);
            s1.Add(new StatementSimpleStatement("var1"));

            var val2 = new Variables.ValSimple("true", typeof(bool));
            var s2 = new StatementFilter(val2);
            s2.Add(new StatementSimpleStatement("var2"));

            Assert.IsTrue(s1.TryCombineStatement(s2, null), "statement shoudl have combined");
            Assert.AreEqual(2, s1.Statements.Count(), "# of combined statements");

        }

        [TestMethod]
        public void TestSecondLevelCombine()
        {
            var val1 = new Variables.ValSimple("true", typeof(bool));
            var s1 = new StatementFilter(val1);

            var val11 = new Variables.ValSimple("true", typeof(bool));
            var s11 = new StatementFilter(val11);
            s11.Add(new StatementSimpleStatement("var11"));

            s1.Add(s11);

            var val2 = new Variables.ValSimple("true", typeof(bool));
            var s2 = new StatementFilter(val2);

            var val21 = new Variables.ValSimple("true", typeof(bool));
            var s21 = new StatementFilter(val21);
            s21.Add(new StatementSimpleStatement("var21"));

            s2.Add(s21);

            Assert.IsTrue(s1.TryCombineStatement(s2, null), "statement shoudl have combined");
            Assert.AreEqual(1, s1.Statements.Count(), "# of combined statements");
            var deep = s1.Statements.First() as StatementInlineBlockBase;
            Assert.IsNotNull(deep, "couldn't find interior statement");
            Assert.AreEqual(2, deep.Statements.Count(), "Number of statements isn't right here");
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public IStatement TestRename([PexAssumeUnderTest] StatementFilter statement, [PexAssumeNotNull] string oldname, [PexAssumeNotNull]string newname)
        {
            var origianllines = statement.CodeItUp().ToArray();
            statement.RenameVariable(oldname, newname);
            var finallines = statement.CodeItUp().ToArray();

            Assert.AreEqual(origianllines.Length, finallines.Length, "# of lines change during variable rename");

            var varReplacer = new Regex(string.Format(@"\b{0}\b", oldname));

            var sharedlines = origianllines.Zip(finallines, (o, n) => Tuple.Create(o, n));
            foreach (var pair in sharedlines)
            {
                var orig = pair.Item1;
                var origReplafce = varReplacer.Replace(orig, newname);
                Assert.AreEqual(origReplafce, pair.Item2, "expected the renaming to be pretty simple.");
            }

            return statement;
        }
    }
}
