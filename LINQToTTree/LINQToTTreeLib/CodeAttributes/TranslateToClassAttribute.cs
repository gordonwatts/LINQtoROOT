using System;

namespace LINQToTTreeLib.CodeAttributes
{
    /// <summary>
    /// Any expression involving this class should be translated into another type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class TranslateToClassAttribute : Attribute
    {
        // This is a positional argument
        public TranslateToClassAttribute(Type targetType)
        {
            TargetClassType = targetType;
        }

        public Type TargetClassType { get; set; }
    }
}
