// <copyright file="CombinedGeneratedCodeTest.cs" company="Microsoft">Copyright © Microsoft 2010</copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTTreeInterfacesLib;
using Microsoft.ExtendedReflection.DataAccess;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Using;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQToTTreeLib
{
    /// <summary>This class contains parameterized unit tests for CombinedGeneratedCode</summary>
    [PexClass(typeof(CombinedGeneratedCode))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class CombinedGeneratedCodeTest
    {
        /// <summary>Test stub for AddGeneratedCode(IGeneratedQueryCode)</summary>
        [PexMethod]
        [PexUseType(typeof(GeneratedCode)), PexAllowedException(typeof(ArgumentException)), PexAllowedException(typeof(TermDestructionException)), PexAllowedException(typeof(ArgumentNullException))]
        internal void AddGeneratedCode(
            [PexAssumeUnderTest]CombinedGeneratedCode target,
            [PexAssumeNotNull] IExecutableCode code
        )
        {
            ///
            /// Do the adding
            /// 

            target.AddGeneratedCode(code);

            ///
            /// Check that it all went ok!
            /// 

#if false
            Assert.AreEqual(originalVars, varNames.Count, "Non-unique variable names and no error");
            Assert.AreEqual(originalVars, targetVarsToTransfer.Count(), "variables to transfer");

            Assert.AreEqual(resultNames.Count, totalResultCount, "Some duplicate result values but no error");
            Assert.AreEqual(resultNames.Count, target.ResultValues.Count(), "bad # of result values");

            Assert.AreEqual(includeSuperSet.Count, target.IncludeFiles.Count(), "improper # of include files");

            Assert.AreEqual(nStatments, target.CodeStatements.Statements.Count(), "improper # of statements");
#endif
        }

        [PexMethod, PexAllowedException(typeof(ArgumentException))]
        internal void CheckAddSameVariableNamesToTransfer([PexAssumeNotNull] string[] varnames)
        {
            var cc = new CombinedGeneratedCode();
            foreach (var item in varnames)
            {
                var gc = new GeneratedCode();
                gc.QueueForTransfer(item, 44);
                cc.AddGeneratedCode(gc);
            }

            HashSet<string> unique = new HashSet<string>();
            foreach (var item in varnames)
            {
                unique.Add(item);
            }
            Assert.AreEqual(unique.Count, varnames.Length, "there are non-unique names and there are no errors!");
            Assert.AreEqual(varnames.Length, cc.VariablesToTransfer.Count(), "bad number added");
        }

        [PexMethod]
        internal void CheckAddIncludeFiles([PexAssumeNotNull] string[] incnames)
        {
            var cc = new CombinedGeneratedCode();
            foreach (var item in incnames)
            {
                var gc = new GeneratedCode();
                gc.AddIncludeFile(item);
                cc.AddGeneratedCode(gc);
            }

            HashSet<string> unique = new HashSet<string>(incnames);
            Assert.AreEqual(unique.Count, cc.IncludeFiles.Count(), "wrong # of unqiue names");
        }

        /// <summary>Test stub for AddIncludeFile(String)</summary>
        [PexMethod]
        internal void AddIncludeFile(
            [PexAssumeUnderTest]CombinedGeneratedCode target,
            string includeName
        )
        {
            HashSet<string> hs = new HashSet<string>(target.IncludeFiles);
            target.AddIncludeFile(includeName);
            Assert.IsFalse(string.IsNullOrWhiteSpace(includeName), "bad include file was successfully added");
            hs.Add(includeName);
            Assert.AreEqual(hs.Count, target.IncludeFiles.Count(), "incorrect # of include files");
        }

        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        internal CombinedGeneratedCode Constructor()
        {
            CombinedGeneratedCode target = new CombinedGeneratedCode();
            return target;
            // TODO: add assertions to method CombinedGeneratedCodeTest.Constructor()
        }

        /// <summary>Test stub for get_IncludeFiles()</summary>
        [PexMethod]
        internal IEnumerable<string> IncludeFilesGet([PexAssumeUnderTest]CombinedGeneratedCode target)
        {
            IEnumerable<string> result = target.IncludeFiles;
            return result;
            // TODO: add assertions to method CombinedGeneratedCodeTest.IncludeFilesGet(CombinedGeneratedCode)
        }

        /// <summary>Test stub for get_ResultValues()</summary>
        [PexMethod]
        internal IEnumerable<IVariable> ResultValuesGet([PexAssumeUnderTest]CombinedGeneratedCode target)
        {
            IEnumerable<IVariable> result = target.ResultValues;
            return result;
            // TODO: add assertions to method CombinedGeneratedCodeTest.ResultValuesGet(CombinedGeneratedCode)
        }

        /// <summary>Test stub for get_VariablesToTransfer()</summary>
        [PexMethod]
        internal IEnumerable<KeyValuePair<string, object>> VariablesToTransferGet([PexAssumeUnderTest]CombinedGeneratedCode target)
        {
            IEnumerable<KeyValuePair<string, object>> result = target.VariablesToTransfer;
            return result;
            // TODO: add assertions to method CombinedGeneratedCodeTest.VariablesToTransferGet(CombinedGeneratedCode)
        }
    }
}
