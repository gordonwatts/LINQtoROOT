using LinqToTTreeInterfacesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LINQToTTreeLib.Utils;
using ROOTNET.Interface;
using System.Text.RegularExpressions;

namespace LINQToTTreeLib.Utils
{
    public static class RunInfoUtil
    {
        /// <summary>
        /// Thrown when what we get back from a cache doesn't make sense.
        /// </summary>
        [Serializable]
        public class InvalidCachedObjectException : Exception
        {
            public InvalidCachedObjectException() { }
            public InvalidCachedObjectException(string message) : base(message) { }
            public InvalidCachedObjectException(string message, Exception inner) : base(message, inner) { }
            protected InvalidCachedObjectException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        /// <summary>
        /// Parse the name that we cached
        /// </summary>
        private static Regex _parseRunInfoName = new Regex(@"^__(\d+)_(\w+)$");

        /// <summary>
        /// See if we can't get this object back.
        /// </summary>
        /// <param name="source">The TMap object to be translated into a RunInfo object.</param>
        /// <returns></returns>
        public static RunInfo ToRunInfo(this ROOTNET.Interface.NTObject source)
        {
            var r = _parseRunInfoName.Match(source.Name);
            if (!r.Success)
            {
                throw new InvalidCachedObjectException($"Cached returned an object with a name {source.Name} - but it isn't in the format __NNN_NAME. Boom!");
            }

            return new RunInfo()
            {
                _cycle = int.Parse(r.Groups[1].Value),
                _result = source.Clone(r.Groups[2].Value)
            };

        }

        /// <summary>
        /// Convert from a object and a name to a RunInfo with the cycle, etc., parsed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static RunInfo ToRunInfo(this ROOTNET.Interface.NTObject source, string name)
        {
            var r = _parseRunInfoName.Match(name);
            if (!r.Success)
            {
                throw new InvalidCachedObjectException($"Cached returned an object with a name {source.Name} - but it isn't in the format __NNN_NAME. Boom!");
            }

            return new RunInfo()
            {
                _cycle = int.Parse(r.Groups[1].Value),
                _result = source.Clone(r.Groups[2].Value)
            };

        }

        /// <summary>
        /// Convert a RUnInfo object into a TMap object which can then be cached in a ROOT file.
        /// </summary>
        /// <param name="source">The RunInfo object be translated into a TMap</param>
        /// <param name="clone">If ture then the result is cloned before being put in the map object</param>
        /// <returns>A TMap object that holds the same info as the source parameter</returns>
        /// <remarks>Sadly, there is no native and non-template way to store an integer for teh cycle number in ROOT. So we have to
        /// fall back to using a TH1I. Bummer.</remarks>
        public static NTObject ToTMap(this RunInfo source)
        {
            var cobj = source._result.Clone();
            return cobj;
        }

        /// <summary>
        /// Given a RunInfo return the name that we should be writing this out to
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ROOTFileKey(this RunInfo source) => $"__{source._cycle}_{source._result.Name}";
    }
}
