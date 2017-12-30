using AtlasSSH;
using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.Utils;
using Polly;
using ROOTNET.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class RemoteBashExecutor : CommandLineCommonExecutor, IQueryExectuor
    {
        /// <summary>
        /// Return the executor name to help with error messages.
        /// </summary>
        protected override string ExecutorName => "RemoteBashExecutor";

        /// <summary>
        /// Alter to change the default setting for the root version number
        /// </summary>
        private const string DefaultROOTVersionString = "6.10.06-x86_64-slc6-gcc62-opt";

        /// <summary>
        /// The version number for root
        /// </summary>
        public static string ROOTVersionNumber { get; set; } = DefaultROOTVersionString;

        /// <summary>
        /// Reset all of our internal variables. Used for
        /// testing.
        /// </summary>
        internal static void ResetRemoteBashExecutor()
        {
            ROOTVersionNumber = DefaultROOTVersionString;
        }

        /// <summary>
        /// Place holder for a temp directory
        /// </summary>
        private string _linuxTempDir = null;

        /// <summary>
        /// Track the command phase - so we can see when we need to keep it the same or change it.
        /// </summary>
        private string _currentLinuxPhase = "";

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
        internal override async Task ExecuteRootScript(string prefix, string cmds, DirectoryInfo tmpDir, Action<string> dumpLine = null, bool verbose = false,
            IEnumerable<Uri> extraFiles = null,
            IEnumerable<Uri> receiveFiles = null,
            TimeSpan? timeout = null)
        {
            // Run against a temp directory on the remote host
            await ExecuteRemoteWithTemp(prefix, async sshConnection => {

                // Parse for <><> style file replacements. This will call normalize to send over
                // files, so we need to do this inside the execution environment.
                var tcommands = this.ReWritePathsInQuery(cmds);

                // First, create the file for output. This has to be done in Linux line endings (as we assume we are
                // going to a linux machine for this).
                var scriptFile = new FileInfo(Path.Combine(tmpDir.FullName, $"{prefix}.C"));
                using (var rdr = new StringReader(tcommands))
                {
                    using (var wtr = scriptFile.CreateText())
                    {
                        foreach (var line in rdr.EnumerateLines())
                        {
                            wtr.Write($"{line}\n");
                        }
                    }
                }

                // Files we want to send or recv first.
                _filesToCopyOver.Add(new RemoteFileCopyInfo() { localFileName = scriptFile, remoteLinuxDirectory = _linuxTempDir });
                if (extraFiles != null)
                {
                    foreach (var f in extraFiles)
                    {
                        _filesToCopyOver.Add(new RemoteFileCopyInfo(new FileInfo(f.LocalPath), tmpDir, _linuxTempDir));
                    }
                }
                if (receiveFiles != null)
                {
                    foreach (var f in receiveFiles)
                    {
                        _filesToBringBack.Add(new RemoteFileCopyInfo(new FileInfo(f.LocalPath), tmpDir, _linuxTempDir));
                    }
                }

                // Send over all files
                var logForError = new StringBuilder();
                await SendAllFiles(sshConnection, s => RecordLine(logForError, s, dumpLine));

                // Next, lets see if we can't run the file against root.
                try
                {
                    await sshConnection.ExecuteLinuxCommandAsync($"cd {_linuxTempDir}", processLine: s => RecordLine(logForError, s, dumpLine));
                    using (var lck = sshConnection.EnterNoRecoverRegion())
                    {
                        await sshConnection.ExecuteLinuxCommandAsync($"root -l -b -q {scriptFile.Name} | cat", processLine: s => RecordLine(logForError, s, dumpLine),
                            secondsTimeout: timeout.HasValue ? (int)timeout.Value.TotalSeconds : 60 * 60);
                    }
                } catch (Exception e)
                {
                    throw new RemoteBashCommandFailureException($"Failed to execute script: {ReformatLog(logForError)}", e);
                }

                // Finally, if there are any files to bring back, we should!
                await ReceiveAllFiles(sshConnection, dumpLine);

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
        internal async Task ExecuteBashScriptAsync(string fnameRoot, string commands, Action<string> dumpLine, bool verbose)
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
            await ExecuteRemoteWithTemp(fnameRoot, async connection =>
            {
                // Queue up files to send and recv
                _filesToCopyOver.Add(new RemoteFileCopyInfo() { localFileName = scriptFile, remoteLinuxDirectory = _linuxTempDir });

                // Run, pushing and pulling files we need
                await SendAllFiles(connection, dumpLine);
                var logForError = new StringBuilder();
                try
                {
                    await connection.ExecuteLinuxCommandAsync($"bash {_linuxTempDir}/{scriptFile.Name} | cat", s => RecordLine(logForError, s, dumpLine));
                } catch (Exception e)
                {
                    throw new RemoteBashCommandFailureException($"Failed to execute bash script: {ReformatLog(logForError)}.", e);
                }
                await ReceiveAllFiles(connection, dumpLine);
            }, dumpLine);
        }

        /// <summary>
        /// Make sure execute has a directory where it can go to!
        /// </summary>
        /// <param name="queryFile"></param>
        /// <param name="queryDirectory"></param>
        /// <param name="varsToTransfer"></param>
        /// <returns></returns>
        public override async Task<IDictionary<string, NTObject>> Execute(Uri[] files, FileInfo queryFile, DirectoryInfo queryDirectory, IEnumerable<KeyValuePair<string, object>> varsToTransfer)
        {
            Action<string> dumper = l =>
            {
                if (Environment.CompileDebug)
                {
                    Console.WriteLine(l);
                }
            };
            return await ExecuteRemoteWithTemp($"Query", async SSHConnection =>
            {
                // Load up extra files that need to be shipped over.
                foreach (var f in Environment.ExtraComponentFiles)
                {
                    _filesToCopyOver.Add(new RemoteFileCopyInfo() { localFileName = f, remoteLinuxDirectory = _linuxTempDir });
                }

                return await base.Execute(files, queryFile, queryDirectory, varsToTransfer);
            }, dumper);
        }

        /// <summary>
        /// Run on the remote guy in a temp dir that we clean up.
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="act"></param>
        /// <param name="dumpLine"></param>
        /// <returns></returns>
        internal async Task ExecuteRemoteWithTemp(string phase, Func<SSHRecoveringConnection, Task> act, Action<string> dumpLine = null)
        {
            await ExecuteRemoteWithTemp(phase,
                async c =>
                {
                    await act(c);
                    return 1;
                }, dumpLine);
        }

        /// <summary>
        /// Run on the remote guy in a temp dir that we clean up
        /// </summary>
        /// <param name="tempDir"></param>
        /// <param name="act"></param>
        /// <param name="dumpLine"></param>
        internal async Task<T> ExecuteRemoteWithTemp<T>(string phase, Func<SSHRecoveringConnection, Task<T>> act, Action<string> dumpLine = null)
        {
            if (phase.Contains("/") || phase.Contains(" "))
            {
                throw new InvalidOperationException($"Phase can't contain directory specications or spaces: '{phase}'");
            }

            // Build the temp directory
            var remoteDirectory = $"/tmp/{System.Environment.MachineName}/{phase}.{Guid.NewGuid().ToString()}";

            // Make the connection, and create the directory.
            var sshConnection = await MakeSSHConnection(dumpLine);
            var oldLinuxTempDir = _linuxTempDir;
            var oldLinuxPhase = _currentLinuxPhase;
            _currentLinuxPhase = phase;
            IDisposable lck = null;
            try
            {
                // Get the temp directory setup and going
                if (_currentLinuxPhase != oldLinuxPhase)
                {
                    _linuxTempDir = remoteDirectory;
                    await sshConnection.ExecuteLinuxCommandAsync($"rm -rf {_linuxTempDir}", processLine: l => RecordLine(null, l, dumpLine));
                    await sshConnection.ExecuteLinuxCommandAsync($"mkdir -p {_linuxTempDir}", processLine: l => RecordLine(null, l, dumpLine));
                    lck = sshConnection.EnterNoRecoverRegion();
                }
                dumpLine?.Invoke($"Executing commands in new directory {_linuxTempDir}");

                return await act(sshConnection);
            }
            finally
            {
                lck?.Dispose();
                if (_currentLinuxPhase != oldLinuxPhase)
                {
                    await sshConnection.ExecuteLinuxCommandAsync($"rm -rf {_linuxTempDir}", processLine: l => RecordLine(null, l, dumpLine));
                    _linuxTempDir = oldLinuxTempDir;
                    _currentLinuxPhase = oldLinuxPhase;
                }
            }
        }

        /// <summary>
        /// Send all files over to the remote client. Make sure the directory exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="dumpLine"></param>
        private async Task SendAllFiles(ISSHConnection connection, Action<string> dumpLine)
        {
            foreach (var f in _filesToCopyOver)
            {
                string linuxPath = $"{f.remoteLinuxDirectory}/{f.localFileName.Name}";
                dumpLine?.Invoke($"Copying {f.localFileName.Name} -> {linuxPath}");
                await connection.ExecuteLinuxCommandAsync($"mkdir -p {f.remoteLinuxDirectory}", dumpLine);
                await connection.CopyLocalFileRemotelyAsync(f.localFileName, linuxPath);
            }
            _filesToCopyOver.Clear();
        }

        /// <summary>
        /// Fetch back all files we probably need to know about.
        /// </summary>
        /// <param name="sshConnection"></param>
        /// <param name="dumpLine"></param>
        private async Task ReceiveAllFiles(ISSHConnection connection, Action<string> dumpLine)
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
                await connection.CopyRemoteFileLocallyAsync(linuxPath, f.localFileName);
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
        private void ReceiveFile (FileInfo fileToGet, string dirName, ISSHConnection connection, Action<string>statusUpdate = null)
        {
            string linuxPath = $"{dirName}/{fileToGet.Name}";
            connection.CopyRemoteFileLocally(linuxPath, fileToGet, statusUpdate);
        }

        /// <summary>
        /// Cache the connection
        /// </summary>
        private SSHRecoveringConnection _connection = null;

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
        private async Task<SSHRecoveringConnection> MakeSSHConnection(Action<string> dumpLine = null)
        {
            if (_connection == null)
            {
                await Policy
                    .Handle<SSHConnection.SSHConnectFailureException>()
                    .WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(60) })
                    .ExecuteAsync(async () =>
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
                                    var rep = line.Replace("ROOTVersionNumber", ROOTVersionNumber);
                                    await _connection.ExecuteLinuxCommandAsync(rep, processLine: s => RecordLine(logForError, s, localdumper));
                                }
                            }
                            catch (Exception e)
                            {
                                throw new RemoteBashCommandFailureException($"Error making a SSH connection: {ReformatLog(logForError)}", e);
                            }

                        }
                    });
            }
            return _connection;
        }

        /// <summary>
        /// Create an SSH connection to a remote machine.
        /// </summary>
        /// <param name="remoteSSHConnectionString"></param>
        /// <returns></returns>
        private SSHRecoveringConnection CreateSSHConnectionTo(string remoteSSHConnectionString)
        {
            return new SSHRecoveringConnection(() =>
            {
                return new SSHConnectionTunnel(remoteSSHConnectionString);
            });
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
        protected override Task<string> NormalizeFileForTarget(Uri finfo)
        {
            // See if this file has already been setup as file that is remote
            if (finfo.Scheme == "remotebash")
            {
                if (!finfo.AbsolutePath.Contains(":"))
                {
                    return Task.FromResult(finfo.AbsolutePath);
                }
            }

            // See if the file exists or not locally.
            var f = new FileInfo(finfo.LocalPath.StartsWith("/") ? finfo.LocalPath.Substring(1) : finfo.LocalPath);
            if (f.Exists)
            {
                // Push it to the remote host.
                if (_linuxTempDir == null)
                {
                    throw new InvalidOperationException($"Attempt to copy over file {f.Name} when we aren't in the middle of an operation!");
                }
                _filesToCopyOver.Add(new RemoteFileCopyInfo() { localFileName = f, remoteLinuxDirectory = _linuxTempDir });
            } else
            {
                // Perhaps it doesn't exist because we want to copy it back here?
                _filesToBringBack.Add(new RemoteFileCopyInfo() { localFileName = new FileInfo(finfo.LocalPath), remoteLinuxDirectory = _linuxTempDir});
            }

            // It will just be in the local directory where we live.
            return Task.FromResult($"{_linuxTempDir}/{f.Name}");
        }

        /// <summary>
        /// We want to send a whole directory over. This is bad. We have a bunch of work to do here.
        /// </summary>
        /// <param name="finfo"></param>
        /// <returns></returns>
        protected override async Task<string> NormalizeFileForTarget(DirectoryInfo finfo)
        {
            if (finfo.Exists)
            {
                var goodExtensiosn = new[] { ".h", ".hpp", ".c", ".cxx" };
                await MakeSSHConnection();
                foreach (var f in finfo.EnumerateFiles().Where(sf => goodExtensiosn.Contains(sf.Extension.ToLower())))
                {
                    _filesToCopyOver.Add(new RemoteFileCopyInfo() { localFileName = f, remoteLinuxDirectory = _linuxTempDir });
                }
            }
            return _linuxTempDir;
        }

        /// <summary>
        /// Need to see if the remote machine has ROOT installed.
        /// </summary>
        /// <param name="dumpLine"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        internal override async Task<bool> CheckForROOTInstall(Action<string> dumpLine, bool verbose)
        {
            // Simple script to execute
            var cmd = new StringBuilder();
            cmd.AppendLine("void testForRoot() {int i = 10;}");

            try
            {
                await ExecuteRootScript("testForRoot", cmd.ToString(), new DirectoryInfo(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)), dumpLine, verbose);
            }
            catch (RemoteBashCommandFailureException e) when (e.Message.Contains("version for root"))
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
                    ConfigureLines = new[] { "setupATLAS", $"lsetup \"root ROOTVersionNumber\"" }
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
        internal override Task InstallROOT(Action<string> dumpLine, bool verbose)
        {
            throw new ROOTCantBeInstalledRemotelyException("Unable to install root on a remote system over ssh");
        }
    }
}
