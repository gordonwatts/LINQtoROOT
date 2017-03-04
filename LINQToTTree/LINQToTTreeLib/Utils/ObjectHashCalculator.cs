
using System;
using System.Collections.Generic;

namespace LINQToTTreeLib.Utils
{
    /// <summary>
    /// Helps with calculating hash values for objects
    /// </summary>
    internal class ObjectHashCalculator
    {
        /// <summary>
        /// Initalize to calculate a hash.
        /// </summary>
        internal ObjectHashCalculator()
        {
            unchecked
            {
                Hash = (int)2166136261;
            }
        }

        /// <summary>
        /// Adds to our internal hash value the hash calculated from the current object.
        /// </summary>
        /// <param name="o"></param>
        internal void AccumutlateHash(object o)
        {
            ///
            /// We have to first figure out what kind of object this is, unfortunatley. Perhaps
            /// eventually this will be MEF, but right now we only have simple guys.
            /// 

            if (o is ROOTNET.Interface.NTNamed)
            {
                InternalAccumulateNamedHash(o as ROOTNET.Interface.NTNamed);
            }
            else if (o is ROOTNET.Interface.NTObject)
            {
                InternalAccumulateHash(o as ROOTNET.Interface.NTObject);
            }
            else
            {
                throw new NotImplementedException("Unable to calculate the hassh for objects of type '" + o.GetType().Name + "'.");
            }
        }

        /// <summary>
        /// Keep track of # of unique names!
        /// </summary>
        private int _namedObjectCount = 0;

        /// <summary>
        /// Normalize the naming of an object. We don't really care about the names for a hash... so...
        /// </summary>
        /// <param name="namedObj"></param>
        private void InternalAccumulateNamedHash(ROOTNET.Interface.NTNamed namedObj)
        {
            string oName = namedObj.Name;
            string oTitle = namedObj.Title;

            namedObj.Name = "name_" + _namedObjectCount.ToString();
            namedObj.Title = "title_" + _namedObjectCount.ToString();

            InternalAccumulateHash(namedObj);

            namedObj.Name = oName;
            namedObj.Title = oTitle;
        }

        /// <summary>
        /// Dip into and return the buffer data. Avoids a memory allocation.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static IEnumerable<SByte> BufferAsBytes(ROOTNET.Interface.NTBuffer buffer)
        {
            int l = buffer.Length();
            var bufferData = buffer.Buffer();
            for (int i = 0; i < l; i++)
            {
                // Access the byte, but make sure to bomb with a clear error message if something
                // funny happens (we've seen real problems in the field).
                SByte r;
                try
                {
                    r = bufferData[i];
                } catch (Exception e)
                {
                    throw new InvalidOperationException($"Fatal error accessing the {i} element of an array that is {l} long.", e);
                }
                yield return r;
            }
        }

        /// <summary>
        /// Do the accumulation for a root object.
        /// </summary>
        /// <param name="nTObject"></param>
        private void InternalAccumulateHash(ROOTNET.Interface.NTObject nTObject)
        {
            if (nTObject == null)
                throw new ArgumentNullException("The argument to InternalAccumulateHash was null - not allowed.");

            // Write it to a ROOT buffer
            var buffer = new ROOTNET.NTBufferFile(ROOTNET.Interface.NTBuffer.EMode.kWrite);
            buffer.WriteObject(nTObject);
            //var result = buffer.Buffer().as_array(buffer.Length());
            var result = BufferAsBytes(buffer);

            // And update the hash value we are holding onto.
            ComputeHash(result);
        }

        /// <summary>
        /// Get the hash value as accumulated up to now by the object.
        /// </summary>
        public int Hash { get; private set; }

        /// <summary>
        /// Internal hash calculation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private void ComputeHash(IEnumerable<SByte> data)
        { 
            unchecked
            {
                const int p = 16777619;
                int hash = Hash;
                foreach (var d in data)
                {
                    hash = (hash ^ d) * p;
                }
                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;

                Hash = hash;
            }
        }
    }
}
