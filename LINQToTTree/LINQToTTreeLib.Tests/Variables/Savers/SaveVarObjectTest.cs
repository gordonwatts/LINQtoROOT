using System;
using System.Collections.Generic;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib.Variables.Savers
{
    /// <summary>This class contains parameterized unit tests for SaveVarObject</summary>
    [TestClass]
    public partial class SaveVarObjectTest
    {
        /// <summary>Test stub for CanHandle(IVariable)</summary>
        ///[PexMethod]
        internal bool CanHandle(SaveVarObject target, IDeclaredParameter iVariable)
        {
            bool result = target.CanHandle(iVariable);
            return result;
            // TODO: add assertions to method SaveVarObjectTest.CanHandle(SaveVarObject, IVariable)
        }

        [TestMethod]
        public void TestForNonROOTObject()
        {
            var v = DeclarableParameter.CreateDeclarableParameterArrayExpression(typeof(int));
            Assert.IsFalse(CanHandle(new SaveVarObject(), v), "int array should be false");
        }

        [TestMethod]
        public void TestForROOTTObject()
        {
            var v = DeclarableParameter.CreateDeclarableParameterExpression(typeof(ROOTNET.Interface.NTObject));
            Assert.IsFalse(CanHandle(new SaveVarObject(), v), "TObject array should be false");
        }

        [TestMethod]
        public void TestForROOTTH1F()
        {
            var v = DeclarableParameter.CreateDeclarableParameterExpression(typeof(ROOTNET.Interface.NTH1F));
            Assert.IsTrue(CanHandle(new SaveVarObject(), v), "TH1F");
        }

#if false
        /// <summary>Test stub for IncludeFiles(IVariable)</summary>
        [PexMethod]
        internal IEnumerable<string> IncludeFiles([PexAssumeUnderTest]SaveVarObject target, IDeclaredParameter iVariable)
        {
            IEnumerable<string> result = target.IncludeFiles(iVariable);
            return result;
            // TODO: add assertions to method SaveVarObjectTest.IncludeFiles(SaveVarObject, IVariable)
        }

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

        /// <summary>Test stub for SaveToFile(IVariable)</summary>
        [PexMethod]
        internal IEnumerable<string> SaveToFile([PexAssumeUnderTest]SaveVarObject target, IDeclaredParameter iVariable)
        {
            IEnumerable<string> result = target.SaveToFile(iVariable);
            return result;
            // TODO: add assertions to method SaveVarObjectTest.SaveToFile(SaveVarObject, IVariable)
        }
#endif
    }
}
