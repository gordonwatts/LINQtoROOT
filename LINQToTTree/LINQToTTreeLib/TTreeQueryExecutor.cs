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
            this.rootFile = rootFile;
            this.treeName = treeName;
        }

        public IEnumerable<T> ExecuteCollection<T>(QueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        public T ExecuteScalar<T>(QueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        public T ExecuteSingle<T>(QueryModel queryModel, bool returnDefaultWhenEmpty)
        {
            throw new NotImplementedException();
        }
    }
}
