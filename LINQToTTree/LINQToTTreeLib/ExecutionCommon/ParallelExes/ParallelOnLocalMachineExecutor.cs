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
        /// The number of parallel processings we will have running at any one time.
        /// </summary>
        public int NumberOfStreams { get; set; }

        /// <summary>
        /// Initialize everything
        /// </summary>
        public ParallelOnLocalMachineExecutor()
        {
            NumberOfStreams = 1;
        }

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
            // Split running into N inputs.
            //

            int filesPerJob = Environment.RootFiles.Length / NumberOfStreams;
            if (filesPerJob * NumberOfStreams < Environment.RootFiles.Length)
                filesPerJob++;
            var fileLists = (from jobCounter in Enumerable.Range(0, NumberOfStreams)
                             let files = (from f in Environment.RootFiles.Skip(jobCounter * filesPerJob).Take(filesPerJob)
                                          select f).ToArray()
                             where files.Length > 0
                             select files).ToArray();

            //
            // Generate the job file and run for each one of these guys!
            //

            var processTokens = (from list in fileLists
                                 let outputData = GenerateRunFile(queryDirectory, varsFile, templateFile, list)
                                 select new
                                 {
                                     ProcessToken = RunProcess(outputData.Item1),
                                     ResultsLocation = outputData.Item2
                                 }).ToArray();

            foreach (var p in processTokens)
            {
                p.ProcessToken.WaitForExit();
            }

            //
            // Now we have to load and combine the data.
            //

            return CombineResults(processTokens.Select(p => p.ResultsLocation));
        }

        /// <summary>
        /// Load all the results adata, and combine it the best we can.
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private IDictionary<string, ROOTNET.Interface.NTObject> CombineResults(IEnumerable<FileInfo> files)
        {
            var allResults = from r in files
                             select LoadResults(r);

            var groupedResults = from rtable in allResults
                                 from item in rtable
                                 group item.Value by item.Key;


            var results = new Dictionary<string, ROOTNET.Interface.NTObject>();
            foreach (var itemCollection in groupedResults)
            {
                var allItems = itemCollection.ToArray();
                var asHisto = allItems[0] as ROOTNET.Interface.NTH1;
                if (asHisto == null)
                    throw new InvalidOperationException(string.Format("Attempting to merge returned objects of type {0}, but we only know how to deal with TH1's", allItems[0].GetType().Name));
                var collection = new ROOTNET.NTList();
                collection.SetOwner(false);
                foreach (var item in allItems.Skip(1))
                {
                    collection.Add(item);
                }
                asHisto.Merge(collection);
                results[itemCollection.Key] = asHisto;
            }

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
            pInfo.CreateNoWindow = true;
            pInfo.WindowStyle = ProcessWindowStyle.Hidden;

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
        private Tuple<FileInfo, FileInfo> GenerateRunFile(DirectoryInfo queryDirectory, FileInfo varsFile, FileInfo templateFile, FileInfo[] inputRootFiles)
        {
            var outputObj = new SubProcessRunInfo();

            outputObj.ClassesToDictify = Environment.ClassesToDictify;
            outputObj.ExtraComponentFiles = Environment.ExtraComponentFiles.Select(s => s.FullName).ToArray();
            outputObj.TemplateFile = templateFile.FullName;
            outputObj.TreeName = Environment.TreeName;
            outputObj.RootFiles = inputRootFiles.Select(s => s.FullName).ToArray();
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
