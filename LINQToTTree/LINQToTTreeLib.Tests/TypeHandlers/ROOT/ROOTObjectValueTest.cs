// <copyright file="ROOTObjectValueTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Text;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOTNET.Interface;

namespace LINQToTTreeLib.TypeHandlers.ROOT
{
    /// <summary>This class contains parameterized unit tests for ROOTObjectValue</summary>
    [PexClass(typeof(ROOTObjectVariable))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class ROOTObjectValueTest
    {
        /// <summary>Test stub for .ctor(NTObject)</summary>
        [PexMethod]
        internal ROOTObjectVariable Constructor<T>([PexAssumeNotNull] T nTObject)
            where T : NTObject
        {
            ROOTObjectVariable target = new ROOTObjectVariable(nTObject);

            StringBuilder expected = new StringBuilder();
            expected.AppendFormat("LoadFromInputList<{0}>(\"{1}\")", typeof(T).Name.Substring(1), target.VariableName);
            Assert.AreEqual(expected.ToString(), target.InitialValue.RawValue, "inital value incorrect");

            return target;
        }

        /// <summary>Test stub for get_Declare()</summary>
        [PexMethod]
        internal bool DeclareGet([PexAssumeUnderTest]ROOTObjectVariable target)
        {
            bool result = target.Declare;
            Assert.IsTrue(result, "declare is false");
            return result;
        }

        [TestMethod]
        public void TestSimpleDeclare()
        {
            DeclareGet(new ROOTObjectVariable(new ROOTNET.NTObject()));
        }

        [TestMethod]
        public void TestCheckObject()
        {
            Constructor<NTObject>(new ROOTNET.NTObject());
        }

        [TestMethod]
        public void TestCheckTH1F()
        {
            Constructor<NTH1F>(new ROOTNET.NTH1F("hi", "there", 10, 0.0, 10.0));
        }
    }
}
