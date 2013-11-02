using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// <copyright file="ExpressionStringConverterTest.cs" company="Microsoft">Copyright � Microsoft 2010</copyright>
using System;
using System.Linq.Expressions;

namespace LINQToTTreeLib.Expressions
{
    /// <summary>This class contains parameterized unit tests for ExpressionStringConverter</summary>
    [PexClass(typeof(ExpressionStringConverter))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class ExpressionStringConverterTest
    {
        /// <summary>Test stub for Format(Expression)</summary>
        [PexMethod, PexAllowedException(typeof(ArgumentNullException))]
        public string Format(Expression expression)
        {
            string result = ExpressionStringConverter.Format(expression);
            return result;
        }

        private Tuple<Expression, Expression>[] NotSameExpressions = new Tuple<Expression, Expression>[]
            {
                Tuple.Create<Expression, Expression> (Expression.Constant(5), Expression.Constant(10)),
                Tuple.Create<Expression, Expression> (Expression.Constant(new ROOTNET.NTH1F("hi", "there", 20, 0.0, 10.0)), Expression.Constant(new ROOTNET.NTH1F("hi", "there", 30, 0.0, 10.0))),
                Tuple.Create<Expression, Expression> (Expression.Constant(new ROOTNET.NTH1F("hi", "there", 20, 0.0, 10.0)), Expression.Constant(new ROOTNET.NTH1F("hi", "there", 20, 0.0, 10.0))),
                Tuple.Create<Expression, Expression> (Expression.Constant(new ROOTNET.NTH1F("hi", "there", 20, 0.0, 10.0)), Expression.Constant(new ROOTNET.NTH1F("hidude", "therefork", 20, 0.0, 10.0))),
                Tuple.Create<Expression, Expression> (Expression.Constant(new ROOTNET.NTLorentzVector(1.0, 1.0, 1.0, 10.0)), Expression.Constant(new ROOTNET.NTLorentzVector(1.0, 1.0, 1.0, 10.0))),
            };

        [TestMethod]
        public void TestNotSame()
        {
            /// Loop through the pairs of expressions and make sure they aren't the same when cvt to string.
            foreach (var item in NotSameExpressions)
            {
                var s1 = ExpressionStringConverter.Format(item.Item1);
                var s2 = ExpressionStringConverter.Format(item.Item2);

                Console.WriteLine("s1 = '{0}' s2 = '{1}'", s1, s2);
                Assert.AreNotEqual(s1, s2, "Didn't expect '" + s1 + "' == '" + s2 + "'");
            }
        }

        private static ROOTNET.NTH1F _protoHisto = new ROOTNET.NTH1F("hi", "there", 20, 0.0, 10.0);
        private static ROOTNET.NTLorentzVector _protoTLZ = new ROOTNET.NTLorentzVector(1.0, 1.0, 1.0, 10.0);

        private Tuple<Expression, Expression>[] SameExpressions = new Tuple<Expression, Expression>[]
            {
                Tuple.Create<Expression, Expression> (Expression.Constant(5), Expression.Constant(5)),
                Tuple.Create<Expression, Expression> (Expression.Constant(_protoHisto), Expression.Constant(_protoHisto)),
                Tuple.Create<Expression, Expression> (Expression.Constant(_protoTLZ), Expression.Constant(_protoTLZ)),
            };

        [TestMethod]
        public void TestSame()
        {
            /// Loop through the pairs of expressions and make sure they are the same when cvt to string.
            foreach (var item in SameExpressions)
            {
                var s1 = ExpressionStringConverter.Format(item.Item1);
                var s2 = ExpressionStringConverter.Format(item.Item2);

                Console.WriteLine("s1 = '{0}' s2 = '{1}'", s1, s2);
                Assert.AreEqual(s1, s2, "Didn't expect '" + s1 + "' == '" + s2 + "'");
            }
        }
    }
}
