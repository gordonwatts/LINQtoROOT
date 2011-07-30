using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace LINQToTTreeLib.ExecutionCommon.ParallelExes
{
    /// <summary>
    /// Execute the query by using multiple executables
    /// to run in parallel.
    /// </summary>
    class ParallelOnLocalMachineExecutor : LocalBuildBase, IQueryExectuor
    {
        /// <summary>
        /// Execute the query
        /// </summary>
        /// <param name="templateFile"></param>
        /// <param name="queryDirectory"></param>
        /// <param name="varsToTransfer"></param>
        /// <returns></returns>
        public IDictionary<string, ROOTNET.Interface.NTObject> Execute(FileInfo templateFile, DirectoryInfo queryDirectory, IEnumerable<KeyValuePair<string, object>> varsToTransfer)
        {
            ROOTNET.NTH1.AddDirectory(false);

            //
            // Get the envrionment setup for us. Among other things, we want to make sure everything is compiled. We do this
            // because we want to make sure that none of our sub-processes has to compile anything.
            //

            Init();
            PreExecutionInit(Environment.ClassesToDictify);

            TraceHelpers.TraceInfo(12, "ExecuteQueuedQueries: Loading all extra objects");
            AssembleAndLoadExtraObjects(Environment.ExtraComponentFiles);

            CompileAndLoad(templateFile);

            //
            // Write out the vars to transfer.
            //

            var varsFile = FlattenVarList(queryDirectory, varsToTransfer);

            //
            // Write out a file that contains what we need to transfer for the sub-process to run.
            //

            var outputData = GenerateRunFile(queryDirectory, varsFile, templateFile);

            //
            // Now that is done, lets run it!
            //

            var go = RunProcess(outputData.Item1);
            go.WaitForExit();

            //
            // Final task is to load up the data for the process!
            //

            var results = LoadResults(outputData.Item2);
            return results;
        }

        /// <summary>
        /// The process has completed. Load up the data for it!
        /// </summary>
        /// <param name="outputData"></param>
        /// <returns></returns>
        private IDictionary<string, ROOTNET.Interface.NTObject> LoadResults(FileInfo outputData)
        {
            var results = new Dictionary<string, ROOTNET.Interface.NTObject>();
            var input = ROOTNET.NTFile.Open(outputData.FullName, "READ");

            foreach (var k in input.ListOfKeys)
            {
                results[k.Name] = input.Get(k.Name).Clone();
            }
            input.Close();

            return results;
        }

        /// <summary>
        /// Run a sub-process that uses this outputData as its running input.
        /// </summary>
        /// <param name="outputData"></param>
        private Process RunProcess(FileInfo outputData)
        {
            var pInfo = new ProcessStartInfo(@"C:\Users\gwatts\Documents\ATLAS\Code\LINQtoROOT\LINQToTTree\QueryExecutor\bin\Debug\QueryExecutor.exe", outputData.FullName);

            var p = Process.Start(pInfo);
            return p;
        }

        /// <summary>
        /// Keep track of the number of sub-invokations we've done.
        /// </summary>
        private int _run_count = 0;

        /// <summary>
        /// Generate the XML file that will hold onto the runnign file - the data that will
        /// be used by our sub-process to do the work!
        /// </summary>
        /// <param name="queryDirectory"></param>
        /// <param name="varsFile"></param>
        /// <param name="templateFile"></param>
        /// <returns></returns>
        private Tuple<FileInfo, FileInfo> GenerateRunFile(DirectoryInfo queryDirectory, FileInfo varsFile, FileInfo templateFile)
        {
            var outputObj = new SubProcessRunInfo();

            outputObj.ClassesToDictify = Environment.ClassesToDictify;
            outputObj.ExtraComponentFiles = Environment.ExtraComponentFiles.Select(s => s.FullName).ToArray();
            outputObj.TemplateFile = templateFile.FullName;
            outputObj.TreeName = Environment.TreeName;
            outputObj.RootFiles = Environment.RootFiles.Select(s => s.FullName).ToArray();
            outputObj.VarsToTransferFile = varsFile.FullName;
            outputObj.ResultsFile = new FileInfo(string.Format(@"{0}\SubProcOutput_{1}.root", queryDirectory.FullName, _run_count)).FullName;

            //
            // Now write it out in XML
            //

            var trans = new XmlSerializer(typeof(SubProcessRunInfo));
            var outputFile = new FileInfo(string.Format(@"{0}\SubProcRunInfo_{1}.xml", queryDirectory.FullName, _run_count));
            _run_count++;
            using (var output = File.CreateText(outputFile.FullName))
            {
                trans.Serialize(output, outputObj);
            }

            return Tuple.Create(outputFile, new FileInfo(outputObj.ResultsFile));
        }

        /// <summary>
        /// Take all the vars we have and flatten them to a root file. This is what will be used to send them
        /// to our sub-executables.
        /// </summary>
        /// <param name="varsToTransfer"></param>
        /// <returns></returns>
        private FileInfo FlattenVarList(DirectoryInfo queryDirectory, IEnumerable<KeyValuePair<string, object>> varsToTransfer)
        {
            FileInfo outFile = new FileInfo(string.Format("{0}\\varsToTransfer.root", queryDirectory.FullName));
            var output = ROOTNET.NTFile.Open(outFile.FullName, "RECREATE");

            foreach (var pair in varsToTransfer)
            {
                var o = pair.Value as ROOTNET.Interface.NTNamed;
                if (o == null)
                    throw new InvalidOperationException("Can only transfer named objects as initalizers");
                var cloned = o.Clone(pair.Key);
                cloned.Write();
            }

            output.Close();
            return outFile;
        }

        /// <summary>
        /// Keep track of the local execution environment!
        /// </summary>
        public ExecutionEnvironment Environment { set; get; }
    }
}
