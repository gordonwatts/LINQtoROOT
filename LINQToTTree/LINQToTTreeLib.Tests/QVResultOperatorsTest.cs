// <copyright file="QVResultOperatorsTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>

using System;
using LINQToTTreeLib.Utils;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LINQToTTreeLib.Tests;
using LinqToTTreeInterfacesLib;
using System.ComponentModel.Composition;
using Microsoft.Pex.Framework.Generated;

namespace LINQToTTreeLib.Utils
{
    [TestClass]
    [PexClass(typeof(QVResultOperators))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class QVResultOperatorsTest
    {
        [TestInitialize]
        public void Setup()
        {
            MEFUtilities.MyClassInit();
        }

        [TestCleanup]
        public void Cleanup()
        {
            MEFUtilities.MyClassDone();
        }

        [PexMethod]
        internal QVResultOperators Constructor()
        {
            QVResultOperators target = new QVResultOperators();
            return target;
            // TODO: add assertions to method QVResultOperatorsTest.Constructor()
        }

        [Export(typeof(IQVResultOperator))]
        class DummyRO : IQVResultOperator
        {
            public bool CanHandle(Type resultOperatorType)
            {
                return (resultOperatorType == typeof(DummyRO));
            }


            public IVariable ProcessResultOperator(Remotion.Data.Linq.Clauses.ResultOperatorBase resultOperator, Remotion.Data.Linq.QueryModel queryModel, IGeneratedCode _codeEnv)
            {
                throw new NotImplementedException();
            }
        }

        [PexMethod]
        internal void TestLookupNothing(int tindex)
        {
            var t = GenerateAType(tindex);
            var target = new QVResultOperators();
            var r = target.FindROProcessor(t);
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

        [PexMethod]
        internal void TestLookup(int tindex)
        {
            var t = GenerateAType(tindex);
            var dummy = new DummyRO();
            MEFUtilities.AddPart(dummy);
            var target = new QVResultOperators();
            MEFUtilities.Compose(target);

            var result = target.FindROProcessor(t);

            if (t == typeof(DummyRO))
            {
                Assert.AreEqual(dummy, result, "expected to get back the right object");
            }
            else
            {
                Assert.IsNull(result, "Expected no found guy to come back!");
            }
        }

        [PexMethod]
        internal void TestLookupTwice(int tindex)
        {
            var t = GenerateAType(tindex);
            MEFUtilities.AddPart(new DummyRO());
            var target = new QVResultOperators();
            MEFUtilities.Compose(target);

            var result1 = target.FindROProcessor(t);
            var result2 = target.FindROProcessor(t);

            Assert.AreEqual(result1, result2, "Expected the same result when called with the same inputs!");
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
