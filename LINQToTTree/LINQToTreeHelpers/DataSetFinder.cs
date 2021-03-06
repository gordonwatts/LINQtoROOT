﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sprache;

namespace LINQToTreeHelpers
{
    /// <summary>
    /// Use this class to parse a text file that contains per-machine definitions of datasets. Then refer to
    /// the data-set names by strings or tags and have them expand to a list of files or url's that can be
    /// processed by the LINQ infrastructure.
    /// 
    /// Lookup is performed based on data set name or tag name and the current machine name. This allows different
    /// locations for datasets depending upon what machine the queries are being run on. If the actual machine name
    /// is not found, it will default back to the "empty" machine name.
    ///
    /// Parse error reporting is not great.
    /// </summary>
    /// <remarks>
    /// Makes use of the cominator-parser library: http://nblumhardt.com/2010/01/building-an-external-dsl-in-c/
    /// </remarks>
    /// <example>
    /// By convention, create a dataset-list.txt file in your project that gets copied during your build.
    /// 
    /// Example file:
    /// machine HIGGS
    /// {
    ///   macro dataloc = \\tango.phys.washington.edu\tev-scratch3\users\HV\JetBackToBack_006
    ///
    ///   JetStream = $dataloc\user.Gordon.data11_7TeV.periodG*\*.root*
    /// }
    /// 
    /// Then, in your code:
    ///     var files = DataSetFinder.FindROOTFilesForDS(dsname);
    ///     Console.WriteLine("There are {0} files in dataset '{1}'.", files.Length, dsname);
    ///     var data = ROOTLINQ.QueryableCollectionTree.Create(files);
    /// </example>
    public class DataSetFinder
    {
        /// <summary>
        /// Basic string token.
        /// </summary>
        private static readonly Parser<string> Identifier = (Parse.LetterOrDigit.Or(Parse.Char('_')).Or(Parse.Char('-'))).AtLeastOnce().Text().Token().Named("Identifier");

        private static readonly Parser<string[]> IdentifierListSingle =
            from id1 in Identifier
            select new string[] { id1 };

        private static readonly Parser<string[]> IdenifierListMoreThanOne =
            (from id1 in IdentifierListSingle
            from wh1 in Parse.WhiteSpace.Many()
            from comma in Parse.Char(',')
            from wh2 in Parse.WhiteSpace.Many()
            from idRest in IdentifierList
            select id1.Concat(idRest).ToArray()).Named("Comma Seperated Identifier List");

        private static readonly Parser<string[]> IdentifierList =
            IdenifierListMoreThanOne.Or(IdentifierListSingle);

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
            .Or(Parse.Char('/'))
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
            from wh2 in Parse.WhiteSpace.Many()
            select searchstring
            ).Named("FilePathStringQuoted");

        private static readonly Parser<string> FilePathString =
            (
            from whitespace in Parse.WhiteSpace.Many()
            from fs in FilePathStringQuoted.XOr(FilePathStringUnquoted)
            select fs
            ).Named("FilePathString");

        private static readonly Parser<string[]> FilePathStringAsArrayParser =
            (from f in FilePathString
             select new string[] { f }).Named("FilePathStringAsArrayParser");

        /// <summary>
        /// Look for file-path | file-path | file-path... always return the first
        /// one that matches!
        /// </summary>
        private static readonly Parser<string[]> MultiFileSearchString =
            (
            from first in FilePathString
            from rest in FilePathOrString.XMany()
            select (new string[] { first }.Concat(rest)).ToArray()
            ).Named("List of file search strings");

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
            (
            from n in Identifier
            from eq in Parse.Char('=')
            from v in Identifier
            select new NameValue() { Name = n, Value = v }
            ).Named("Name-Value Pair");

        private static readonly Parser<NameValue> NamePathFinder =
            (
            from n in Identifier
            from wh1 in Parse.WhiteSpace.Many()
            from eq in Parse.Char('=')
            from wh2 in Parse.WhiteSpace.Many()
            from v in FilePathString
            select new NameValue() { Name = n, Value = v }
            ).Named("Name-Path Pair");

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
            (
            from openParen1 in Parse.Char('(')
            from tagNames in IdentifierList
            from openParen in Parse.Char(')')
            select tagNames
            ).Named("Tag List Parser");

        private static readonly Parser<DataSetDefinition> DataSetDefinitionWithTagParser =
            (
            from n in Identifier
            from wp1 in Parse.WhiteSpace.Many()
            from tags in TagListParser
            from wp2 in Parse.WhiteSpace.Many()
            from eq in Parse.Char('=')
            from wp3 in Parse.WhiteSpace.Many()
            from v in MultiFileSearchString.Or(FilePathStringAsArrayParser)
            select new DataSetDefinition() { Name = n, SearchStrings = v, Tags = tags }
            ).Named("Data Set Definition With Tags");

        private static readonly Parser<DataSetDefinition> DataSetDefinitionWithoutTagParser =
            (
            from n in Identifier
            from wp2 in Parse.WhiteSpace.Many()
            from eq in Parse.Char('=')
            from wp3 in Parse.WhiteSpace.Many()
            from v in MultiFileSearchString.Or(FilePathStringAsArrayParser)
            select new DataSetDefinition() { Name = n, SearchStrings = v, Tags = new string[0] }
            ).Named("Data Set Definition Without Tags");

        private static readonly Parser<DataSetDefinition> DataSetDefinitionParser =
            DataSetDefinitionWithTagParser.Or(DataSetDefinitionWithoutTagParser);

        /// <summary>
        /// Parse a name value macro definition
        /// </summary>
        private static readonly Parser<NameValue> MacroDefinitionParser =
            (
            from mdef in Parse.String("macro")
            from nv in NamePathFinder
            select nv
            ).Named("Macro definition");

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
            (
            from whitespace0 in Parse.WhiteSpace.Many()
            from mh in Parse.String("machine")
            from mName in Identifier
            from openBracket in Parse.Char('{')
            from whitespace1 in Parse.WhiteSpace.Many()
            from macros in MacroDefinitionParser.XMany()
            from whitespace2 in Parse.WhiteSpace.Many()
            from dataSets in DataSetDefinitionParser.XMany()
            from whitespace3 in Parse.WhiteSpace.Many()
            from closeBracket in Parse.Char('}')
            from whitespace4 in Parse.WhiteSpace.Many()
            from eol in Parse.Char('\n').Many()
            select new Machine() { Name = mName, DS = dataSets.ToArray(), Macros = macros.ToArray() }
            ).Named("Top Level Machine");

        /// <summary>
        /// Pretend everything we can read is a single machine.
        /// </summary>
        private static readonly Parser<Machine> ParseNoMachineDef =
            from ds in DataSetDefinitionParser.XMany()
            select new Machine() { Name = "", DS = ds.ToArray() };

        /// <summary>
        /// Get/Set the name of the machine we are running on. Defaults to current machine name. Use this
        /// if you want to make one machine look like another or during testing or anything else that needs to
        /// be machine agnostic.
        /// </summary>
        public static string MachineName { get; set; }

        /// <summary>
        /// Some inital configuration. Not efficient, but since we are all about static here...
        /// </summary>
        static DataSetFinder()
        {
            MachineName = System.Environment.MachineName;
        }

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
        private static List<string> gFiles = new List<string>() { "dataset-list.txt" };

        /// <summary>
        /// Reset the list of known files. Useful mostly for testing.
        /// </summary>
        public static void ClearFileList()
        {
            gFiles.Clear();
        }

        /// <summary>
        /// Add a file to the list of files to be loaded. Shoudl be called before
        /// first access.
        /// </summary>
        /// <param name="fname"></param>
        public static void AddFile(string fname)
        {
            gFiles.Add(fname);
        }

        /// <summary>
        /// Fetches the list of files associated with the passed in dataset name. The (default) machine
        /// name and the dataset files are used to find the proper set of files. Full wildcard resolution
        /// is also performed, of course.
        /// </summary>
        /// <param name="dsName">The dataset for which the list of root files is desired.</param>
        /// <returns>A complete list of files with full wildcard resolution. The files are returned in alphabetical order.</returns>
        /// <example>
        /// By convention, create a dataset-list.txt file in your project that gets copied during your build.
        /// 
        /// Example file:
        /// machine HIGGS
        /// {
        ///   macro dataloc = \\tango.phys.washington.edu\tev-scratch3\users\HV\JetBackToBack_006
        ///
        ///   JetStream = $dataloc\user.Gordon.data11_7TeV.periodG*\*.root*
        /// }
        /// 
        /// Then, in your code:
        ///     var files = DataSetFinder.FindROOTFilesForDS(dsname);
        ///     Console.WriteLine("There are {0} files in dataset '{1}'.", files.Length, dsname);
        ///     var data = ROOTLINQ.QueryableCollectionTree.Create(files);
        /// </example>
        public static Uri[] FindROOTFilesForDS(string dsName)
        {
            var result = FindMachinesDatasets();

            //
            // Get the dataset from the listing
            // 

            var resultDS = FindDataSetDefinition(dsName, result);

            var macroReplacedSearchStrings = resultDS.SearchStrings.Select(s => MacroReplacement(s, result.Macros));
            try
            {
                var files = FindFilesInSearchStrings(macroReplacedSearchStrings.ToArray());
                var sortedFiles = from f in files
                                  orderby f.OriginalString ascending
                                  select f;
                return sortedFiles.ToArray();
            }
            catch (Exception e)
            {
                throw new Exception("Error while searching for root files in data set " + dsName, e);
            }
        }

        private static DataSetDefinition FindDataSetDefinition(string dsName, Machine result)
        {
            var ds = from d in result.DS
                     where d.Name == dsName
                     select d;
            var resultDS = ds.FirstOrDefault();
            if (resultDS == null)
                throw new ArgumentException("Dataset '" + dsName + "' was not known in this file for the machine '" + MachineName + "'.");
            return resultDS;
        }

        /// <summary>
        /// Returns the list of datasets for a machine.
        /// </summary>
        /// <returns></returns>
        private static Machine FindMachinesDatasets()
        {
            LoadDSInfo();

            //
            // Find the machine.
            // 

            var mname = MachineName;
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
        /// Returns true if the data set is known about and will return real root files for processing.
        /// </summary>
        /// <param name="dsName">The name of the dataset to lookup</param>
        /// <returns>True if the dataset is defined for this machine and has at least one file in it.</returns>
        public static bool HasDS(string dsName)
        {
            try
            {
                var result = FindROOTFilesForDS(dsName);
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }

        /// <summary>
        /// Returns the set of files for the first search string we find.
        /// </summary>
        /// <param name="searchStrings"></param>
        /// <returns></returns>
        private static Uri[] FindFilesInSearchStrings(string[] searchStrings)
        {
            foreach (var ss in searchStrings)
            {
                try
                {
                    var r = FindFilesInSearchString(ss);
                    if (r.Length > 0)
                        return r;
                }
                catch { }
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
        private static Uri[] FindFilesInSearchString(string searchString)
        {
            //
            // Depending on the type of search string we do different things. For example, if this is a listing of a dataset on a
            // proof end-point, we support no wildcards!
            //

            if (searchString.StartsWith("proof://"))
            {
                return new Uri[] { new Uri(searchString) };
            }

            //
            // Seperate out any leading "\\". In this case we need to do \\\\server\\share before we even get started
            // 

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
            else if (searchString.Contains(":\\"))
            {
                leadingSpecifier = searchString.Substring(0, 3);
                searchString = searchString.Substring(3);
            }

            //
            // Great, now use recursion to build up the filename
            // 

            return FindFilesRecursive(leadingSpecifier, searchString).ToArray();
        }

        /// <summary>
        /// Given the leading part of the directory, see if we can't find stuff in the search string.
        /// </summary>
        /// <param name="leadingDir"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        private static IEnumerable<Uri> FindFilesRecursive(string leadingDir, string searchString)
        {
            //
            // First, if there is just a file spec left, then do the search!
            // 

            if (!searchString.Contains("\\"))
            {
                foreach (var f in Directory.GetFiles(leadingDir, searchString))
                {
                    yield return new Uri(string.Format("file://{0}", f));
                }

                yield break;
            }

            //
            // Ok - so there is more work to do - pull off the next directory and see if we can't
            // find all directories that match its search string
            // 

            int backslash = searchString.IndexOf('\\');
            string dirname = searchString.Substring(0, backslash);
            string restSearchString = searchString.Substring(backslash + 1);

            //
            // Ok, do the search for directories!
            // 

            string[] alldirectories;
            try
            {
                alldirectories = Directory.GetDirectories(leadingDir, dirname);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Failed to load the directories in path '{0}' with a name like '{1}'.", leadingDir, dirname), e);
            }

            foreach (var d in alldirectories)
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
        /// Return a list of dataset names that have all the given tags associated
        /// with them.
        /// </summary>
        /// <param name="tags">List of tags</param>
        /// <returns>List of dataset names that each have all the provided tags assoicated with them</returns>
        /// <example>
        /// Example dataset file:
        /// machine HIGGS
        /// {
        ///   macro dataloc = \\tango.phys.washington.edu\tev-scratch3\users\HV\JetBackToBack_006
        ///
        ///   JetStream (jet, 7TeV) = $dataloc\user.Gordon.data11_7TeV.periodG*\*.root*
        ///   EMStream (EM, 7TeV) = $dataloc\user.Gordon.data11_7TeV.EM.periodG*\*.root*
        ///   EMStreamExtra (EM, 7TeV) = $dataloc\user.Gordon.data11_7TeV.EM.periodG*\*.root*
        /// }
        /// 
        /// Then, in your code:
        ///     var dsNames = DataSetFinder.DatasetNamesForTag(new [] {"jet"});
        /// will return "JetStream" and
        ///     var dsNames = DataSetFinder.DatasetNamesForTag(new [] {"7TeV"});
        /// will return "JetStream" and "EM" stream.
        ///     var dsNames = DataSetFinder.DatasetNamesForTag(new [] {"EM", "7TeV"});
        /// will return "EMStream" and "EMStreamExtra"
        /// </example>
        public static string[] DatasetNamesForTag(params string[] tags)
        {
            var machine = FindMachinesDatasets();
            var allds = from ds in machine.DS
                        where tags.All(t => ds.Tags.Contains(t))
                        select ds.Name;
            return allds.ToArray();
        }

        /// <summary>
        /// Returns all the tags that are associated with a dataset.
        /// </summary>
        /// <param name="dsname">Name of the dataset that the tag lookup will be done on</param>
        /// <returns>List of the tags associated with the dataset. Could be the empty array (but not null).</returns>
        public static string[] DSTags(string dsname)
        {
            var machine = FindMachinesDatasets();
            var ds = FindDataSetDefinition(dsname, machine);
            if (ds == null)
                return new string[0];
            return ds.Tags;
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
        /// Clear out the read in files, re-prime the read in. Useful in test harnesses.
        /// </summary>
        public static void ResetCache()
        {
            gFilesLoaded = false;
            gLoadedMachines.Clear();
        }

        /// <summary>
        /// Load a single file up
        /// </summary>
        /// <param name="fileInfo"></param>
        private static void LoadAFile(FileInfo fileInfo)
        {
            //
            // Load the dataset file
            //

            string text = "";
            foreach (var line in ReadFromFile(fileInfo))
            {
                text += line + "\r\n";
            }

            //
            // Parse it and add it to the list of known machines.
            // 

            try
            {
                ParseSpecFromString(text);
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Parse error in file {0}: {1}", fileInfo.FullName, e.Message));
            }
        }

        /// <summary>
        /// Parse a string for dataset definitions. NOTE: there is no attempt to remove
        /// comments - this should be pure text by now! This is useful if you need to inject
        /// the dataset specification from some source other than a file.
        /// </summary>
        /// <param name="text">The text that we will parse</param>
        public static void ParseSpecFromString(string text)
        {
            var result = ParseNoMachineDef.Many().Or(MachineParser.Many()).End().Parse(text);
            foreach (var item in result)
            {
                gLoadedMachines.Add(item);
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
                    var line = reader.ReadLine().Trim();
                    if (line.StartsWith("//"))
                        continue;

                    var commentIndex = line.IndexOf("#");
                    if (commentIndex >= 0)
                    {
                        line = line.Substring(0, commentIndex);
                    }
                    line = line.Trim();

                    if (line.Length != 0)
                        yield return line;
                }
            }
        }
    }
}
