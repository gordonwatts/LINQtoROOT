using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using TTreeDataModel;

namespace TTreeParser
{
    /// <summary>
    /// Given a TTree interface, inspect it, and return a complete set of classes that have to be generated
    /// to mock it on the .NET side.
    /// </summary>
    public class ParseTTree
    {
        /// <summary>
        /// Init the proxy generator. Default output is the current directory.
        /// </summary>
        public ParseTTree()
        {
            ProxyGenerationLocation = new DirectoryInfo(".");
        }

        /// <summary>
        /// Look at everything we have in this tree and see if we can't generate a class correctly.
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public IEnumerable<ROOTClassShell> GenerateClasses(ROOTNET.Interface.NTTree tree)
        {
            if (tree == null)
                throw new ArgumentNullException("tree must not be null");

            ///
            /// The outlying class is going to be called by the name of the tree.
            /// 

            var masterClass = new ROOTClassShell(tree.Name);

            foreach (var c in ExtractClassesFromBranchList(masterClass, tree.GetListOfBranches().Cast<ROOTNET.Interface.NTBranch>()))
            {
                yield return c;
            }

            ///
            /// Last thing we need to do is create a proxy for the class.
            /// 

            FileInfo f = MakeProxy(tree);
            masterClass.NtupleProxyPath = f.FullName;

            ///
            /// Work around a ROOT bug that doesn't allow for unloading of classes
            /// in the STL after a query is run. Actually, it will unload them, but somehow keeps
            /// references in memory.
            /// 

            masterClass.ClassesToGenerate = ExtractSTLDictionaryReferences(f);

            ///
            /// Analyze the TTree arrays.
            /// 

            var at = new ArrayAnalyzer();
            var groupInfo = at.AnalyzeTTree(masterClass, tree, 100);

            ///
            /// Any item that isn't an array should be added to the list and
            /// put in the "ungrouped" array.
            /// 

            AddNonArrays(masterClass, groupInfo, "ungrouped");

            ///
            /// Write out the user info
            /// 

            masterClass.UserInfoPath = WriteUserInfo(groupInfo, tree.SanitizedName()).FullName;

            ///
            /// Return the master class
            /// 

            yield return masterClass;
        }

        /// <summary>
        /// When an ntuple is parsed it contains many dictionary classes that have to be
        /// generated. It turns out that if vector<int> and similar are generated then you end
        /// up with the the query not being allowed to unload safely. To get around this we
        /// remove dictionary generation for stl classes here and place them as dictoinaries that
        /// are generated ahead of time.
        /// 
        /// We do a bunch of the work in memory as the files should be no more than a few
        /// kilobytes.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private ClassForDictionary[] ExtractSTLDictionaryReferences(FileInfo f)
        {
            var fixedFile = new StringBuilder();
            List<ClassForDictionary> genRequests = new List<ClassForDictionary>();

            var knownSTLHeaders = new string[] { "map", "vector", "list" };

            using (var reader = f.OpenText())
            {
                /// #pragma link6 C++ class vector<float>;
                var classExtractions = new Regex("(link|link6) C\\+\\+ class (?<cls>[^;]+);");
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    bool writeLine = true;
                    if (line.StartsWith("#pragma link"))
                    {
                        var m = classExtractions.Match(line);
                        if (m.Success)
                        {
                            var className = m.Groups["cls"].Value;
                            writeLine = !knownSTLHeaders.Any(stlname => className.StartsWith(stlname));
                            if (!writeLine)
                            {
                                var headers = new StringBuilder();
                                foreach (var stl in knownSTLHeaders)
                                {
                                    if (className.Contains(stl))
                                    {
                                        if (headers.Length > 0)
                                            headers.Append(",");
                                        headers.Append(stl);
                                    }
                                }
                                genRequests.Add(new ClassForDictionary() { classSpec = className, includeFiles = headers.ToString() });
                            }
                        }
                    }

                    if (writeLine)
                        fixedFile.AppendLine(line);
                }
            }

            ///
            /// Now, replace the proxy file
            /// 

            using (var writer = f.CreateText())
            {
                writer.Write(fixedFile.ToString());
                writer.Close();
            }

            ///
            /// And return everything needed
            /// 

            return genRequests.ToArray();
        }

        /// <summary>
        /// Find all non-arrays and add them to our ungrouped group. Since this
        /// grouping is what drives the class generation, this is very much something
        /// we have to do! :-)
        /// </summary>
        /// <param name="masterClass"></param>
        /// <param name="groupToAddTo">What is the name of the group we should add them to?</param>
        private void AddNonArrays(ROOTClassShell masterClass, List<ArrayGroup> groups, string groupToAddTo)
        {
            ///
            /// First, get all non-arrays. Arrays are all declared with [] right now,
            /// so we need only look for that.
            /// 

            var nonarrays = (from item in masterClass.Items
                             where !item.ItemType.Contains("[]")
                             select item.Name).ToArray();
            if (nonarrays.Length == 0)
                return;

            ///
            /// Turn the names into variables
            /// 

            var varInfo = nonarrays.ToVariableInfo();

            ///
            /// See if the group we should add them to exists.
            ///

            var grp = (from g in groups
                       where g.Name == groupToAddTo
                       select g).FirstOrDefault();
            if (grp == null)
            {
                grp = new ArrayGroup() { Name = groupToAddTo, Variables = varInfo.ToArray() };
                groups.Add(grp);
            }
            else
            {
                grp.Variables = (grp.Variables.Concat(varInfo)).ToArray();
            }
        }

        /// <summary>
        /// Write out the user info file. This file contains things like group names, etc., that the user
        /// might want to change. Do not destroy the old one. Rather, move it out of the way!
        /// </summary>
        /// <param name="groupInfo"></param>
        /// <returns></returns>
        private FileInfo WriteUserInfo(List<ArrayGroup> groupInfo, string treeName)
        {
            ///
            /// First job is to figure out where we will put the file. If there is one there already, then
            /// we chose a new filename. :-)
            /// 

            FileInfo userInfoFile = null;
            int index = 0;
            do
            {
                userInfoFile = new FileInfo(ProxyGenerationLocation.FullName + "\\" + treeName + "Config-" + index.ToString("000") + ".ntup");
                index = index + 1;
            } while (userInfoFile.Exists);

            ///
            /// Write out the info.
            /// 

            using (var writer = userInfoFile.CreateText())
            {
                XmlSerializer output = new XmlSerializer(typeof(TTreeUserInfo));
                var userInfo = new TTreeUserInfo() { Groups = groupInfo.ToArray() };
                output.Serialize(writer, userInfo);
                writer.Close();
            }

            return userInfoFile;

        }

        /// <summary>
        /// Get/Set where we shoudl generate the proxy files. Normally, it is the current directory.
        /// </summary>
        public DirectoryInfo ProxyGenerationLocation { get; set; }

        /// <summary>
        /// Generate a proxy for the tree. This involves making the proxy and then putting in the proper
        /// text substitutions for later processing by our framework. Some ugly text hacking!
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        private FileInfo MakeProxy(ROOTNET.Interface.NTTree tree)
        {
            ///
            /// First create the proxy. We have to create a darn temp macro name for that.
            /// 

            var oldDir = Environment.CurrentDirectory;
            var createItDir = new DirectoryInfo(Environment.CurrentDirectory);
            if (ProxyGenerationLocation != null)
                createItDir = ProxyGenerationLocation;
            FileInfo macroFile = new FileInfo(createItDir + @"\junk_macro_parsettree_" + tree.SanitizedName() + ".C");
            FileInfo result = null;
            try
            {
                ///
                /// We can only use local directories
                /// 

                if (ProxyGenerationLocation != null)
                    Environment.CurrentDirectory = ProxyGenerationLocation.FullName;

                using (var writer = macroFile.CreateText())
                {
                    writer.WriteLine("void {0} () {{}}", Path.GetFileNameWithoutExtension(macroFile.Name));
                    writer.Close();
                }

                result = new FileInfo("ntuple_" + tree.SanitizedName() + ".h");
                tree.MakeProxy(Path.GetFileNameWithoutExtension(result.Name), macroFile.Name, null, "nohist");
            }
            finally
            {

                ///
                /// Go back to where we were
                /// 

                Environment.CurrentDirectory = oldDir;
            }

            ///
            /// Next, add the proper things and remove some stuff
            /// 

            ///
            /// Return the location of the cpp file
            /// 

            return result;
        }

        /// <summary>
        /// Populate a class with the stuff we find in the branches we are looking at.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="branches"></param>
        /// <returns></returns>
        private IEnumerable<ROOTClassShell> ExtractClassesFromBranchList(ROOTClassShell container, IEnumerable<ROOTNET.Interface.NTBranch> branches)
        {
            ///
            /// Now, loop through each branch and see if we can't convert everything!
            /// 

            foreach (var branch in branches)
            {
                ///
                /// What kind of branch is this? Is it a leaf branch or is it a "class" branch. That is, does it have
                /// sub leaves on it? The key question to ask is how many sub-branches are there here?
                /// 

                if (branch.ListOfBranches.Entries == 0)
                {
                    foreach (var leaf in branch.GetListOfLeaves().Cast<ROOTNET.Interface.NTLeaf>())
                    {
                        try
                        {
                            IClassItem toAdd = ExtractSimpleItem(leaf);
                            if (toAdd != null)
                                container.Add(toAdd);
                        }
                        catch (Exception e)
                        {
                            SimpleLogging.Log("Warning: Unable to transltae ntuple leaf '" + leaf.Name + "': " + e.Message);
                        }
                    }
                }
                else
                {
                    IEnumerable<ROOTClassShell> rc = null;
                    try
                    {
                        rc = ExtractROOTClass(container, branch);
                    }
                    catch (Exception e)
                    {
                        SimpleLogging.Log("Warning: Unable to transltae ntuple leaf '" + branch.Name + "': " + e.Message);
                    }

                    if (rc != null)
                    {
                        ROOTClassShell lastOne = null;
                        foreach (var rootClass in rc.IgnoreExceptions())
                        {
                            yield return rootClass;
                            lastOne = rootClass;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Extract info for a sub-class! The last one is the top level class we are currently parsing!
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        private IEnumerable<ROOTClassShell> ExtractROOTClass(ROOTClassShell container, ROOTNET.Interface.NTBranch branch)
        {
            ///
            /// First, figure out what kind of class this is. For example, might it be a stl vector? If so,
            /// then we need to parse that up!
            /// 

            if (branch.GetClassName().Contains("<"))
            {
                return ExtractROOTTemplateClass(container, branch);
            }
            else
            {
                return ExtractROOTPlainClass(container, branch);
            }
        }

        /// <summary>
        /// This is a plane root class. Do the extraction!
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        private IEnumerable<ROOTClassShell> ExtractROOTTemplateClass(ROOTClassShell container, ROOTNET.Interface.NTBranch branch)
        {
            ///
            /// Currently only setup to deal with some very specific types of vectors!
            /// 

            var parsedMatch = TemplateParser.ParseForTemplates(branch.GetClassName()) as TemplateParser.TemplateInfo;
            if (parsedMatch == null)
            {
                throw new InvalidOperationException("Can't parse a template, but that is the only thing we can do!");
            }

            if (parsedMatch.TemplateName != "vector")
                throw new NotImplementedException("We can't deal with a template other than a vector: " + branch.GetClassName() + ".");

            if (!(parsedMatch.Arguments[0] is TemplateParser.RegularDecl))
            {
                throw new NotImplementedException("We can't deal with nested templates: " + branch.GetClassName() + ".");
            }

            var templateArgClass = (parsedMatch.Arguments[0] as TemplateParser.RegularDecl).Type;

            ///
            /// Now we take a look at the class. This class must be known by ROOT - it just is not reliably possible
            /// to re-build a class from the TTree/TLeaf structure, unforunately (except under very limited circumstances).
            /// To that end, check to see if the class is a stub...
            /// 

            var classInfo = ROOTNET.NTClass.GetClass(templateArgClass);
            if (classInfo.ListOfBases == null)
            {
                /// Hopefully this check isn't too fragile!!
                throw new NotImplementedException("ROOT doesn't know about the class '" + templateArgClass + "' so it can't be parsed");
            }

            ///
            /// Add a new item into the container
            /// 

            container.Add(new ItemVector(TemplateParser.TranslateToCSharp(parsedMatch), branch.Name));

            ///
            /// If this is a ROOT class (like TLorentzVector) then we are done.
            /// 

            return Enumerable.Empty<ROOTClassShell>();
        }

        /// <summary>
        /// There is a plain class that needs to be extracted. We usually end up here becuase it is a split class of
        /// some sort. For now, we will assume it is a real root class.
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        private IEnumerable<ROOTClassShell> ExtractROOTPlainClass(ROOTClassShell container, ROOTNET.Interface.NTBranch branch)
        {
            ///
            /// Double chekc that this is a root class
            /// 

            var cls = ROOTNET.NTClass.GetClass(branch.GetClassName());
            if (cls == null)
            {
                throw new NotImplementedException("The class '" + branch.GetClassName() + "' is not known to ROOT's type systems - and I can't proceed unless it is");
            }
            if (IsShim(cls))
            {
                throw new NotImplementedException("The class '" + branch.GetClassName() + "' is known as a class to ROOT, but nothign is specified - this looks like a shim generated by the I/O system rather than a real class - which it needs to be for ntuple conversion to continue. Please make sure the correct libraries are loaded!");
            }
            if (cls.Name == "string")
            {
                throw new NotImplementedException("The class 'string' is not translated yet!");
            }

            container.Add(new ItemROOTClass(branch.Name, branch.GetClassName()));

            return Enumerable.Empty<ROOTClassShell>();
        }

        /// <summary>
        /// Attempt to figre out if this is a real ROOT class or a shim that was
        /// generated by the data layer.
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        private bool IsShim(ROOTNET.Interface.NTClass cls)
        {
            if (cls.ListOfAllPublicDataMembers == null || cls.ListOfAllPublicMethods == null)
                return true;

            if (cls.ListOfAllPublicMethods.Entries == 0
                && cls.ListOfAllPublicDataMembers.Entries == 0
                && string.IsNullOrWhiteSpace(cls.ImplFileName_GetSetProperty))
                return true;

            return false;
        }

        /// <summary>
        /// A simple item, or an item, at least, that root knows how to deal with and for which
        /// we should have a complete, portable, guy ready for.
        /// </summary>
        /// <param name="leaf"></param>
        /// <returns></returns>
        private IClassItem ExtractSimpleItem(ROOTNET.Interface.NTLeaf leaf)
        {
            string className = TypeDefTranslator.ResolveTypedef(leaf.TypeName);

            ///
            /// First, see if this is a template of some sort.
            /// 

            var result = TemplateParser.ParseForTemplates(className);
            if (result is TemplateParser.TemplateInfo)
            {
                return ExtractTemplateItem(leaf, result as TemplateParser.TemplateInfo);
            }

            ///
            /// Next, check if it is an array. We are currently not translating
            /// raw C++ arrays.
            /// 

            if (leaf.Title.Contains("["))
                return null;

            ///
            /// Ok - so it is a single "object" or similar. So we need to look at it and figure
            /// out how to deal with it. It could be a root object or just an "int"
            ///

            var ln = NormalizeLeafName(leaf.Name);

            IClassItem toAdd = null;
            if (IsROOTClass(className) && className != "string")
            {
                toAdd = new ItemROOTClass(ln, className);
            }
            else
            {
                toAdd = new ItemSimpleType(ln, className.SimpleCPPTypeToCSharpType());
            }

            if (toAdd == null || toAdd.ItemType == null)
            {
                throw new InvalidOperationException("Unknown type - cant' translate '" + className + "'.");
            }
            return toAdd;
        }

        /// <summary>
        /// It is possible to use C# reserved keywords as leaf names. We have to alter them.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string NormalizeLeafName(string p)
        {
            if (p == "event")
                return "event_";
            return p;
        }

        /// <summary>
        /// We are going to deal with a template of some sort for a simple item. At the moment we can deal only
        /// with simple STL's...
        /// </summary>
        /// <param name="leaf"></param>
        /// <param name="templateInfo"></param>
        /// <returns></returns>
        private IClassItem ExtractTemplateItem(ROOTNET.Interface.NTLeaf leaf, TemplateParser.TemplateInfo templateInfo)
        {
            if (templateInfo.TemplateName == "vector")
            {
                return new ItemVector(TemplateParser.TranslateToCSharp(templateInfo), leaf.Name);
            }
            else
            {
                throw new NotImplementedException("We rae not able to handle template classes other than vector: '" + templateInfo.TemplateName + "'");
            }
        }

        /// <summary>
        /// Return true if this is a vector we are dealing with.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private bool IsSimpleVector(string className)
        {
            return className.StartsWith("vector<");
        }

        /// <summary>
        /// Figure out if this is a root class or not!
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        private bool IsROOTClass(string className)
        {
            var c = ROOTNET.NTClass.GetClass(className);
            return c != null;
        }
    }
}
