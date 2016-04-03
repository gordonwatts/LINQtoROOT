using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;
using LINQToTTreeLib.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace LINQToTTreeLib.Tests
{


    /// <summary>
    ///This is a test class for ExpressionUtilitiesTest and is intended
    ///to contain all ExpressionUtilitiesTest Unit Tests
    ///</summary>
    [TestClass()]
    public partial class ExpressionUtilitiesTest
    {
        /// <summary>
        ///A test for ApplyParensIfNeeded
        ///</summary>
        [TestMethod()]
        public void ApplyParensIfNeededTest()
        {
            Assert.AreEqual("a", new ValSimple("a", typeof(int)).ApplyParensIfNeeded(), "single term");
            Assert.AreEqual("a1", new ValSimple("a1", typeof(int)).ApplyParensIfNeeded(), "single term");
            Assert.AreEqual("a_1", new ValSimple("a_1", typeof(int)).ApplyParensIfNeeded(), "single term");
            Assert.AreEqual("(a+b)", new ValSimple("a+b", typeof(int)).ApplyParensIfNeeded(), "single term");
            Assert.AreEqual("(a/b)", new ValSimple("a/b", typeof(int)).ApplyParensIfNeeded(), "single term");
            Assert.AreEqual("(-a)", new ValSimple("-a", typeof(int)).ApplyParensIfNeeded(), "single term");
            Assert.AreEqual("(a)", new ValSimple("(a)", typeof(int)).ApplyParensIfNeeded(), "single term");
            Assert.AreEqual("((a)-(b))", new ValSimple("(a)-(b)", typeof(int)).ApplyParensIfNeeded(), "single term");
        }

#if false
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public string TestApply(string value)
        {
            return value.ApplyParensIfNeeded();
        }
#endif

        [TestMethod]
        public void TestArrayRemovalNull()
        {
            Expression<Func<int[], int>> t1 = a => a[0];
            var r = t1.RemoveArrayReferences();
            Assert.AreEqual(t1, r, "Didn't return the same thing");
        }

        [TestMethod]
        public void TestArrayRemoval1D()
        {
            Expression<Func<int[], int>> t1 = a => a[0];
            var r = t1.Body.RemoveArrayReferences();
            Assert.AreEqual("a", r.ToString(), "Didn't return the same thing");
        }

        [TestMethod]
        public void TestArrayRemoval2D()
        {
            Expression<Func<int[][], int>> t1 = a => a[0][5];
            var r = t1.Body.RemoveArrayReferences();
            Assert.AreEqual("a", r.ToString(), "Didn't return the same thing");
        }

        [TestMethod]
        public void AsExpressionNonExpression()
        {
            var p = new dummyDecl();
            var expr = p.AsExpression();
            Assert.IsNotNull(expr, "expr should not b enull");
            Assert.AreNotEqual(p, expr, "Should be something new");
            Assert.AreEqual(p.RawValue, expr.ToString(), "content");
            Assert.AreEqual(typeof(int), p.Type, "type");
        }

        [TestMethod]
        public void AsExpressionWithExpr()
        {
            var p = DeclarableParameter.CreateDeclarableParameterExpression(typeof(int));
            var expr = p.AsExpression();
            Assert.AreEqual(p, expr, "translation was not transparent");
        }

        /// <summary>
        /// Dummy param for testing.
        /// </summary>
        class dummyDecl : IDeclaredParameter
        {
            public string ParameterName { get { return "dude"; } }

            public IValue InitialValue
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

            public Type Type { get { return typeof(int); } }

            public void RenameParameter(string oldname, string newname)
            {
                throw new NotImplementedException();
            }

            public string RawValue
            {
                get { return "dude"; }
            }

            public IEnumerable<IDeclaredParameter> Dependants
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool DeclareAsStatic
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public IExecutableCode InitialValueCode
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

            public void RenameRawValue(string oldname, string newname)
            {
                throw new NotImplementedException();
            }
        }

    }
}
