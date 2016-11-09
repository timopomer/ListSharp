using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ListSharp
{
    public static class codeParsing
    {

        public static bool startsWithSymbol(this string line)
        {
            return !Char.IsLetter(line[0]);
        }
        public static string processOperators(string line, int line_num)
        {
            if (line.StartsWith("{"))
                return "{";  

            if (line.StartsWith("}"))
                return "}";    

            if (line.StartsWith("//")) //to see if the code is commented out so it does net get into the final code (replaced with //skipped for debugging porpuses
                return "//skipped";

            if (line.StartsWith("#")) //to see if the code is commented out so it does net get into the final code (replaced with //skipped for debugging porpuses
                return "//command executed: " + line;
            
            if (line.StartsWith("<") && line.EndsWith(">")) //c# code
                return line;

            if (line.StartsWith("/*") || line.StartsWith("*/")) //to see if the code is commented out so it does net get into the final code
                return line;
            
            if (line.StartsWith("[") && line.EndsWith("]"))
                return processExpression(line, line_num);

            if (line.StartsWith("(") && line.EndsWith(")"))
                return processFlag(line, line_num);

            debug.throwException("Parsing error, invalid Operator", $"Line: {line_num}", debug.importance.Fatal);

            return "";
        }
        public static string processExpression(string line, int line_num)
        {
            line = new Regex(@"\[(.*)\]").Match(line).Groups[1].Value; //everything between the square brackets "[]"
            line = processCommand(line, line_num);
            if (line.StartsWith("FOREACH"))
            {
                if (line.StartsWith("FOREACH NUMB"))
                {
                    GroupCollection gc = new Regex(@"FOREACH NUMB IN (.*) AS (.*)").Match(line).Groups;
                    return $"foreach (int {gc[2].Value} in {serializeNumericRange(gc[1].Value)})";
                }
                if (line.StartsWith("FOREACH STRG"))
                {
                    GroupCollection gc = new Regex(@"FOREACH STRG IN (.*) AS (.*)").Match(line).Groups;
                    return $"foreach (string {gc[2].Value} in {gc[1].Value})";
                }
            }

            if (line.StartsWith("IF"))
                return $"if({buildIfQuery(new Regex(@"IF (.*)").Match(line).Groups[1].Value)})";
            

            debug.throwException("Parsing error, invalid Expression", $"Line: {line_num}", debug.importance.Fatal);

            return "";
        }
        public static string processFlag(string line, int line_num)
        {
            line = new Regex(@"\((.*)\)").Match(line).Groups[1].Value; //everything between the square brackets "[]"
            switch (line)
            {
                case "exit":
                    return "Environment.Exit(0);";
            }

            debug.throwException("Parsing error, invalid Flag", $"Line: {line_num}", debug.importance.Fatal);

            return "";
        }


        #region SHOW command
        public static string processShow(string line, int line_num)
        {
            switch (line)
            {
                case "STRG":
                case "ROWS":
                case "ALL":
                    {
                        return (genericShow(line));
                    }
                default:
                    {
                        return $"output += SHOW_F({line}){Environment.NewLine};";
                    }
            }
        }
        public static string genericShow(string type)
        {
            string returnedCode = "";
            returnedCode += "output += System.Environment.NewLine + \"Listing " + type + " variables\" + System.Environment.NewLine;";
            switch (type)
            {
                case "ALL":
                    {
                        returnedCode += typeShow("STRG") + Environment.NewLine + typeShow("ROWS");
                        break;
                    }
                default:
                    {
                        returnedCode += typeShow(type);
                        break;
                    }
            }
            return returnedCode;
        }
        public static string typeShow(string type)
        {
            string toReturn = "";
            foreach (Variable ver in memory.variables[type])
            {
                toReturn += $"output += \"Listing {ver.name} :\" ;";
                toReturn += $"output += SHOW_F({ver.name}) + System.Environment.NewLine;"; //call SHOW_F() on any type of variable the users wants to display
            }
            return toReturn;
        }
        #endregion


        public static string processDisp(string line)
        {
            return $"DISP_F({line});";
        }
        public static string processOutput(string line, int line_num)
        {
            GroupCollection gc = new Regex(@"(.*?) HERE\[(.*?)\]").Match(line).Groups;
            return $"OUTP_F({gc[2].Value},{gc[1].Value});";
        }
        public static string processOpen(string line, int line_num)
        {
            GroupCollection gc = new Regex(@"HERE\[(.*?)\]").Match(line).Groups;
            return $"OPEN_F({gc[1].Value});";
        }
        public static string processInput(string type)
        {
            switch (type)
            {
                case "STRG":
                    return processStrg("INPT_F(typeof(string))");

                case "NUMB":
                    return "(int)(long)INPT_F(typeof(int))";
            }
            return "";
        }

        public static string processStrg(string line) => $"((string)ADD_F(typeof(string),{line}))";
        public static string processRows(string line) => $"((stringarr)ADD_F(typeof(stringarr),{line}))";
        public static string processNumb(string line) => serializeNumericString(line);

        public static string processCommand(string line, int line_num)
        {
            string processedLine = patternMatching.evaluateAllMatches(line);
            return processedLine == line ? line : processCommand(processedLine, line_num);
        }
        public static string processLine(string line, int line_num)
        {
            if (line == "")
                return "";
            if (line.startsWithSymbol())
                return processOperators(line, line_num);


            string[] splitline = line.Split(new char[] { '=' }, 2); //splitting the line of "variable = evaluated string later to be parsed
            string varname = splitline[0].Substring(4).Trim(); //the first 4 characters will always be the variable type ex: strg,rows
            string start_argument = splitline[0].Substring(0, 4);
            splitline[1] = splitline[1].Substring(1);

            switch (start_argument)
            {
                case "STRG":
                    return varname + " = " + processStrg(processCommand(splitline[1], line_num)) + ";";

                case "ROWS":
                    return varname + " = " + processRows(processCommand(splitline[1], line_num)) + ";";

                case "NUMB":
                    return varname + " = " + processNumb(processCommand(splitline[1], line_num)) + ";";

                case "SHOW":
                    return processShow(processCommand(splitline[1], line_num), line_num);

                case "OUTP":
                    return processOutput(processCommand(splitline[1], line_num), line_num);

                case "DISP":
                    return processDisp(processCommand(splitline[1], line_num));

                case "OPEN":
                    return processOpen(processStrg(processCommand(splitline[1], line_num)), line_num);
            }


            debug.throwException("Parsing error, invalid line", $"Line: {line_num}", debug.importance.Fatal);
            return "";

        }

        #region queryFunctions
        public static string buildIfQuery(string query)
        {
            ifQuery rawQuery = new ifQuery(query);
            return rawQuery.returnQuery();
        }
        public static string buildSelectQuery(string variableName, string query)
        {
            GroupCollection gc = new Regex(@"(LENGTH |)(ISOVER|ISUNDER|ISEQUALOVER|ISEQUALUNDER|ISEQUAL|ISNOT|IS|CONTAINSNOT|CONTAINS) (ANY|EVERY|)(?: STRG|)(?: LENGTH|)(?: IN|)(.*)").Match(query).Groups;
            selectQuery rawQuery = new selectQuery(variableName,gc); 
            return rawQuery.returnQuery();
        }
        #endregion

        public static string serializeNumericString(string input)
        {
            foreach (Match m in Regex.Matches(input, @"(\S*?) LENGTH"))
                input = input.Replace(m.Groups[0].Value, $"returnLength({m.Groups[1].Value})");

            return input;
        }
        public static string serializeNumericRange(string input)
        {
            string[] rangeElements = Regex.Split(input," AND ");
            string query = "new List<IEnumerable<int>>() {";

            for (int i = 0; i < rangeElements.Length; i++)
            {
                string element = rangeElements[i];
                string[] splitElement = Regex.Split(element, " TO ");
                splitElement = splitElement.Select(n => serializeNumericString(n)).ToArray();

                query += element.Contains(" TO ") ? $"EdgeRange({splitElement[0]},{splitElement[1]})" : $"EdgeRange({splitElement[0]},{splitElement[0]})";
                if (i != rangeElements.Length - 1)
                    query += ",";
            }
            return query + "}.SelectMany(n => n).ToList()";
        }


    }
    public class ifQuery
    {
        IEnumerable<string> queryParts;
        public ifQuery(string query)
        {
            this.queryParts = Regex.Split(query, @"(AND|OR)");
        }
        public string returnQuery()
        {
            this.queryParts = this.queryParts.Select(n => n.Replace("AND", "&&").Replace("OR", "||"));
            this.queryParts = this.queryParts.Select(n => codeParsing.serializeNumericString(n));


            foreach (var patternpair in baseDefinitions.operatorConversion)
            {
                this.queryParts = this.queryParts.Select(n => n.Contains(patternpair.Key) ? patternpair.Value(Regex.Split(n, $" {patternpair.Key} ")[0], Regex.Split(n, $" {patternpair.Key} ")[1]) : n);
            }
            return string.Join("",this.queryParts);
        }
        }
    public class selectQuery
        {
            public enum selector
            {
                Any,
                Every,
                None
            };
            selector queryRange;
            bool isNumeric, isPositive;
            string rowsVariable, comparedTo;
            Func<String, String, String> usedOperator;
            public selectQuery(string _rowsVariable, GroupCollection attribCollection)
            {
                string[] holdCollection = attribCollection.Cast<Group>().Skip(1).Select(n => n.Value.Trim()).ToArray();
                this.rowsVariable = _rowsVariable;
                this.isNumeric = holdCollection[0] == "LENGTH";
                this.isPositive = holdCollection[1] == "IS";
                this.usedOperator = baseDefinitions.operatorConversion[holdCollection[1]];
                this.queryRange = holdCollection[2] == "ANY" ? selector.Any : holdCollection[2] == "EVERY" ? selector.Every : selector.None;
                this.comparedTo = codeParsing.serializeNumericString(holdCollection[3]);
            }

            public string returnQuery()
            {
                if (isNumeric)
                    return usedOperator(rowsVariable + ".Where(temp => returnLength(temp)", comparedTo) + ((queryRange == selector.Every) ?
                        ".Max(temp_2 => temp_2.Length)" : (queryRange == selector.Any) ?
                        ".Min(temp_2 => temp_2.Length)" :
                        ""
                        ) + ").ToArray()";

                return rowsVariable + ".Where(temp => " + ((queryRange == selector.Every && this.isPositive || queryRange == selector.Any && !this.isPositive) ?
                    comparedTo + ".Where(temp_2 => " + usedOperator("temp_2", "temp") + $").ToArray().Length == {comparedTo}.Length" : (queryRange == selector.None) ?
                    usedOperator("temp", comparedTo) :
                    comparedTo + ".Where(temp_2 => " + usedOperator("temp_2", "temp") + ").ToArray().Length > 0"
                    ) + ").ToArray()";

            }
        }
    
}
