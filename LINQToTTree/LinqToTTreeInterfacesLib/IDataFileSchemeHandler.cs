using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqToTTreeInterfacesLib
{
    /// <summary>
    /// Gather info about a Uri - date, etc. Allows for a single Uri to point to a collection of files.
    /// MEF is used to collect these, so they are meant to be implemented in exteranl code.
    /// </summary>
    /// <remarks>
    /// Every datafile processed by us comes in as a Uri, with a scheme. While we know only ow to
    /// handle a fall small number of schemes, this allows someone to plug in something much more
    /// interesting in the client. Like a dataset that exists only on the GRID... this would be used
    /// to translate it into files we could actually handle. However, everything about caching for results
    /// would be based on the original Uri - so even if the underlying file's location changes, there would
    /// be no triggering a reprocessing.
    /// 
    /// Make sure to mark your classes with a "[Export(typeof(IDataFileSchemeHandler))]" so that they
    /// are picked up by the system
    /// </remarks>
    public interface IDataFileSchemeHandler
    {
        /// <summary>
        /// Return the scheme that we can handle
        /// </summary>
        string Scheme { get; }

        /// <summary>
        /// Check to make sure the uri is good and has a good chance of being resolved. 
        /// </summary>
        /// <param name="u">The Uri to be checked. Will only be a Uri of a type that this guy can handle</param>
        /// <returns>True if we can trust this uri</returns>
        /// <remarks>
        /// </remarks>
        bool GoodUri(Uri u);

        /// <summary>
        /// Resolve the given uri. Return the new one. If the same one returned is the one given, then no further resolution
        /// is required. Otherwise, resolution with the appropriate Uri type will continue until changes top. A single Uri
        /// may also turn into multiple Uri's.
        /// 
        /// This is only called on a Uri that GoodUri has returned true on.
        /// </summary>
        /// <param name="u">Uri to be resolved</param>
        /// <returns>List of Uri's that should be processed.</returns>
        IEnumerable<Uri> ResolveUri(Uri u);

        /// <summary>
        /// Return the last time this Uri was modified - used to help understand
        /// if the input has changed (or not).
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        DateTime GetUriLastModificationDate(Uri u);
    }
}
