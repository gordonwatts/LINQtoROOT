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
        /// We have no way of checking, so we just return true.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public bool GoodUri(Uri u)
        {
            return true;
        }

        public IEnumerable<Uri> ResolveUri(Uri u)
        {
            throw new NotImplementedException();
        }
    }
}
