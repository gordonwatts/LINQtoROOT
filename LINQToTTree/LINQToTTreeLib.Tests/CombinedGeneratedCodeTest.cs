using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.QMFunctions;
using LINQToTTreeLib.Statements;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.Tests.QMFunctions;
using LINQToTTreeLib.Variables;
using Microsoft.ExtendedReflection.DataAccess;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Using;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq;
using Remotion.Linq.Clauses;
// <copyright file="CombinedGeneratedCodeTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for CombinedGeneratedCode</summary>
    [PexClass(typeof(CombinedGeneratedCode))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class CombinedGeneratedCodeTest
    {
        /// <summary>Test stub for AddGeneratedCode(IGeneratedQueryCode)</summary>
        [PexMethod]
        [PexUseType(typeof(GeneratedCode)), PexAllowedException(typeof(ArgumentException)), PexAllowedException(typeof(TermDestructionException)), PexAllowedException(typeof(ArgumentNullException))]
        internal void AddGeneratedCode(
            [PexAssumeUnderTest]CombinedGeneratedCode target,
            [PexAssumeNotNull] IExecutableCode code
        )
        {
            int initialQueryCount = target.QueryCode().Count();
            target.AddGeneratedCode(code);
            Assert.AreEqual(initialQueryCount + 1, target.QueryCode().Count(), "Should always increase the query count by one");
        }

        [PexMethod, PexAllowedException(typeof(ArgumentException)), PexAllowedException(typeof(ArgumentNullException))]
        internal void CheckAddSameVariableNamesToTransfer([PexAssumeNotNull] ROOTNET.NTObject[] varnames)
        {
            var cc = new CombinedGeneratedCode();
            HashSet<string> unique = new HashSet<string>();
            foreach (var item in varnames)
            {
                var gc = new GeneratedCode();
                var name = gc.QueueForTransfer(item);
                unique.Add(name);
                cc.AddGeneratedCode(gc);
            }

            Assert.AreEqual(unique.Count, varnames.Length, "there are non-unique names and there are no errors!");
            Assert.AreEqual(varnames.Length, cc.VariablesToTransfer.Count(), "bad number added");
        }

        [PexMethod]
        internal void CheckAddIncludeFiles([PexAssumeNotNull] string[] incnames)
        {
            var cc = new CombinedGeneratedCode();
            foreach (var item in incnames)
            {
                var gc = new GeneratedCode();
                gc.AddIncludeFile(item);
                cc.AddGeneratedCode(gc);
            }

            HashSet<string> unique = new HashSet<string>(incnames);
            Assert.AreEqual(unique.Count, cc.IncludeFiles.Count(), "wrong # of unqiue names");
        }

        /// <summary>Test stub for AddIncludeFile(String)</summary>
        [PexMethod]
        internal void AddIncludeFile(
            [PexAssumeUnderTest]CombinedGeneratedCode target,
            string includeName
        )
        {
            HashSet<string> hs = new HashSet<string>(target.IncludeFiles);
            target.AddIncludeFile(includeName);
            Assert.IsFalse(string.IsNullOrWhiteSpace(includeName), "bad include file was successfully added");
            hs.Add(includeName);
            Assert.AreEqual(hs.Count, target.IncludeFiles.Count(), "incorrect # of include files");
        }

        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        internal CombinedGeneratedCode Constructor()
        {
            CombinedGeneratedCode target = new CombinedGeneratedCode();
            return target;
            // TODO: add assertions to method CombinedGeneratedCodeTest.Constructor()
        }

        /// <summary>Test stub for get_IncludeFiles()</summary>
        [PexMethod]
        internal IEnumerable<string> IncludeFilesGet([PexAssumeUnderTest]CombinedGeneratedCode target)
        {
            IEnumerable<string> result = target.IncludeFiles;
            return result;
            // TODO: add assertions to method CombinedGeneratedCodeTest.IncludeFilesGet(CombinedGeneratedCode)
        }

        /// <summary>Test stub for get_ResultValues()</summary>
        [PexMethod]
        internal IEnumerable<IDeclaredParameter> ResultValuesGet([PexAssumeUnderTest]CombinedGeneratedCode target)
        {
            IEnumerable<IDeclaredParameter> result = target.ResultValues;
            return result;
            // TODO: add assertions to method CombinedGeneratedCodeTest.ResultValuesGet(CombinedGeneratedCode)
        }

        /// <summary>Test stub for get_VariablesToTransfer()</summary>
        [PexMethod]
        internal IEnumerable<KeyValuePair<string, object>> VariablesToTransferGet([PexAssumeUnderTest]CombinedGeneratedCode target)
        {
            IEnumerable<KeyValuePair<string, object>> result = target.VariablesToTransfer;
            return result;
            // TODO: add assertions to method CombinedGeneratedCodeTest.VariablesToTransferGet(CombinedGeneratedCode)
        }

        /// <summary>
        /// Explicit test to see if the combining works correctly.
        /// </summary>
        [TestMethod]
        public void TestSimpleCombine()
        {
            var q1 = new GeneratedCode();
            var q2 = new GeneratedCode();

            var s1 = new Statements.StatementSimpleStatement("dude1");
            var s2 = new Statements.StatementSimpleStatement("dude2");

            q1.Add(s1);
            q2.Add(s2);

            var v1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var v2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            q1.SetResult(v1);
            q2.SetResult(v2);

            var target = new CombinedGeneratedCode();
            target.AddGeneratedCode(q1);
            target.AddGeneratedCode(q2);

            Assert.AreEqual(1, target.QueryCode().Count(), "didn't combine blocks correctly");
            var c = target.QueryCode().First();
            Assert.AreEqual(2, c.Statements.Count(), "bad # of statements in combined query");
            var st1 = c.Statements.First();
            var st2 = c.Statements.Skip(1).First();
            Assert.IsInstanceOfType(st1, typeof(Statements.StatementSimpleStatement), "st1");
            Assert.IsInstanceOfType(st2, typeof(Statements.StatementSimpleStatement), "st2");

            var sst1 = st1 as Statements.StatementSimpleStatement;
            var sst2 = st2 as Statements.StatementSimpleStatement;
            Assert.IsTrue("dude1" == sst1.Line || "dude1" == sst2.Line, "sst1");
            Assert.IsTrue("dude2" == sst1.Line || "dude2" == sst2.Line, "sst2");
        }

        [TestMethod]
        public void CombineTwoTopLevelFunctions()
        {
            var q1 = new GeneratedCode();
            var q2 = new GeneratedCode();

            var f1 = QMFuncUtils.GenerateFunction();
            var r1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new Statements.StatementAssign(r1, new Variables.ValSimple(f1.Name + "()", typeof(int)), new IDeclaredParameter[] { });
            var f2 = QMFuncUtils.GenerateFunction();
            var r2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new Statements.StatementAssign(r2, new Variables.ValSimple(f2.Name + "()", typeof(int)), new IDeclaredParameter[] { });

            q1.Add(f1);
            q1.Add(s1);
            q1.SetResult(r1);

            q2.Add(f2);
            q2.Add(s2);
            q2.SetResult(r2);

            var target = new CombinedGeneratedCode();
            target.AddGeneratedCode(q1);
            target.AddGeneratedCode(q2);

            target.DumpCodeToConsole();

            Assert.AreEqual(1, target.Functions.Count(), "# of functions should be combined to 1");
            Assert.AreEqual(1, target.QueryCode().Count(), "# of query code blocks");
            Assert.AreEqual(2, target.QueryCode().First().Statements.Count(), "# of statements in the combined block.");
            Assert.IsFalse(target.DumpCode().Where(l => l.Contains(f2.Name)).Any(), "The new function was still in there");
        }

        [TestMethod]
        public void CombineTwoTwoLevelFunctions()
        {
            var q1 = new GeneratedCode();
            var q2 = new GeneratedCode();

            var f1 = GenerateFunction2();
            var r1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new Statements.StatementAssign(r1, new Variables.ValSimple(f1[1].Name + "()", typeof(int)), new IDeclaredParameter[] { });
            var f2 = GenerateFunction2();
            var r2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new Statements.StatementAssign(r2, new Variables.ValSimple(f2[1].Name + "()", typeof(int)), new IDeclaredParameter[] { });

            q1.Add(f1[0]);
            q1.Add(f1[1]);
            q1.Add(s1);
            q1.SetResult(r1);

            q2.Add(f2[0]);
            q2.Add(f2[1]);
            q2.Add(s2);
            q2.SetResult(r2);

            var target = new CombinedGeneratedCode();
            target.AddGeneratedCode(q1);
            target.AddGeneratedCode(q2);

            target.DumpCodeToConsole();

            Assert.AreEqual(2, target.Functions.Count(), "# of functions should be combined to 2");
            Assert.AreEqual(1, target.QueryCode().Count(), "# of query code blocks");
            Assert.AreEqual(2, target.QueryCode().First().Statements.Count(), "# of statements in the combined block.");
            Assert.IsFalse(target.DumpCode().Where(l => l.Contains(f2[0].Name)).Any(), "The new function was still in there");
            Assert.IsFalse(target.DumpCode().Where(l => l.Contains(f2[1].Name)).Any(), "The new function was still in there");
        }

        [TestMethod]
        public void CombineOneTwoLevelAndOneOneLevelFunctions1()
        {
            var q1 = new GeneratedCode();
            var q2 = new GeneratedCode();

            var f1 = GenerateFunction2();
            var r1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new Statements.StatementAssign(r1, new Variables.ValSimple(f1[1].Name + "()", typeof(int)), new IDeclaredParameter[] { });
            var f2 = QMFuncUtils.GenerateFunction();
            var r2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new Statements.StatementAssign(r2, new Variables.ValSimple(f2.Name + "()", typeof(int)), new IDeclaredParameter[] { });

            q1.Add(f1[0]);
            q1.Add(f1[1]);
            q1.Add(s1);
            q1.SetResult(r1);

            q2.Add(f2);
            q2.Add(s2);
            q2.SetResult(r2);

            var target = new CombinedGeneratedCode();
            target.AddGeneratedCode(q1);
            target.AddGeneratedCode(q2);

            target.DumpCodeToConsole();

            Assert.AreEqual(2, target.Functions.Count(), "# of functions should be combined to 2");
            Assert.AreEqual(1, target.QueryCode().Count(), "# of query code blocks");
            Assert.AreEqual(2, target.QueryCode().First().Statements.Count(), "# of statements in the combined block.");
            Assert.IsFalse(target.DumpCode().Where(l => l.Contains(f2.Name)).Any(), "The new function was still in there");
        }

        [TestMethod]
        public void CombineOneTwoLevelAndOneOneLevelFunctions2()
        {
            var q1 = new GeneratedCode();
            var q2 = new GeneratedCode();

            var f1 = QMFuncUtils.GenerateFunction();
            var r1 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s1 = new Statements.StatementAssign(r1, new Variables.ValSimple(f1.Name + "()", typeof(int)), new IDeclaredParameter[] { });
            var f2 = GenerateFunction2();
            var r2 = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var s2 = new Statements.StatementAssign(r2, new Variables.ValSimple(f2[1].Name + "()", typeof(int)), new IDeclaredParameter[] { });

            q1.Add(f1);
            q1.Add(s1);
            q1.SetResult(r1);

            q2.Add(f2[0]);
            q2.Add(f2[1]);
            q2.Add(s2);
            q2.SetResult(r2);

            var target = new CombinedGeneratedCode();
            target.AddGeneratedCode(q1);
            target.AddGeneratedCode(q2);

            target.DumpCodeToConsole();

            Assert.AreEqual(2, target.Functions.Count(), "# of functions should be combined to 2");
            Assert.AreEqual(1, target.QueryCode().Count(), "# of query code blocks");
            Assert.AreEqual(2, target.QueryCode().First().Statements.Count(), "# of statements in the combined block.");
            Assert.IsFalse(target.DumpCode().Where(l => l.Contains(f2[0].Name)).Any(), "The new function was still in there");
        }

        private QMFuncSource[] GenerateFunction2()
        {
            var fsub = QMFuncUtils.GenerateFunction();

            int[] ints = new int[10];
            var h = new QMFuncHeader() { Arguments = new object[] { }, QM = new QueryModel(new MainFromClause("i", typeof(int), Expression.Constant(ints)), new SelectClause(Expression.Constant(10))) };
            h.QMText = h.QM.ToString();
            var f = new QMFuncSource(h);

            var p = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var st = new StatementAssign(p, new ValSimple(fsub.Name + "()", typeof(int)), new IDeclaredParameter[] { });
            var inlineblock = new StatementInlineBlock();
            inlineblock.Add(st);
            inlineblock.Add(new StatementReturn(p));
            f.SetCodeBody(inlineblock);

            return new QMFuncSource[] { fsub, f };
        }
    }
}
