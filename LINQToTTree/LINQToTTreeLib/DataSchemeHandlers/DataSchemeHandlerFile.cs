using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.DataSchemeHandlers
{
    /// <summary>
    /// Handle a normal local file.
    /// </summary>
    [Export(typeof(IDataFileSchemeHandler))]
    class DataSchemeHandlerFile : IDataFileSchemeHandler
    {
        public string Scheme => "file";

        /// <summary>
        /// Check to make sure the file exists!
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public bool GoodUri(Uri u)
        {
            return File.Exists(u.LocalPath);
        }

        public IEnumerable<Uri> ResolveUri(Uri u)
        {
            throw new NotImplementedException();
        }
    }
}
