
using System;
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
        public ObjectHashCalculator()
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
        /// Do the accumulation for a root object.
        /// </summary>
        /// <param name="nTObject"></param>
        private void InternalAccumulateHash(ROOTNET.Interface.NTObject nTObject)
        {
            var buffer = new ROOTNET.NTBufferFile(ROOTNET.Interface.NTBuffer.EMode.kWrite);
            buffer.WriteObject(nTObject);
            var result = buffer.Buffer().as_array(buffer.Length());
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
        public void ComputeHash(params SByte[] data)
        {
            unchecked
            {
                const int p = 16777619;
                int hash = Hash;
                for (int i = 0; i < data.Length; i++)
                    hash = (hash ^ data[i]) * p;
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
