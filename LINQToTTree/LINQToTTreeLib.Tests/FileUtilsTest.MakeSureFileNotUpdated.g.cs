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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Pex.Framework.Generated;

namespace LINQToTTreeLib.Utils
{
    public partial class FileUtilsTest
    {
        [TestMethod]
        [PexGeneratedBy(typeof(FileUtilsTest))]
        public void MakeSureFileNotUpdated358()
        {
            this.MakeSureFileNotUpdated((string)null, (string)null);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(FileUtilsTest))]
        public void MakeSureFileNotUpdated684()
        {
            this.MakeSureFileNotUpdated((string)null, "");
        }
        [TestMethod]
        [PexGeneratedBy(typeof(FileUtilsTest))]
        public void MakeSureFileNotUpdated163()
        {
            this.MakeSureFileNotUpdated("", (string)null);
        }
        [TestMethod]
        [PexGeneratedBy(typeof(FileUtilsTest))]
        public void MakeSureFileNotUpdated194()
        {
            this.MakeSureFileNotUpdated((string)null, "\0");
        }
        [TestMethod]
        [PexGeneratedBy(typeof(FileUtilsTest))]
        public void MakeSureFileNotUpdated588()
        {
            this.MakeSureFileNotUpdated((string)null, "\0\0");
        }
[TestMethod]
[PexGeneratedBy(typeof(FileUtilsTest))]
[Ignore]
[PexDescription("the test state was: path bounds exceeded")]
public void MakeSureFileNotUpdated584()
{
    this.MakeSureFileNotUpdated((string)null, new string('\0', 1024));
}
    }
}
