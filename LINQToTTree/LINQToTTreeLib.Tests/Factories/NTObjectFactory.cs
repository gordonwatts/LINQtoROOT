using Microsoft.Pex.Framework;

namespace ROOTNET
{
    /// <summary>A factory for ROOTNET.NTObject instances</summary>
    public static partial class NTObjectFactory
    {
        /// <summary>A factory for ROOTNET.NTObject instances</summary>
        [PexFactoryMethod(typeof(NTObject))]
        public static NTObject Create()
        {
            NTObject nTObject = new NTObject();
            return nTObject;
        }
    }
}
