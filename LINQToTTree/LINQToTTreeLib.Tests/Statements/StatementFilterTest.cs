// <copyright file="StatementFilterTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LINQToTTreeLib.Variables;
using LINQToTTreeLib.Expressions;

namespace LINQToTTreeLib.Statements
{
    /// <summary>This class contains parameterized unit tests for StatementFilter</summary>
    [TestClass]
    public partial class StatementFilterTest
    {
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
            var val = new Variables.ValSimple("1 == 1", typeof(bool));
            var statement = new StatementFilter(val);
            statement.Add(new StatementSimpleStatement("dude"));

            var result = statement.CodeItUp().ToArray();
            Assert.AreEqual(4, result.Length, "no statements, so wasn't expecting any sort of output at all");
            Assert.AreEqual("if (1 == 1)", result[0], "if statement isn't correct");
            Assert.AreEqual("{", result[1], "open braket");
            Assert.AreEqual("  dude;", result[2], "statement isn't in the proper spot");
            Assert.AreEqual("}", result[3], "end of block not right");
        }

        [TestMethod]
        public void TestTrueExpr()
        {
            var val = new Variables.ValSimple("true", typeof(bool));
            var statement = new StatementFilter(val);
            statement.Add(new StatementSimpleStatement("dude"));

            var result = statement.CodeItUp().ToArray();
            Assert.AreEqual(3, result.Length, "true test means only the involved statements should be in here!");
            Assert.AreEqual("{", result[0], "open braket");
            Assert.AreEqual("  dude;", result[1], "statement isn't in the proper spot");
            Assert.AreEqual("}", result[2], "end of block not right");
        }

        [TestMethod]
        public void TestFalseExpr()
        {
            var val = new Variables.ValSimple("false", typeof(bool));
            var statement = new StatementFilter(val);
            statement.Add(new StatementSimpleStatement("dude"));

            var result = statement.CodeItUp().ToArray();
            Assert.AreEqual(0, result.Length, "Expect no statements for a false if statement");
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

        [TestMethod]
        public void CombineFilterWithHiddenBehindIf()
        {
            // Seen in the wild. We have two identical fi statements, one outside, and one inside another
            // (different) if statement. It is ok to combine these two as the code is identical.
            // See test CombineFilterWithHiddenBehindIfAndExtraStatements for the case where at
            // least one statement needs to be left behind.

            // Top level guy. This is the unique filter statement.
            var filterUnique = new StatementFilter(new ValSimple("fUnique", typeof(bool)));

            // Next, we will do the two common ones.
            var f1 = new StatementFilter(new ValSimple("f1", typeof(bool)));
            var f2 = new StatementFilter(new ValSimple("f1", typeof(bool)));

            var p = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var a1 = new StatementAssign(p, new ValSimple("5", typeof(int)), new IDeclaredParameter[] { }, true);
            var a2 = new StatementAssign(p, new ValSimple("5", typeof(int)), new IDeclaredParameter[] { }, true);
            f1.Add(a1);
            f2.Add(a2);

            filterUnique.Add(f1);

            Assert.IsTrue(f1.TryCombineStatement(f2, null), "Two of the same if statements, and the combine should have worked");
            Assert.AreEqual(1, f1.Statements.Count());
            Assert.AreEqual(0, f2.Statements.Count());
        }

        [TestMethod]
        public void CombineFilterWithHiddenBehindIfAndExtraStatements()
        {
            // Seen in the wild. We have two identical fi statements, one outside, and one inside another
            // (different) if statement. It is ok to combine these two as the code is identical.
            // See test CombineFilterWithHiddenBehindIfAndExtraStatements for the case where at
            // least one statement needs to be left behind.

            // Top level guy. This is the unique filter statement.
            var filterUnique = new StatementFilter(new ValSimple("fUnique", typeof(bool)));

            // Next, we will do the two common ones.
            var f1 = new StatementFilter(new ValSimple("f1", typeof(bool)));
            var f2 = new StatementFilter(new ValSimple("f1", typeof(bool)));

            var p = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var a1 = new StatementAssign(p, new ValSimple("5", typeof(int)), new IDeclaredParameter[] { }, true);
            var a2 = new StatementAssign(p, new ValSimple("5", typeof(int)), new IDeclaredParameter[] { }, true);
            f1.Add(a1);
            f2.Add(a2);

            // Now, a unique assignment. This can't be lifted b.c. it is hidden behind a different if statement in
            // the outside (the filterUnique).

            var pSpecial = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var aUnique = new StatementAssign(pSpecial, new ValSimple("10", typeof(int)), new IDeclaredParameter[] { }, true);
            f1.Add(aUnique);

            filterUnique.Add(f1);

            // The combine should fail.
            Assert.IsFalse(f2.TryCombineStatement(f1, null), "The two are different if statements, so it should have failed");

            // But some statements should have been moved! (note that f1 normally has two statements).
            Assert.AreEqual(1, f1.Statements.Count());
            Assert.AreEqual(1, f2.Statements.Count());
        }

        [TestMethod]
        public void DeclarationsAreIgnoredDuringLowerLevelMove()
        {
            // In this new world of moving things around, we move decl and statements, but they aren't really connected.
            // So we should make sure that decl aren't moved accidentally when they shouldn't be.

            // Inline block at the top
            var topLevel = new StatementInlineBlock();

            // Top level guy. This is the unique filter statement.
            var filterUnique = new StatementFilter(new ValSimple("fUnique", typeof(bool)));
            topLevel.Add(filterUnique);

            // Next, we will do the two common ones.
            var f1 = new StatementFilter(new ValSimple("f1", typeof(bool)));
            var f2 = new StatementFilter(new ValSimple("f1", typeof(bool)));

            var p = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            topLevel.Add(p);
            var a1 = new StatementAssign(p, new ValSimple("5", typeof(int)), new IDeclaredParameter[] { }, false);
            var a2 = new StatementAssign(p, new ValSimple("5", typeof(int)), new IDeclaredParameter[] { }, false);
            f1.Add(a1);
            f2.Add(a2);

            filterUnique.Add(f1);
            topLevel.Add(f2);

            Assert.IsTrue(f1.TryCombineStatement(f2, null), "Two of the same if statements, and the combine should have worked");
            Assert.AreEqual(1, f1.Statements.Count());
            Assert.AreEqual(0, f2.Statements.Count());
        }

#if false
        // This is a bug, but not a bug that should matter at all. So lets see how far we can get with
        // ignoring it.
        [TestMethod]
        public void DeclarationsAreMovedCorrectlyWhenStatementsReassigned()
        {
            // In this new world of moving things around, we move decl and statements, but they aren't really connected.
            // So we should make sure that decl aren't moved accidentally when they shouldn't be.

            // Inline block at the top
            var topLevel = new StatementInlineBlock();

            // Top level guy. This is the unique filter statement.
            var filterUnique = new StatementFilter(new ValSimple("fUnique", typeof(bool)));
            topLevel.Add(filterUnique);

            // Next, we will do the two common ones.
            var f1 = new StatementFilter(new ValSimple("f1", typeof(bool)));
            var f2 = new StatementFilter(new ValSimple("f1", typeof(bool)));

            var p1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var p2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            filterUnique.Add(p1);
            topLevel.Add(p2);
            var a1 = new StatementAssign(p1, new ValSimple("5", typeof(int)), new IDeclaredParameter[] { }, false);
            var a2 = new StatementAssign(p2, new ValSimple("5", typeof(int)), new IDeclaredParameter[] { }, false);
            f1.Add(a1);
            f2.Add(a2);

            filterUnique.Add(f1);
            topLevel.Add(f2);

            Console.WriteLine("Before optimization:");
            foreach (var l in topLevel.CodeItUp())
            {
                Console.WriteLine(l);
            }

            Assert.IsTrue(f2.TryCombineStatement(f1, null), "Two of the same if statements, and the combine should have worked");

            Console.WriteLine("After optimization:");
            foreach (var l in topLevel.CodeItUp())
            {
                Console.WriteLine(l);
            }
            Assert.AreEqual(0, f1.Statements.Count());
            Assert.AreEqual(1, f2.Statements.Count());

            Assert.AreEqual(0, filterUnique.DeclaredVariables.Count(), "Declared Variables");
        }
#endif
    }
}
