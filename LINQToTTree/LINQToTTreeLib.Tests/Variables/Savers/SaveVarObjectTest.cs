// <copyright file="SaveVarObjectTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Variables.Savers
{
    /// <summary>This class contains parameterized unit tests for SaveVarObject</summary>
    [PexClass(typeof(SaveVarObject))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class SaveVarObjectTest
    {
        /// <summary>Test stub for CanHandle(IVariable)</summary>
        ///[PexMethod]
        internal bool CanHandle([PexAssumeUnderTest]SaveVarObject target, IVariable iVariable)
        {
            bool result = target.CanHandle(iVariable);
            return result;
            // TODO: add assertions to method SaveVarObjectTest.CanHandle(SaveVarObject, IVariable)
        }

        [TestMethod]
        public void TestForNonROOTObject()
        {
            var v = new VarObject(typeof(int[]));
            Assert.IsFalse(CanHandle(new SaveVarObject(), v), "int array should be false");
        }

        [TestMethod]
        public void TestForROOTTObject()
        {
            var v = new VarObject(typeof(ROOTNET.Interface.NTObject));
            Assert.IsFalse(CanHandle(new SaveVarObject(), v), "TObject array should be false");
        }

        [TestMethod]
        public void TestForROOTTH1F()
        {
            var v = new VarObject(typeof(ROOTNET.Interface.NTH1F));
            Assert.IsTrue(CanHandle(new SaveVarObject(), v), "TH1F");
        }

        /// <summary>Test stub for IncludeFiles(IVariable)</summary>
        [PexMethod]
        internal IEnumerable<string> IncludeFiles([PexAssumeUnderTest]SaveVarObject target, IVariable iVariable)
        {
            IEnumerable<string> result = target.IncludeFiles(iVariable);
            return result;
            // TODO: add assertions to method SaveVarObjectTest.IncludeFiles(SaveVarObject, IVariable)
        }

#if false
        /// <summary>Test stub for LoadResult(IVariable, NTObject)</summary>
        [PexGenericArguments(typeof(int))]
        [PexMethod]
        internal T LoadResult<T>(
            [PexAssumeUnderTest]SaveVarObject target,
            IVariable iVariable,
            ROOTNET.Interface.NTObject obj
        )
        {
            T result = target.LoadResult<T>(iVariable, obj);
            return result;
            // TODO: add assertions to method SaveVarObjectTest.LoadResult(SaveVarObject, IVariable, NTObject)
        }
#endif

        /// <summary>Test stub for SaveToFile(IVariable)</summary>
        [PexMethod]
        internal IEnumerable<string> SaveToFile([PexAssumeUnderTest]SaveVarObject target, IVariable iVariable)
        {
            IEnumerable<string> result = target.SaveToFile(iVariable);
            return result;
            // TODO: add assertions to method SaveVarObjectTest.SaveToFile(SaveVarObject, IVariable)
        }
    }
}
