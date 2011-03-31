using System.Collections.Generic;
using System.IO;
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

            foreach (var item in objectKeys.Cast<ROOTNET.Interface.NTKey>())
            {
                var c = ROOTNET.NTClass.GetClass(item.GetClassName());
                if (c.InheritsFrom(basename))
                {
                    yield return (T)item.ReadObj();
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
                           NETName = v,
                           TTreeName = v
                       };
            return vars;
        }

        /// <summary>
        /// Parses a file in INI format.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Dictionary<string, string[]> ParseINIFormat(this TextReader input)
        {
            var result = new Dictionary<string, List<string>>();

            string line;
            string sectionName = null;
            do
            {
                line = input.ReadLine();
                if (line != null)
                {
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        sectionName = line.Substring(1, line.Length - 2);
                        if (!result.ContainsKey(sectionName))
                            result[sectionName] = new List<string>();
                    }
                    else
                    {
                        if (sectionName != null)
                        {
                            result[sectionName].Add(line);
                        }
                    }
                }
            } while (line != null);

            var arrayified = from p in result
                             select new
                             {
                                 Key = p.Key,
                                 Val = p.Value.ToArray()
                             };
            var resultArray = new Dictionary<string, string[]>();
            foreach (var p in arrayified)
            {
                resultArray[p.Key] = p.Val;
            }

            return resultArray;
        }

        /// <summary>
        /// Parse a file as an ini file.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="okIfMissing"></param>
        /// <returns></returns>
        public static Dictionary<string, string[]> ParseAsINIFile(this FileInfo input, bool okIfMissing = true)
        {
            if (!input.Exists)
            {
                if (okIfMissing)
                    return new Dictionary<string, string[]>();
                throw new FileNotFoundException("Unable to locate file", input.FullName);
            }

            using (var reader = input.OpenText())
            {
                return reader.ParseINIFormat();
            }
        }
    }
}
