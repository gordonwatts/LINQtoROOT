using System;
using System.Text;
using LinqToTTreeInterfacesLib;

namespace LINQToTTreeLib.Variables
{
    public static class VarUtils
    {
        public static string AsCastString(this IValue val)
        {
            if (val == null)
                throw new ArgumentNullException("Value can't be null");

            StringBuilder bld = new StringBuilder();
            bld.AppendFormat("(({0}){1})", val.Type.AsCPPType(), val.AsObjectReference());
            return bld.ToString();
        }

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

            if (destType == sourceValue.Type)
            {
                return sourceValue.AsCastString();
            }
            StringBuilder bld = new StringBuilder();
            bld.AppendFormat("(({0}){1})", destType.AsCPPType(), sourceValue.AsCastString());
            return bld.ToString();
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

        public static int _variableNameCounter = 0;
        public static string CreateUniqueVariableName(this Type sourceType)
        {
            return sourceType.Name.CreateUniqueVariableName();
        }

        public static string CreateUniqueVariableName(this string varbasename)
        {
            _variableNameCounter += 1;
            return varbasename + "_" + _variableNameCounter.ToString();
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
