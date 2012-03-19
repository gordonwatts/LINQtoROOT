using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Text;

namespace MSBuildTasks
{
    /// <summary>
    /// This task will take input ntuple spec files and produce output spec files. It looks only
    /// for the ntupom file. The other files are just "meta-data" for this task and are internally
    /// </summary>
    public class BuildTTreeDataModel : Task
    {
        /// <summary>
        /// List of input files - .ntupom files - that we will process to make an
        /// output file.
        /// </summary>
        [Required]
        public ITaskItem[] InputFiles { get; set; }

        /// <summary>
        /// Output .cs files that contain everything required to make and use the object model
        /// </summary>
        [Output]
        public ITaskItem[] OutputFiles { get; set; }

        /// <summary>
        /// The namespace we will use to put all the items in
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Run the task!
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            ///
            /// Go through each of the input items and match them to output items, and parse them.
            /// 

            var pairedFiles = InputFiles.Zip(OutputFiles, (i1, i2) => Tuple.Create(i1, i2));

            foreach (var p in pairedFiles)
            {
                if (!RenderDataModel(p.Item1, p.Item2))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Render and parse one of the items.
        /// </summary>
        /// <param name="iTaskItem"></param>
        /// <param name="iTaskItem_2"></param>
        private bool RenderDataModel(ITaskItem inputDM, ITaskItem outputFile)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Namespace))
                    Namespace = "ROOTLINQ";

                var inputXMLFile = new FileInfo(inputDM.GetMetadata("FullPath"));
                var outputCSFile = new FileInfo(outputFile.GetMetadata("FullPath"));
                var generator = new TTreeClassGenerator.ClassGenerator();
                generator.GenerateClasss(inputXMLFile, outputCSFile, Namespace);
            }
            catch (Exception e)
            {
                Log.LogError("ParseError", "ParseError", "ParseError", inputDM.ItemSpec, 0, 0, 0, 0, "Failed to parse: " + DumpExceptoin(e));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Dump an error out
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private string DumpExceptoin(Exception e)
        {
            StringBuilder msg = new StringBuilder();
            msg.AppendFormat("{0} ({1})", e.Message, e.StackTrace);
            if (e.InnerException != null)
                msg.AppendFormat(" - which was caused by {0}", DumpExceptoin(e.InnerException));
            return msg.ToString();
        }
    }
}
