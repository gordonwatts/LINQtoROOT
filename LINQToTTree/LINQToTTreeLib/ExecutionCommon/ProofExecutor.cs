using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LINQToTTreeLib.ExecutionCommon
{
    /// <summary>
    /// Execute a query on a remote proof cluster.
    /// </summary>
    public class ProofExecutor : IQueryExectuor
    {
        [Serializable]
        public class ProofException : Exception
        {
            /// <summary>
            /// Use the log info and a custom text message to generate an exception.
            /// </summary>
            /// <param name="message"></param>
            /// <param name="log"></param>
            /// <returns></returns>
            private static string MakeMessage(string message, ROOTNET.Interface.NTMacro log)
            {
                var bld = new StringBuilder();

                bld.AppendLine(message);
                if (log != null)
                {
                    bld.AppendLine("  PROOF log:");
                    foreach (var line in log.AsStrings())
                    {
                        bld.AppendLine(string.Format("  -> {0}", line));
                    }
                }

                return bld.ToString();
            }

            private static string MakeMessage(string message, IEnumerable<ROOTNET.Interface.NTMacro> logs)
            {
                var bld = new StringBuilder();

                bld.AppendLine(message);
                if (logs != null)
                {
                    bld.AppendLine("  PROOF log:");
                    foreach (var log in logs)
                    {
                        foreach (var line in log.AsStrings())
                        {
                            bld.AppendLine(string.Format("  -> {0}", line));
                        }
                    }
                }

                return bld.ToString();
            }

            public ProofException() { }
            public ProofException(string message, ROOTNET.Interface.NTMacro log) : base(MakeMessage(message, log)) { }
            public ProofException(string message, Exception inner, ROOTNET.Interface.NTMacro log) : base(MakeMessage(message, log), inner) { }

            public ProofException(string message, IEnumerable<ROOTNET.Interface.NTMacro> log) : base(MakeMessage(message, log)) { }
            public ProofException(string message, Exception inner, IEnumerable<ROOTNET.Interface.NTMacro> log) : base(MakeMessage(message, log), inner) { }
            protected ProofException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }

        /// <summary>
        /// Keep track of all modules that we've loaded
        /// </summary>
        private List<string> _loadedModuleNames = new List<string>();

        /// <summary>
        /// Run the execution on the PROOF cluster specified in the list of URI's we need to execute.
        /// </summary>
        /// <param name="templateFile"></param>
        /// <param name="queryDirectory"></param>
        /// <param name="varsToTransfer"></param>
        /// <returns></returns>
        public IDictionary<string, ROOTNET.Interface.NTObject> Execute(FileInfo templateFile, DirectoryInfo queryDirectory, IEnumerable<KeyValuePair<string, object>> varsToTransfer)
        {
            //
            // x-check parameters
            //

            if (templateFile == null)
                throw new ArgumentNullException("A template file must exist for this to work!");
            if (!templateFile.Exists)
                throw new FileNotFoundException("Unable to find C++ code to use for PROOF query", templateFile.FullName);

            if (Environment.ExtraComponentFiles != null && Environment.ExtraComponentFiles.Length > 0)
                throw new NotSupportedException("PROOF executor does not support extra component files!");

            //
            // Make sure everythign is inialized and ready for this query.
            //

            ExecutionUtilities.Init();
            OrganizeUris();

            //
            // Put us in the query directory
            //

            var oldDir = System.Environment.CurrentDirectory;
            System.Environment.CurrentDirectory = queryDirectory.FullName;
            ROOTNET.NTSystem.gSystem.ChangeDirectory(queryDirectory.FullName);
            var cdr = ROOTNET.NTSystem.gSystem.WorkingDirectory();

            //
            // A key "funny" thing about this is we need to track all the files used by this query and make sure they make it up to the
            // PROOF server. So we need to scan for all files and figure out where they are, and add them to a list of files that needs
            // to be sent.
            //

            var pc = GetProofConnection();

            var allIncludeFiles = ExecutionUtilities.FindGoodIncludeFilesRecursive(templateFile);
            var fList = new StringBuilder();
            fList.Append(string.Format("{0}+", templateFile.Name));
            foreach (var f in allIncludeFiles)
            {
                fList.Append(string.Format(",{0}", f.Name));
                PushFileToMaster(pc, f.Name);
            }

            //
            // Next, we need to build the proof job and the objects, and make sure something silly didn't go wrong!
            //

            var r = pc.Load(fList.ToString());
            if (r != 0)
                throw new InvalidOperationException(string.Format("Unable to compile and load selector in PROOF. Error: {0}", r));
            _loadedModuleNames.Add(templateFile.Name.Replace(".", "_"));

            var objName = Path.GetFileNameWithoutExtension(templateFile.Name);
            var cls = ROOTNET.NTClass.GetClass(objName);
            if (cls == null)
                throw new InvalidOperationException(string.Format("Unable to locate compiled object named '{0}'.", objName));

            //
            // Load up the variables that need to go over to the cluster.
            //

            TraceHelpers.TraceInfo(20, "RunNtupleQuery: Saving the objects we are going to ship over");
            pc.InputList.Clear();

            var oldHSet = ROOTNET.NTH1.AddDirectoryStatus();
            ROOTNET.NTH1.AddDirectory(false);
            foreach (var item in varsToTransfer)
            {
                var obj = item.Value as ROOTNET.Interface.NTNamed;
                if (obj == null)
                    throw new InvalidOperationException("Can only deal with named objects");
                var cloned = obj.Clone(item.Key);
                pc.InputList.Add(cloned);
            }
            ROOTNET.NTH1.AddDirectory(oldHSet);

            //
            // Now run the PROOF query
            //

            var rResult = pc.Process(PROOFDSNames(), objName, "");

            //
            // We need to do two things to get at the actual status of the run:
            //  1. Look at the TStatus object called "PROOF_Status"
            //  2. Check to see if GetMissingFIles() is empty.
            // http://root.cern.ch/phpBB3/viewtopic.php?f=13&t=14536&p=63081#p63081
            //

            bool runOK = true;
            string failReason = "";
            var tstatus = pc.GetOutputList().Where(i => i is ROOTNET.Interface.NTStatus).Cast<ROOTNET.Interface.NTStatus>().Where(i => i.Name == "PROOF_Status").FirstOrDefault();
            if (tstatus == null)
            {
                runOK = false;
            }
            else
            {
                runOK = tstatus.ExitStatus == 0;
                if (!runOK)
                    failReason = string.Format("PROOF query status, {0}, was not zero.", tstatus.ExitStatus);
            }

            if (runOK)
            {
                runOK = pc.GetMissingFiles().NFiles == 0;
                if (!runOK)
                    failReason = string.Format("PROOF query failed because there were {0} missing files", pc.GetMissingFiles().NFiles);
            }


            //
            // There is an error if the result is non zero or there is an error in the short log.
            // If there is an error, build a full log message and throw it!
            //

            //if (rResult != 0 || logMessages.ContainsString("kPROOF_FATAL"))
            if (!runOK)
            {
                var logMessages = pc.GetLastLog();
                var mgr = pc.GetManager();
                var logs = mgr.GetSessionLogs();
                var macroLogs = logs.ListOfLogs.Cast<ROOTNET.Interface.NTProofLogElem>().Select(mc => mc.Macro);
                throw new ProofException(string.Format("Faild to run PROOF query (error from Process method was {0})", rResult), new ROOTNET.Interface.NTMacro[] { logMessages }.Concat(macroLogs));
            }

            //
            // Next, grab all the PROOF objects
            //

            var outputList = pc.GetOutputList();
            var result = new Dictionary<string, ROOTNET.Interface.NTObject>();
            foreach (var o in outputList)
            {
                var name = o.Name;
                result[name] = o;
            }

            //
            // Clean up
            //

            System.Environment.CurrentDirectory = oldDir;
            Console.WriteLine(" ****** Can't delete the directory containing the source code because PROOF won't let it go.");
#if false
            //
            // Now that it is run, we can unload everything we loaded up!
            //

            _proofConnection.Close();
            _proofConnection = null;

            ExecutionUtilities.UnloadAllModules(_loadedModuleNames);

            if (Environment.CleanupQuery)
            {
                if (queryDirectory != null)
                    if (queryDirectory.Exists)
                        queryDirectory.Delete();
            }
#endif

            //
            // Return back all the objects that came back from the script
            //

            return result;
        }

        /// <summary>
        /// Generate the dataset names for PROOF to address in the proper format.
        /// </summary>
        /// <returns></returns>
        private string PROOFDSNames()
        {
            //
            // Some x-checks to make sure we are not doing anything crazy
            //

            if (_datasets.Length != 1)
                throw new InvalidOperationException("Currently can't deal a PROOF query that isn't just one dataset");

            if (Environment.TreeName == null)
                throw new ArgumentNullException("Tree name can't be null");

            return string.Format("{0}#{1}", _datasets[0], Environment.TreeName);
        }


        /// <summary>
        /// True if we've parsed the info we need from the environment.
        /// </summary>
        private bool _urisOrganized = false;

        /// <summary>
        /// The net name of the proof host.
        /// </summary>
        private string _proofHost;

        /// <summary>
        /// The list of PROOF datasets
        /// </summary>
        private string[] _datasets;

        /// <summary>
        /// Executed once, to x-check the Uri's we are going to be looking at.
        /// </summary>
        private void OrganizeUris()
        {
            if (_urisOrganized)
                return;

            //
            // Check for correctly formatted URL's
            //

            var allgood = (from u in Environment.RootFiles select u.Segments.Length).All(c => c == 2);
            if (!allgood)
                throw new ArgumentException("Dataset formats are not correct: only a proof machine name and a dataset name are allowed");

            //
            // We can deal with multiple data-sets, but only a single host. Strip the leading "/" off the dataset name when we
            // look at this as a URI segment (the substring below).
            //

            var hosts = (from u in Environment.RootFiles
                         group u.PathAndQuery.Substring(1) by u.Host).ToArray();
            if (hosts.Length != 1)
                throw new ArgumentException("There were more than one host for the list of PROOF datasets. This is not supported");

            _proofHost = hosts[0].Key;
            _datasets = hosts[0].ToArray();

            //
            // Finally, check to see if these datasets exist up there!
            //

            var proof = GetProofConnection();

            //
            // Next, see if the data sets are good.
            //

            foreach (var ds in _datasets)
            {
                if (!proof.ExistsDataSet(ds))
                {
                    throw new ArgumentException(string.Format("The proof server at {0} does not know about the dataset '{1}'", _proofHost, ds));
                }
            }

            //
            // Made it all the way through!
            //

            _urisOrganized = true;
        }

        /// <summary>
        /// Keep track of the local proof connection.
        /// </summary>
        private ROOTNET.Interface.NTProof _proofConnection;

        /// <summary>
        /// Open and return a connection to a proof cluster.
        /// </summary>
        /// <returns></returns>
        private ROOTNET.Interface.NTProof GetProofConnection()
        {
            if (_proofConnection == null)
            {
                _proofConnection = ROOTNET.NTProof.Open(string.Format("proof://{0}", _proofHost));
                if (_proofConnection == null)
                    throw new ArgumentException(string.Format("Unable to open proof server at '{0}'", _proofHost));
            }
            if (!_proofConnection.IsValid())
                throw new ArgumentException(string.Format("Failed to open proof server at '{0}'", _proofHost));
            return _proofConnection;
        }

        /// <summary>
        /// Contains the environment needed to run the execution.
        /// </summary>
        public ExecutionEnvironment Environment { get; set; }

        /// <summary>
        /// Push a file to make sure it gets up there.
        /// </summary>
        /// <param name="proof"></param>
        /// <param name="file"></param>
        private static void PushFileToMaster(ROOTNET.Interface.NTProof proof, string file)
        {
            var masterLocation = string.Format("~/session-{0}/master-0-{1}/", proof.SessionTag, proof.SessionTag);

            var mgr = proof.Manager;
            mgr.PutFile(file, masterLocation);
        }
    }

    /// <summary>
    /// Some helpers for us.
    /// </summary>
    public static class ProofHelpers
    {
        /// <summary>
        /// Does the log contain anything "interesting"?
        /// </summary>
        /// <param name="macro"></param>
        /// <returns></returns>
        public static bool ContainsString(this ROOTNET.Interface.NTMacro macro, string whatisinthere)
        {
            return macro.AsStrings().Any(l => l.Contains(whatisinthere));
        }

        /// <summary>
        /// Return the log as a series of strings.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public static IEnumerable<string> AsStrings(this ROOTNET.Interface.NTMacro log)
        {
            foreach (var line in log.GetListOfLines().Cast<ROOTNET.Interface.NTObjString>())
            {
                if (line != null)
                {
                    yield return line.Name;
                }
            }
        }
    }
}
