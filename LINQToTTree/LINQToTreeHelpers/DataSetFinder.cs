using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sprache;

namespace LINQToTreeHelpers
{
    /// <summary>
    /// Parse the dataset file, return CollecitonTree's.
    /// Using a combinator-parser - very cool!
    /// http://nblumhardt.com/2010/01/building-an-external-dsl-in-c/
    /// </summary>
    public class DataSetFinder
    {
        /// <summary>
        /// Basic string token.
        /// </summary>
        private static readonly Parser<string> Identifier = (Parse.LetterOrDigit.Or(Parse.Char('_'))).AtLeastOnce().Text().Token();

        /// <summary>
        /// A file path. It might be surrounded by quotes, actually...
        /// </summary>

        private static readonly Parser<char> FilePathNormalChars =
            Parse.LetterOrDigit
            .Or(Parse.Char('\\'))
            .Or(Parse.Char(':'))
            .Or(Parse.Char('.'))
            .Or(Parse.Char('_'))
            .Or(Parse.Char('-'))
            .Or(Parse.Char('$'))
            .Or(Parse.Char('*'));

        private static readonly Parser<string> FilePathStringUnquoted =
            (
            FilePathNormalChars.AtLeastOnce().Text().Token()
            ).Named("FilePathStringUnquoted");

        private static readonly Parser<string> FilePathStringQuoted =
            (
            from openQ in Parse.Char('"')
            from searchstring in Parse.CharExcept('"').AtLeastOnce().Text()
            from closeQ in Parse.Char('"')
            select searchstring
            ).Named("FilePathStringQuoted");

        private static readonly Parser<string> FilePathString =
            (
            FilePathStringQuoted.Or(FilePathStringUnquoted)
            ).Named("FilePathString");

        private static readonly Parser<string[]> FilePathStringAsArrayParser =
            (from f in FilePathString
             select new string[] { f }).Named("FilePathStringAsArrayParser");

        /// <summary>
        /// Look for file-path | file-path | file-path... always return the first
        /// one that matches!
        /// </summary>
        private static readonly Parser<string[]> MultiFileSearchString =
            from first in FilePathString
            from rest in FilePathOrString.Many()
            select (new string[] { first }.Concat(rest)).ToArray();

        private static readonly Parser<string> FilePathOrString =
            from orBar in Parse.Char('|')
            from fname in FilePathString
            select fname;

        /// <summary>
        /// Keep track of a name-value pairing, and parsers for its general form
        /// follow this class definition.
        /// </summary>
        class NameValue
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        private static readonly Parser<NameValue> NameValueFinder =
            from n in Identifier
            from eq in Parse.Char('=')
            from v in Identifier
            select new NameValue() { Name = n, Value = v };

        private static readonly Parser<NameValue> NamePathFinder =
            from n in Identifier
            from wh1 in Parse.WhiteSpace.Many()
            from eq in Parse.Char('=')
            from wh2 in Parse.WhiteSpace.Many()
            from v in FilePathString
            select new NameValue() { Name = n, Value = v };

        /// <summary>
        /// A dataset. The dataset has a name and then it has a list of search strings. The list is sperated by
        /// "|" which means one should "or" the search strings. The semantics are that the first search string
        /// that yeilds a valid file will be used.
        /// </summary>

        class DataSetDefinition
        {
            public string Name { get; set; }
            public string[] Tags { get; set; }
            public string[] SearchStrings { get; set; }
        }

        private static readonly Parser<string[]> TagListParser =
            from openParen1 in Parse.Char('(')
            from tagName in Identifier
            from openParen in Parse.Char(')')
            select new string[] { tagName };

        private static readonly Parser<DataSetDefinition> DataSetDefinitionWithTagParser =
            from n in Identifier
            from wp1 in Parse.WhiteSpace.Many()
            from tags in TagListParser
            from wp2 in Parse.WhiteSpace.Many()
            from eq in Parse.Char('=')
            from wp3 in Parse.WhiteSpace.Many()
            from v in MultiFileSearchString.Or(FilePathStringAsArrayParser)
            select new DataSetDefinition() { Name = n, SearchStrings = v, Tags = tags };

        private static readonly Parser<DataSetDefinition> DataSetDefinitionWithoutTagParser =
            from n in Identifier
            from wp2 in Parse.WhiteSpace.Many()
            from eq in Parse.Char('=')
            from wp3 in Parse.WhiteSpace.Many()
            from v in MultiFileSearchString.Or(FilePathStringAsArrayParser)
            select new DataSetDefinition() { Name = n, SearchStrings = v, Tags = new string[0] };

        private static readonly Parser<DataSetDefinition> DataSetDefinitionParser =
            DataSetDefinitionWithTagParser.Or(DataSetDefinitionWithoutTagParser);

        /// <summary>
        /// Parse a name value macro definition
        /// </summary>
        private static readonly Parser<NameValue> MacroDefinitionParser =
            from mdef in Parse.String("macro")
            from nv in NamePathFinder
            select nv;

        /// <summary>
        /// Attempt to parse a machine string.
        /// </summary>
        class Machine
        {
            public string Name { get; set; }
            public DataSetDefinition[] DS { get; set; }
            public NameValue[] Macros { get; set; }
        }

        private static readonly Parser<Machine> MachineParser =
            from mh in Parse.String("machine")
            from mName in Identifier
            from openBracket in Parse.Char('{')
            from whitespace0 in Parse.WhiteSpace.Many()
            from macros in MacroDefinitionParser.Many()
            from whitespace1 in Parse.WhiteSpace.Many()
            from dataSets in DataSetDefinitionParser.Many()
            from whitespace2 in Parse.WhiteSpace.Many()
            from closeBracket in Parse.Char('}')
            from whitespace3 in Parse.WhiteSpace.Many()
            from eol in Parse.Char('\n').Many()
            select new Machine() { Name = mName, DS = dataSets.ToArray(), Macros = macros.ToArray() };

        /// <summary>
        /// Pretend everything we can read is a single machine.
        /// </summary>
        private static readonly Parser<Machine> ParseNoMachineDef =
            from ds in DataSetDefinitionParser.Many()
            select new Machine() { Name = "", DS = ds.ToArray() };

        /// <summary>
        /// Return the "proper" item
        /// </summary>
        /// <param name="nv"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private static string FindSetting(IEnumerable<NameValue> nv, string p)
        {
            var r = from n in nv
                    where n.Name == p
                    select n.Value;
            return r.First();
        }

        /// <summary>
        /// Files that we will load
        /// </summary>
        private static string[] gFiles = new string[] { "dataset-list.txt" };

        /// <summary>
        /// Look at the dataset files and fine the dsName and return the files. They are sorted in alpha order.
        /// </summary>
        /// <param name="dsName"></param>
        /// <returns></returns>
        public static FileInfo[] FindROOTFilesForDS(string dsName)
        {
            var result = FindMachinesDatasets();

            ///
            /// Get the dataset from the listing
            /// 

            var ds = from d in result.DS
                     where d.Name == dsName
                     select d;
            var resultDS = ds.FirstOrDefault();
            if (resultDS == null)
                throw new ArgumentException("Dataset '" + dsName + "' was not known in this file for the machine '" + System.Environment.MachineName + "'.");

            var macroReplacedSearchStrings = resultDS.SearchStrings.Select(s => MacroReplacement(s, result.Macros));

            var files = FindFilesInSearchStrings(macroReplacedSearchStrings.ToArray());
            var sortedFiles = from f in files
                              orderby f.FullName ascending
                              select f;
            return sortedFiles.ToArray();
        }

        /// <summary>
        /// Returns the list of datasets for a machine.
        /// </summary>
        /// <returns></returns>
        private static Machine FindMachinesDatasets()
        {
            LoadDSInfo();

            ///
            /// Find the machine.
            /// 

            var mname = System.Environment.MachineName;
            var result = FindROOTFilesForMachine(mname);
            if (result == null)
                result = FindROOTFilesForMachine("");

            if (result == null)
                throw new ArgumentException("Machine '" + mname + "' not known in this file.");

            return result;
        }

        /// <summary>
        /// Do a macro substitiution for a string. Allow depth macro replacement, but do protect from doing it
        /// forever! :-)
        /// </summary>
        /// <param name="src"></param>
        /// <param name="macroList"></param>
        /// <returns></returns>
        private static string MacroReplacement(string src, NameValue[] macroList)
        {
            Regex macroFinder = new Regex(@"\$(\w+)");
            int count = 0;
            bool nomatches = false;
            while (count < 100 && !nomatches)
            {
                nomatches = true;
                foreach (Match m in macroFinder.Matches(src))
                {
                    nomatches = false;
                    var macro = (from mac in macroList where mac.Name == m.Groups[1].Value select mac).FirstOrDefault();
                    if (macro == null)
                        throw new InvalidDataException(string.Format("Macro '{0}' is not defined in this dataset", m.Groups[1].Value));
                    src = src.Replace(m.Value, macro.Value);
                }
                count++;
            }

            return src;
        }

        /// <summary>
        /// Returns true if the data set is known about and will return real FileInfo's.
        /// </summary>
        /// <param name="dsName"></param>
        /// <returns></returns>
        public static bool HasDS(string dsName)
        {
            try
            {
                var result = FindROOTFilesForDS(dsName);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Failed to parse the dataset file for dataset '{0}'. Parse error: {1}", dsName, e.Message));
            }
            return false;
        }

        /// <summary>
        /// Returns the set of files for the first search string we find.
        /// </summary>
        /// <param name="searchStrings"></param>
        /// <returns></returns>
        private static FileInfo[] FindFilesInSearchStrings(string[] searchStrings)
        {
            foreach (var ss in searchStrings)
            {
                var r = FindFilesInSearchString(ss);
                if (r.Length > 0)
                    return r;
            }

            StringBuilder bld = new StringBuilder();
            bld.Append("Unable to locate any root files in the search strings: ");
            foreach (var ss in searchStrings)
            {
                bld.AppendFormat("'{0}'", ss);
            }
            throw new FileNotFoundException(bld.ToString());
        }

        /// <summary>
        /// Search string resolution. Allow search strings in directory names too.
        /// Recursive. :-)
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        private static FileInfo[] FindFilesInSearchString(string searchString)
        {
            ///
            /// Seperate out any leading "\\". In this case we need to do \\\\server\\share before we even get started
            /// 

            string leadingSpecifier = "";
            if (searchString.StartsWith("\\\\"))
            {
                var serverEnd = searchString.IndexOf('\\', 2);
                if (serverEnd < 0)
                    throw new ArgumentException(string.Format("Search string '{0}' has no server!", searchString));
                var shareEnd = searchString.IndexOf('\\', serverEnd + 1);
                if (shareEnd < 0)
                    throw new ArgumentException(string.Format("Search string '{0}' has no share name!", searchString));
                leadingSpecifier = searchString.Substring(0, shareEnd);
                searchString = searchString.Substring(shareEnd + 1);
            }

            ///
            /// Great, now use recursion to build up the filename
            /// 

            return FindFilesRecursive(leadingSpecifier, searchString).ToArray();
        }

        /// <summary>
        /// Given the leading part of the directory, see if we can't find stuff in the search string.
        /// </summary>
        /// <param name="leadingDir"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        private static IEnumerable<FileInfo> FindFilesRecursive(string leadingDir, string searchString)
        {
            ///
            /// First, if there is just a file spec left, then do the search!
            /// 

            if (!searchString.Contains("\\"))
            {
                foreach (var f in Directory.GetFiles(leadingDir, searchString))
                {
                    yield return new FileInfo(f);
                }

                yield break;
            }

            ///
            /// Ok - so there is more work to do - pull off the next directory and see if we can't
            /// find all directories that match its search string
            /// 

            int backslash = searchString.IndexOf('\\');
            string dirname = searchString.Substring(0, backslash);
            string restSearchString = searchString.Substring(backslash + 1);

            ///
            /// Ok, do the search for directories!
            /// 

            foreach (var d in Directory.GetDirectories(leadingDir, dirname))
            {
                foreach (var f in FindFilesRecursive(d, restSearchString))
                {
                    yield return f;
                }
            }
        }

        /// <summary>
        /// See if we can find a machine
        /// </summary>
        /// <param name="mname"></param>
        /// <returns></returns>
        private static Machine FindROOTFilesForMachine(string mname)
        {
            var match = from m in gLoadedMachines
                        where m.Name == mname
                        select m;
            return match.FirstOrDefault();
        }

        /// <summary>
        /// Return a list of all tags associated with the datasets for this machine.
        /// </summary>
        public static string[] AllTags
        {
            get
            {
                var machine = FindMachinesDatasets();
                var alltags = from ds in machine.DS
                              from t in ds.Tags
                              group t by t;
                var tags = from taggroup in alltags
                           select taggroup.Key;
                return tags.ToArray();
            }
        }

        /// <summary>
        /// Return a list of data set names for each tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string[] DatasetNamesForTag(string tag)
        {
            var machine = FindMachinesDatasets();
            var allds = from ds in machine.DS
                        where ds.Tags.Contains(tag)
                        select ds.Name;
            return allds.ToArray();
        }

        /// <summary>
        /// Have the files been loaded?
        /// </summary>
        static bool gFilesLoaded = false;

        /// <summary>
        /// List of the files that we have loaded.
        /// </summary>
        static List<Machine> gLoadedMachines = new List<Machine>();

        /// <summary>
        /// Load all the dataset files we should know about.
        /// </summary>
        private static void LoadDSInfo()
        {
            if (gFilesLoaded)
                return;
            gFilesLoaded = true;

            foreach (var item in gFiles)
            {
                LoadAFile(new FileInfo(item));
            }
        }

        /// <summary>
        /// Load a single file up
        /// </summary>
        /// <param name="fileInfo"></param>
        private static void LoadAFile(FileInfo fileInfo)
        {
            ///
            /// Load the dataset file
            ///

            string text = "";
            foreach (var line in ReadFromFile(fileInfo))
            {
                text += line + "\r\n";
            }

            ///
            /// Parse it and add it to the list of known machines.
            /// 

            try
            {
                var result = ParseNoMachineDef.Many().Or(MachineParser.Many()).End().Parse(text);
                foreach (var item in result)
                {
                    gLoadedMachines.Add(item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Parse error in file {0}: {1}", fileInfo.FullName, e.Message));
            }
        }

        /// <summary>
        /// Read strings from a file and remove the comment lines before handing it back.
        /// </summary>
        /// <param name="finfo"></param>
        /// <returns></returns>
        private static IEnumerable<string> ReadFromFile(FileInfo finfo)
        {
            using (var reader = finfo.OpenText())
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    var commentIndex = line.IndexOf("//");
                    if (commentIndex >= 0)
                    {
                        line = line.Substring(0, commentIndex);
                    }

                    if (line.Length != 0)
                        yield return line;
                }
            }
        }
    }
}
