﻿using LinqToTTreeInterfacesLib;
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
        /// We can't look at the file - so we will pretend it is a "certain" date.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public DateTime GetUriLastModificationDate(Uri u)
        {
            return new DateTime(1990, 12, 1);
        }

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
        /// We need to be agnostic of the location we are running on and options. It doesn't really matter, for example,
        /// how many connections we are running against. Nor does it matter that we used machine A vs B.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        /// <remarks>
        /// One side effect of this is we are assuming a global name space for file naming.
        /// </remarks>
        public Uri Normalize(Uri u)
        {
            return new UriBuilder(u) { Host = "machine", UserName = "dude", Password = "other", Query = "" }.Uri;
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
