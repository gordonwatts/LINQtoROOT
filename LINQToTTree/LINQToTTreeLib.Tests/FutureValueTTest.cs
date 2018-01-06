using System;
using System.Threading;
using System.Threading.Tasks;
using LinqToTTreeInterfacesLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for FutureValue`1</summary>
    [TestClass]
    public partial class FutureValueTTest
    {
        private class SimpleFutureValue : IFutureValue<int>
        {
            public int Value => 10;

            public bool HasValue => true;

            public Task GetAvailibleTask()
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public async Task FutureValueAwait()
        {
            var fb = new SimpleFutureValue();
            var v = await fb;
            Assert.AreEqual(10, v);
        }

        [TestMethod]
        public async Task FutureValueAwaitALittle()
        {
            var fb = new SimpleFutureValueWait();
            var v = await fb;
            Assert.AreEqual(5, v);
        }

        private class SimpleFutureValueWait : IFutureValue<int>
        {
            readonly Task _t;

            public SimpleFutureValueWait()
            {
                var delay = Task.Delay(200);
                _t = delay.ContinueWith(t => CallBack());
                
            }

            public int Value { get; private set; } = 10;

            public bool HasValue { get; private set; } = false;

            public Task GetAvailibleTask()
            {
                return _t;
            }

            private void CallBack()
            {
                Value = 5;
                HasValue = true;
            }

        }
    }
}
