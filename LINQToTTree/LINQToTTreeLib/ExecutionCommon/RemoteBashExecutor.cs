using AtlasSSH;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using ROOTNET.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;

namespace LINQToTTreeLib.ExecutionCommon
{    /// <summary>
     /// Craete the executor when needed
     /// </summary>
    [Export(typeof(IQueryExecutorFactory))]
    public class RemoteBashExecutorFactory : IQueryExecutorFactory
    {
        public string Scheme => "remotebash";

        public IQueryExectuor Create(IExecutionEnvironment exeReq, string[] referencedLeafNames)
        {
            return new RemoteBashExecutor() { Environment = exeReq, LeafNames = referencedLeafNames };
        }
    }


    /// <summary>
    /// Used to execute remotely (via an ssh connection)
    /// </summary>
    class RemoteBashExecutor : CommandLineCommonExecutor, IQueryExectuor
    {
        protected override string ExecutorName => "RemoteBashExecutor";

        /// <summary>
        /// Reset all of our internal variables. Used for
        /// testing.
        /// </summary>
        internal static void ResetRemoteBashExecutor()
        {
        }

        /// <summary>
        /// Place holder for a temp directory
        /// </summary>
        string linuxTempDir = null;


        /// <summary>
        /// Blast this out when we fail to execute some sort of command.
        /// </summary>
        [Serializable]
        public class RemoteBashCommandFailureException : Exception
        {
            public RemoteBashCommandFailureException() { }
            public RemoteBashCommandFailureException(string message) : base(message) { }
            public RemoteBashCommandFailureException(string message, Exception inner) : base(message, inner) { }
            protected RemoteBashCommandFailureException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// Given a ROOT script, run it on the remote machine.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="cmds"></param>
        /// <param name="tmpDir"></param>
        /// <param name="dumpLine"></param>
        /// <param name="verbose"></param>
        /// <remarks>
        /// We assume that ROOT is already configured. If the command line execution fails, we will
        /// throw.
        /// </remarks>
        internal override void ExecuteRootScript(string prefix, string cmds, DirectoryInfo tmpDir, Action<string> dumpLine = null, bool verbose = false,
            IEnumerable<Uri> extraFiles = null,
            IEnumerable<Uri> receiveFiles = null,
            TimeSpan? timeout = null)
        {
            // First, create the file for output. This has to be done in Linux line endings (as we assume we are
            // going to a linux machine for this).
            var scriptFile = new FileInfo(Path.Combine(tmpDir.FullName, $"{prefix}.C"));
            using (var rdr = new StringReader(cmds))
            {
                using (var wtr = scriptFile.CreateText())
                {
                    foreach (var line in rdr.EnumerateLines())
                    {
                        wtr.Write($"{line}\n");
                    }
                }
            }

            // Send the file to the remote host
            ExecuteRemoteWithTemp($"/tmp/{tmpDir.Name}", sshConnection => {
                // Files we want to send or recv first.
                _filesToCopyOver.Add(new RemoteFileCopyInfo() { localFileName = scriptFile, remoteLinuxDirectory = linuxTempDir });
                if (extraFiles != null)
                {
                    foreach (var f in extraFiles)
                    {
                        _filesToCopyOver.Add(new RemoteFileCopyInfo(new FileInfo(f.LocalPath), tmpDir, linuxTempDir));
                    }
                }
                if (receiveFiles != null)
                {
                    foreach (var f in receiveFiles)
                    {
                        _filesToBringBack.Add(new RemoteFileCopyInfo(new FileInfo(f.LocalPath), tmpDir, linuxTempDir));
                    }
                }

                // Send over all files
                var logForError = new StringBuilder();
                SendAllFiles(sshConnection, s => RecordLine(logForError, s, dumpLine));

                // Next, lets see if we can't run the file against root.
                try
                {
                    sshConnection.Connection.ExecuteLinuxCommand($"cd {linuxTempDir}", processLine: s => RecordLine(logForError, s, dumpLine));
                    sshConnection.Connection.ExecuteLinuxCommand($"root -l -b -q {scriptFile.Name}", processLine: s => RecordLine(logForError, s, dumpLine),
                        secondsTimeout: timeout.HasValue ? (int) timeout.Value.TotalSeconds : 60*60);
                } catch (Exception e)
                {
                    throw new RemoteBashCommandFailureException($"Failed to execute script: {logForError}", e);
                }

                // Finally, if there are any files to bring back, we should!
                ReceiveAllFiles(sshConnection, dumpLine);

                return (object) null;
            }, dumpLine);
        }

        /// <summary>
        /// Return the machine config
        /// </summary>
        private MachineConfig Machine
        {
            get
            {
                // This will use the URI's to determine what machine config (once) and then cache it.
                // Not implemented yet.
                return GetMachineInfo("bogus");
            }
        }

        /// <summary>
        /// Run a bash script on the remote node
        /// </summary>
        /// <param name="fnameRoot"></param>
        /// <param name="commands"></param>
        /// <param name="dumpLine"></param>
        /// <param name="verbose"></param>
        internal void ExecuteBashScript(string fnameRoot, string commands, Action<string> dumpLine, bool verbose)
        {
            // Create the file that will contain the scripts
            var tmpDir = new DirectoryInfo(Path.GetTempPath());
            var scriptFile = new FileInfo(Path.Combine(tmpDir.FullName, $"{fnameRoot}.sh"));
            using (var rdr = new StringReader(commands))
            {
                using (var wtr = scriptFile.CreateText())
                {
                    foreach (var line in rdr.EnumerateLines())
                    {
                        wtr.Write($"{line}\n");
                    }
                }
            }

            // Now, run with the remote stuff setup.
            ExecuteRemoteWithTemp($"/tmp/{fnameRoot}", connection =>
            {
                // Queue up files to send and recv
                _filesToCopyOver.Add(new RemoteFileCopyInfo() { localFileName = scriptFile, remoteLinuxDirectory = linuxTempDir });

                // Run, pushing and pulling files we need
                SendAllFiles(connection, dumpLine);
                var logForError = new StringBuilder();
                try
                {
                    connection.Connection.ExecuteLinuxCommand($"bash {linuxTempDir}/{scriptFile.Name}", s => RecordLine(logForError, s, dumpLine));
                } catch (Exception e)
                {
                    throw new RemoteBashCommandFailureException($"Failed to execute bash script: {logForError.ToString()}.", e);
                }
                ReceiveAllFiles(connection, dumpLine);
                return (object)null;
            }, dumpLine);
        }

        /// <summary>
        /// Generate a proxy.
        /// </summary>
        /// <param name="rootFiles"></param>
        /// <param name="treeName"></param>
        /// <param name="queryDirectory"></param>
        /// <returns></returns>
        public override FileInfo GenerateProxyFile(Uri[] rootFiles, string treeName, DirectoryInfo queryDirectory)
        {
            Action<string> dumpLine = l =>
            {
                if (Environment.CompileDebug)
                {
                    Console.WriteLine(l);
                }
            };
            return ExecuteRemoteWithTemp("/tmp/proxygen", SSHConnection =>
            {
                return base.GenerateProxyFile(rootFiles, treeName, queryDirectory);
            }, dumpLine);
        }

        /// <summary>
        /// Make sure execute has a directory where it can go to!
        /// </summary>
        /// <param name="queryFile"></param>
        /// <param name="queryDirectory"></param>
        /// <param name="varsToTransfer"></param>
        /// <returns></returns>
        public override IDictionary<string, NTObject> Execute(FileInfo queryFile, DirectoryInfo queryDirectory, IEnumerable<KeyValuePair<string, object>> varsToTransfer)
        {
            Action<string> dumper = l =>
            {
                if (Environment.CompileDebug)
                {
                    Console.WriteLine(l);
                }
            };
            return ExecuteRemoteWithTemp($"/tmp/{queryDirectory.Name}", SSHConnection =>
            {
                // Load up extra files that need to be shipped over.
                foreach (var f in Environment.ExtraComponentFiles)
                {
                    _filesToCopyOver.Add(new RemoteFileCopyInfo() { localFileName = f, remoteLinuxDirectory = linuxTempDir });
                }

                return base.Execute(queryFile, queryDirectory, varsToTransfer);
            }, dumper);
        }

        /// <summary>
        /// Run on the remote guy in a temp dir that we clean up
        /// </summary>
        /// <param name="tempDir"></param>
        /// <param name="act"></param>
        /// <param name="dumpLine"></param>
        internal T ExecuteRemoteWithTemp<T>(string tempDir, Func<SSHTunneledConnection, T> act, Action<string> dumpLine = null)
        {
            // Protection against stupid mistakes
            if (!tempDir.StartsWith("/tmp"))
            {
                throw new InvalidOperationException($"Trying to execute linux commands in a place other than /tmp: '{tempDir}'.");
            }

            // Make the connection, and create the directory.
            var sshConnection = MakeSSHConnection(dumpLine);
            var oldLinuxTempDir = linuxTempDir;
            linuxTempDir = tempDir;
            try
            {
                // Get the temp directory setup and going
                if (linuxTempDir != oldLinuxTempDir)
                {
                    sshConnection.Connection.ExecuteLinuxCommand($"rm -rf {linuxTempDir}", processLine: l => RecordLine(null, l, dumpLine));
                    sshConnection.Connection.ExecuteLinuxCommand($"mkdir {linuxTempDir}", processLine: l => RecordLine(null, l, dumpLine));
                }
                dumpLine?.Invoke($"Executing commands in new directory {linuxTempDir}");

                return act(sshConnection);
            }
            finally
            {
                if (linuxTempDir != oldLinuxTempDir)
                {
                    sshConnection.Connection.ExecuteLinuxCommand($"rm -rf {linuxTempDir}", processLine: l => RecordLine(null, l, dumpLine));
                    linuxTempDir = oldLinuxTempDir;
                }
            }
        }

        /// <summary>
        /// Send all files over to the remote client. Make sure the directory exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="dumpLine"></param>
        private void SendAllFiles(SSHTunneledConnection connection, Action<string> dumpLine)
        {
            foreach (var f in _filesToCopyOver)
            {
                string linuxPath = $"{f.remoteLinuxDirectory}/{f.localFileName.Name}";
                dumpLine?.Invoke($"Copying {f.localFileName.Name} -> {linuxPath}");
                connection.Connection.ExecuteLinuxCommand($"mkdir -p {f.remoteLinuxDirectory}", dumpLine);
                connection.Connection.CopyLocalFileRemotely(f.localFileName, linuxPath);
            }
            _filesToCopyOver.Clear();
        }

        /// <summary>
        /// Fetch back all files we probably need to know about.
        /// </summary>
        /// <param name="sshConnection"></param>
        /// <param name="dumpLine"></param>
        private void ReceiveAllFiles(SSHTunneledConnection connection, Action<string> dumpLine)
        {
            foreach (var f in _filesToBringBack)
            {
                string linuxPath = $"{f.remoteLinuxDirectory}/{f.localFileName.Name}";
                dumpLine?.Invoke($"Copying {linuxPath} -> {f.localFileName.Directory}");
                f.localFileName.Directory.Refresh();
                if (!f.localFileName.Directory.Exists)
                {
                    f.localFileName.Directory.Create();
                }
                connection.Connection.CopyRemoteFileLocally(linuxPath, f.localFileName.Directory);
            }
            _filesToBringBack.Clear();
        }


        /// <summary>
        /// Fetch a file from the remote location down here.
        /// </summary>
        /// <param name="fileToGet"></param>
        /// <param name="dirName"></param>
        /// <param name="connection"></param>
        /// <param name="statusUpdate"></param>
        private void ReceiveFile (FileInfo fileToGet, string dirName, SSHTunneledConnection connection, Action<string>statusUpdate = null)
        {
            string linuxPath = $"{dirName}/{fileToGet.Name}";
            connection.Connection.CopyRemoteFileLocally(linuxPath, fileToGet.Directory, statusUpdate);
        }

        /// <summary>
        /// Cache the connection
        /// </summary>
        private SSHTunneledConnection _connection = null;

        class RemoteFileCopyInfo
        {
            public FileInfo localFileName;
            public string remoteLinuxDirectory;

            /// <summary>
            /// Default ctor
            /// </summary>
            public RemoteFileCopyInfo()
            {
            }

            /// <summary>
            /// Create a copy over tag, calculate a relative directory if need be
            /// </summary>
            /// <param name="localFile"></param>
            /// <param name="relativeDirectory"></param>
            /// <param name="remoteLinuxPath"></param>
            public RemoteFileCopyInfo(FileInfo localFile, DirectoryInfo relativeDirectory, string remoteLinuxPath)
            {
                localFileName = localFile;
                
                // Deal with a relative path trigger
                if (localFile.Directory.FullName.StartsWith(relativeDirectory.FullName))
                {
                    var remoteLinuxPathStub = localFile.Directory.FullName
                        .Substring(relativeDirectory.FullName.Length)
                        .Replace(@"\", "/");
                    if (remoteLinuxPathStub.StartsWith("/"))
                    {
                        remoteLinuxPathStub = remoteLinuxPathStub.Substring(1);
                    }
                    // Remove the filename - this is a directory we want to record!
                    remoteLinuxDirectory = $"{remoteLinuxPath}/{remoteLinuxPathStub}";
                } else
                {
                    remoteLinuxDirectory = remoteLinuxPath;
                }
            }

            public override int GetHashCode()
            {
                return localFileName.FullName.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (!(obj is RemoteFileCopyInfo))
                {
                    return false;
                }
                return localFileName.FullName == (obj as RemoteFileCopyInfo).localFileName.FullName;
            }
        }

        /// <summary>
        /// Track all the files we need to move over to the remote area.
        /// </summary>
        private HashSet<RemoteFileCopyInfo> _filesToCopyOver = new HashSet<RemoteFileCopyInfo>();

        /// <summary>
        /// A list of files we should bring back
        /// </summary>
        private HashSet<RemoteFileCopyInfo> _filesToBringBack = new HashSet<RemoteFileCopyInfo>();

        /// <summary>
        /// Create an SSH connection.
        /// </summary>
        /// <param name="dumpLine">Dump output from running various setup lines</param>
        /// <returns></returns>
        private SSHTunneledConnection MakeSSHConnection(Action<string> dumpLine = null)
        {
            if (_connection == null)
            {
                _connection = CreateSSHConnectionTo(Machine.RemoteSSHConnectionString);

                if (Machine.ConfigureLines != null)
                {
                    var logForError = new StringBuilder();
                    try
                    {
                        // Don't dump setup unless the user really wants to see it!
                        var localdumper = Environment.Verbose | Environment.CompileDebug
                            ? dumpLine
                            : (Action<string>)null;

                        foreach (var line in Machine.ConfigureLines)
                        {
                            _connection.Connection.ExecuteLinuxCommand(line, processLine: s => RecordLine(logForError, s, localdumper));
                        }
                    } catch (Exception e)
                    {
                        throw new RemoteBashCommandFailureException($"Error making a SSH connection: {logForError.ToString()}", e);
                    }
                }
            }
            return _connection;
        }

        /// <summary>
        /// Disposable holder for the full connection
        /// </summary>
        internal class SSHTunneledConnection : IDisposable
        {
            /// <summary>
            /// List of our connections.
            /// </summary>
            List<SSHConnection> _connections = new List<SSHConnection>();

            /// <summary>
            /// Get the connection to the inner most tunnel
            /// </summary>
            public SSHConnection Connection { get; private set; }

            /// <summary>
            /// Add a new connection
            /// </summary>
            /// <param name="c"></param>
            public void Add (SSHConnection c)
            {
                _connections.Add(c);
                Connection = c;
            }

            /// <summary>
            /// Make sure we exit everything outside and close off this connection!
            /// </summary>
            public void Dispose()
            {
                foreach (var t in _connections.Reverse<SSHConnection>())
                {
                    t.Dispose();
                }
            }
        }

        /// <summary>
        /// Create an SSH connection to a remote machine
        /// </summary>
        /// <param name="remoteSSHConnectionString"></param>
        /// <returns></returns>
        private SSHTunneledConnection CreateSSHConnectionTo(string remoteSSHConnectionString)
        {
            var mlist = remoteSSHConnectionString
                .Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Select(s => ExtractUserAndPassword(s));

            var r = new SSHTunneledConnection();
            var firstInfo = mlist.First();
            var c = new SSHConnection(firstInfo.Item2, firstInfo.Item1);
            r.Add(c);
            foreach (var mConnection in mlist.Skip(1))
            {
                var newC = r.Connection.SSHTo(mConnection.Item2, mConnection.Item1);
            }

            return r;
        }

        /// <summary>
        /// Extract a username and password
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private Tuple<string,string> ExtractUserAndPassword(string v)
        {
            var t = v.Split('@');
            if (t.Length != 2)
            {
                throw new ArgumentException($"Looking for a SSH connection in the form of user@machine, but found '{v}' instead!");
            }
            return Tuple.Create(t[0], t[1]);
        }

        /// <summary>
        /// Turn the file into one that we can see over on our remote host. The trick here is that we will have to copy it over.
        /// </summary>
        /// <param name="finfo"></param>
        /// <returns></returns>
        protected override string NormalizeFileForTarget(Uri finfo)
        {
            // See if this file has already been setup as file that is remote
            if (finfo.Scheme == "remotebash")
            {
                if (!finfo.AbsolutePath.Contains(":"))
                {
                    // TODO: with new version of AtlasSSH we can remove this line.
                    return finfo.AbsolutePath.Replace("//", "/");
                }
            }

            // See if this file matches a remote file.
            var f = new FileInfo(finfo.LocalPath.StartsWith("/") ? finfo.LocalPath.Substring(1) : finfo.LocalPath);
            var remoteFile = MatchToRemoteFile(f.FullName);
            if (remoteFile != null)
            {
                return remoteFile;
            }

            // See if the file exists or not locally.
            if (f.Exists)
            {
                // Push it to the remote host.
                if (linuxTempDir == null)
                {
                    throw new InvalidOperationException($"Attempt to copy over file {f.Name} when we aren't in the middle of an operation!");
                }
                _filesToCopyOver.Add(new RemoteFileCopyInfo() { localFileName = f, remoteLinuxDirectory = linuxTempDir });
            } else
            {
                // Perhaps it doesn't exist because we want to copy it back here?
                _filesToBringBack.Add(new RemoteFileCopyInfo() { localFileName = new FileInfo(finfo.LocalPath), remoteLinuxDirectory = linuxTempDir});
            }

            // It will just be in the local directory where we live.
            return $"{linuxTempDir}/{f.Name}";
        }

        /// <summary>
        /// Look at the machine definition to see if this makes sense or not.
        /// </summary>
        /// <param name="localPath"></param>
        /// <returns></returns>
        private string MatchToRemoteFile(string localPath)
        {
            return Machine.Matches
                .Select(m => m.ReplaceWithThis(localPath))
                .FirstOrDefault();
        }

        /// <summary>
        /// We want to send a whole directory over. This is bad. We have a bunch of work to do here.
        /// </summary>
        /// <param name="finfo"></param>
        /// <returns></returns>
        protected override string NormalizeFileForTarget(DirectoryInfo finfo)
        {
            if (finfo.Exists)
            {
                var goodExtensiosn = new[] { ".h", ".hpp", ".c", ".cxx" };
                var c = MakeSSHConnection();
                foreach (var f in finfo.EnumerateFiles().Where(sf => goodExtensiosn.Contains(sf.Extension.ToLower())))
                {
                    _filesToCopyOver.Add(new RemoteFileCopyInfo() { localFileName = f, remoteLinuxDirectory = linuxTempDir });
                }
            }
            return linuxTempDir;
        }

        /// <summary>
        /// Need to see if the remote machine has ROOT installed.
        /// </summary>
        /// <param name="dumpLine"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        internal override bool CheckForROOTInstall(Action<string> dumpLine, bool verbose)
        {
            // Simple script to execute
            var cmd = new StringBuilder();
            cmd.AppendLine("void testForRoot() {int i = 10;}");

            try
            {
                ExecuteRootScript("testForRoot", cmd.ToString(), new DirectoryInfo(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)), dumpLine, verbose);
            } catch (ArgumentOutOfRangeException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// The config for a machine. We load it and hold it in memory.
        /// </summary>
        public class MachineConfig
        {
            /// <summary>
            /// Contains the connection string that we will use to "dial" up the other location
            /// (that we pass to AtlasSSH).
            /// </summary>
            public string RemoteSSHConnectionString;

            /// <summary>
            /// Lines we need to execute in order to configure the
            /// the remote machine to be ready to run ROOT.
            /// </summary>
            public string[] ConfigureLines;

            public class FileMatchPattern
            {
                private string _matchString;
                private string _replaceString;

                public FileMatchPattern (string matchString, string replaceString)
                {
                    _matchString = matchString;
                    _replaceString = replaceString;
                }

                /// <summary>
                /// Return the remote file location or null if we don't match
                /// </summary>
                /// <param name="localFile"></param>
                /// <returns></returns>
                public string ReplaceWithThis (string localFile)
                {
                    if (localFile == _matchString)
                    {
                        return _replaceString;
                    }
                    return null;
                }
            }

            /// <summary>
            /// List of matches
            /// </summary>
            public List<FileMatchPattern> Matches { get; } = new List<FileMatchPattern>();

            /// <summary>
            /// Add a pattern for a file that isn't coiped over to the remote machine
            /// </summary>
            /// <param name="localPath">Match string on this local machine we want to conver</param>
            /// <param name="remotePath">The location on the remote host that should get returned</param>
            internal void AddFileNoCopyPattern(string localPath, string remotePath)
            {
                Matches.Add(new FileMatchPattern(localPath, remotePath));
            }
        }

        static MachineConfig _s_global_config = null;
        /// <summary>
        /// Find the config for a particular machine.
        /// </summary>
        /// <returns></returns>
        public static MachineConfig GetMachineInfo(string clusterName)
        {
            if (_s_global_config == null)
            {
                _s_global_config = new MachineConfig()
                {
                    RemoteSSHConnectionString = "gwatts@tev01.phys.washington.edu",
                    ConfigureLines = new[] { "setupATLAS", "lsetup root" }
                };
            }
            return _s_global_config;
        }


        /// <summary>
        /// Thrown when we try to install root remotely.
        /// </summary>
        [Serializable]
        public class ROOTCantBeInstalledRemotelyException : Exception
        {
            public ROOTCantBeInstalledRemotelyException() { }
            public ROOTCantBeInstalledRemotelyException(string message) : base(message) { }
            public ROOTCantBeInstalledRemotelyException(string message, Exception inner) : base(message, inner) { }
            protected ROOTCantBeInstalledRemotelyException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// We have been requested to install ROOT on the remote system.
        /// Currently we fail at this - we expect it to be properly installed.
        /// </summary>
        /// <param name="dumpLine"></param>
        /// <param name="verbose"></param>
        /// <remarks>
        /// We aren't installing root because these are often big machines where ROOT is already present.
        /// If we need this, this functionality can be added.
        /// </remarks>
        internal override void InstallROOT(Action<string> dumpLine, bool verbose)
        {
            throw new ROOTCantBeInstalledRemotelyException("Unable to install root on a remote system over ssh");
        }
    }
}
