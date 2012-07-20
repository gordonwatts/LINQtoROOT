using System;
using System.Linq;
using System.Linq.Expressions;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.TypeHandlers;
using LINQToTTreeLib.TypeHandlers.ROOT;
using Microsoft.Pex.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Tests
{
    [TestClass, PexClass]
    public partial class ExpressionVisitorMethodCallTests
    {
        [TestInitialize]
        public void Setup()
        {
            TestUtils.ResetLINQLibrary();
            MEFUtilities.AddPart(new TypeHandlerROOT());
            MEFUtilities.AddPart(new TypeHandlerHelpers());
            MEFUtilities.Compose(new TypeHandlers.TypeHandlerCache());
        }

        [TestCleanup]
        public void Cleanup()
        {
            MEFUtilities.MyClassDone();
        }

        [TestMethod]
        public void TestApplyReturnFirstMethodCall()
        {
            ///
            /// Get the method we need to get! :-)
            /// 

            var clsGeneric = typeof(Helpers).GetMethod("ApplyReturnFirst");
            var cls = clsGeneric.MakeGenericMethod(new Type[] { typeof(ROOTNET.NTH1F), typeof(double) });
            Expression<Action<ROOTNET.NTH1F, double>> applyIt = (h, item) => h.Fill(item);
            MethodCallExpression mc = Expression.Call(cls, Expression.Parameter(typeof(ROOTNET.NTH1F), "myhist"), Expression.Constant(10.2), applyIt);

            ///
            /// Now do the actual call
            /// 

            GeneratedCode gc = new GeneratedCode();
            var result = ExpressionToCPP.GetExpression(mc, gc, null, MEFUtilities.MEFContainer);

            ///
            /// And check the results!
            /// 

            Assert.AreEqual(typeof(ROOTNET.NTH1F), result.Type, "incorrect result type");
            Assert.AreEqual("myhist", result.RawValue, "didn't get back the accumulator!");

            Assert.AreEqual(1, gc.CodeBody.Statements.Count(), "Expected a statement body to do the filling!");
            Assert.IsInstanceOfType(gc.CodeBody.Statements.First(), typeof(LINQToTTreeLib.Statements.StatementSimpleStatement), "incorrect statement saved");
            var statement = gc.CodeBody.Statements.First() as LINQToTTreeLib.Statements.StatementSimpleStatement;
            Assert.AreEqual("(*myhist).Fill(10.2)", statement.Line, "incorrect fill statement");
        }
    }
}
