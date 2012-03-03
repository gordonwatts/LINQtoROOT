using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Statements;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NVelocity.App;

namespace LINQToTTreeLib.Tests.Statements
{
    [TestClass, PexClass(typeof(StatementForLoop))]
    public partial class StatementVectorLoopTest
    {
        [TestInitialize]
        public void TestInit()
        {
            MEFUtilities.MyClassInit();
            DummyQueryExectuor.GlobalInitalized = false;

            var eng = new VelocityEngine();
            eng.Init();

            QueryResultCacheTest.SetupCacheDir();
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        public class LocalNtup
        {
#pragma warning disable 0169
            public int[] myvectorofint;
#pragma warning restore 0169
        }

        [TestMethod]
        public void TestSimpleLoopCombine()
        {
            var q = new QueriableDummy<LocalNtup>();
            var dudeQ1 = from evt in q
                         from l in evt.myvectorofint
                         select l;
            var dude1 = dudeQ1.Count();

            var gc1 = DummyQueryExectuor.FinalResult;

            var dudeQ2 = from evt in q
                         from l in evt.myvectorofint
                         select l;
            var dude2 = dudeQ2.Count();

            var gc2 = DummyQueryExectuor.FinalResult;

            // Combine them!

            Assert.IsTrue(gc1.CodeBody.TryCombineStatement(gc2.CodeBody, null), "Unable to do combine!");

            gc1.DumpCodeToConsole();

            Assert.AreEqual(1, gc1.CodeBody.Statements.Count(), "# of statements at top level");
            var booking = gc1.CodeBody.Statements.First() as IBookingStatementBlock;
            Assert.AreEqual(2, booking.Statements.Count(), "# of statements in inside loop");
        }

        /// <summary>Test stub for CodeItUp()</summary>
        [PexMethod]
        public IEnumerable<string> CodeItUp([PexAssumeUnderTest]StatementForLoop target)
        {
            IEnumerable<string> result = target.CodeItUp();
            return result;
        }

        [PexMethod]
        public void TestTryCombine([PexAssumeUnderTest]StatementForLoop target, IStatement s)
        {
            /// We should never be able to combine any filter statements currently!

            var val = new Variables.ValSimple("true", typeof(bool));
            var result = target.TryCombineStatement(s, null);

            if (s.GetType() != typeof(StatementForLoop))
            {
                Assert.IsFalse(result, "Types not right");
            }
            else
            {
                var other = s as StatementForLoop;
                Assert.AreEqual(other.ArrayLength == target.ArrayLength, result, "for loops not conssitent");
            }
        }

        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public IStatement TestRename([PexAssumeUnderTest] StatementForLoop statement, [PexAssumeNotNull] string oldname, [PexAssumeNotNull]string newname)
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
