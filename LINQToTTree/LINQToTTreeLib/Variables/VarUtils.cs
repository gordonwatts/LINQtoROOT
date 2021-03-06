﻿using LinqToTTreeInterfacesLib;
using LINQToTTreeLib.CodeAttributes;
using LINQToTTreeLib.Expressions;
using LINQToTTreeLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace LINQToTTreeLib.Variables
{
    public static class VarUtils
    {
        /// <summary>
        /// Make sure it is a solid reference, not a -> and ignore type issues. :-)
        /// 
        /// 1) If the object is an array (like int[]) then:
        ///    If the array is a member access from some obj, then we assume we need a pointer
        ///    If the array is off another array, then we assume we don't need a pointer.
        /// 
        /// 2) If it is a class, we assume it needs to be dereferenced.
        /// 
        /// 3) We assume no deref is required.
        /// 
        /// A few consequences of this logic:
        ///   1) Any non .NET class doesn't get dereferenced. We are probably pretty safe here
        ///   2) Any array doesn't get dereferenced unless it is coming from a root object. So if you call a function that returns a pointer
        ///      to an array then you'll get the wrong thing.
        ///   3) Any object that is part of a class will get dereferenced, even if it doesn't need it. This can cause problems
        ///      when there is a sub-object that is completely contained.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string AsObjectReference(this IValue val, Expression destExpression = null)
        {
            StringBuilder bld = new StringBuilder();

            //
            // If it isn't a pointer type (that is, a class) then
            // this is easy
            // 

            if (!val.Type.IsPointerType())
                return val.RawValue;

            //
            // If this is a "null" reference, then just return it.
            //

            if (val.RawValue == "0")
                return val.RawValue;

            //
            // We are adding a member de-referencing. If we know something about the thing we are de-referencing,
            // then use that in deciding if in C++ this is a pointer or not. The basic problem is that C# doesn't tell
            // the difference between a pointer or an object - so we have to infer it.
            // 

            bool isObject = true;
            if (destExpression != null)
            {
                if (destExpression.NodeType == ExpressionType.MemberAccess
                    && (destExpression as MemberExpression).Member.TypeHasAttribute<NotAPointerAttribute>() != null)
                {
                    // We are looking at a.b.c, and c is declared as not being a pointer. So we don't do a pointer de-ref.

                    isObject = false;
                }
                else if (val.Type.IsArray)
                {
                    if (destExpression.NodeType == ExpressionType.ArrayIndex)
                    {
                        //
                        // Dealing with the proper array acces is complex:
                        //   vector<vector<int>> stuff; -> stuff[0] -> object should be false.
                        //   vector<vector<TLorentzVector>> stuff; -> stuff[0] -> object should be false.
                        //   TClonesArray vector<int> stuff[] -> stuff[0] -> object should be true!
                        //   int[] parameter -> not object!
                        //

                        var rootMember = destExpression.RemoveArrayReferences();
                        isObject = false;
                        if (rootMember.NodeType == ExpressionType.MemberAccess)
                        {
                            var rootType = (rootMember as MemberExpression).Expression.Type;
                            isObject = rootType.TypeHasAttribute<TClonesArrayImpliedClassAttribute>() != null;
                        }
                    }
                    else if (destExpression.NodeType == ExpressionType.Parameter)
                    {
                        // A parameter inserted by our own code.
                        isObject = false;
                    }
                    else if (destExpression is DeclarableParameter)
                    {
                        // Same wiht a declarable parameter - don't defreference it.
                        isObject = false;
                    } else if (destExpression.NodeType != ExpressionType.MemberAccess)
                    {
                        // An array that is getting returned from a function.
                        isObject = false;
                    }
                }
            }

            ///
            /// Now, return our result!
            /// 

            if (isObject)
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
        /// Takes the value, out puts it, and casts it to the type destType. We use heuristics at the moment to figure out what
        /// the reference is. We are forced to do this at the moment because in C++ there are points and also references to objects,
        /// a distinction not made in .NET...
        /// 
        /// 1) If the object is an array (like int[]) then:
        ///    If the array is a member access from some obj, then we assume we need a pointer
        ///    If the array is off another array, then we assume we don't need a pointer.
        /// 
        /// 2) If it is a class, we assume it needs to be dereferenced.
        /// 
        /// 3) We assume no deref is required.
        /// 
        /// A few consequences of this logic:
        ///   1) Any non .NET class doesn't get dereferenced. We are probably pretty safe here
        ///   2) Any array doesn't get dereferenced unless it is coming from a root object. So if you call a function that returns a pointer
        ///      to an array then you'll get the wrong thing.
        ///   3) Any object that is part of a class will get dereferenced, even if it doesn't need it. This can cause problems
        ///      when there is a sub-object that is completely contained.
        /// 
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="expressionDestType"></param>
        /// <returns></returns>
        public static string CastToType(this IValue sourceValue, Expression expressionDestType, bool ignorePointer = false)
        {
            if (sourceValue == null || expressionDestType == null)
                throw new ArgumentNullException("Cannot pass ource or dest type/vars as null");

            string objRefForm = sourceValue.RawValue;
            if (!ignorePointer)
                objRefForm = sourceValue.AsObjectReference(expressionDestType);

            ///
            /// If the type is already there or if the type is an array, then we will do no conversion.
            /// Already there: not needed
            /// Array: vector or regular C style array - we don't know this deep in the code, so
            ///        conversion would probably make a mess of things!
            ///

            if (!RequiresConversion(expressionDestType.Type, sourceValue.Type) || expressionDestType.Type.IsArray)
            {
                return objRefForm;
            }

            ///
            /// Type conversion required!
            /// 

            StringBuilder bld = new StringBuilder();
            bld.AppendFormat("(({0}){1})", expressionDestType.Type.AsCPPType(), objRefForm);
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
                if (sourceType == typeof(float))
                    return false;
            }

            /// Oh well. convert!

            return true;
        }

        /// <summary>
        /// Track a term search string.
        /// </summary>
        static Regex singleTerm = new Regex(@"^\w+$");

        /// <summary>
        /// Returns true if this is ValSimple is simple (like aInt32_1) vs complex (v.Phi()).
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        internal static bool IsSimpleTerm(this IValue v)
        {
            return singleTerm.Match(v.RawValue).Success;
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

            //
            // Simple type or array?
            //

            if (t.IsArray)
            {
                return string.Format("vector<{0}>", t.GetElementType().AsCPPType());
            }
            else if (t.IsGenericType)
            {
                var genericDef = t.GetGenericTypeDefinition();
                if (genericDef.Name == "Dictionary`2")
                {
                    var tlist = t.GetGenericArguments();
                    return string.Format("map<{0}, {1} >", tlist[0].AsCPPType(), tlist[1].AsCPPType());
                }
                else if (genericDef == typeof(IEnumerable<int>).GetGenericTypeDefinition())
                {
                    return string.Format("{0}::const_iterator", t.GetGenericArguments()[0].AsCPPType());
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Do not know how to convert generic type '{0}' to a C++ type", t.FullName));
                }
            }
            else
            {

                if (t == typeof(int))
                {
                    return "int";
                }
                if (t == typeof(uint))
                {
                    return "unsigned int";
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
            }

            // See if there is a custom attribute assocaited that we should use
            var attr = t.GetCustomAttributes(typeof(CPPObjectRepresentationTypeAttribute), false).FirstOrDefault();
            if (attr != null)
            {
                return (attr as CPPObjectRepresentationTypeAttribute).CPPTypeName;
            }
            
            ///
            /// Ok - if this is an object, for example, the enclosing ntuple object
            /// 

            return t.Name;
        }

        /// <summary>
        /// Returns true if this is a pointer to a class of some sort.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
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
            if (t == null)
                throw new ArgumentNullException("type must not be null");
            return t.Name.Substring(1).IsROOTClass();
        }

        /// <summary>
        /// Performs all expression subsitutions known by the code context on the input. It does not
        /// touch the input - but returns a new IValue. Types are not tracked.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cc"></param>
        /// <returns></returns>
        /// <remarks>
        /// This exists because sometimes you need to do this sort of replacement when dealing with complex
        /// expressions that don't translated to C# easily. In partiuclar, for example, the iterator expressions
        /// that are used to move through maps in the Group By operators. Otherwise, the regular GetExpression would
        /// do this just fine.
        /// </remarks>
        public static IValue PerformAllSubstitutions(this IValue input, ICodeContext cc)
        {
            var vFinder = new Regex(@"\b(?<vname>[\w]*)\b");
            string v = input.RawValue;
            bool subDone = true;
            int count = 0;
            while (subDone)
            {
                count++;
                if (count > 100)
                    throw new InvalidOperationException(string.Format("Unable to translate '{0}' due to too many sutstitutions.", input.RawValue));

                subDone = false;
                foreach (Match match in vFinder.Matches(v))
                {
                    var vname = match.Groups["vname"].Value;
                    var r = cc.GetReplacement(vname);
                    if (r != null)
                    {
                        v = Regex.Replace(v, string.Format(@"\b{0}\b", vname), r.ParameterName());
                        subDone = true;
                    }
                }
            }

            if (count == 1)
                return input;

            return new ValSimple(v, input.Type, input.Dependants);
        }
    }
}
