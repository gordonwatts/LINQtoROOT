using System;
using System.IO;
using System.Linq;
using LINQToTTreeLib;
using LINQToTTreeLib.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NVelocity.App;

namespace LINQToTreeHelpers.Tests
{
    [TestClass]
    public class t_PlottingUtils
    {
        [TestInitialize]
        public void TestInit()
        {
            MEFUtilities.MyClassInit();
            DummyQueryExectuor.GlobalInitalized = false;

            /// Get the path for the other nutple guy correct! Since Pex and tests run from different places in the directory structure we have to
            /// do some work to find the top leve!

            var currentDir = new DirectoryInfo(Environment.CurrentDirectory);
            while (currentDir.FindAllFiles("LINQToTTree.sln").Count() == 0)
            {
                currentDir = currentDir.Parent;
            }
            var projectDir = currentDir.Parent;

            ntuple._gCINTLines = null;
            ntuple._gObjectFiles = null;
            ntuple._gProxyFile = null;

            var eng = new VelocityEngine();
            eng.Init();

            QueryResultCacheTest.SetupCacheDir();
        }

        [TestCleanup]
        public void TestDone()
        {
            MEFUtilities.MyClassDone();
        }

        [TestMethod]
        public void TestPlotFromTemplateForSimple()
        {
            // Create the plot

            var p = PlottingUtils.MakePlotterSpec<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupe>(10, 0.0, 10.0, evt => evt.run, "dude", "fork");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupe>();
            var r = q.Plot(p, "fork");

            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            Assert.IsTrue(DummyQueryExectuor.FinalResult.CodeBody.CodeItUp().Where(l => l.Contains("Fill(((double)(*this).run),1.0)")).Any(), "no line contains the proper size call!");
        }

        [TestMethod]
        public void TestPlotFromTemplateForTranslated()
        {
            // Create the plot

            var p = PlottingUtils.MakePlotterSpec<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>(10, 0.0, 10.0, evt => evt.jets.Select(j => j.myvectorofint).First(), "dude", "fork");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>();
            var r = q.Plot(p, "fork");

            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            Assert.IsTrue(DummyQueryExectuor.FinalResult.CodeBody.CodeItUp().Where(l => l.Contains("(*(*this).myvectorofint).size()")).Any(), "no line contains the proper size call!");
        }

        [TestMethod]
        public void TestPlotFromTemplateWithFromDataTranslation()
        {
            // Create the plot.
            var pRaw = PlottingUtils.MakePlotterSpec<TTreeQueryExecutorTest.TestNtupeArrJets>(10, 0.0, 10.0, v => v.myvectorofint, "dude", "fork");
            var pEvent = pRaw.FromType((LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents evt) => evt.jets, "more jets");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>();
            var r = q.Plot(pEvent, "fork");

            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            Assert.IsTrue(DummyQueryExectuor.FinalResult.CodeBody.CodeItUp().Where(l => l.Contains("(*(*this).myvectorofint).size()")).Any(), "no line contains the proper size call!");
        }

        [TestMethod]
        public void TestFromTemplateWithStraightSequenceDefinition()
        {
            // Create the plot

            var p = PlottingUtils.MakePlotterSpec<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>(10, 0.0, 10.0, evt => evt.jets.Select(j => (double)j.myvectorofint), "dude", "fork");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>();
            var r = q.Plot(p, "fork");

            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            Assert.IsTrue(DummyQueryExectuor.FinalResult.CodeBody.CodeItUp().Where(l => l.Contains("(*(*this).myvectorofint).size()")).Any(), "no line contains the proper size call!");
        }
    }
}
