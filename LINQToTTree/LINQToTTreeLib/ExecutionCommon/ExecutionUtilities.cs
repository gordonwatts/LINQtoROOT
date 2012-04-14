﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LINQToTTreeLib.ExecutionCommon
{
    /// <summary>
    /// Some utilities to help with running code and copying and compiling, etc.
    /// </summary>
    class ExecutionUtilities
    {
        /// <summary>
        /// Copies a source file to a directory. Also copies over any "valid" includes we can find.
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destDirectory"></param>
        public static FileInfo CopyToDirectory(FileInfo sourceFile, DirectoryInfo destDirectory)
        {
            ///
            /// See if the dest file is already there. If so, don't copy over
            /// 

            FileInfo destFile = new FileInfo(destDirectory.FullName + "\\" + sourceFile.Name);
            if (destFile.Exists)
            {
                if (destFile.LastWriteTime >= sourceFile.LastWriteTime
                    && destFile.Length == sourceFile.Length)
                {
                    return destFile;
                }
            }
            sourceFile.CopyTo(destFile.FullName, true);

            ///
            /// Next, if there are any include files we need to move
            /// 

            CopyIncludedFilesToDirectory(sourceFile, destDirectory);

            ///
            /// Return what we know!
            /// 

            destFile.Refresh();
            return destFile;
        }

        /// <summary>
        /// Copy over any files that are included in the source file to the destination
        /// directory
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destDirectory"></param>
        public static void CopyIncludedFilesToDirectory(FileInfo sourceFile, DirectoryInfo destDirectory)
        {
            var includeFiles = FindGoodIncludeFiles(sourceFile);

            foreach (var item in includeFiles)
            {
                CopyToDirectory(item, destDirectory);
            }
        }

        /// <summary>
        /// Returns a list of good include files - those likely to be located in a place
        /// that makes sense.
        /// </summary>
        /// <param name="sourceCPPFile"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> FindGoodIncludeFiles(FileInfo sourceCPPFile)
        {
            var goodIncludeFiles = from f in FindIncludeFiles(sourceCPPFile)
                                   where !Path.IsPathRooted(f)
                                   let full = new FileInfo(sourceCPPFile.DirectoryName + "\\" + f)
                                   where full.Exists
                                   select full;
            return goodIncludeFiles;
        }

        /// <summary>
        /// Scan include files recursively - so if an include file includes other files, etc.
        /// </summary>
        /// <param name="sourceCPPFile"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> FindGoodIncludeFilesRecursive(FileInfo sourceCPPFile)
        {
            var toProcess = new Stack<FileInfo>();
            var alreadyDone = new List<FileInfo>();
            toProcess.Push(sourceCPPFile);
            while (toProcess.Count != 0)
            {
                var todo = toProcess.Pop();
                alreadyDone.Add(todo);
                foreach (var newIncludeFile in FindGoodIncludeFiles(todo))
                {
                    if (!alreadyDone.Contains(newIncludeFile))
                    {
                        yield return newIncludeFile;
                        toProcess.Push(newIncludeFile);
                    }
                }
            }
        }

        /// <summary>
        /// Return the include files that we find in this guy.
        /// </summary>
        /// <param name="_proxyFile"></param>
        /// <returns></returns>
        public static IEnumerable<string> FindIncludeFiles(FileInfo _proxyFile)
        {
            Regex reg = new Regex("#include \"(?<file>[^\"]+)\"");
            using (var reader = _proxyFile.OpenText())
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        continue;

                    var m = reg.Match(line);
                    if (m.Success)
                    {
                        var s = m.Groups["file"].Value;
                        yield return s;
                    }
                }
            }
        }

        /// <summary>
        /// Has the global initialization been run for local execution yet?
        /// </summary>
        static bool gGlobalInit = false;


        /// <summary>
        /// The location where we put temp files we need to build against, etc. and then
        /// ship off and run... No perm stuff here (so no results, etc.).
        /// </summary>
        public static DirectoryInfo TempDirectory = null;

        /// <summary>
        /// The directory where we put all our dictionary generation stuff.
        /// </summary>
        public static DirectoryInfo DictDirectory
        {
            get
            {
                return new DirectoryInfo(string.Format(@"{0}\DictGeneration", ExecutionUtilities.TempDirectory.FullName));
            }
        }

        /// <summary>
        /// Global init for an executor that needs to do local compiles on this ROOT machine.
        /// </summary>
        public static void Init()
        {
            if (gGlobalInit)
                return;
            gGlobalInit = true;

            ///
            /// A directory where we can store all of the temp files we need to create
            /// 

            TempDirectory = new DirectoryInfo(Path.GetTempPath() + "\\LINQToROOT");
            if (!TempDirectory.Exists)
            {
                TempDirectory.Create();
                TempDirectory.Refresh();
            }

            if (!DictDirectory.Exists)
            {
                DictDirectory.Create();
            }

            ///
            /// Next the common source files. Make sure that the include files passed to the old compiler has
            /// this common file directory in there!
            /// 

            var cf = CommonSourceDirectory();
            if (!cf.Exists)
            {
                cf.Create();
            }

            if (!ROOTNET.NTSystem.gSystem.IncludePath.Contains(cf.FullName))
            {
                ROOTNET.NTSystem.gSystem.AddIncludePath("-I\"" + cf.FullName + "\"");
            }

            //
            // Make sure the environment is setup correctly!
            //

            ExecutionUtilities.SetupENV();
        }


        /// <summary>
        /// Generate the common directory. Called only after the temp directory has been created!!
        /// </summary>
        /// <returns></returns>
        public static DirectoryInfo CommonSourceDirectory()
        {
            return new DirectoryInfo(TempDirectory.FullName + "\\CommonFiles");
        }


        /// <summary>
        /// Copy this source file (along with any includes in it) to
        /// our common area.
        /// </summary>
        /// <param name="sourceFile"></param>
        public static FileInfo CopyToCommonDirectory(FileInfo sourceFile)
        {
            return ExecutionUtilities.CopyToDirectory(sourceFile, CommonSourceDirectory());
        }

        /// <summary>
        /// Make sure the environment is setup to run the C++ compiler. If it isn't adjust it.
        /// </summary>
        /// <remarks>
        /// We should be called only once per execution, though I guess we are protected!
        /// </remarks>
        private static void SetupENV()
        {
            //
            // If "cl" is already visible, then we don't have to do anything.
            //

            if (FindFileInEnv("PATH", "cl.exe"))
                return;

            //
            // Ok - it isn't in there. Now we need to actually load it in.
            //

            // Get the install directory

            var vcInstallDir = GetVCRegistryEntry(@"Microsoft\VisualStudio\SxS\VC7", "10.0");
            if (vcInstallDir == null)
                throw new NotSupportedException("Visual Studio C++ v10.0 must be installed or already setup otherwise we cannot run!");

            var vsInstallDir = GetVCRegistryEntry(@"Microsoft\VisualStudio\SxS\VS7", "10.0");
            if (vsInstallDir == null)
                throw new NotSupportedException("Visual Studio IDE v10.0 must be installed already otherwise setup cannot run!");

            var winSDKDir = GetVCRegistryEntry(@"Microsoft\Microsoft SDKs\Windows\v7.0A", "InstallationFolder");
            if (winSDKDir == null)
                throw new NotSupportedException("Unable to locate the windows SDK directory to link against! Cannot run!");

            AddToPathEnv("PATH", string.Format(@"{0}\bin", vcInstallDir));
            AddToPathEnv("PATH", string.Format(@"{0}\Common7\IDE", vsInstallDir));
            AddToPathEnv("INCLUDE", string.Format(@"{0}\include", winSDKDir));
            AddToPathEnv("INCLUDE", string.Format(@"{0}\include", vcInstallDir));
            AddToPathEnv("LIB", string.Format(@"{0}\lib", winSDKDir));
            AddToPathEnv("LIB", string.Format(@"{0}\lib", vcInstallDir));
            AddToPathEnv("LIBPATH", string.Format(@"{0}\lib", vcInstallDir));

            if (!FindFileInEnv("PATH", "cl.exe"))
                throw new InvalidOperationException("Despite defining PATH variabels to the compiler, we can't find cl.exe!");
        }

        /// <summary>
        /// Add to a semi-colon seperated environment variable
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        private static void AddToPathEnv(string envName, string newPath)
        {
            if (!Directory.Exists(newPath))
                throw new InvalidOperationException(string.Format("Path does not exist - will not add '{0}'", newPath));

            var oldEnv = System.Environment.GetEnvironmentVariable(envName);
            var newEnv = newPath.Replace(@"\\", @"\");
            if (oldEnv != null)
                newEnv = String.Format("{0};{1}", newEnv, oldEnv);

            System.Environment.SetEnvironmentVariable(envName, newEnv);
        }

        /// <summary>
        /// Search the registry entry for a particular key to load.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        /// <returns></returns>
        private static string GetVCRegistryEntry(string baseRegPath, string keyName)
        {
            // A little tricky because we have 64 bit and 32 bit stuff!

            var r = GetVCRegistryEntryAbs(string.Format(@"SOFTWARE\{0}", baseRegPath), keyName);
            if (r == null)
                return GetVCRegistryEntryAbs(string.Format(@"SOFTWARE\Wow6432Node\{0}", baseRegPath), keyName);
            return r;
        }

        /// <summary>
        /// Load in the proper registry item here
        /// </summary>
        /// <param name="regPath"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        private static string GetVCRegistryEntryAbs(string regPath, string keyName)
        {
            using (var reg = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(regPath, false))
            {
                if (reg == null)
                    return null;
                var regv = reg.GetValue(keyName);
                return regv as string;
            }
        }

        /// <summary>
        /// Search the environment for a file.
        /// </summary>
        /// <param name="envVariable">Name of the environment variable, with paths, seperated by semi-colons.</param>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static bool FindFileInEnv(string envVariable, string filename)
        {
            var foundFiles = from dir in System.Environment.GetEnvironmentVariable(envVariable).Split(';')
                             let fstr = string.Format(@"{0}\{1}", dir, filename)
                             where File.Exists(fstr)
                             select fstr;
            return foundFiles.Any();
        }
    }
}
