using LINQToTTreeLib.ExecutionCommon;
using Microsoft.Pex.Framework.Generated;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
using System.IO;

namespace LINQToTTreeLib.Tests
{
    public partial class TestProofExecutor
    {
        [TestMethod]
        [PexGeneratedBy(typeof(TestProofExecutor))]
        public void TestForSaveExeEnvironment781()
        {
            ExecutionEnvironment s0 = new ExecutionEnvironment();
            s0.ClassesToDictify = (string[][])null;
            s0.ExtraComponentFiles = (FileInfo[])null;
            s0.CleanupQuery = false;
            s0.TreeName = (string)null;
            s0.RootFiles = (Uri[])null;
            this.TestForSaveExeEnvironment(s0);
        }
    }
}
