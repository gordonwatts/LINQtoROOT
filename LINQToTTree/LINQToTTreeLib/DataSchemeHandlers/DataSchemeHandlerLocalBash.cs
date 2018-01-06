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
        /// Return the last modification date of this file
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public DateTime GetUriLastModificationDate(Uri u)
        {
            return File.GetLastWriteTime(new UriBuilder(u) { Scheme = "file" }.Uri.LocalPath);
        }

        /// <summary>
        /// Check to see if the file exists
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public bool GoodUri(Uri u)
        {
            return File.Exists(new UriBuilder(u) { Scheme = "file" }.Uri.LocalPath);
        }

        /// <summary>
        /// Return a normalized Uri. The Uri is unique, so just return ourselves.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public Uri Normalize(Uri u)
        {
            return u;
        }

        /// <summary>
        /// Resolve this Uri. We do nothing to it, so just return it.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public Task<IEnumerable<Uri>> ResolveUri(Uri u)
        {
            return Task.FromResult(new[] { u }.AsEnumerable());
        }
    }
}
