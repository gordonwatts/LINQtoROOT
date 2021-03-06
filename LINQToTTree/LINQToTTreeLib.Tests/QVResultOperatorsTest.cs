// <copyright file="QVResultOperatorsTest.cs" company="Microsoft">Copyright � Microsoft 2010</copyright>

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq.Expressions;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Utils
{
    [TestClass]
    public partial class QVResultOperatorsTest
    {
        [TestInitialize]
        public void Setup()
        {
            TestUtils.ResetLINQLibrary();
        }

        [TestCleanup]
        public void Cleanup()
        {
            MEFUtilities.MyClassDone();
        }

        internal QVResultOperators Constructor()
        {
            QVResultOperators target = new QVResultOperators();
            return target;
        }

        [Export(typeof(IQVScalarResultOperator))]
        class DummyRO : IQVScalarResultOperator
        {
            public bool CanHandle(Type resultOperatorType)
            {
                return (resultOperatorType == typeof(DummyRO));
            }


            public Expression ProcessResultOperator(Remotion.Linq.Clauses.ResultOperatorBase resultOperator, Remotion.Linq.QueryModel queryModel, IGeneratedQueryCode _codeEnv, ICodeContext codeContext, CompositionContainer container)
            {
                throw new NotImplementedException();
            }

            public Tuple<bool, Expression> ProcessIdentityQuery(Remotion.Linq.Clauses.ResultOperatorBase resultOperator, Remotion.Linq.QueryModel queryModel, IGeneratedQueryCode _codeEnv, ICodeContext _codeContext, CompositionContainer container)
            {
                return null;
            }
        }

        internal void TestLookupTwice(int tindex)
        {
            var t = GenerateAType(tindex);
            MEFUtilities.AddPart(new DummyRO());
            var target = new QVResultOperators();
            MEFUtilities.Compose(target);

            var result1 = target.FindScalarROProcessor(t);
            var result2 = target.FindScalarROProcessor(t);

            Assert.AreEqual(result1, result2, "Expected the same result when called with the same inputs!");
        }

        internal void TestLookupNothing(int tindex)
        {
            var t = GenerateAType(tindex);
            var target = new QVResultOperators();
            var r = target.FindScalarROProcessor(t);
            Assert.IsNull(r);
        }

        Type GenerateAType(int index)
        {
            if (index == 0)
            {
                return typeof(int);
            }
            else if (index == 1)
            {
                return typeof(DummyRO);
            }
            else
                return null;
        }

        internal void TestLookup(int tindex)
        {
            var t = GenerateAType(tindex);
            var dummy = new DummyRO();
            MEFUtilities.AddPart(dummy);
            var target = new QVResultOperators();
            MEFUtilities.Compose(target);

            var result = target.FindScalarROProcessor(t);

            if (t == typeof(DummyRO))
            {
                Assert.AreEqual(dummy, result, "expected to get back the right object");
            }
            else
            {
                Assert.IsNull(result, "Expected no found guy to come back!");
            }
        }

        [TestMethod]
        public void Constructor360()
        {
            QVResultOperators qVResultOperators;
            qVResultOperators = this.Constructor();
            Assert.IsNotNull((object)qVResultOperators);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestLookupThrowsArgumentNullException437()
        {
            this.TestLookup(2);
        }
        [TestMethod]
        public void TestLookup601()
        {
            this.TestLookup(0);
        }
        [TestMethod]
        public void TestLookup165()
        {
            this.TestLookup(1);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestLookupNothingThrowsArgumentNullException704()
        {
            this.TestLookupNothing(2);
        }
        [TestMethod]
        public void TestLookupNothing601()
        {
            this.TestLookupNothing(0);
        }
        [TestMethod]
        public void TestLookupNothing165()
        {
            this.TestLookupNothing(1);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestLookupTwiceThrowsArgumentNullException922()
        {
            this.TestLookupTwice(2);
        }
        [TestMethod]
        public void TestLookupTwice601()
        {
            this.TestLookupTwice(0);
        }
        [TestMethod]
        public void TestLookupTwice165()
        {
            this.TestLookupTwice(1);
        }
    }
}
