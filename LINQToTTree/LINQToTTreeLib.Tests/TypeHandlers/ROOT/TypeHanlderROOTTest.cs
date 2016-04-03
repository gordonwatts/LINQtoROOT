using System;
using System.Linq;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Tests;
using LINQToTTreeLib.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace LINQToTTreeLib.TypeHandlers.ROOT
{
    /// <summary>This class contains parameterized unit tests for TypeHanlderROOT</summary>
    [TestClass]
    public partial class TypeHanlderROOTTest
    {
        [TestInitialize]
        public void Setup()
        {
            TestUtils.ResetLINQLibrary();
            MEFUtilities.AddPart(new QVResultOperators());
            MEFUtilities.Compose(new TypeHandlerCache());
        }

        [TestCleanup]
        public void Cleanup()
        {
            MEFUtilities.MyClassDone();
        }

        [TestMethod]
        public void TestQueueForTransferNoNameChange()
        {
            var t = new TypeHandlerROOT();

            var origRootObj = new ROOTNET.NTH1F("hi", "there", 10, 10.0, 20.0);
            var rootObj = Expression.Constant(origRootObj);

            var gc = new GeneratedCode();
            var result = t.ProcessConstantReference(rootObj, gc, MEFUtilities.MEFContainer);

            Assert.IsNotNull(result);

            Assert.AreEqual(1, gc.VariablesToTransfer.Count(), "Variables to transfer");
            Assert.AreEqual("hi", origRootObj.Name, "Name of original root object");
            Assert.AreEqual("there", origRootObj.Title, "Title of original root object");
        }

#if false
        // While the cache over the wire carefully tests to make sure it is the same object, determining the scope of the
        // last time this was loaded (or not) is actually quite difficult - because we have to figure out all sorts of
        // scoping rules. It shouldn't hurt performance (though it does make code look uglier). So leave this in for now.
        [TestMethod]
        public void TestQueueTwice()
        {
            var t = new TypeHandlerROOT();

            var origRootObj = new ROOTNET.NTH1F("hi", "there", 10, 10.0, 20.0);
            var rootObj = Expression.Constant(origRootObj);

            var gc = new GeneratedCode();
            var result1 = t.ProcessConstantReference(rootObj, gc, MEFUtilities.MEFContainer);
            var result2 = t.ProcessConstantReference(rootObj, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            Assert.AreEqual(1, gc.VariablesToTransfer.Count(), "Variables to transfer");

            Assert.AreEqual(result1.RawValue, result2.RawValue, "Expected the same raw value for the same object in the same expression");
        }
#endif

#if false
        // We have to send over TNamed objects only, unfortunately. The result is the following which would be great
        // if they worked, but do not.
        [TestMethod]
        public void QueueNamespaceObject()
        {
            var t = new TypeHandlerROOT();

            var origRootObj = new ROOTNET.NROOT.NMath.NBasicMinimizer();
            var rootObj = Expression.Constant(origRootObj);

            var gc = new GeneratedCode();
            var result = t.ProcessConstantReference(rootObj, gc, MEFUtilities.MEFContainer);

            Assert.IsNotNull(result);

            Assert.AreEqual(1, gc.VariablesToTransfer.Count(), "Variables to transfer");
        }

        [TestMethod]
        public void QueueTMVAObject()
        {
            var t = new TypeHandlerROOT();

            var f = new ROOTNET.NTFile("hi.root", "RECREATE");
            try
            {
                var origRootObj = new ROOTNET.NTMVA.NFactory(new ROOTNET.NTString("hi"), f);
                var rootObj = Expression.Constant(origRootObj);

                var gc = new GeneratedCode();
                var result = t.ProcessConstantReference(rootObj, gc, MEFUtilities.MEFContainer);

                Assert.IsNotNull(result);

                Assert.AreEqual(1, gc.VariablesToTransfer.Count(), "Variables to transfer");
            } finally
            {
                f.Close();
            }
        }

#endif
        [TestMethod]
        public void TestROOTMethodReference()
        {
            var expr = Expression.Variable(typeof(ROOTNET.NTH1F), "myvar");
            var getEntriesMethod = typeof(ROOTNET.NTH1F).GetMethod("GetEntries", new Type[0]);
            var theCall = Expression.Call(expr, getEntriesMethod);

            var target = new TypeHandlerROOT();

            var gc = new GeneratedCode();
            var returned = target.CodeMethodCall(theCall, gc, MEFUtilities.MEFContainer);

            Assert.AreEqual("(*myvar).GetEntries()", returned.RawValue, "call is incorrect");
        }

        [TestMethod]
        public void TestStaticMethodCall()
        {
            var expr = Expression.Variable(typeof(double), "dude");
            var phiMethod = typeof(ROOTNET.NTVector2).GetMethod("Phi_0_2pi");
            var theCall = Expression.Call(phiMethod, expr);

            var target = new TypeHandlerROOT();
            var gc = new GeneratedCode();
            var returned = target.CodeMethodCall(theCall, gc, MEFUtilities.MEFContainer);

            Assert.AreEqual("TVector2::Phi_0_2pi(dude)", returned.RawValue, "static call is incorrect");
        }

        [TestMethod]
        public void TestBasicProcessNew()
        {
            /// Test a very simple process new

            var createTLZ = Expression.New(typeof(ROOTNET.NTLorentzVector).GetConstructor(new Type[0]));
            var target = new TypeHandlerROOT();
            IValue resultOfCall;
            var gc = new GeneratedCode();
            var expr = target.ProcessNew(createTLZ, out resultOfCall, gc, MEFUtilities.MEFContainer);

            gc.DumpCodeToConsole();

            Assert.AreEqual(createTLZ.ToString(), expr.ToString(), "Returned expression");
            Assert.AreEqual(2, gc.CodeBody.Statements.Count(), "# of coded statements");
            var s1 = gc.CodeBody.Statements.First();
            var s2 = gc.CodeBody.Statements.Skip(1).First();
            Assert.IsInstanceOfType(s1, typeof(Statements.StatementSimpleStatement), "s1 type");
            Assert.IsInstanceOfType(s2, typeof(Statements.StatementSimpleStatement), "s1 type");
            var s1s = s1 as Statements.StatementSimpleStatement;
            var s2s = s2 as Statements.StatementSimpleStatement;

            Assert.IsTrue(s1s.Line.Contains("TLorentzVector"), "first line is not that good");
            Assert.IsTrue(s2s.Line.Contains("TLorentzVector *"), "second line is not that good");
        }
    }
}
