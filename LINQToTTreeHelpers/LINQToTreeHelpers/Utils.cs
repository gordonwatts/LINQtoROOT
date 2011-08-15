
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
namespace LINQToTreeHelpers
{
    /// <summary>
    /// Class holding extension methods for ROOT/string manipulation. Strings can be cleaned up for names,
    /// and replacement can be done for LaTeX strings in ROOT histogram titles, etc.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Write out an object. Eventually, with ROOTNET improvements this will work better and perahps
        /// won't be needed!
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dir"></param>
        internal static void InternalWriteObject(this ROOTNET.Interface.NTObject obj, ROOTNET.Interface.NTDirectory dir)
        {
            var h = obj as ROOTNET.Interface.NTH1;
            if (h != null)
            {
                var copy = h.Clone();
                dir.WriteTObject(copy); // Ugly from a memory pov, but...
                copy.SetNull();
            }
            else
            {
                dir.WriteTObject(obj);
                obj.SetNull();
            }
        }

        /// <summary>
        /// Take a string and "sanatize" it for a root name.
        /// </summary>
        /// <param name="name">Text name to be used as a ROOT name</param>
        /// <returns>argument name with spaces removes, as well as other characters</returns>
        public static string FixupForROOTName(this string name)
        {
            var result = name.Replace(" ", "");
            result = result.Replace("_{", "");
            result = result.Replace("{", "");
            result = result.Replace("}", "");
            result = result.Replace("-", "");
            result = result.Replace("\\", "");
            result = result.Replace("%", "");
            result = result.Replace("<", "lt");
            result = result.Replace(">", "gt");
            return result;
        }

        /// <summary>
        /// Find pT string.
        /// </summary>
        static List<Tuple<Regex, string>> gLatexReplacements = null;

        /// <summary>
        /// Look for a certian set of sequences and replace them with the ROOT LaTeX to make them
        /// "pretty".
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ReplaceLatexStrings(this string s)
        {
            InitLatexReplacementStrings();

            var result = s;
            foreach (var r in gLatexReplacements)
            {
                result = r.Item1.Replace(result, r.Item2);
            }

            return result;
        }

        /// <summary>
        /// Add a new symbol replacement (or abbreviation) to the list of latex replacements we keep track of.
        /// </summary>
        /// <param name="matcher"></param>
        /// <param name="replacement"></param>
        public static void AddLatexReplacement(Regex matcher, string replacement)
        {
            InitLatexReplacementStrings();
            gLatexReplacements.Add(Tuple.Create(matcher, replacement));
        }

        public static void AddLatexReplacement(string matcher, string replacement)
        {
            InitLatexReplacementStrings();
            gLatexReplacements.Add(Tuple.Create(new Regex(string.Format(@"\b{0}\b", matcher)), replacement));
        }

        /// <summary>
        /// Setup replacement strings
        /// </summary>
        private static void InitLatexReplacementStrings()
        {
            if (gLatexReplacements == null)
            {
                gLatexReplacements = new List<Tuple<Regex, string>>()
                {
                    Tuple.Create(new Regex(@"\bpT\b"), "p_{T}"),
                    Tuple.Create(new Regex(@"\bET\b"), "E_{T}"),
                    Tuple.Create(new Regex(@"\bMET\b"), "#slash{E}_{T}"),
                    Tuple.Create(new Regex(@"\bDR\b"), "#DeltaR"),
                    Tuple.Create(new Regex(@"\beta\b"), "#eta"),
                    Tuple.Create(new Regex(@"\bphi\b"), "#phi"),
                };
            }
        }
    }
}
