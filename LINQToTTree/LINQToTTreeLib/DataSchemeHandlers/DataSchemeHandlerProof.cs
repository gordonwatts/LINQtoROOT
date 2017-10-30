using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LINQToTTreeLib.DataSchemeHandlers
{
    [Export(typeof(IDataFileSchemeHandler))]
    internal class DataSchemeHandlerProof : IDataFileSchemeHandler
    {
        public string Scheme => "proof";

        /// <summary>
        /// Since we can't look at the guy - we ignore it.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public DateTime GetUriLastModificationDate(Uri u)
        {
            return new DateTime(1990, 12, 1);
        }

        /// <summary>
        /// We have no way of checking, so we just return true.
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
