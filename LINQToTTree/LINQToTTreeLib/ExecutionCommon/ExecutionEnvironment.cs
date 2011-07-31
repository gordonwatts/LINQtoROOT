
namespace LINQToTTreeLib.ExecutionCommon
{
    /// <summary>
    /// A list of various things that are needed to actually run one of our runs. Hopefully,
    /// other than the stuff specific for a particular query, this is everything.
    /// </summary>
    class ExecutionEnvironment
    {
        /// <summary>
        /// Some classes will require a dict to be made in order for the loading
        /// to proceed. This includes things like user classes.
        /// </summary>
        public string[][] ClassesToDictify { get; set; }

        /// <summary>
        /// These are extra files that have to be loaded in order to run
        /// the query. These include things like user objects.
        /// </summary>
        public System.IO.FileInfo[] ExtraComponentFiles { get; set; }

        /// <summary>
        /// True if we should clean up the files after teh query is run. Otherwise
        /// not. Helpful with debugging.
        /// </summary>
        public bool CleanupQuery { get; set; }

        /// <summary>
        /// Name of the tree we will be loading and processing.
        /// </summary>
        public string TreeName { get; set; }

        /// <summary>
        /// List of the root files that need to be run over.
        /// </summary>
        public System.IO.FileInfo[] RootFiles { get; set; }
    }
}
