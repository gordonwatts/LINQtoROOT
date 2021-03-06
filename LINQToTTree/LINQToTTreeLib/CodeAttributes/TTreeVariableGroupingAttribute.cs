﻿using System;

namespace LINQToTTreeLib.CodeAttributes
{
    /// <summary>
    /// Adorns a object variable reference when it is a grouping object. The variables
    /// referred to in this object are actually arrays, and this object is grouped around them.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class TTreeVariableGroupingAttribute : Attribute
    {
        // This is a positional argument
        public TTreeVariableGroupingAttribute()
        {
        }
    }
}
