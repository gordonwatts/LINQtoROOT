using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TTreeParser
{
    /// <summary>
    /// Aid with parsing C++ template classes
    /// </summary>
    public static class TemplateParser
    {
        public interface IDecl
        {
        }

        public class TemplateInfo : IDecl
        {
            public string TemplateName { get; private set; }

            public IDecl[] Arguments { get; private set; }

            public TemplateInfo(string templateName, IDecl[] args)
            {
                TemplateName = templateName;
                Arguments = args;
            }
        }

        public class RegularDecl : IDecl
        {
            /// <summary>
            /// Get the type string for this declaration.
            /// </summary>
            public string Type { get; private set; }

            public RegularDecl(string typeDef)
            {
                Type = typeDef;
            }
        }

        private static Regex templateParameterFinder = new Regex("(?<tclass>\\w+)\\<(?<types>.*)\\>$");

        /// <summary>
        /// Given a declaration return a structure which pulls it apart, including all template
        /// arguments. It deals correctly with nested templates as well.
        /// </summary>
        /// <param name="typeDef"></param>
        public static IDecl ParseForTemplates(this string typeDef)
        {
            if (typeDef == null)
                throw new ArgumentNullException("typeDef");

            ///
            /// Is the current typeDef a template?
            /// 

            var m = templateParameterFinder.Match(typeDef);
            if (!m.Success)
            {
                return new RegularDecl(typeDef);
            }

            ///
            /// Now we have to carefully split up arguments. This is painful! We split by ",", but we
            /// have to be careful of sub template definitions that might contain commas.
            /// 

            var argStrings = SplitTemplateArguments(m.Groups["types"].Value);
            var args = (from a in argStrings
                        select ParseForTemplates(a)).ToArray();

            ///
            /// Ok, now create a new template definition from that!
            /// 

            var t = new TemplateInfo(m.Groups["tclass"].Value, args);

            return t;
        }

        /// <summary>
        /// Split template arguments up (by the ".", but ignore sub-template definitions.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static string[] SplitTemplateArguments(string argumentString)
        {
            string toParse = argumentString.Trim();
            List<string> result = new List<string>();

            while (toParse.Length > 0)
            {
                int locationOfNextGoodComma = FindNextNonTemplateComma(toParse);
                if (locationOfNextGoodComma < 0)
                {
                    result.Add(toParse);
                    toParse = "";
                }
                else
                {
                    result.Add(toParse.Substring(0, locationOfNextGoodComma));
                    toParse = toParse.Substring(locationOfNextGoodComma + 1).Trim();
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Go forward looking for the next comma that isn't in a <>.
        /// </summary>
        /// <param name="toParse"></param>
        /// <returns></returns>
        private static int FindNextNonTemplateComma(string toParse)
        {
            int index = 0;
            while (index < toParse.Length)
            {
                if (toParse[index] == ',')
                {
                    return index;
                }
                else if (toParse[index] == '<')
                {
                    index += FindMatchingClosingBraket(toParse.Substring(index + 1));
                }
                index += 1;
            }

            ///
            /// When we drop through here we didn't find a comma!
            /// 

            return -1;
        }

        /// <summary>
        /// Look for the matching closing braket...
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static int FindMatchingClosingBraket(string p)
        {
            int index = 0;
            while (index < p.Length)
            {
                if (p[index] == '>')
                    return index;

                if (p[index] == '<')
                {
                    index += FindMatchingClosingBraket(p.Substring(index + 1));
                }
                else
                {
                    index += 1;
                }
            }

            return p.Length;
        }

        /// <summary>
        /// Translate the exploded guy into a C# declaration
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        internal static string TranslateToCSharp(IDecl r)
        {
            if (r is RegularDecl)
            {
                var typ = (r as RegularDecl).Type;
                typ = TypeDefTranslator.ResolveTypedef(typ);

                if (typ == "string")
                {
                    throw new NotImplementedException("Unable to translate the C++ type of string");
                }

                if (ROOTNET.NTClass.GetClass(typ) != null)
                {
                    typ = "ROOTNET.Interface.N" + typ;
                }
                else
                {
                    typ = typ.SimpleCPPTypeToCSharpType();
                }
                if (typ == null)
                    throw new InvalidOperationException("Don't know how to deal with type '" + (r as RegularDecl).Type + "'.");

                return typ;
            }

            var t = r as TemplateInfo;
            if (t.TemplateName != "vector")
            {
                throw new NotImplementedException("Don't know how to deal with anything but a vector right now!");
            }

            var argTrans = TranslateToCSharp(t.Arguments[0]);
            return argTrans + "[]";
        }
    }
}
