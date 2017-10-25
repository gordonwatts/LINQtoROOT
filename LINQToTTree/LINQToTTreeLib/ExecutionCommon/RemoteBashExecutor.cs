using AtlasSSH;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using LINQToTTreeLib.Utils;
using System.Threading.Tasks;
using ROOTNET.Interface;

namespace LINQToTTreeLib.ExecutionCommon
{
    /// <summary>
    /// Used to execute remotely (via an ssh connection)
    /// </summary>
    class RemoteBashExecutor : CommandLineCommonExecutor, IQueryExectuor
    {
        protected override string ExecutorName => throw new NotImplementedException();

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
            IEnumerable<Uri> receiveFiles = null)
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
                SendFile(scriptFile, linuxTempDir, sshConnection, dumpLine);
                if (extraFiles != null)
                {
                    foreach (var f in extraFiles)
                    {
                        SendFile(new FileInfo(f.LocalPath), linuxTempDir, sshConnection, dumpLine);
                    }
                }

                // Next, lets see if we can't run the file against root.
                sshConnection.Connection.ExecuteLinuxCommand($"cd {linuxTempDir}", processLine: dumpLine);
                sshConnection.Connection.ExecuteLinuxCommand($"root -l -b -q {scriptFile.Name}", processLine: dumpLine);

                // Finally, if there are any files to bring back, we should!
                if (receiveFiles != null)
                {
                    foreach (var f in receiveFiles)
                    {
                        ReceiveFile(new FileInfo(f.LocalPath), linuxTempDir, sshConnection, dumpLine);
                    }
                }
                return (object) null;
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
            Action<string> dumper = Environment.CompileDebug ? s => Console.WriteLine(s) : (Action<string>) null;
            return ExecuteRemoteWithTemp("/tmp/proxygen", SSHConnection =>
            {
                return base.GenerateProxyFile(rootFiles, treeName, queryDirectory);
            }, dumper);
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
            Action<string> dumper = Environment.CompileDebug ? s => Console.WriteLine(s) : (Action<string>)null;
            return ExecuteRemoteWithTemp($"/tmp/{queryDirectory.Name}", SSHConnection =>
            {
                return base.Execute(queryFile, queryDirectory, varsToTransfer);
            }, dumper);
        }

        /// <summary>
        /// Run on the remote guy in a temp dir that we clean up
        /// </summary>
        /// <param name="tempDir"></param>
        /// <param name="act"></param>
        /// <param name="dumpLine"></param>
        private T ExecuteRemoteWithTemp<T>(string tempDir, Func<SSHTunneledConnection, T> act, Action<string> dumpLine = null)
        {
            var sshConnection = MakeSSHConnection(dumpLine);
            var oldLinuxTempDir = linuxTempDir;
            linuxTempDir = tempDir;
            try
            {
                // Get the temp directory setup and going
                if (linuxTempDir != oldLinuxTempDir)
                {
                    sshConnection.Connection.ExecuteLinuxCommand($"rm -rf {linuxTempDir}", processLine: dumpLine);
                    sshConnection.Connection.ExecuteLinuxCommand($"mkdir {linuxTempDir}", processLine: dumpLine);
                }
                dumpLine?.Invoke($"Executing commands in new directory {linuxTempDir}");

                return act(sshConnection);
            }
            finally
            {
                if (linuxTempDir != oldLinuxTempDir)
                {
                    sshConnection.Connection.ExecuteLinuxCommand($"rm -rf {linuxTempDir}", processLine: dumpLine);
                    linuxTempDir = oldLinuxTempDir;
                }
            }
        }

        /// <summary>
        /// Low level routine that will send a file to the remote host
        /// </summary>
        /// <param name="fileToCopy">The local file that should be sent</param>
        /// <param name="dirName">Directory name where the file should be copied</param>
        /// <param name="connection">Connection over which to do the copy</param>
        private void SendFile(FileInfo fileToCopy, string dirName, SSHTunneledConnection connection, Action<string> statusUpdate = null)
        {
            string linuxPath = $"{dirName}/{fileToCopy.Name}";
            connection.Connection.CopyLocalFileRemotely(fileToCopy, linuxPath, statusUpdate);
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

        /// <summary>
        /// Create an SSH connection.
        /// </summary>
        /// <param name="dumpLine">Dump output from running various setup lines</param>
        /// <returns></returns>
        private SSHTunneledConnection MakeSSHConnection(Action<string> dumpLine = null)
        {
            if (_connection == null)
            {
                var minfo = GetMachineInfo();
                _connection = CreateSSHConnectionTo(minfo.RemoteSSHConnectionString);

                if (minfo.ConfigureLines != null)
                {
                    foreach (var line in minfo.ConfigureLines)
                    {
                        _connection.Connection.ExecuteLinuxCommand(line, processLine: dumpLine);
                    }
                }
            }
            return _connection;
        }

        /// <summary>
        /// Disposable holder for the full connection
        /// </summary>
        private class SSHTunneledConnection : IDisposable
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
            // See if the file exists or not locally.
            var f = new FileInfo(finfo.LocalPath.StartsWith("/") ? finfo.LocalPath.Substring(1) : finfo.LocalPath);
            if (f.Exists)
            {
                // Push it to the remote host.
                if (linuxTempDir == null)
                {
                    throw new InvalidOperationException($"Attempt to copy over file {f.Name} when we aren't in the middle of an operation!");
                }
                var c = MakeSSHConnection();
                c.Connection.CopyLocalFileRemotely(f, linuxTempDir);
            }

            // It will just be in the local directory where we live.
            return $"{linuxTempDir}/{f.Name}";
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
                var goodExtensiosn = new[] { ".h", ".root", ".c", ".cxx" };
                var c = MakeSSHConnection();
                foreach (var f in finfo.EnumerateFiles().Where(sf => goodExtensiosn.Contains(sf.Extension.ToLower())))
                {
                    SendFile(f, linuxTempDir, c);
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
        private class MachineConfig
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

        /// <summary>
        /// Find the config for a particular machine
        /// </summary>
        /// <returns></returns>
        private MachineConfig GetMachineInfo()
        {
            return new MachineConfig()
            {
                RemoteSSHConnectionString = "gwatts@tev01.phys.washington.edu",
                ConfigureLines = new[] { "setupATLAS", "lsetup root" }
            };
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
