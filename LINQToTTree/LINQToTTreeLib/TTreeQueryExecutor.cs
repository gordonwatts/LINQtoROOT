using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
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
        /// We are going to be executing over a particular file and tree
        /// </summary>
        /// <param name="rootFile"></param>
        /// <param name="treeName"></param>
        public TTreeQueryExecutor(FileInfo rootFile, string treeName)
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
            throw new NotImplementedException();
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
