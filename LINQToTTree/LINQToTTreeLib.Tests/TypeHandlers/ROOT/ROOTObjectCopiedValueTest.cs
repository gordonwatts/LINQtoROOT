// <copyright file="ROOTObjectCopiedVariableTest.cs" company="Microsoft">Copyright � Microsoft 2010</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.TypeHandlers.ROOT
{
    /// <summary>This class contains parameterized unit tests for ROOTObjectCopiedVariable</summary>
    [PexClass(typeof(ROOTObjectCopiedValue))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class ROOTObjectCopiedVariableTest
    {
        /// <summary>Test stub for .ctor(String, Type, String)</summary>
        ///[PexMethod]
        internal ROOTObjectCopiedValue Constructor(
            string varName,
            Type rootType,
            string CPPType,
            string origname
        )
        {
            ROOTObjectCopiedValue target
               = new ROOTObjectCopiedValue(varName, rootType, CPPType, origname);

            Assert.AreEqual(rootType, target.Type, "reported Type incorrect");
            Assert.AreEqual("LoadFromInputList<" + CPPType + ">(\"" + varName + "\")", target.RawValue, "loader string incorrect");
            Assert.AreEqual(origname, target.OriginalName, "original name");

            return target;
        }

        [TestMethod]
        public void TestSimple()
        {
            Constructor("test", typeof(ROOTNET.NTH1F), "TH1F", "bogus");
        }
    }
}
