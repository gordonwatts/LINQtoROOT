using System;

namespace Microsoft.ExtendedReflection.DataAccess
{
    [Serializable]
    public class TermDestructionException : Exception
    {
        public TermDestructionException() { }
        public TermDestructionException(string message) : base(message) { }
        public TermDestructionException(string message, Exception inner) : base(message, inner) { }
        protected TermDestructionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
