
namespace LINQToTTreeLib.ExecutionCommon.ParallelExes
{
    /// <summary>
    /// Contains all the info required to run a sub-process, and it can
    /// also be serlized to XML.
    /// </summary>
    public class SubProcessRunInfo
    {
        /// <summary>
        /// The list of processes that need to be dict-i-fied.
        /// </summary>
        public string[][] ClassesToDictify { get; set; }

        /// <summary>
        /// Extra files that have to be loaded and run.
        /// </summary>
        public string[] ExtraComponentFiles { get; set; }

        /// <summary>
        /// Location of the template file that contains the query code.
        /// </summary>
        public string TemplateFile { get; set; }

        /// <summary>
        /// The name of the tree we will be running on.
        /// </summary>
        public string TreeName { get; set; }

        /// <summary>
        /// List of root files this process shoudl go over.
        /// </summary>
        public string[] RootFiles { get; set; }

        /// <summary>
        /// List of variables that needs to be transfered
        /// </summary>
        public string VarsToTransferFile { get; set; }

        /// <summary>
        /// Get/Set the location where the output files shoudl be written.
        /// </summary>
        public string ResultsFile { get; set; }
    }
}
