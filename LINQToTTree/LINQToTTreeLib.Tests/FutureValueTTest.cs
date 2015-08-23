using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for FutureValue`1</summary>
    [TestClass]
    public partial class FutureValueTTest
    {
#if false
        /// <summary>Test stub for .ctor(!0)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        internal FutureValue<T> Constructor<T>(T result)
        {
            FutureValue<T> target = new FutureValue<T>(result);
            Assert.AreEqual(result, target.Value, "value incorrect");
            Assert.IsTrue(target.HasValue, "it should be marked as having a value!!");
            return target;
        }

        /// <summary>Test stub for .ctor(TTreeQueryExecutor)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        internal FutureValue<T> Constructor01<T>(TTreeQueryExecutor tTreeQueryExecutor)
        {
            FutureValue<T> target = new FutureValue<T>(tTreeQueryExecutor);
            Assert.IsFalse(target.HasValue, "has value is not right!!");
            return target;
        }

        /// <summary>Test stub for SetValue(!0)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        internal void SetValue<T>([PexAssumeUnderTest]FutureValue<T> target, T val)
        {
            target.SetValue(val);
            Assert.IsTrue(target.HasValue, "shoudl be marked as having a value now!");
            Assert.AreEqual(val, target.Value, "Value shoudl have been set");
        }

        /// <summary>Test stub for get_Value()</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        internal T ValueGet<T>([PexAssumeUnderTest]FutureValue<T> target)
        {
            T result = target.Value;
            Assert.IsTrue(target.HasValue, "Value must be set in order to get the value here");
            return result;
        }
#endif
    }
}
