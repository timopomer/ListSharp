using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ListSharp
{
    public static class baseDefinitions
    {
        public static Dictionary<String,String> constantPairs;
        public static Dictionary<String, Func<String, String, String>> operatorConversion;
        public static Dictionary<string, Tuple<Regex, Func<GroupCollection, string>>> regexPatterns;
        public static Regex commandPattern;
        public static void initialize()
        {
            #region regex
            regexPatterns = new Dictionary<string, Tuple<Regex, Func<GroupCollection, string>>>()
            {
                {"GETLINES",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"GETLINES (.*?) \[(.*?)\]"), (gc) => $"GETLINES_F({codeParsing.processRows(gc[1].Value)},{codeParsing.serializeNumericRange(gc[2].Value)})")},
                {"DOWNLOAD",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"DOWNLOAD\[(.*?)\]"), (gc) => $"DOWNLOAD_F({codeParsing.processStrg(gc[1].Value)},{(int)launchArguments.flags["downloadtries"]})")},
                {"CHOOSEFILE",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"CHOOSEFILE\[(.*?)\]"), (gc) => $"CHOOSEFILE_F({codeParsing.processStrg(gc[1].Value)})")},
                {"CHOOSEFOLDER",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"CHOOSEFOLDER\[(.*?)\]"), (gc) => $"CHOOSEFOLDER_F({codeParsing.processStrg(gc[1].Value)})")},
                {"READ",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"READ\[(.*?)\]"), (gc) => $"System.IO.File.ReadAllText({codeParsing.processStrg(gc[1].Value)})")},
                {"ROWSPLIT",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"ROWSPLIT (.*?) BY \[(.*?)\]"), (gc) => $"ROWSPLIT_F({gc[1].Value},{codeParsing.processStrg(gc[2].Value)})")},
                {"SELECT",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"SELECT FROM (.*?) WHERE\[(.*?)\]"), (gc) => codeParsing.buildSelectQuery(codeParsing.processRows(gc[1].Value),gc[2].Value))},
                {"EXTRACT",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"EXTRACT COLLUM\[(.*?)\] FROM (.*?) SPLIT BY \[(.*?)\]"), (gc) => $"EXTRACT_F({codeParsing.processRows(gc[2].Value)},{codeParsing.processStrg(gc[3].Value)},{codeParsing.serializeNumericString(gc[1].Value)})")},
                {"COMBINE",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"COMBINE\[(.*?)\] WITH \[(.*?)\]"), (gc) => $"COMBINE_F(new string[][] {{{gc[1].Value}}},{codeParsing.processStrg(gc[2].Value)})")},
                {"GETBETWEEN",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"GETBETWEEN (.*?) \[(.*?)\] AND \[(.*?)\]"), (gc) => $"GETBETWEEN_F({gc[1].Value},{codeParsing.processStrg(gc[2].Value)},{codeParsing.processStrg(gc[3].Value)})")},
                {"GETRANGE",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"GETRANGE (.*?) FROM \[(.*?)\] TO \[(.*?)\]"), (gc) => $"GETRANGE_F({gc[1].Value},{codeParsing.serializeNumericString(gc[2].Value)},{codeParsing.serializeNumericString(gc[3].Value)})")},
                {"REPLACE",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"REPLACE \[(.*?)\] WITH \[(.*?)\] IN (.*)"), (gc) => $"REPLACE_F({codeParsing.processStrg(gc[1].Value)},{codeParsing.processStrg(gc[2].Value)},{gc[3].Value})")},
                {"STRG",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"STRG\[(.*?)\]"), (gc) => codeParsing.processStrg(gc[1].Value))},
                {"ROWS",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"ROWS\[(.*?)\]"), (gc) => codeParsing.processRows(gc[1].Value))},
                {"NUMB",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"NUMB\[(.*?)\]"), (gc) => codeParsing.processNumb(gc[1].Value))},
                {"INPUT",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"INPUT AS (STRG|NUMB)"), (gc) => codeParsing.processInput(gc[1].Value))},
            };

            commandPattern = new Regex($"({String.Join("|", regexPatterns.Select(n => n.Key))})(?!_F)");
            #endregion

            #region contstants
            constantPairs = new Dictionary<String, String>()
            {
                {"<scriptname>", $"@\"{IO.filename}\""},
                {"<here>", $"@\"{IO.currentdir}\""},
                {"<newline>", "System.Environment.NewLine"}
            };
            #endregion

            #region operators
            operatorConversion = new Dictionary<String, Func<String, String, String>>()
            {
                {"ISOVER", (s1,s2)=>$"{s1} > {s2}"},
                {"ISUNDER", (s1,s2)=>$"{s1} < {s2}"},
                {"ISEQUALOVER",(s1,s2)=>$"{s1} >= {s2}"},
                {"ISEQUALUNDER", (s1,s2)=>$"{s1} <= {s2}"},
                {"ISEQUAL",(s1,s2)=>$"{s1} == {s2}"},
                {"ISDIFF", (s1,s2)=>$"{s1} != {s2}"},
                {"ISNOT", (s1,s2)=>$"!Enumerable.SequenceEqual({s1},{s2})"},
                {"IS", (s1,s2)=>$"Enumerable.SequenceEqual({s1},{s2})"},
                {"CONTAINSNOT", (s1,s2)=>$"!{s1}.Contains({s2})"},
                {"CONTAINS", (s1,s2)=>$"{s1}.Contains({s2})"}
            };
            #endregion
        }

    }
}
