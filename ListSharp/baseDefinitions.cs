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
        public static Dictionary<String, String> operatorConversion;
        public static Dictionary<string, Tuple<Regex, Func<GroupCollection, string>>> regexPatterns;
        public static Regex commandPattern;
        public static void initialize()
        {
            #region regex
            regexPatterns = new Dictionary<string, Tuple<Regex, Func<GroupCollection, string>>>()
            {
                {"GETLINES",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"GETLINES (.*?) \[(.*?)\]"), (gc) => "GETLINES_F(" + codeParsing.processRows(gc[1].Value) + "," + codeParsing.processNumb(gc[2].Value) + ")")},
                //STRG FUNCTIONS
                {"GETLINE",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"GETLINE (.*?) \[(.*?)\]"), (gc) => "GETLINE_F(" + codeParsing.processRows(gc[1].Value) + "," + codeParsing.serializeNumericString(gc[2].Value) + ")")},
                {"DOWNLOAD",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"DOWNLOAD\[(.*?)\]"), (gc) => "DOWNLOAD_F(" + codeParsing.processStrg(gc[1].Value) + "," + launchArguments.downloadtries + ")")},
                {"CHOOSEFILE",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"CHOOSEFILE\[(.*?)\]"), (gc) => "CHOOSEFILE_F(" + codeParsing.processStrg(gc[1].Value) + ")")},
                {"CHOOSEFOLDER",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"CHOOSEFOLDER\[(.*?)\]"), (gc) => "CHOOSEFOLDER_F(" + codeParsing.processStrg(gc[1].Value) + ")")},
                {"READ",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"READ\[(.*?)\]"), (gc) => "System.IO.File.ReadAllText(" + codeParsing.processStrg(gc[1].Value) + ")")},
                {"ROWSPLIT",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"ROWSPLIT (.*?) BY \[(.*?)\]"), (gc) => "ROWSPLIT_F(" + gc[1].Value + "," + codeParsing.processStrg(gc[2].Value) + ")")},
                {"SELECT",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"SELECT FROM (.*?) WHERE\[(.*?)\]"), (gc) => codeParsing.selectBuilder(gc[1].Value,gc[2].Value))},
                {"EXTRACT",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"EXTRACT COLLUM\[(.*?)\] FROM (.*?) SPLIT BY \[(.*?)\]"), (gc) => "EXTRACT_F(" + codeParsing.processRows(gc[2].Value) + "," + codeParsing.processStrg(gc[3].Value) + "," + codeParsing.serializeNumericString(gc[1].Value) + ")")},
                {"COMBINE",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"COMBINE\[(.*?)\] WITH \[(.*?)\]"), (gc) => "COMBINE_F(new string[][] {" + gc[1].Value + "}," + codeParsing.processStrg(gc[2].Value) + ")")},
                {"GETBETWEEN",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"GETBETWEEN (.*?) \[(.*?)\] AND \[(.*?)\]"), (gc) => "GETBETWEEN_F(" + gc[1].Value + "," + codeParsing.processStrg(gc[2].Value) + "," +codeParsing.processStrg(gc[3].Value) + ")")},
                {"GETRANGE",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"GETRANGE (.*?) FROM \[(.*?)\] TO \[(.*?)\]"), (gc) => "GETRANGE_F(" + gc[1].Value + "," + codeParsing.serializeNumericString(gc[2].Value) + "," + codeParsing.serializeNumericString(gc[3].Value) + ")")},
                {"REPLACE",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"REPLACE \[(.*?)\] WITH \[(.*?)\] IN (.*)"), (gc) => "REPLACE_F(" + codeParsing.processStrg(gc[1].Value) + "," + codeParsing.processStrg(gc[2].Value) + "," + gc[3].Value + ")")},
                {"MULTIPLY",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"MULTIPLY (.*?) BY (.*?)"), (gc) => "MULTIPY_F(" + gc[1].Value + "," + codeParsing.serializeNumericString(gc[2].Value) + ");")},
                {"STRG",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"STRG\[(.*?)\]"), (gc) => codeParsing.processStrg(gc[1].Value))},
                {"ROWS",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"ROWS\[(.*?)\]"), (gc) => codeParsing.processRows(gc[1].Value))},
                {"NUMB",new Tuple<Regex, Func<GroupCollection, string>>(new Regex(@"NUMB\[(.*?)\]"), (gc) => codeParsing.serializeNumericString(gc[1].Value))},

            };


            //(?<!string)\[(.*?)\]

            /*
            if (line.StartsWith("INPUT"))
            {
                GroupCollection gc = new Regex(@"\[(.*)\]").Match(line).Groups;
                return " (string)INPT_F(" + gc[1].Value + ",typeof(string));";
            }
            */


            commandPattern = new Regex("(" + String.Join("|", regexPatterns.Select(n => n.Key)) + ")(?!_F)");
            #endregion



            #region contstants
            constantPairs = new Dictionary<String, String>()
            {
                {"<scriptname>", "@\"" + IO.filename + "\"" },
                {"<here>", "@\"" + IO.currentdir + "\"" },
                {"<newline>", "\"\\n\""}
            };
            #endregion

            #region operators
            operatorConversion = new Dictionary<String, String>()
            {
                {"ISOVER", ">"},
                {"ISUNDER", "<"},
                {"ISEQUALOVER", "=>"},
                {"ISEQUALUNDER", "=<"},
                {"ISEQUAL", "=="},
                {"ISDIFF", "!="},
                {"IS", "=="},
                {"ISNOT", "!="},
                {"CONTAINS", ""},
                {"CONTAINSNOT", "!"}
            };
            #endregion

        }

    }
}
