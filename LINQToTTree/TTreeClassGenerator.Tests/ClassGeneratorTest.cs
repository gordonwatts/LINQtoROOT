// <copyright file="ClassGeneratorTest.cs" company="Microsoft">Copyright © Microsoft 2011</copyright>
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TTreeDataModel;

namespace TTreeClassGenerator
{
    /// <summary>This class contains parameterized unit tests for ClassGenerator</summary>
    [PexClass(typeof(ClassGenerator))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class ClassGeneratorTest
    {
        /// <summary>Test stub for GenerateClasss(FileInfo, FileInfo, String)</summary>
        [PexMethod(MaxBranches = 20000)]
        public void GenerateClasss(
            [PexAssumeUnderTest]ClassGenerator target,
            int inputChoice,
            int outputChoice,
            string namespaceName
        )
        {
            FileInfo inputXMLFile, outputCSFile;

            if (inputChoice == 1)
            {
                inputXMLFile = new FileInfo(@"..\..\..\TTreeClassGenerator.Tests");
            }
            else
            {
                inputXMLFile = null;
            }

            if (outputChoice == 1)
            {
                outputCSFile = new FileInfo("output.cs");
            }
            else
            {
                outputCSFile = null;
            }

            target.GenerateClasss(inputXMLFile, outputCSFile, namespaceName);

            Assert.IsTrue(outputCSFile.Exists, "Output file should exist if we ran GenerateClass ok!");
        }

        [PexMethod]
        public void GenerateClassFromClasses(
            [PexAssumeUnderTest]ClassGenerator target,
            int outputChoice,
            string nameSName,
            int NumObjectCollection)
        {
            ///
            /// Setup the input stuff so Pex can play
            /// 

            FileInfo outputCSFile;
            if (outputChoice == 1)
            {
                outputCSFile = new FileInfo("output.cs");
            }
            else
            {
                outputCSFile = null;
            }

            ROOTClassShell[] objCollect = null;
            if (NumObjectCollection > 0)
            {
                List<ROOTClassShell> objs = new List<ROOTClassShell>();

                for (int i = 0; i < NumObjectCollection; i++)
                {
                    ROOTClassShell rcs = new ROOTClassShell();
                    rcs.Name = "dude_" + i.ToString();
                    rcs.SubClassName = null;

                    for (int j = 0; j < i; j++)
                    {
                        IClassItem item = null;
                        switch (j % 4)
                        {
                            case 0:
                                item = null;
                                break;

                            case 1:
                                var itm = new ItemSimpleType() { ItemType = "int" };
                                item = itm;
                                break;

                            case 2:
                                var itmv = new ItemVector() { ItemType = "int[]" };
                                item = itmv;
                                break;

                            case 3:
                                var itmr = new ItemROOTClass() { ItemType = "TLorentzVector" };
                                item = itmr;
                                break;
                        }
                        if (item != null)
                            item.Name = "item_" + j.ToString();
                        rcs.Items.Add(item);
                    }
                    objs.Add(rcs);
                }

                objCollect = objs.ToArray();
            }

            target.GenerateClasss(objCollect, outputCSFile, nameSName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullInputFile()
        {
            ClassGenerator target = new ClassGenerator();
            target.GenerateClasss((FileInfo)null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNullOutputFile()
        {
            FileInfo f = new FileInfo("junk.xml");
            ClassGenerator target = new ClassGenerator();
            target.GenerateClasss(f, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestNullNamespace()
        {
            FileInfo f = new FileInfo("junk.xml");
            ClassGenerator target = new ClassGenerator();
            target.GenerateClasss(f, f, null);
        }
    }

}
