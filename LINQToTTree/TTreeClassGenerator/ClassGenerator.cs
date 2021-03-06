﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using TTreeDataModel;

namespace TTreeClassGenerator
{
    /// <summary>
    /// Starting from the class interface, generate a class that can be used in LINQ queries.
    /// All classes are generated in a single output file.
    /// </summary>
    public class ClassGenerator
    {
        /// <summary>
        /// GIven an existing file that contains the XML class spec, generate a CS file
        /// that contains all the definitions needed. Everything will be buried inside
        /// the namespaceName.
        /// </summary>
        /// <param name="inputXMLFile"></param>
        /// <param name="outputCSFile"></param>
        public void GenerateClasss(FileInfo inputXMLFile, FileInfo outputCSFile, string namespaceName)
        {
            ///
            /// Quick checks
            /// 

            if (inputXMLFile == null)
                throw new ArgumentNullException("inputXMLFile");
            if (!inputXMLFile.Exists)
                throw new ArgumentException("inputXMLFile does not currently exist!");

            ///
            /// Load up the XML file
            /// 

            var classSpec = LoadFromXMLFile(inputXMLFile);
            FillInDefaults(classSpec);

            ///
            /// Next, see if we can find the user specification file. We get a search path and use that to find this file.
            /// 

            Dictionary<string, TTreeUserInfo> userInfoMap = new Dictionary<string, TTreeUserInfo>();
            foreach (var c in classSpec.Classes)
            {
                var xmlFile = FindFileInDefaultPaths(c.UserInfoPath, inputXMLFile);
                if (xmlFile == null)
                {
                    Console.WriteLine("WARNING: No grouping file specified or found for the ntuple '" + c.Name + "' (the xxxConfig.ntup file).");
                }
                else if (!xmlFile.Exists)
                {
                    throw new FileNotFoundException("ERROR: Unable to find ntuple grouping file '" + c.UserInfoPath + "' in any of the standard places (working dir, the full path, near the executable)");
                }
                else
                {
                    Console.WriteLine("INFO: Loading grouping file '" + xmlFile.FullName + "' for class '" + c.Name + "'.");
                    userInfoMap[c.Name] = LoadUserInfoFromXMLFile(xmlFile);
                }
            }

            ///
            /// Now do the actual work!
            /// 

            GenerateClasss(classSpec, outputCSFile, namespaceName, userInfoMap);
        }

        /// <summary>
        /// Search for a file with fpath.Name in:
        /// 1) The current directory
        /// 2) The fully specified path
        /// 3) The location of the executable
        /// 
        /// If we get a null, return a null - if we can't find the file, return a FileInfo of the full filePath
        /// with .Exists == false.
        /// </summary>
        /// <param name="filePath">String of where we think the file might be</param>
        /// <returns></returns>
        private FileInfo FindFileInDefaultPaths(string filePath, params FileInfo[] otherFiles)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return null;

            string name = Path.GetFileName(filePath);

            /// Local directory??
            var result = new FileInfo(name);
            if (result.Exists)
                return result;

            /// Near any of the other files?
            foreach (var of in otherFiles)
            {
                result = new FileInfo(of.DirectoryName + "\\" + name);
                if (result.Exists)
                    return result;
            }

            /// Full path??
            result = new FileInfo(filePath);
            if (result.Exists)
                return result;

            /// Wow. Give up!

            return result;
        }

        /// <summary>
        /// Generate the classes given the input classes
        /// </summary>
        /// <param name="classSpec"></param>
        /// <param name="outputCSFile"></param>
        /// <param name="namespaceName"></param>
        public void GenerateClasss(NtupleTreeInfo classSpec, FileInfo outputCSFile, string namespaceName, IDictionary<string, TTreeUserInfo> userInfo = null)
        {
            ///
            /// Parameter checks
            /// 

            if (outputCSFile == null)
                throw new ArgumentNullException("outputCSFile");
            if (string.IsNullOrWhiteSpace(namespaceName))
                throw new ArgumentNullException("namespaceName");

            bool isbad = namespaceName.Any(c => !char.IsLetterOrDigit(c));
            if (isbad || !char.IsLetter(namespaceName[0]))
            {
                throw new ArgumentException("Namespace is not a legal C++ name");
            }

            if (classSpec == null)
                throw new ArgumentNullException("classSpec");

            if (classSpec.Classes == null)
                throw new ArgumentNullException("classSpec.Classes");

            var classByName = (from c in classSpec.Classes
                               group c by c.Name into g
                               where g.Count() > 1
                               select g.Key).ToArray();
            if (classByName.Length > 0)
            {
                var msg = new StringBuilder();
                msg.AppendFormat("The following classes have the same name:");
                foreach (var cname in classByName)
                {
                    msg.AppendFormat(" {0}", cname);
                }
                throw new InvalidDataException(msg.ToString());
            }

            foreach (var c in classSpec.ClassImplimintationFiles)
            {
                if (c == null)
                    throw new ArgumentNullException("Class support files can't be null");
                if (!File.Exists(c))
                    throw new ArgumentException("Can't fine class support file '" + c + "'.");
            }

            //
            // Fill in some defaults
            //

            userInfo = FillInDefaults(classSpec, userInfo);

            //
            // Do some quick checks for consistency
            //

            foreach (var cls in classSpec.Classes)
            {
                // Check that the index variable for all c-style arrays is "good".
                var CStyleArrays = from item in cls.Items
                                   where item is ItemCStyleArray
                                   let cSpec = item as ItemCStyleArray
                                   from cindex in cSpec.Indicies
                                   where !cindex.indexConst && cindex.indexBoundName != "implied"
                                   select cindex.indexBoundName;

                var IndexTypes = (from indexer in CStyleArrays
                                  let match = (from item in cls.Items
                                               where item.Name == indexer
                                               select item).FirstOrDefault()
                                  select Tuple.Create(indexer, match)).ToArray();

                var missingIndexTypes = (from indexer in IndexTypes
                                         where indexer.Item2 == null
                                         select indexer).ToArray();

                if (missingIndexTypes.Length > 0)
                {
                    var msg = new StringBuilder();
                    msg.Append("C Style arrays used index variables that I can't find in the class: ");
                    foreach (var vname in missingIndexTypes)
                    {
                        msg.Append(vname + " ");
                    }
                    throw new InvalidOperationException(msg.ToString());
                }

                var nonIntIndexTypes = (from index in IndexTypes
                                        where index.Item2.ItemType != "int"
                                        select index).ToArray();
                if (nonIntIndexTypes.Length > 0)
                {
                    var msg = new StringBuilder();
                    msg.Append("C Style arrays used index variables that are not integers: ");
                    foreach (var vname in nonIntIndexTypes)
                    {
                        msg.AppendFormat("{0}({1}) ", vname.Item2.Name, vname.Item2.ItemType);
                    }
                    throw new InvalidOperationException(msg.ToString());
                }
            }

            //
            // Now, start generating the output classes!
            //

            using (var output = outputCSFile.CreateText())
            {
                ///
                /// Write out the header and using clauses
                /// 

                output.WriteLine("//");
                output.WriteLine("// Automatically Generated File - do not modify!");
                output.WriteLine("// Generated on {0} by TTreeClassGenerator::ClassGenerator", DateTime.Now);
                output.WriteLine("// Translated ntuple classes");
                foreach (var cls in classSpec.Classes)
                {
                    output.WriteLine("// - ntuple {0}", cls.Name.FixupClassName());
                }

                output.WriteLine("//");
                output.WriteLine();

                output.WriteLine("using System.IO;");
                output.WriteLine("using LINQToTTreeLib;");
                output.WriteLine("using LinqToTTreeInterfacesLib;");
                output.WriteLine("using LINQToTTreeLib.CodeAttributes;");
                output.WriteLine("using System.Linq.Expressions;");

                output.WriteLine();
                output.WriteLine("namespace {0} {{", namespaceName);

                ///
                /// Now, write something out for each class
                /// 

                foreach (var cls in classSpec.Classes)
                {
                    try
                    {
                        ///
                        /// Write out the info class that contains everything needed to process this.
                        /// We could use attribute programming here, but that takes more code at the other
                        /// end, so until there is a real reason, we'll do it this way.
                        /// 
                        /// This is a kludge until we do more work to fix things with attributes, etc.
                        /// 

                        void writeOutExtra()
                        {
                            if (cls.IsTopLevelClass)
                            {
                                output.WriteLine("    public static string[] _gObjectFiles= {");
                                foreach (var item in classSpec.ClassImplimintationFiles)
                                {
                                    output.WriteLine("      @\"" + item + "\",");
                                }
                                output.WriteLine("    };");

                                output.WriteLine("    public static string[] _gCINTLines= {");
                                if (cls.CINTExtraInfo != null)
                                {
                                    foreach (var item in cls.CINTExtraInfo)
                                    {
                                        output.WriteLine("      @\"" + item + "\",");
                                    }
                                }
                                output.WriteLine("    };");

                                output.WriteLine("    public static string[] _gClassesToDeclare= {");
                                if (cls.ClassesToGenerate != null)
                                {
                                    foreach (var item in cls.ClassesToGenerate)
                                    {
                                        output.WriteLine("      @\"" + item.classSpec + "\",");
                                    }
                                }
                                output.WriteLine("    };");

                                output.WriteLine("    public static string[] _gClassesToDeclareIncludes = {");
                                if (cls.ClassesToGenerate != null)
                                {
                                    foreach (var item in cls.ClassesToGenerate)
                                    {
                                        output.WriteLine("      @\"" + item.includeFiles + "\",");
                                    }
                                }
                                output.WriteLine("    };");
                            }
                        }

                        ///
                        /// First, if there is a translated object model, write it out.
                        /// 

                        var rawClassName = cls.Name.FixupClassName();
                        if (userInfo != null)
                        {
                            if (userInfo.ContainsKey(cls.Name))
                            {
                                if (RequiresTranslation(userInfo[cls.Name]))
                                {
                                    rawClassName = rawClassName + "TranslatedTo";
                                    var varTypes = FindVariableTypes(cls.Items);
                                    WriteTranslatedObjectStructure(output, userInfo[cls.Name], cls.Name, rawClassName, varTypes, writeOutExtra);
                                    output.WriteLine();
                                    output.WriteLine();
                                }
                            }
                        }

                        //
                        // Write out the class header.
                        // If this is a generated class, then we need an attribute to mark it.
                        //

                        if (cls.IsTClonesArrayClass)
                        {
                            output.WriteLine("  [TClonesArrayImpliedClass]");
                        }

                        if (!cls.IsTopLevelClass)
                        {
                            output.WriteLine("  public partial class {0}", rawClassName);
                            output.WriteLine("  {");
                        }
                        else
                        {
                            output.WriteLine("  public partial class {0} : IExpressionHolder", rawClassName);
                            output.WriteLine("  {");
                            output.WriteLine("    public Expression HeldExpression {get; private set;}");
                            output.WriteLine("    public {0} (Expression expr) {{ HeldExpression = expr; }}", rawClassName);
                        }
                        output.WriteLine();

                        ///
                        /// These fields will never be set or accessed - so we turn off some warnings the compiler will generate.
                        /// They are dummies so that intellisense works and we can do the translation properly.
                        /// 

                        output.WriteLine("#pragma warning disable 0649");

                        foreach (var item in cls.Items)
                        {
                            WriteItem(item, output);
                        }

                        output.WriteLine("#pragma warning restore 0649");

                        writeOutExtra();

                        output.WriteLine("  }"); // End of the class
                        output.WriteLine();
                        output.WriteLine();

                        ///
                        /// Next, write out the queriable classes so we can actually do the queries
                        /// against the trees!
                        /// 

                        if (cls.IsTopLevelClass)
                        {
                            output.WriteLine("  /// Helper classes");
                            output.WriteLine("  public static class Queryable{0}", cls.Name.FixupClassName());
                            output.WriteLine("  {");
                            output.WriteLine("    /// Create a LINQ to TTree interface for a file and optional tree name");
                            output.WriteLine("    public static QueriableTTree<{0}> CreateQueriable (this FileInfo rootFile, string treeName = \"{1}\")", cls.Name.FixupClassName(), cls.Name);
                            output.WriteLine("    {");
                            output.WriteLine("      return new QueriableTTree<{0}>(rootFile, treeName);", cls.Name.FixupClassName());
                            output.WriteLine("    }");
                            output.WriteLine("    /// Create a LINQ to TTree interface for a list of files and optional tree name");
                            output.WriteLine("    public static QueriableTTree<{0}> CreateQueriable (this FileInfo[] rootFiles, string treeName = \"{1}\")", cls.Name.FixupClassName(), cls.Name);
                            output.WriteLine("    {");
                            output.WriteLine("      return new QueriableTTree<{0}>(rootFiles, treeName);", cls.Name.FixupClassName());
                            output.WriteLine("    }");
                            output.WriteLine("    /// Create a LINQ to TTree interface for a list of URLs and optional tree name");
                            output.WriteLine("    public static QueriableTTree<{0}> CreateQueriable (this System.Uri[] rootURLs, string treeName = \"{1}\")", cls.Name.FixupClassName(), cls.Name);
                            output.WriteLine("    {");
                            output.WriteLine("      return new QueriableTTree<{0}>(rootURLs, treeName);", cls.Name.FixupClassName());
                            output.WriteLine("    }");
                            output.WriteLine("  }");
                        }
                    }
                    catch (Exception e)
                    {
                        var errorMessage = new StringBuilder();
                        errorMessage.AppendFormat("Error while processing class '{0}", cls.Name);
                        var ex = e;
                        while (ex != null)
                        {
                            errorMessage.AppendFormat(": {0}", e.Message);
                            ex = e.InnerException;
                        }

                        // Kill off the output file!
                        output.Close();
                        outputCSFile.Delete();

                        // Propagate the error upwards.
                        throw new Exception(errorMessage.ToString(), e);
                    }
                }

                ///
                /// Done!
                /// 

                output.WriteLine("}"); // For the namespace

                output.Close();
            }
        }

        /// <summary>
        /// Keep track of a class item that is for a group (helper item).
        /// </summary>
        private class GroupItem : IClassItem
        {
            public override string ItemType { get; set; }
            public override string Name { get; set; }
            public override bool NotAPointer { get; set; }
        }

        /// <summary>
        /// Find a mapping from varname -> type. Fix up to deal with the groups.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private IDictionary<string, string> FindVariableTypes(List<IClassItem> list)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var v in list)
            {
                result[v.Name] = v.ItemType;
            }
            return result;
        }

        /// <summary>
        /// We have to write out the translated class. Sigh!! :-)
        /// </summary>
        /// <param name="tTreeUserInfo"></param>
        /// <param name="p"></param>
        /// <param name="rawClassName"></param>
        private void WriteTranslatedObjectStructure(TextWriter output, TTreeUserInfo tTreeUserInfo, string className, string translateToName, IDictionary<string, string> varTypes, Action kludgeWriter)
        {
            ///
            /// Main class header
            /// 

            output.WriteLine("  [TranslateToClass(typeof({0}))]", translateToName);
            output.WriteLine("  public partial class {0}", className);
            output.WriteLine("  {");

            ///
            /// First, do the ungrouped variables
            /// 

            var ungrouped = (from g in tTreeUserInfo.Groups
                             where g.Name == "ungrouped"
                             select g).FirstOrDefault();
            if (ungrouped != null)
            {
                output.WriteLine("#pragma warning disable 0649");
                foreach (var v in ungrouped.Variables)
                {
                    WriteVariableRenameDefinition(output, v, varTypes, className, false, tTreeUserInfo);
                }
                output.WriteLine("#pragma warning restore 0649");
            }


            ///
            /// Now do the groups
            /// 

            foreach (var grp in tTreeUserInfo.Groups.Where(g => g.Name != "ungrouped"))
            {
                if (!string.IsNullOrWhiteSpace(grp.Comment))
                {
                    output.WriteLine("    /// <summary>");
                    output.WriteLine("    /// {0}", grp.Comment);
                    output.WriteLine("    /// </summary>");
                }
                output.WriteLine("    [TTreeVariableGrouping]");
                output.WriteLine("    public {0}[] {1};", grp.ClassName, grp.Name);
            }

            ///
            /// And the object is finished.
            /// 

            kludgeWriter();
            output.WriteLine("  }");
            output.WriteLine();
            output.WriteLine();

            ///
            /// Now do all the group classes
            /// 

            foreach (var grp in tTreeUserInfo.Groups.Where(g => g.Name != "ungrouped"))
            {
                output.WriteLine("  public partial class {0}", grp.ClassName);
                output.WriteLine("  {");

                output.WriteLine("#pragma warning disable 0649");
                foreach (var v in grp.Variables)
                {
                    var attrs = v.NETName == grp.NETNameOfVariableToUseAsArrayLength
                        ? new[] { "[TTreeVariableGrouping]", "[UseAsArrayLengthVariable]" }
                        : new[] { "[TTreeVariableGrouping]" };
                    WriteVariableRenameDefinition(output, v, varTypes, className, true, tTreeUserInfo, attrs);
                }
                output.WriteLine("#pragma warning restore 0649");

                output.WriteLine("  }");
                output.WriteLine();
                output.WriteLine();
            }
        }

        /// <summary>
        /// Write out a variable definition.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="v"></param>
        /// <param name="extraAttributes">Extra attributes to be written before the funciton. Important to do them here otherwise any XML comments we write won't actually be recognized as such!</param>
        private void WriteVariableRenameDefinition(TextWriter output,
            VariableInfo v,
            IDictionary<string, string> varTypes,
            string baseTypeName,
            bool removeOneArrayDecl,
            TTreeUserInfo groupInfo,
            params string[] extraAttributes)
        {
            ///
            /// Figure out the comment that we put at the top
            /// 

            string comment = v.Comment;
            if (string.IsNullOrWhiteSpace(comment))
                comment = "";

            if (v.NETName != v.TTreeName)
            {
                comment += string.Format("(TTree Leaf name: {0})", v.TTreeName.FixupLeafName());
            }

            if (!string.IsNullOrWhiteSpace(comment))
            {
                output.WriteLine("    /// <summary>");
                output.WriteLine("    /// {0}", comment);
                output.WriteLine("    /// </summary>");
            }

            //
            // Write out extra attributes
            //

            foreach (var att in extraAttributes)
            {
                output.WriteLine("    {0}", att);
            }

            ///
            /// If this is a rename, then do teh rename
            /// 

            var cppVarName = v.NETName;
            if (v.NETName != v.TTreeName)
            {
                cppVarName = v.TTreeName;
                output.WriteLine("    [RenameVariable(\"{0}\")]", v.TTreeName.FixupLeafName());
            }

            ///
            /// Figure out what type the variable is
            /// 

            if (!varTypes.ContainsKey(cppVarName))
            {
                throw new ArgumentException("The variable '" + cppVarName + "' is not known!");
            }

            var typeName = varTypes[cppVarName];

            ///
            /// If this is an index, then reset the type and also write
            /// out the index spec. We do, also, have to copy over all the array references
            /// that are found on this guy.
            /// 

            if (!string.IsNullOrEmpty(v.IndexToGroup))
            {
                var grpReference = (from g in groupInfo.Groups
                                    where g.Name == v.IndexToGroup
                                    select g).FirstOrDefault();
                if (grpReference == null)
                    throw new ArgumentException("Group '" + v.IndexToGroup + "' is not a defined group!");
                if (!typeName.StartsWith("int"))
                    throw new ArgumentException("Variable of type '" + typeName + "' marked as index - only integers and integer arrays can index into other objects");

                typeName = baseTypeName + v.IndexToGroup + ArrayReferences(typeName);

                output.WriteLine("    [IndexToOtherObjectArray(typeof({0}), \"{1}\")]", baseTypeName, v.IndexToGroup);
            }

            if (removeOneArrayDecl)
            {
                if (!typeName.EndsWith("[]"))
                    throw new ArgumentException("Attempting to remove an array indirection from type '" + typeName + "' and can't find a [] at the end!");
                typeName = typeName.Substring(0, typeName.Length - 2);
            }

            output.WriteLine("    public {0} {1};", typeName, v.NETName.FixupLeafName());
        }

        /// <summary>
        /// Return the array references on the type that is passed to us.
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private string ArrayReferences(string typeName)
        {
            var i = typeName.IndexOf("[");
            if (i < 0)
                return "";

            return typeName.Substring(i);
        }

        /// <summary>
        /// See if any translation is required.
        /// </summary>
        /// <param name="tTreeUserInfo"></param>
        /// <returns></returns>
        private bool RequiresTranslation(TTreeUserInfo tTreeUserInfo)
        {
            if (tTreeUserInfo.Groups.Length == 0)
                return false;

            if (tTreeUserInfo.Groups.Length > 1)
                return true;
            if (tTreeUserInfo.Groups[0].Name != "ungrouped")
                return true;

            var anyRenames = tTreeUserInfo.Groups[0].Variables.Where(g => g.TTreeName != g.NETName).Any();
            return anyRenames;
        }

        /// <summary>
        /// Write out the data in a single item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="output"></param>
        private void WriteItem(IClassItem item, StreamWriter output)
        {
            if (item == null)
                throw new ArgumentNullException("item - can't have a null item in an ntuple!");

            // If there are any attributes associated with this guy, dump them out.
            if (item is IClassItemExtraAttributes)
            {
                var e = item as IClassItemExtraAttributes;
                foreach (var attr in e.GetAttributes())
                {
                    output.WriteLine("    [{0}]", attr);
                }
            }
            if (item.NotAPointer)
                output.WriteLine("    [NotAPointer]");

            string t = item.ItemType;
            output.WriteLine("    public {0} {1};", t, item.Name.FixupLeafName());
        }

        /// <summary>
        /// Loads in an input file and returns the classes.
        /// </summary>
        /// <param name="inputXMLFile"></param>
        /// <returns></returns>
        private NtupleTreeInfo LoadFromXMLFile(FileInfo inputXMLFile)
        {
            XmlSerializer x = new XmlSerializer(typeof(NtupleTreeInfo));
            using (var reader = inputXMLFile.OpenText())
            {
                var result = x.Deserialize(reader) as NtupleTreeInfo;
                return result;
            }
        }

        /// <summary>
        /// Load in a user info file
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        private TTreeUserInfo LoadUserInfoFromXMLFile(FileInfo inputFile)
        {
            XmlSerializer x = new XmlSerializer(typeof(TTreeUserInfo));
            using (var reader = inputFile.OpenText())
            {
                var result = x.Deserialize(reader) as TTreeUserInfo;
                return result;
            }
        }

        /// <summary>
        /// After reading in, it is nice to fill in some defaults if the user didn't first. That makes it a lot easier to
        /// write code for the output of this - so it isn't filled with lots of special cases.
        /// </summary>
        /// <param name="classSpec"></param>
        private void FillInDefaults(NtupleTreeInfo classSpec)
        {
        }

        /// <summary>
        /// Similar to the filling in above - we fill in any defaults to make the code cleaner
        /// than might have been done above.
        /// </summary>
        /// <param name="cls"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private TTreeUserInfo FillInDefaults(ROOTClassShell cls, TTreeUserInfo userInfo)
        {
            //
            // Group class names are often left up to use to determine. Fill in the defaults here.
            //

            foreach (var g in userInfo.Groups)
            {
                if (string.IsNullOrWhiteSpace(g.ClassName))
                {
                    g.ClassName = string.Format("{0}{1}", cls.Name, g.Name);
                }
            }

            return userInfo;
        }

        /// <summary>
        /// Fill in some defaults for later user!
        /// </summary>
        /// <param name="classSpec"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private IDictionary<string, TTreeUserInfo> FillInDefaults(NtupleTreeInfo classSpec, IDictionary<string, TTreeUserInfo> userInfo)
        {
            if (userInfo == null)
                return null;

            Dictionary<string, TTreeUserInfo> result = new Dictionary<string, TTreeUserInfo>();
            foreach (var c in classSpec.Classes)
            {
                if (userInfo.ContainsKey(c.Name))
                {
                    result[c.Name] = FillInDefaults(c, userInfo[c.Name]);
                }
            }
            return result;
        }
    }
}
