using System;
using System.Collections.Generic;
using System.IO;
using Remotion.Data.Linq;

namespace LINQToTTreeLib
{
    /// <summary>
    /// Executes the query.
    /// </summary>
    public class TTreeQueryExecutor : IQueryExecutor
    {
        private FileInfo rootFile;
        private string treeName;

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

            this.rootFile = rootFile;
            this.treeName = treeName;
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
            /// Parse the query
            /// 



            throw new NotImplementedException();
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
