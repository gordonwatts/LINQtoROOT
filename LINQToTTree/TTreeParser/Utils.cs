using System.Collections.Generic;
using System.IO;
using System.Linq;
using TTreeDataModel;
using System;

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
        /// If this class is a shell class - that is, it has nothing defined in it, then...
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        public static bool IsShellTClass (this ROOTNET.Interface.NTClass cls)
        {
            var v = (cls.ListOfAllPublicDataMembers != null && cls.ListOfAllPublicDataMembers.Entries > 0)
                || (cls.ListOfAllPublicMethods != null && cls.ListOfAllPublicMethods.Entries > 0);
            return !v;
        }

        /// <summary>
        /// Return true if this class is a STL class.
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        public static bool IsSTLClass(this ROOTNET.Interface.NTClass cls)
        {
            var name = cls.Name;
            if (name.StartsWith("vector"))
            {
                if (name.Contains("<"))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Is this class a template type or not?
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        public static bool IsTemplateClass (this ROOTNET.Interface.NTClass cls)
        {
            return cls.Name.Contains("<");
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

        /// <summary>
        /// Makes sure a created formula is "good".
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static bool IsGoodFormula(this ROOTNET.Interface.NTTreeFormula f)
        {
            f.QuickLoad = true;
            return f.Ndim != 0;
        }

        /// <summary>
        /// Returns a tree name sanitized for use as C++ object name.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string SanitizedName(this ROOTNET.Interface.NTTree t)
        {
            var n = t.Name;
            return n.SanitizedName();
        }

        /// <summary>
        /// Returns a string that has any funny characters replaced by good ones - so we can use them in variable names.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static string SanitizedName(this string n)
        {
            n = n.Replace("#", "_");
            n = n.Replace(",", "_");
            n = n.Replace("<", "");
            n = n.Replace(">", "");
            n = n.Replace(" ", "");
            n = n.Replace(":", "");
            n = n.Replace(".", "_");
            return n;
        }

        /// <summary>
        /// Given a list of ROOTClassShell's, make sure they are identical.
        /// </summary>
        /// <param name="scg"></param>
        /// <returns></returns>
        public static bool ClassesAreIdnetical(this IEnumerable<ROOTClassShell> scg)
        {
            var f = scg.First();
            foreach (var o in scg.Skip(1))
            {
                if (f.IsTClonesArrayClass != o.IsTClonesArrayClass)
                    throw new InvalidDataException(string.Format("IsTClonesArrayClass not the same in duplicate {0} classes.", f.Name));
                if (f.IsTopLevelClass != o.IsTopLevelClass)
                    throw new InvalidDataException(string.Format("IsTopLevelClass is not the same in duplicate {0} classes.", f.Name));
                if (f.Items.Count != o.Items.Count)
                    throw new InvalidDataException(string.Format("Number of items is not the same in duplicate {0} classes.", f.Name));
                foreach (var item in f.Items.Zip(o.Items, (n1, n2) => Tuple.Create(n1, n2)))
                {
                    if (item.Item1.Name != item.Item2.Name)
                        throw new InvalidDataException(string.Format("Duplicate classes {0} are defined with different item names", f.Name));
                    if (item.Item1.ItemType != item.Item2.ItemType)
                        throw new InvalidDataException(string.Format("Duplicate classes {0} are defined with different item types", f.Name));

                }
            }

            return true;
        }

    }
}
