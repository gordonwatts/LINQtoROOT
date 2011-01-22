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

            return default(T);
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
        /// Compile and load a file
        /// </summary>
        /// <param name="templateRunner"></param>
        private void CompileAndLoad(FileInfo templateRunner)
        {
            var gSystem = ROOTNET.NTSystem.gSystem;

            var result = gSystem.CompileMacro(templateRunner.FullName, "k");

            /// This should never happen - but we are depending on so many different things to go right here!
            if (result != 1)
                throw new InvalidOperationException("Failed to compile '" + templateRunner.FullName + "'");
        }

        /// <summary>
        /// If there are some extra files we need to be loading, go after them here.
        /// </summary>
        private void AssembleAndLoadExtraObjects()
        {
            ///
            /// First, go after the common items/classes.
            /// 

        }

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
        /// <param name="_proxyFile"></param>
        private void CopyToQueryDirectory(FileInfo _proxyFile)
        {
            var queryDirectory = GetQueryDirectory();
            _proxyFile.CopyTo(queryDirectory.FullName + "\\" + _proxyFile.Name);

            ///
            /// Next, if there are any include files we need to move
            /// 

            var includeFiles = FindIncludeFiles(_proxyFile);
            var goodIncludeFiles = from f in includeFiles
                                   let full = new FileInfo(_proxyFile.DirectoryName + "\\" + f)
                                   where full.Exists
                                   select full;

            foreach (var item in goodIncludeFiles)
            {
                item.CopyTo(queryDirectory.FullName + "\\" + item.Name);
            }
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
            var cf = new DirectoryInfo(TempDirectory.FullName + "\\CommonFiles");
            if (!cf.Exists)
            {
                cf.Create();
            }

            ///
            /// Finally, make sure TApplication has been started. It will init a bunch of stuff
            /// 

            //if (ROOTNET.NTApplication.gApplication == null)
            //{
            var app = new ROOTNET.NTApplication("LINQToTTree", new int[] { 0 }, new string[] { });
            //}
            ///ROOTNET.NTROOT.gROOT.ProcessLineSync("");
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
