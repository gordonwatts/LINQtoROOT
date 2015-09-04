using LINQToTTreeLib.QueryVisitors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Remotion.Linq.Parsing.ExpressionTreeVisitors.Transformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.Tests.QueryVisitors
{
    /// <summary>
    /// Test out the simple functionality of the expression replacement code
    /// </summary>
    [TestClass]
    public class PropertyExpressionTransformerTest
    {
        [TestMethod]
        public void PropertyExpressionCTor()
        {
            var c = new PropertyExpressionTransformer();
        }

        /// <summary>
        /// A few properties to test this replacement code against.
        /// </summary>
        class PETest
        {
            /// <summary>
            /// A simple constant replacement
            /// </summary>
            public int theConstant { get; set; }
            public static Expression<Func<PETest, int>> theConstantExpression = f => 10;

            /// <summary>
            /// Test out a field
            /// </summary>
#pragma warning disable 0649
            public int theFieldRaw;
            public int theField;
#pragma warning restore 0649
            public static Expression<Func<PETest, int>> theFieldExpression = f => f.theFieldRaw;

            /// <summary>
            /// Test out a property
            /// </summary>
            public int thePropertyRaw { get; set; }
            public int theProperty { get; set; }
            public static Expression<Func<PETest, int>> thePropertyExpression = f => f.thePropertyRaw;

            /// <summary>
            /// Used for a bad return type - int and double don't match.
            /// </summary>
#pragma warning disable 0649
            public int theFieldBadReturn;
#pragma warning restore 0649
            public static Expression<Func<PETest, double>> theFieldBadReturnExpression = f => f.thePropertyRaw;

            /// <summary>
            /// Just a bad signature all together!
            /// </summary>
#pragma warning disable 0649
            public int theFieldBadSignature;
#pragma warning restore 0649
            public static int theFieldBadSignatureExpression = 2;

            /// <summary>
            /// Forgot the static type!
            /// </summary>
#pragma warning disable 0649
            public int theFieldBadStatic;
#pragma warning restore 0649
            public Expression<Func<PETest, int>> theFieldBadStaticExpression = f => f.thePropertyRaw;
        }

        [TestMethod]
        public void PropertyExpressionNoReplacementP()
        {
            var c = new PropertyExpressionTransformer();
            var f = new PETest();
            var paccess = Expression.Property(Expression.Constant(f), "thePropertyRaw");

            var pnew = c.Transform(paccess);

            Assert.IsNotNull(pnew);
            Assert.IsInstanceOfType(pnew, typeof(MemberExpression));
            var pnewme = pnew as MemberExpression;
            Assert.AreEqual("thePropertyRaw", pnewme.Member.Name);
        }

        [TestMethod]
        public void PropertyExpressionNoReplacementF()
        {
            var c = new PropertyExpressionTransformer();
            var f = new PETest();
            var paccess = Expression.Field(Expression.Constant(f), "theFieldRaw");

            var pnew = c.Transform(paccess);

            Assert.IsNotNull(pnew);
            Assert.IsInstanceOfType(pnew, typeof(MemberExpression));
            var pnewme = pnew as MemberExpression;
            Assert.AreEqual("theFieldRaw", pnewme.Member.Name);
        }

        [TestMethod]
        public void PropertyExpressionConstant()
        {
            var c = new PropertyExpressionTransformer();
            var f = new PETest();
            var paccess = Expression.Property(Expression.Constant(f), "theConstant");

            var pnew = c.Transform(paccess);

            Assert.IsNotNull(pnew);
            Assert.AreEqual("f => 10.Invoke(value(LINQToTTreeLib.Tests.QueryVisitors.PropertyExpressionTransformerTest+PETest))", pnew.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PropertyExpressionWrongReturnType()
        {
            // everything right but return type wrong.
            var c = new PropertyExpressionTransformer();
            var f = new PETest();
            var paccess = Expression.Field(Expression.Constant(f), "theFieldBadReturn");

            var pnew = c.Transform(paccess);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PropertyExpressionWrongSignature()
        {
            // Totally wrong signature
            var c = new PropertyExpressionTransformer();
            var f = new PETest();
            var paccess = Expression.Field(Expression.Constant(f), "theFieldBadSignature");

            var pnew = c.Transform(paccess);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PropertyExpressionForgetStatic()
        {
            // Forgets to mark the item as static
            var c = new PropertyExpressionTransformer();
            var f = new PETest();
            var paccess = Expression.Field(Expression.Constant(f), "theFieldBadStatic");

            var pnew = c.Transform(paccess);
        }
    }
}
