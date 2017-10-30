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
    /// Handle a local windows file - where we are planning on running externally.
    /// </summary>
    [Export(typeof(IDataFileSchemeHandler))]
    class DataSchemeHandlerLocalWin : IDataFileSchemeHandler
    {
        public string Scheme => "localwin";

        /// <summary>
        /// When was this file last modified?
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public DateTime GetUriLastModificationDate(Uri u)
        {
            return File.GetLastWriteTime(new UriBuilder(u) { Scheme = "file" }.Uri.LocalPath);
        }

        /// <summary>
        /// Check to make sure the file exists.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public bool GoodUri(Uri u)
        {
            // We have to convert the Uri - otherwise it won't render a UNC path
            // properly.
            return File.Exists(new UriBuilder(u) { Scheme = "file" }.Uri.LocalPath);
        }

        /// <summary>
        /// Resolve this Uri. We do nothing to it, so just return it.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public IEnumerable<Uri> ResolveUri(Uri u)
        {
            return new[] { u };
        }
    }
}
