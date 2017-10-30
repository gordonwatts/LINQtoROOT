using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.DataSchemeHandlers
{
    /// <summary>
    /// Remote bash handler.
    /// </summary>
    [Export(typeof(IDataFileSchemeHandler))]
    class DataSchemeHandlerRemoteBash : IDataFileSchemeHandler
    {
        public string Scheme => "remotebash";

        /// <summary>
        /// We don't really have a way of checking fast right now, so we don't.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public bool GoodUri(Uri u)
        {
            return true;
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
