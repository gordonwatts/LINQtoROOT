using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
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
            /// The query visitor is what we will use to scan the actual guys
            /// 

            var result = new GeneratedCode();
            var qv = new QueryVisitor(result);

            ///
            /// We have to init everything - which means using MEF!
            /// 

            Init();

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

            AssembleAndLoadTemplates();

            return default(T);
        }

        /// <summary>
        /// Scan for the templates and the main files we are supposed to be loading.
        /// </summary>
        private void AssembleAndLoadTemplates()
        {
            ///
            /// First, go after the common items/classes.
            /// 

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
