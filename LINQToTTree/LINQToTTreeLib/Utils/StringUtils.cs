using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LINQToTTreeLib.Utils
{
    public static class StringUtils
    {
        /// <summary>
        /// Search a string for all matches of regexpr. Replace each occurance with a unique new string,
        /// normalizing. Do it in sorted order.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="regexpr"></param>
        /// <returns></returns>
        public static string SwapOutWithUninqueString(this string src, string regexpr)
        {
            Regex search = new Regex(regexpr);
            var matches = search.Matches(src);
            HashSet<string> allMatches = new HashSet<string>();
            foreach (Match m in matches)
            {
                allMatches.Add(m.Value);
            }

            var sortedMatches = from m in allMatches
                                orderby m ascending
                                select m;

            var result = src;
            int index = 0;
            foreach (var m in sortedMatches)
            {
                result = result.Replace(m, "gen_" + index.ToString());
                index++;
            }
            return result;
        }

        /// <summary>
        /// Find all words that match and replace them.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="replacement"></param>
        /// <returns></returns>
        public static string ReplaceVariableNames(this string source, string varname, string replacement)
        {
            var search = new Regex(string.Format(@"\b{0}\b", varname));
            return search.Replace(source, replacement);
        }

        /// <summary>
        /// Replace all words from a list of possible replacements
        /// </summary>
        /// <param name="source"></param>
        /// <param name="replacements"></param>
        /// <returns></returns>
        public static string ReplaceVariableNames(this string source, IEnumerable<Tuple<string,string>> replacements)
        {
            if (replacements == null)
                return source;

            foreach (var i in replacements)
            {
                source = source.ReplaceVariableNames(i.Item1, i.Item2);
            }
            return source;
        }

        /// <summary>
        /// If there are characters in this string that must be escaped before the string is turned from its internal rep into
        /// one for C++, do it.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static string AddCPPEscapeCharacters(this string original)
        {
            return original
                .Replace("\\", "\\\\");
        }
    }
}
