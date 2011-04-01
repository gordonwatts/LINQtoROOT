﻿using System;
using System.Text;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Variables
{
    public static class VarUtils
    {
#if false
        public static string CastToType(this IValue val)
        {
            if (val == null)
                throw new ArgumentNullException("Value can't be null");

            if (!val.Type.IsArray)
            {
                StringBuilder bld = new StringBuilder();
                bld.AppendFormat("(({0}){1})", val.Type.AsCPPType(), val.AsObjectReference());
                return bld.ToString();
            }
            else
            {
                return val.AsObjectReference();
            }
        }
#endif

        /// <summary>
        /// Make sure it is a solid reference, not a -> and ignore type issues. :-)
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string AsObjectReference(this IValue val)
        {
            StringBuilder bld = new StringBuilder();
            if (val.Type.IsPointerType())
            {
                bld.AppendFormat("(*{0})", val.RawValue);
                return bld.ToString();
            }
            else
            {
                return val.RawValue;
            }
        }

        /// <summary>
        /// Takes the value, out puts it, and casts it to the type destType
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="destType"></param>
        /// <returns></returns>
        public static string CastToType(this IValue sourceValue, Type destType)
        {
            if (sourceValue == null || destType == null)
                throw new ArgumentNullException("Cannot pass ource or dest type/vars as null");

            var objRefForm = sourceValue.AsObjectReference();

            ///
            /// If the type is already there or if the type is an array, then we will do no conversion.
            /// Already there: not needed
            /// Array: vector or regular C style array - we don't know this deep in the code, so
            ///        conversion would probably make a mess of things!
            ///

            if (!RequiresConversion(destType, sourceValue.Type) || destType.IsArray)
            {
                return objRefForm;
            }

            ///
            /// Type conversion required!
            /// 

            StringBuilder bld = new StringBuilder();
            bld.AppendFormat("(({0}){1})", destType.AsCPPType(), objRefForm);
            return bld.ToString();
        }

        /// <summary>
        /// Determine if a conversion is required.
        /// </summary>
        /// <param name="destType"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool RequiresConversion(Type destType, Type sourceType)
        {
            if (destType == sourceType)
                return false;

            /// If the dest is double and the source if float, or int or something like that, ignore it

            if (destType == typeof(double))
            {
                if (sourceType == typeof(float) || sourceType == typeof(int))
                    return false;
            }

            /// Oh well. convert!

            return true;
        }

        /// <summary>
        /// Returns the type as a CPP type
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string AsCPPType(this Type t)
        {
            if (t == null)
                throw new ArgumentNullException("Type must not be null");
            if (t.IsArray)
                throw new ArgumentException("Type '" + t.Name + "' is an array - clean conversion to C++ is not possible!");

            if (t == typeof(int))
            {
                return "int";
            }
            else if (t == typeof(double))
            {
                return "double";
            }
            else if (t == typeof(float))
            {
                return "float";
            }
            else if (t == typeof(bool))
            {
                return "bool";
            }

            ///
            /// Is this a ROOT type? then translate it as well!
            /// 

            if (t.FullName.StartsWith("ROOTNET.N"))
            {
                return t.FullName.Substring(9) + "*";
            }
            if (t.FullName.StartsWith("ROOTNET.Interface.N"))
            {
                return t.FullName.Substring(19) + "*";
            }

            ///
            /// Ok - if this is an object, for example, the enclosing ntuple object
            /// 

            return t.Name;

            ///
            /// No good. We are outta here!
            /// 

            throw new ArgumentException("Unknown type '" + t.ToString() + "' - don't know how to convert!!");
        }

        public static bool IsPointerType(this Type t)
        {
            if (t == null)
                throw new ArgumentNullException();

            return t.IsClass;
        }

        /// <summary>
        /// Figure out if this is a root class or not!
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static bool IsROOTClass(this string className)
        {
            var c = ROOTNET.NTClass.GetClass(className);
            return c != null;
        }

        /// <summary>
        /// Returns true if this type is a ROOT class
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsROOTClass(this Type t)
        {
            return t.Name.Substring(1).IsROOTClass();
        }
    }
}
