
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace LINQToTTreeLib.ExecutionCommon.ParallelExes
{
    /// <summary>
    /// This object runs the sub-process. Given a filename as input, which points to an XML file, it will
    /// run the query, write the results out, and then "return".
    /// </summary>
    public class SubProcessRunner : LocalBuildBase
    {
        /// <summary>
        /// Master run - should be called poiting to the XML file we are going to run on!
        /// </summary>
        /// <param name="inputFile"></param>
        public void Run(FileInfo inputFile)
        {
            var info = LoadDataFromFile(inputFile);

            //
            // Get the environment setup for this call
            //

            Init();
            PreExecutionInit(info.ClassesToDictify);
            ROOTNET.NTH1.AddDirectory(false);

            TraceHelpers.TraceInfo(12, "ExecuteQueuedQueries: Loading all extra objects");
            AssembleAndLoadExtraObjects(info.ExtraComponentFiles.Select(s => new FileInfo(s)).ToArray());

            //
            // Load the query up
            //

            CompileAndLoad(new FileInfo(info.TemplateFile));

            //
            // Load up the variables
            //

            var varsToTransfer = LoadUpVars(new FileInfo(info.VarsToTransferFile));

            //
            // Run the query...
            //

            var results = RunNtupleQuery(Path.GetFileNameWithoutExtension(info.TemplateFile), varsToTransfer, info.TreeName, info.RootFiles.Select(s => new FileInfo(s)).ToArray());

            //
            // And cache the results!
            //

            SaveResults(results, new FileInfo(info.ResultsFile));
        }

        /// <summary>
        /// Write the files out to the output file and save them for later use!
        /// </summary>
        /// <param name="results"></param>
        /// <param name="outputFileInfo"></param>
        private void SaveResults(Dictionary<string, ROOTNET.Interface.NTObject> results, FileInfo outputFileInfo)
        {
            var output = ROOTNET.NTFile.Open(outputFileInfo.FullName, "RECREATE");

            foreach (var item in results)
            {
                var clone = item.Value.Clone(item.Key);
                clone.Write();
            }

            output.Close();
        }

        /// <summary>
        /// From the files, load up the variables to transfer.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private IDictionary<string, object> LoadUpVars(FileInfo fileInfo)
        {
            var result = new Dictionary<string, object>();
            if (fileInfo.Exists)
            {
                var input = ROOTNET.NTFile.Open(fileInfo.FullName, "READ");
                if (input != null)
                {

                    foreach (var k in input.ListOfKeys)
                    {
                        result[k.Name] = input.Get(k.Name).Clone();
                    }

                    input.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Loads everything we need from the file.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        private SubProcessRunInfo LoadDataFromFile(FileInfo inputFile)
        {
            var trans = new XmlSerializer(typeof(SubProcessRunInfo));
            using (var reader = File.OpenText(inputFile.FullName))
            {
                var obj = trans.Deserialize(reader) as SubProcessRunInfo;
                reader.Close();
                return obj;
            }
        }
    }
}
