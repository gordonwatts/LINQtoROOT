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
    /// Uri to run bash locally in the WSL.
    /// </summary>
    [Export(typeof(IDataFileSchemeHandler))]
    class DataSchemeHandlerLocalBash : IDataFileSchemeHandler
    {
        public string Scheme => "localbash";

        /// <summary>
        /// Check to see if the file exists
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public bool GoodUri(Uri u)
        {
            return File.Exists(new UriBuilder(u) { Scheme = "file" }.Uri.LocalPath);
        }

        public IEnumerable<Uri> ResolveUri(Uri u)
        {
            throw new NotImplementedException();
        }
    }
}
