﻿using System;
using System.Text;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.TypeHandlers.ROOT
{
    /// <summary>
    /// Hold onto a value that represents a ROOT object. This value is actually something that gets loaded from remote locations!
    /// each time this is referenced we will be creating a new copy of it (i.e. memory leak if you aren't careful).
    /// </summary>
    class ROOTObjectCopiedValue : IValue
    {
        /// <summary>
        /// Create a reference to a ROOT variable that is going to be loaded remotely.
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="rootType"></param>
        public ROOTObjectCopiedValue(string varName, Type rootType, string CPPType, string name)
        {
            if (string.IsNullOrWhiteSpace(varName))
                throw new ArgumentException("invalid variable name");

            if (string.IsNullOrWhiteSpace(CPPType))
                throw new ArgumentException("Invalid C++ type name");

            if (rootType == null)
                throw new ArgumentException("Invalid type");

            ///
            /// The type and the loader string
            /// 

            Type = rootType;
            OriginalName = name;

            StringBuilder loadString = new StringBuilder();
            loadString.AppendFormat("LoadFromInputList<{0}>(\"{1}\")", CPPType, varName);
            RawValue = loadString.ToString();
        }

        /// <summary>
        /// Return the Raw value - what we should use when we try to access this value.
        /// </summary>
        public string RawValue { get; private set; }

        /// <summary>
        /// The type of this variable
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// The name of the original root object - we will reset it to store it correctly.
        /// </summary>
        public string OriginalName { get; private set; }
    }
}
