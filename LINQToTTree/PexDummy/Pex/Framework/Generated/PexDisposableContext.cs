using System;

namespace Microsoft.Pex.Framework.Generated
{
    public class PexDisposableContext : IDisposable
    {
        public static PexDisposableContext Create()
        {
            return new PexDisposableContext();
        }


        public void Add(IDisposable d)
        {

        }
        public void Dispose()
        {
        }
    }
}
