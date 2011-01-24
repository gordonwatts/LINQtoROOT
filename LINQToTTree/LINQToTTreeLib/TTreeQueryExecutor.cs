using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using LinqToTTreeInterfacesLib;
using NVelocity;
using NVelocity.App;
using Remotion.Data.Linq;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Executes the query.
    /// </summary>
    public class TTreeQueryExecutor : IQueryExecutor
    {
        /// <summary>
        /// The root file that this exector operates on
        /// </summary>
        private FileInfo _rootFile;

        /// <summary>
        /// The tree name that in the root file that this guy operates on.
        /// </summary>
        private string _treeName;

        /// <summary>
        /// The header file make by the MakeProxy macro.
        /// </summary>
        private FileInfo _proxyFile;

        /// <summary>
        /// Extra objects we might need to load
        /// </summary>
        private FileInfo[] _extraComponentFiles;

        /// <summary>
        /// We are going to be executing over a particular file and tree
        /// </summary>
        /// <param name="rootFile"></param>
        /// <param name="treeName"></param>
        public TTreeQueryExecutor(FileInfo rootFile, string treeName, Type baseNtupleObject)
        {
            ///
            /// Basic checks
            /// 

            if (rootFile == null)
                throw new ArgumentNullException("Must have good root file");
            if (!rootFile.Exists)
                throw new ArgumentException("File '" + rootFile.FullName + "' not found");
            if (treeName == null)
                throw new ArgumentNullException("The tree must have a valid name");
            if (string.IsNullOrWhiteSpace(treeName))
                throw new ArgumentException("The tree name must be valid");
            if (baseNtupleObject == null)
                throw new ArgumentNullException("baseNtupleObject");

            ///
            /// Make sure the object we are using is correct, and that it has non-null values
            /// for the things passed in. We do this now so we don't have to have checks later on.
            /// 

            if (baseNtupleObject.GetField("_gProxyFile") == null)
                throw new ArgumentException("_gProxyFile - object is not a member of " + baseNtupleObject.ToString());
            if (baseNtupleObject.GetField("_gObjectFiles") == null)
                throw new ArgumentException("_gObjectFiles - object is not a member of " + baseNtupleObject.ToString());

            var proxyFileName = baseNtupleObject.GetField("_gProxyFile").GetValue(null) as string;
            if (string.IsNullOrWhiteSpace(proxyFileName))
                throw new ArgumentException("_gProxyFile - points to a null file - must be a real file");
            _proxyFile = new FileInfo(proxyFileName);
            if (!File.Exists(proxyFileName))
                throw new FileNotFoundException("_gProxyFile - '" + proxyFileName + "' was not found.");

            var extraFiles = baseNtupleObject.GetField("_gObjectFiles").GetValue(null) as string[];
            if (extraFiles == null)
            {
                _extraComponentFiles = new FileInfo[0];
            }
            else
            {
                _extraComponentFiles = (from spath in extraFiles
                                        select new FileInfo(spath)).ToArray();
            }
            var badfiles = from f in _extraComponentFiles
                           where !f.Exists
                           select f;
            string bad = "";
            foreach (var badf in badfiles)
            {
                bad = bad + "'" + badf.FullName + "' ";
            }
            if (bad.Length != 0)
                throw new ArgumentException("Extra component files were missing: " + bad + "");

            ///
            /// Save the values
            /// 

            _rootFile = rootFile;
            _treeName = treeName;
        }

        /// <summary>
        /// Return a collection. We currently don't support this, so it remains a
        /// bomb! And it is not likely one would want to move a TB of info back from a
        /// File to another... now, writing it out to a file is a possibility - but
        /// that would be a seperate scalar result. :-)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Execute a scalar result. These are things that end in "count" or "aggregate", etc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            ///
            /// We have to init everything - which means using MEF!
            /// 

            Init();

            ///
            /// The query visitor is what we will use to scan the actual guys
            /// 

            var result = new GeneratedCode();
            var qv = new QueryVisitor(result);

            CompositionBatch b = new CompositionBatch();
            b.AddPart(qv);
            _gContainer.Compose(b);

            ///
            /// Parse the query
            /// 

            qv.VisitQueryModel(queryModel);

            ///
            /// If we got back from that without an error, it is time to assemble the files and templates
            /// 

            AssembleAndLoadExtraObjects();

            ///
            /// Now that those are loaded, we need to go after the
            /// proxy. We create a new file which is dependent on the
            /// one we have been given.
            /// 

            CopyToQueryDirectory(_proxyFile);
            var templateRunner = WriteTSelector(_proxyFile.Name, Path.GetFileNameWithoutExtension(_proxyFile.Name), result);
            CompileAndLoad(templateRunner);

            ///
            /// Fantastic! Now we need to run the object!
            /// 

            RunNtupleQuery(Path.GetFileNameWithoutExtension(templateRunner.Name));

            ///
            /// Last job, extract all the variables!
            /// 

            var final = ExtractResult<T>(result.ResultValue);

            ///
            /// Ok, we are all done. Try to unload everything now.
            /// 

            UnloadAllModules();
            GetQueryDirectory().Delete(true);

            return final;
        }

        /// <summary>
        /// Extract the value for iVariable from the results file.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        private T ExtractResult<T>(IVariable iVariable)
        {
            ///
            /// Open the file, if it isn't there something very serious has gone wrong.
            /// 

            var file = ROOTNET.NTFile.Open("plots.root");
            if (!file.IsOpen())
                throw new FileNotFoundException("Unable to find the output file from the ROOT run!");

            ///
            /// Load the object and try to extract whatever info we need to from it
            /// 

            try
            {
                var o = file.Get(iVariable.RawValue);
                var s = GetVariableSaver(iVariable);
                return s.LoadResult<T>(iVariable, o);
            }
            finally
            {
                file.Close();
            }

        }

        /// <summary>
        /// We actually run the query!
        /// </summary>
        /// <param name="tSelectorClassName">Name of the TSelector object</param>
        private void RunNtupleQuery(string tSelectorClassName)
        {
            ///
            /// Create a new TSelector to run
            /// 

            var cls = ROOTNET.NTClass.GetClass(tSelectorClassName);
            if (cls == null)
                throw new InvalidOperationException("Unable to load class '" + tSelectorClassName + "' - can't run ntuple query");

            var selector = cls.New() as ROOTNET.Interface.NTSelector;

            ///
            /// Fetch out the tree now
            /// 

            var rf = new ROOTNET.NTFile(_rootFile.FullName, "READ");
            if (!rf.IsOpen())
                throw new InvalidOperationException("Unable to open file '" + _rootFile.FullName + "' with root's TFiel!");
            var tree = rf.Get(_treeName) as ROOTNET.Interface.NTTree;
            if (tree == null)
                throw new InvalidOperationException("Unable to find tree '" + _treeName + "' in file '" + _rootFile.FullName + "'.");

            ///
            /// Finally, run the whole thing
            /// 

            var result = tree.Process(selector);
        }

        /// <summary>
        /// Keep track of all modules that we've loaded
        /// </summary>
        private List<string> _loadedModuleNames = new List<string>();

        /// <summary>
        /// Compile and load a file
        /// </summary>
        /// <param name="templateRunner"></param>
        private void CompileAndLoad(FileInfo templateRunner)
        {
            var gSystem = ROOTNET.NTSystem.gSystem;

            var result = gSystem.CompileMacro(templateRunner.FullName, "k");

            /// This should never happen - but we are depending on so many different things to go right here!
            if (result != 1)
                throw new InvalidOperationException("Failed to compile '" + templateRunner.FullName + "' - make sure command 'cl' is defined!!!");

            _loadedModuleNames.Add(templateRunner.Name.Replace(".", "_"));
        }

        /// <summary>
        /// Unload all modules that we've loaded. This should have root release the lock on everything.
        /// </summary>
        private void UnloadAllModules()
        {
            ///
            /// The library names are a simple "_" replacement. However, the full path must be given to the
            /// unload function. To avoid any issues we just scan the library list that ROOT has right now, find the
            /// ones we care about, and unload them. In general this is not a good idea, so when there are random
            /// crashes this might be a good place to come first! :-)
            /// 

            var gSystem = ROOTNET.NTSystem.gSystem;
            var libraries = gSystem.Libraries.Split(' ');
            _loadedModuleNames.Reverse();

            var full_lib_names = from m in _loadedModuleNames
                                 from l in libraries
                                 where l.Contains(m)
                                 select l;

            ///
            /// Now that we have them, unload them. Since repeated unloading
            /// cases erorr messages to the concole, clear the list so we don't
            /// make a mistake later.
            /// 

            foreach (var m in full_lib_names)
            {
                gSystem.Unload(m);
            }

            _loadedModuleNames.Clear();
        }

        /// <summary>
        /// If there are some extra files we need to be loading, go after them here.
        /// </summary>
        private void AssembleAndLoadExtraObjects()
        {
            ///
            /// First, do the files that are part of our infrastructure.
            /// 

            var f = CopyToCommonDirectory(GetInfrastructureFile("FlowOutputObject.cpp"));
            CompileAndLoad(f);

            ///
            /// Next, build any files that are required to build run this ntuple
            /// 

            foreach (var fd in _extraComponentFiles)
            {
                var output = CopyToCommonDirectory(fd);
                CompileAndLoad(output);
            }
        }

        /// <summary>
        /// We are responsible for finding a given file in the infrastructure directory. We go boom if
        /// we can't file the file.
        /// </summary>
        /// <param name="filename">The name (including extension) of the file to find</param>
        /// <returns></returns>
        private FileInfo GetInfrastructureFile(string filename)
        {
            string baseDir = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            var f = new FileInfo(baseDir + "\\InfrastructureSourceFiles\\" + filename);
            if (!f.Exists)
                throw new FileNotFoundException("Unable to find infrastructure source file '" + f.FullName + "'");
            return f;
        }

        /// <summary>
        /// Keep track of all include files we need to pull in. Ugly - because it is global.
        /// </summary>
        private List<string> _includeFiles = new List<string>();

        /// <summary>
        /// Write out the TSelector file derived from the already existing query. This now includes all the code we need
        /// in order to actually run the thing! Use the template to do it.
        /// </summary>
        /// <param name="p"></param>
        private FileInfo WriteTSelector(string proxyFileName, string proxyObjectName, GeneratedCode code)
        {
            ///
            /// Get the template engine all setup
            /// 

            VelocityEngine eng = new VelocityEngine();
            eng.Init();

            string template;
            using (var reader = File.OpenText(TemplateDirectory("TSelectorTemplate.cxx")))
            {
                template = reader.ReadToEnd();
                reader.Close();
            }

            var context = new VelocityContext();
            context.Put("baseClassInclude", proxyFileName);
            context.Put("baseClassName", proxyObjectName);
            context.Put("ResultVariable", TranslateVariable(code.ResultValue));
            context.Put("ProcessStatements", TranslateStatements(code.CodeBody));
            context.Put("SlaveTerminateStatements", TranslateFinalizingVariables(code.ResultValue));
            context.Put("IncludeFiles", _includeFiles);

            ///
            /// Now do it!
            /// 

            var ourSelector = new FileInfo(GetQueryDirectory() + "\\query.cxx");
            using (var writer = ourSelector.CreateText())
            {
                eng.Evaluate(context, writer, null, template);
                writer.Close();
            }

            return ourSelector;
        }

        /// <summary>
        /// MEF list of guys that can help us save and load up variables.
        /// </summary>
#pragma warning disable 0649
        [ImportMany]
        IEnumerable<IVariableSaver> _varSaverList;
#pragma warning restore 0649

        /// <summary>
        /// Given a variable that has to be transmitted back accross the wire,
        /// generate the statements that are required to make sure that it goes there!
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        private IEnumerable<string> TranslateFinalizingVariables(IVariable iVariable)
        {
            var saver = GetVariableSaver(iVariable);

            foreach (var f in saver.IncludeFiles(iVariable))
            {
                if (!_includeFiles.Contains(f))
                    _includeFiles.Add(f);
            }

            return saver.SaveToFile(iVariable);
        }

        /// <summary>
        /// Find a saver for a particular variable.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        private IVariableSaver GetVariableSaver(IVariable iVariable)
        {
            var saver = (from s in _varSaverList
                         where s.CanHandle(iVariable)
                         select s).FirstOrDefault();
            if (saver == null)
                throw new InvalidOperationException("Unable to find an IVariableSaver for " + iVariable.GetType().Name);
            return saver;
        }

        /// <summary>
        /// Translate the incoming statements into something that can be send to the C++ compiler.
        /// </summary>
        /// <param name="statements"></param>
        /// <returns></returns>
        private IEnumerable<string> TranslateStatements(IStatement statements)
        {
            return statements.CodeItUp();
        }

        /// <summary>
        /// Helper var that we send off to the macro processor. We have to massage to get from our internal rep into
        /// somethign that can be used directly by the C++ code.
        /// </summary>
        public class VarInfo
        {
            private IVariable _iVariable;

            public VarInfo(IVariable iVariable)
            {
                // TODO: Complete member initialization
                this._iVariable = iVariable;
            }
            public string VariableName { get { return _iVariable.VariableName; } }
            public string VariableType { get { return Variables.VarUtils.AsCPPType(_iVariable.Type); } } // C++ type
            public string InitialValue { get { return _iVariable.InitialValue.RawValue; } }
        }

        /// <summary>
        /// Trnaslate the variable type/name into something for our output code.
        /// </summary>
        /// <param name="iVariable"></param>
        /// <returns></returns>
        private VarInfo TranslateVariable(LinqToTTreeInterfacesLib.IVariable iVariable)
        {
            return new VarInfo(iVariable);
        }

        /// <summary>
        /// We have some templates we use to run. Find them!
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string TemplateDirectory(string templateName)
        {
            return Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) + "\\Templates\\" + templateName;
        }

        /// <summary>
        /// Get/Set the directory where we create our local query info. Defaults to the users' temp directory.
        /// </summary>
        public static DirectoryInfo QueryCreationDirectory = new DirectoryInfo(Path.GetTempPath() + "\\LINQToTTree");

        /// <summary>
        /// Copy a file to a directory that contains files for this query.
        /// </summary>
        /// <param name="sourceFile"></param>
        private FileInfo CopyToQueryDirectory(FileInfo sourceFile)
        {
            return CopyToDirectory(sourceFile, GetQueryDirectory());
        }

        /// <summary>
        /// Copies a source file to a directory. Also copies over any "valid" includes we can find.
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="destDirectory"></param>
        private FileInfo CopyToDirectory(FileInfo sourceFile, DirectoryInfo destDirectory)
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

            var includeFiles = FindIncludeFiles(sourceFile);
            var goodIncludeFiles = from f in includeFiles
                                   let full = new FileInfo(sourceFile.DirectoryName + "\\" + f)
                                   where full.Exists
                                   select full;

            foreach (var item in goodIncludeFiles)
            {
                CopyToDirectory(item, destDirectory);
            }

            ///
            /// Return what we know!
            /// 

            destFile.Refresh();
            return destFile;
        }

        /// <summary>
        /// Copy this source file (along with any includes in it) to
        /// our common area.
        /// </summary>
        /// <param name="sourceFile"></param>
        private FileInfo CopyToCommonDirectory(FileInfo sourceFile)
        {
            return CopyToDirectory(sourceFile, CommonSourceDirectory());
        }

        /// <summary>
        /// Return the include files that we find in this guy.
        /// </summary>
        /// <param name="_proxyFile"></param>
        /// <returns></returns>
        private IEnumerable<string> FindIncludeFiles(FileInfo _proxyFile)
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
        /// The local query
        /// </summary>
        private DirectoryInfo _queryDirectory;

        /// <summary>
        /// Get the directory for the current query in progress.
        /// </summary>
        /// <returns></returns>
        private DirectoryInfo GetQueryDirectory()
        {
            if (_queryDirectory == null)
            {
                _queryDirectory = new DirectoryInfo(QueryCreationDirectory.FullName + "\\" + Path.GetRandomFileName());
                _queryDirectory.Create();
            }
            return _queryDirectory;
        }

        /// <summary>
        /// When the class has been initalized, we set this to true. Make sure we run MEF.
        /// </summary>
        static CompositionContainer _gContainer = null;

        /// <summary>
        /// The location where we put temp files we need to build against, etc. and then
        /// ship off and run... No perm stuff here (so no results, etc.).
        /// </summary>
        public static DirectoryInfo TempDirectory = null;

        /// <summary>
        /// Run init for this class.
        /// </summary>
        private void Init()
        {
            if (_gContainer != null)
                return;

            ///
            /// Get MEF setup with everything in our assembly.
            /// 

            AggregateCatalog aggCat = new AggregateCatalog();
            aggCat.Catalogs.Add(new AssemblyCatalog(Assembly.GetCallingAssembly()));
            _gContainer = new CompositionContainer(aggCat);
            ExpressionVisitor.TypeHandlers = new TypeHandlers.TypeHandlerCache();
            CompositionBatch b = new CompositionBatch();
            b.AddPart(ExpressionVisitor.TypeHandlers);
            b.AddPart(this);
            _gContainer.Compose(b);

            ///
            /// A directory where we can store all of the temp files we need to create
            /// 

            TempDirectory = new DirectoryInfo(Path.GetTempPath() + "\\LINQToROOT");
            if (!TempDirectory.Exists)
            {
                TempDirectory.Create();
                TempDirectory.Refresh();
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

            ///
            /// Finally, make sure TApplication has been started. It will init a bunch of stuff
            /// 

            ROOTNET.NTApplication.CreateApplication();
        }

        /// <summary>
        /// Generate the common directory. Called only after the temp directory has been created!!
        /// </summary>
        /// <returns></returns>
        private DirectoryInfo CommonSourceDirectory()
        {
            return new DirectoryInfo(TempDirectory.FullName + "\\CommonFiles");
        }

        /// <summary>
        /// Returns the first item... not yet implemented.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryModel"></param>
        /// <param name="returnDefaultWhenEmpty"></param>
        /// <returns></returns>
        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            throw new NotImplementedException();
        }
    }
}
