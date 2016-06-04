using LINQToTTreeLib;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NVelocity.App;
using System;
using System.IO;
using System.Linq;
using static LINQToTTreeLib.TypeHandlers.ROOT.TypeHandlerROOT;

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

            /// Get the path for the other ntuple guy correct! Since Pex and tests run from different places in the directory structure we have to
            /// do some work to find the top level!

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

            TestUtils.ResetLINQLibrary();
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

            Assert.IsTrue(DummyQueryExectuor.FinalResult.CodeBody.CodeItUp().Where(l => l.Contains("Fill(((double)(*this).run),1.0*1.0)")).Any(), "no line contains the proper size call!");
        }

        [TestMethod]
        public void TestPlotFromTemplateWithWeight()
        {
            // Create the plot

            var p = PlottingUtils.MakePlotterSpec<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupe>(10, 0.0, 10.0, evt => evt.run, "dude", "fork", weight: evt => 0.5);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupe>();
            var r = q.Plot(p, "fork");

            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            Assert.IsTrue(DummyQueryExectuor.FinalResult.CodeBody.CodeItUp().Where(l => l.Contains("Fill(((double)(*this).run),0.5*1.0)")).Any(), "no line contains the proper size call!");
        }

        [TestMethod]
        public void TestPlotFromTemplate2DWithWeight()
        {
            // Create the plot

            var p = PlottingUtils.MakePlotterSpec<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupe>(10, 0.0, 10.0, evt => evt.run,
                10, 0.0, 10.0, evt => evt.run,
                "dude", "fork", weight: evt => 0.5);

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupe>();
            var r = q.Plot(p, "fork");

            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            Assert.IsTrue(DummyQueryExectuor.FinalResult.CodeBody.CodeItUp().Where(l => l.Contains("Fill(((double)(*this).run),((double)(*this).run),0.5*1.0)")).Any(), "no line contains the proper size call!");
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
        public void RemoveSpacesFromNames()
        {
            // Create the plot
            var p = PlottingUtils.MakePlotterSpec<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>(10, 0.0, 10.0, evt => evt.jets.Select(j => j.myvectorofint).First(), "dude{0}", "fork {0}");

            // Create the query. We aren't very intersted in the result, but rather the result.
            var q = new QueriableDummy<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>();
            var r = q.Plot(p, " is hell");

            var resultVar = DummyQueryExectuor.FinalResult.ResultValue as DeclarableParameter;
            var rootObj = resultVar.InitialValue as ROOTObjectStaticHolder;

            Assert.AreEqual("dudeishell", rootObj.OriginalName);
            Assert.AreEqual("fork  is hell", rootObj.OriginalTitle);
        }

        [TestMethod]
        public void DealwithAnEmptySubs()
        {
            // Create the plot
            var p = PlottingUtils.MakePlotterSpec<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>(10, 0.0, 10.0, evt => evt.jets.Select(j => j.myvectorofint).First(), "dude{0}abides", "fork {0} this over");

            // Create the query. We aren't very intersted in the result, but rather the result.
            var q = new QueriableDummy<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>();
            var r = q.Plot(p, "");

            var resultVar = DummyQueryExectuor.FinalResult.ResultValue as DeclarableParameter;
            var rootObj = resultVar.InitialValue as ROOTObjectStaticHolder;

            Assert.AreEqual("dudeabides", rootObj.OriginalName);
            Assert.AreEqual("fork this over", rootObj.OriginalTitle);
        }

        [TestMethod]
        public void DealwithTwoEmptySubs()
        {
            // Create the plot
            var p = PlottingUtils.MakePlotterSpec<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>(10, 0.0, 10.0, evt => evt.jets.Select(j => j.myvectorofint).First(), "dude{0}{1}abides", "fork {0} {1} this over");

            // Create the query. We aren't very intersted in the result, but rather the result.
            var q = new QueriableDummy<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>();
            var r = q.Plot(p, "", "");

            var resultVar = DummyQueryExectuor.FinalResult.ResultValue as DeclarableParameter;
            var rootObj = resultVar.InitialValue as ROOTObjectStaticHolder;

            Assert.AreEqual("dudeabides", rootObj.OriginalName);
            Assert.AreEqual("fork this over", rootObj.OriginalTitle);
        }

        [TestMethod]
        public void DealwithTwoEmptySubsAndKeepSpacesCorrectly()
        {
            // Create the plot
            var p = PlottingUtils.MakePlotterSpec<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>(10, 0.0, 10.0, evt => evt.jets.Select(j => j.myvectorofint).First(), "dude{0}{1}abides", "fork {0}{1} this over");

            // Create the query. We aren't very intersted in the result, but rather the result.
            var q = new QueriableDummy<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>();
            var r = q.Plot(p, "", "hi");

            var resultVar = DummyQueryExectuor.FinalResult.ResultValue as DeclarableParameter;
            var rootObj = resultVar.InitialValue as ROOTObjectStaticHolder;

            Assert.AreEqual("dudehiabides", rootObj.OriginalName);
            Assert.AreEqual("fork hi this over", rootObj.OriginalTitle);
        }

        [TestMethod]
        public void TestPlotFromTemplateWithFromDataTranslation()
        {
            // Create the plot.
            var pRaw = PlottingUtils.MakePlotterSpec<TTreeQueryExecutorTest.TestNtupeArrJets>(10, 0.0, 10.0, v => v.myvectorofint, "dude", "fork");
            var pEvent = pRaw.FromManyOfType((LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents evt) => evt.jets, "more jets");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>();
            var r = q.Plot(pEvent, "fork");

            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            Assert.IsTrue(DummyQueryExectuor.FinalResult.CodeBody.CodeItUp().Where(l => l.Contains("(*(*this).myvectorofint).size()")).Any(), "no line contains the proper size call!");
        }

        [TestMethod]
        public void TestPlotFromTemplateNullNameAndDescrip()
        {
            // Create the plot.
            var pRaw = PlottingUtils.MakePlotterSpec<TTreeQueryExecutorTest.TestNtupeArrJets>(10, 0.0, 10.0, v => v.myvectorofint);
            var pEvent = pRaw.FromManyOfType((LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents evt) => evt.jets, "more jets");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>();
            var r = q.Plot(pEvent, "fork");

            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            Assert.IsTrue(DummyQueryExectuor.FinalResult.CodeBody.CodeItUp().Where(l => l.Contains("(*(*this).myvectorofint).size()")).Any(), "no line contains the proper size call!");
        }

        [TestMethod]
        public void TestPlotFromTemplateFromTypeConvertOnly()
        {
            // Create the plot.
            var pRaw1 = PlottingUtils.MakePlotterSpec<double>(10, 0.0, 10.0, v => v, "dude", "fork");
            var pRaw = pRaw1.FromType((TTreeQueryExecutorTest.TestNtupeArrJets j) => j.myvectorofint);
            var pEvent = pRaw.FromManyOfType((LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents evt) => evt.jets, "more jets");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>();
            var r = q.Plot(pEvent, "fork");

            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            Assert.IsTrue(DummyQueryExectuor.FinalResult.CodeBody.CodeItUp().Where(l => l.Contains("(*(*this).myvectorofint).size()")).Any(), "no line contains the proper size call!");
        }

        [TestMethod]
        public void TestPlotFromTemplateFromTypeConvertAndWeight()
        {
            // Create the plot.
            var pRaw1 = PlottingUtils.MakePlotterSpec<double>(10, 0.0, 10.0, v => v, "dude", "fork");
            var pRaw = pRaw1.FromType((TTreeQueryExecutorTest.TestNtupeArrJets j) => j.myvectorofint, weight: j => 0.5);
            var pEvent = pRaw.FromManyOfType((LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents evt) => evt.jets, "more jets");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>();
            var r = q.Plot(pEvent, "fork");

            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            Assert.IsTrue(DummyQueryExectuor.FinalResult.CodeBody.CodeItUp().Where(l => l.Contains("0.5")).Any(), "no line contains the proper size call!");
        }

        [TestMethod]
        public void TestPlotFromTemplateFromTypeWeightOnly()
        {
            // Create the plot.
            var pRaw1 = PlottingUtils.MakePlotterSpec<double>(10, 0.0, 10.0, v => v, "dude", "fork");
            var pRaw2 = pRaw1.FromType(weight: j => 0.5);
            var pRaw = pRaw2.FromType((TTreeQueryExecutorTest.TestNtupeArrJets j) => j.myvectorofint);
            var pEvent = pRaw.FromManyOfType((LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents evt) => evt.jets, "more jets");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>();
            var r = q.Plot(pEvent, "fork");

            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            Assert.IsTrue(DummyQueryExectuor.FinalResult.CodeBody.CodeItUp().Where(l => l.Contains("0.5")).Any(), "no line contains the proper size call!");
        }

        [TestMethod]
        public void TestPlotFromTemplateFromTypeNothing()
        {
            // Create the plot.
            var pRaw1 = PlottingUtils.MakePlotterSpec<double>(10, 0.0, 10.0, v => v, "dude", "fork");
            var pRaw2 = pRaw1.FromType();
            var pRaw = pRaw2.FromType((TTreeQueryExecutorTest.TestNtupeArrJets j) => j.myvectorofint);
            var pEvent = pRaw.FromManyOfType((LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents evt) => evt.jets, "more jets");

            ///
            /// Get a simple query we can "play" with
            /// 

            var q = new QueriableDummy<LINQToTTreeLib.TTreeQueryExecutorTest.TestNtupeArrEvents>();
            var r = q.Plot(pEvent, "fork");

            DummyQueryExectuor.FinalResult.DumpCodeToConsole();

            Assert.IsTrue(DummyQueryExectuor.FinalResult.CodeBody.CodeItUp().All(l => !l.Contains("0.5")), "no line contains the proper size call!");
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

            Assert.IsTrue(DummyQueryExectuor.FinalResult.DumpCode().Where(l => l.Contains("(*(*this).myvectorofint).size()")).Any(), "no line contains the proper size call!");
        }
    }
}
