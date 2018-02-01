
using LINQToTTreeLib.Utils;
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
        /// <param name="obj">The object to be written. Assumed not null.</param>
        /// <param name="dir"></param>
        internal static void InternalWriteObject(this ROOTNET.Interface.NTObject obj, ROOTNET.Interface.NTDirectory dir)
        {
            if (obj == null)
            {
                Console.WriteLine("WARNING: Unable to write out null object to a TDirectory!");
                return;
            }

            using (ROOTLock.Lock())
            {
                if (obj is ROOTNET.Interface.NTH1 h)
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
        /// Do a string.Format, but if there is something like this "hi {0} there" and param[0] is "", then replace it
        /// with "hi there" (only a single space).
        /// </summary>
        /// <param name="format"></param>
        /// <param name="replacements"></param>
        /// <returns></returns>
        public static string FormatStringWithoutDoubleSpaces(string format, params string[] replacements)
        {
            foreach (var p in replacements.ZipWithNumber())
            {
                if (string.IsNullOrEmpty(p.Item1))
                {
                    var refFmt = "{" + p.Item2.ToString() + "}";
                    format = format.Replace($" {refFmt} ", $" ");
                }
            }

            return string.Format(format, replacements);
        }

        /// <summary>
        /// Return a tuple of the items, labeled with a number
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private static IEnumerable<Tuple<T, int>> ZipWithNumber<T>(this IEnumerable<T> source)
        {
            int index = 0;
            foreach (var item in source)
            {
                yield return Tuple.Create(item, index);
                index++;
            }
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
        /// Add a new symbol replacement (or abbreviation) to the list of latex replacements we keep track of using a regular expression.
        /// </summary>
        /// <param name="matcher"></param>
        /// <param name="replacement"></param>
        public static void AddLatexReplacement(Regex matcher, string replacement)
        {
            InitLatexReplacementStrings();
            gLatexReplacements.Add(Tuple.Create(matcher, replacement));
        }

        /// <summary>
        /// Add a new symbol replacement (or abbreviation) to the list of latex replacements we keep track of using a straight word-bounded string replacement.
        /// </summary>
        /// <param name="sourceString">The string to replace in ROOT histogram titles, etc. Only when this appears as a standalone as a word will it be replaced.</param>
        /// <param name="replacement">The string it should be replaced with</param>
        public static void AddLatexReplacement(string sourceString, string replacement)
        {
            InitLatexReplacementStrings();
            gLatexReplacements.Add(Tuple.Create(new Regex(string.Format(@"\b{0}\b", sourceString)), replacement));
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
                    Tuple.Create(new Regex(@"\bLxy\b"), "L_{xy}"),
                    Tuple.Create(new Regex(@"\bET\b"), "E_{T}"),
                    Tuple.Create(new Regex(@"\bMET\b"), "#slash{E}_{T}"),
                    Tuple.Create(new Regex(@"\bDR\b"), "#DeltaR"),
                    Tuple.Create(new Regex(@"\beta\b"), "#eta"),
                    Tuple.Create(new Regex(@"\bphi\b"), "#phi"),
                };
            }
        }

        internal class AxisInfo
        {
            public string Title;
            public string AxisTitle;
        }

        /// <summary>
        /// Return the title info split by the ";" to remove axis info.
        /// </summary>
        /// <param name="htitle"></param>
        /// <returns></returns>
        internal static AxisInfo ExtractHistoTitleInfo (this string htitle)
        {
            var info = new AxisInfo() { Title = htitle, AxisTitle = "" };
            var semi = htitle.IndexOf(";");
            if (semi >= 0)
            {
                info.Title = htitle.Substring(0, semi).Trim();
                info.AxisTitle = htitle.Substring(semi + 1).Trim();
            }
            return info;
        }
    }
}
