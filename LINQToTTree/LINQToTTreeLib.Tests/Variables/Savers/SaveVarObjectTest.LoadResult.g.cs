// <auto-generated>
// This file contains automatically generated unit tests.
// Do NOT modify this file manually.
// 
// When Pex is invoked again,
// it might remove or update any previously generated unit tests.
// 
// If the contents of this file becomes outdated, e.g. if it does not
// compile anymore, you may delete this file and invoke Pex again.
// </auto-generated>
using System;
using LinqToTTreeInterfacesLib;
using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ROOTNET.Interface;

namespace LINQToTTreeLib.Variables.Savers
{
    public partial class SaveVarObjectTest
    {
        [TestMethod]
        [PexGeneratedBy(typeof(SaveVarObjectTest))]
        [ExpectedException(typeof(ArgumentNullException))]
        public void LoadResultThrowsArgumentNullException993()
        {
            int i;
            SaveVarObject s0 = new SaveVarObject();
            i = this.LoadResult<int>(s0, (IVariable)null, (NTObject)null);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(SaveVarObjectTest))]
        [ExpectedException(typeof(ArgumentException))]
        public void LoadResultThrowsArgumentException828()
        {
            using (PexDisposableContext disposables = PexDisposableContext.Create())
            {
                NTObject nTObject;
                int i;
                nTObject = ROOTNET.NTObjectFactory.Create();
                disposables.Add((IDisposable)nTObject);
                SaveVarObject s0 = new SaveVarObject();
                i = this.LoadResult<int>(s0, (IVariable)null, (NTObject)nTObject);
                disposables.Dispose();
            }
        }
    }
}
