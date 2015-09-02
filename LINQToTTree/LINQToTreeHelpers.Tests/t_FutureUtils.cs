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
    /// Test out some of the future utilities.
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

        /// <summary>
        /// Test future that doesn't say its ready until it is asked for.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class test_future_notready<T> : IFutureValue<T>
        {
            public test_future_notready(T v)
            {
                _value = v;
                ValueCalled = 0;
            }
            private T _value;

            public int ValueCalled { get; private set; }
            public T Value
            {
                get
                {
                    _hasValue = true;
                    ValueCalled++;
                    return _value;
                }
            }

            bool _hasValue = false;
            public bool HasValue
            {
                get { return _hasValue; }
            }
        }

        [TestMethod]
        public void TestDivideByTwoFutures()
        {
            var t1 = new test_future<int>(6);
            var t2 = new test_future<int>(3);

            var t3 = t1.DivideBy(t2);
            Assert.IsTrue(t3.HasValue, "has a value");
            Assert.AreEqual(2.0, t3.Value, "Value");
        }

        [TestMethod]
        public void SelectForOneAsMonad()
        {
            var tf1 = new test_future<int>(3);

            var tf2 = from t1 in tf1 select t1 * 2;

            Assert.IsTrue(tf2.HasValue);
            Assert.AreEqual(6, tf2.Value);
        }

        [TestMethod]
        public void SelectForOneAsMonadNotReady()
        {
            var tf1 = new test_future_notready<int>(3);

            var tf2 = from t1 in tf1 select t1 * 2;

            Assert.IsFalse(tf2.HasValue);
            Assert.IsFalse(tf1.HasValue);
            Assert.AreEqual(6, tf2.Value);
            Assert.IsTrue(tf2.HasValue);
            Assert.AreEqual(1, tf1.ValueCalled);
        }

        [TestMethod]
        public void SelectManyTwoMonads()
        {
            var tf1 = new test_future<int>(6);
            var tf2 = new test_future<int>(3);

            var t3 = from t1 in tf1 from t2 in tf2 select t1 / t2;
            Assert.IsTrue(t3.HasValue);
            Assert.AreEqual(2, t3.Value);
        }

        [TestMethod]
        public void SelectManyTwoMonadsNotReady()
        {
            var tf1 = new test_future_notready<int>(6);
            var tf2 = new test_future_notready<int>(3);

            var t3 = from t1 in tf1 from t2 in tf2 select t1 / t2;
            Assert.IsFalse(t3.HasValue);
            Assert.IsFalse(tf1.HasValue);
            Assert.IsFalse(tf2.HasValue);
            Assert.AreEqual(2, t3.Value);
            Assert.AreEqual(1, tf1.ValueCalled);
            Assert.AreEqual(1, tf2.ValueCalled);
            Assert.IsTrue(t3.HasValue);
            Assert.AreEqual(1, tf1.ValueCalled);
            Assert.AreEqual(1, tf2.ValueCalled);
        }

        [TestMethod]
        public void SelectManyThreeMonadsNotReady()
        {
            var tf1 = new test_future_notready<int>(1);
            var tf2 = new test_future_notready<int>(5);
            var tf3 = new test_future_notready<int>(10);

            var tf4 = from t1 in tf1 from t2 in tf2 from t3 in tf3 select t1 + t2 + t3;
            Assert.IsFalse(tf4.HasValue);
            Assert.IsFalse(tf1.HasValue);
            Assert.IsFalse(tf2.HasValue);
            Assert.IsFalse(tf3.HasValue);
            Assert.AreEqual(16, tf4.Value);
            Assert.AreEqual(1, tf1.ValueCalled);
            Assert.AreEqual(1, tf2.ValueCalled);
            Assert.AreEqual(1, tf3.ValueCalled);
            Assert.IsTrue(tf4.HasValue);
            Assert.AreEqual(1, tf1.ValueCalled);
            Assert.AreEqual(1, tf2.ValueCalled);
            Assert.AreEqual(1, tf3.ValueCalled);
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

        [TestMethod]
        public void TestFuturesSubtract()
        {
            var t1 = new test_future<int>(6);
            var t2 = new test_future<int>(6);

            var t3 = t1.Subtract(t2);
            Assert.IsTrue(t3.HasValue, "Has Value");
            Assert.AreEqual(0, t3.Value, "value");
            var t4 = t1.Subtract(6);
            Assert.IsTrue(t4.HasValue, "t4 has value");
            Assert.AreEqual(0, t4.Value, "value for plain subtract");
        }

        [TestMethod]
        public void TestFuturesMultiply()
        {
            var t1 = new test_future<int>(6);
            var t2 = new test_future<int>(6);

            var t3 = t1.MultiplyBy(t2);
            Assert.IsTrue(t3.HasValue, "Has Value");
            Assert.AreEqual(36, t3.Value, "value");
            var t4 = t1.MultiplyBy(6);
            Assert.IsTrue(t4.HasValue, "t4 has value");
            Assert.AreEqual(36, t4.Value, "value for plain subtract");
        }

        [TestMethod]
        public void TestCast()
        {
            var t1 = new test_future<ROOTNET.NTLorentzVector>(new ROOTNET.NTLorentzVector(10, 11, 12, 13));
            var t2 = t1.Cast().To<ROOTNET.Interface.NTLorentzVector>();
            Assert.IsTrue(t2.HasValue, "t2 has value");
            Assert.IsNotNull(t2.Value, "cast guy should not be null");
            var t3 = t1.Cast().To<ROOTNET.Interface.NTNamed>();
            Assert.IsNull(t3.Value, "value is null");
        }
    }
}
