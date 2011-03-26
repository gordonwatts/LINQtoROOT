using System;
using System.Collections.Generic;
using System.Linq;
using TTreeDataModel;

namespace TTreeParser
{
    /// <summary>
    /// Utilities for helping deal with ROOT...
    /// </summary>
    static class Utils
    {
        /// <summary>
        /// Attempt to find all the objects that are of type T or inherrit from T in the input file,
        /// and return them. Note that the file or owner of the directory needs to stay open as long as the sequence
        /// is being parsed!! This is an iterator, and processes on an as-need basis!!!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rootDir"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindAllOfType<T>(ROOTNET.Interface.NTDirectory rootDir)
            where T : ROOTNET.Interface.NTObject
        {
            var objectKeys = rootDir.GetListOfKeys();
            string basename = typeof(T).Name.Substring(1);

            foreach (var item in objectKeys.AsEnumerable().Cast<ROOTNET.Interface.NTKey>())
            {
                var c = ROOTNET.NTClass.GetClass(item.GetClassName());
                if (c.InheritsFrom(basename))
                {
                    yield return (T)item.ReadObj();
                }
            }
        }

        /// <summary>
        /// Turn the list into something we can enumerate over!
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IEnumerable<ROOTNET.Interface.NTObject> AsEnumerable(this ROOTNET.Interface.NTCollection list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            var iter = new ROOTNET.NTIter(list);
            bool done = false;
            while (!done)
            {
                var result = iter.Next() as ROOTNET.Interface.NTObject;
                if (result != null)
                {
                    yield return result;
                }
                else
                {
                    done = true;
                }
            }
        }

        /// <summary>
        /// Figure out what a C++ type would look like in C#. Only simple types are done.
        /// </summary>
        /// <param name="cppTypeName"></param>
        /// <returns></returns>
        public static string SimpleCPPTypeToCSharpType(this string cppTypeName)
        {
            string result = cppTypeName;

            switch (cppTypeName)
            {
                case "char":
                    result = "sbyte";
                    break;

                case "int":
                    break;

                case "bool":
                    break;

                case "float":
                    break;

                case "double":
                    break;

                case "short":
                    break;

                case "unsigned int":
                    result = "uint";
                    break;

                case "unsigned long":
                    result = "uint";
                    break;

                case "unsigned short":
                    result = "ushort";
                    break;

                case "long long":
                    result = "long";
                    break;

                /// I always have trouble with this - but this is the case... at least on
                /// this platform - a long in C++ is a 32 bit integer. int is also a 32 bit
                /// integer...
                case "long":
                    result = "int";
                    break;

                case "unsigned long long":
                    result = "ulong";
                    break;

                default:
                    result = null;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Given a set of variable names, turn them into a list of Variable info
        /// </summary>
        /// <param name="varNames"></param>
        /// <returns></returns>
        public static IEnumerable<VariableInfo> ToVariableInfo(this IEnumerable<string> varNames)
        {
            var vars = from v in varNames
                       select new VariableInfo()
                       {
                           Name = v,
                           RenameTo = v
                       };
            return vars;
        }
    }
}
