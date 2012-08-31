using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LINQToTreeHelpers.FutureUtils;
using LinqToTTreeInterfacesLib;

namespace LINQToTreeHelpers.Tests
{
    /// <summary>
    /// Test out some of the future utils.
    /// </summary>
    [TestClass]
    public class t_FutureUtils
    {
        // Something easy to use as a harness later one.
        public class test_future<T> : IFutureValue<T>
        {
            public test_future(T v)
            {
                _value = v;
            }
            private T _value;

            public T Value
            {
                get { return _value; }
            }

            public bool HasValue
            {
                get { return true; }
            }
        }

        [TestMethod]
        public void TestDivideByTwoFutures1()
        {
            var t1 = new test_future<int>(6);
            var t2 = new test_future<int>(3);

            var t3 = t1.DivideBy(t2);
            Assert.IsTrue(t3.HasValue, "has a value");
            Assert.AreEqual(2.0, t3.Value, "Value");
        }

        [TestMethod]
        public void TestDivideByTwoFutures2()
        {
            var t1 = new test_future<double>(6);
            var t2 = new test_future<double>(3);

            var t3 = t1.DivideBy(t2);
            Assert.IsTrue(t3.HasValue, "has a value");
            Assert.AreEqual(2.0, t3.Value, "Value");
        }

        [TestMethod]
        public void TestDivideByFutureAndNormal()
        {
            var t1 = new test_future<double>(6);

            var t3 = t1.DivideBy(3.0);
            Assert.IsTrue(t3.HasValue, "has a value");
            Assert.AreEqual(2.0, t3.Value, "Value");
        }

        [TestMethod]
        public void TestAddFutures()
        {
            var t1 = new test_future<int>(6);
            var t2 = new test_future<int>(3);

            var t3 = t1.AddTo(t2);
            Assert.IsTrue(t3.HasValue, "has a value");
            Assert.AreEqual(9, t3.Value, "Value");
        }

        [TestMethod]
        public void TestFuturesAndNormal()
        {
            var t1 = new test_future<int>(6);

            var t3 = t1.AddTo(3);
            Assert.IsTrue(t3.HasValue, "has a value");
            Assert.AreEqual(9, t3.Value, "Value");
        }
    }
}
