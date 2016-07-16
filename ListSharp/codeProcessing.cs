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
    public static class codeProcessing
    {

        #region preprocessing
        public static List<string> preProcessCode(this string rawCode)
        {
            rawCode = Regex.Replace(rawCode, @"\/\*(.*?)\*/", "", RegexOptions.Singleline);
            string[] codeLines = rawCode.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 0; i < codeLines.Length; i++)
            {
                codeLines[i] = removePreWhitespace(codeLines[i]);

                if (codeLines[i].Contains('='))
                    codeLines[i] = removePreEqualsAndAfter(codeLines[i]);

            }

            return codeLines.ToList();
        }

        public static List<string> replaceConstants(this List<string> inputLines)
        {
            string[] alllines = inputLines.ToArray();

            for (int i = 0; i < alllines.Length; i++)
                foreach (KeyValuePair<String,String> replacer in baseDefinitions.constantPairs)
                    alllines[i] = alllines[i].Replace(replacer.Key,replacer.Value);

            return alllines.ToList();
        }

        #endregion

        #region process codeline

        public static string processOperators(string line, int line_num)
        {
            if (line.StartsWith("{"))
            {
                return "{";
            }

            if (line.StartsWith("}"))
            {
                return "}";
            }

            if (line.StartsWith("//")) //to see if the code is commented out so it does net get into the final code (replaced with //skipped for debugging porpuses
            {
                return "//skipped";
            }

            if (line.StartsWith("#")) //to see if the code is commented out so it does net get into the final code (replaced with //skipped for debugging porpuses
            {
                return "//command executed: " + line;
            }


            /* shouldnt be able to happen anymore */
            if (line.StartsWith("/*") || line.StartsWith("*/")) //to see if the code is commented out so it does net get into the final code
            {
                return line;
            }
            

            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                return processExpression(line,line_num);
            }

            debug.throwException("Line: " + line_num + " invalid Operator", debug.importance.Fatal);

            return "";
        }

        public static string processExpression(string line, int line_num)
        {
            line = new Regex(@"\[(.*)\]").Match(line).Groups[1].Value; //everything between the square brackets "[]"

            if (line.StartsWith("FOREACH"))
            {
                if (line.StartsWith("FOREACH NUMB"))
                {
                    GroupCollection gc = new Regex(@"FOREACH NUMB IN (.*) AS (.*)").Match(line).Groups;
                    return "foreach (int " + gc[2].Value + " in " + serializeNumericRange(gc[1].Value) + ")";
                }
                if (line.StartsWith("FOREACH STRG"))
                {
                    GroupCollection gc = new Regex(@"FOREACH STRG IN (.*) AS (.*)").Match(line).Groups;
                    return "foreach (string " + gc[2].Value + " in " + gc[1].Value + ")";
                }

            }
            
            if (line.StartsWith("IF"))
            {
                return "if (" + ifBuilder(line) + ")";
            }

            debug.throwException("Line: " + line_num + " invalid Expression", debug.importance.Fatal);

            return "";
        }

        public static string processStrg(string line,int line_num,STRG strgVar)
        {

            if (line.StartsWith("READ")) //read text file into code command is called "read"
            {
                string path = new Regex(@"READ\[([^>]*)\]").Match(line).Groups[1].Value.Trim(); //everything between the square brackets "[path]"

                if (path.StartsWith("\"") && path.EndsWith("\"")) //if the path is a literal string we can check if it exists before compilation
                if (!File.Exists(path.Substring(1, path.Length - 2))) //checking that the file is readable
                {
                debug.throwException("File: " + path + " does not exist before compilation", debug.importance.Regular);
                }

                return strgVar.name + " = System.IO.File.ReadAllText(" + path + ");"; //create the reading file code in interperted form that is read into a tempoary variable
            }


            if (line.StartsWith("GETLINE")) //get single line from ROWS into STRG
            {
                GroupCollection gc = new Regex(@"GETLINE (.*) \[(.*)\]").Match(line).Groups;
                return strgVar.name + " = GETLINE_F(" + gc[1].Value + "," + serializeNumericString(gc[2].Value) + ");"; //interperted code
            }

            if (line.StartsWith("DOWNLOAD")) //download html code into STRG
            {
                string path = new Regex(@"DOWNLOAD\[([^>]*)\]").Match(line).Groups[1].Value.Trim(); //everything between the square brackets "[path]"
                return strgVar.name +  " = DOWNLOAD_F(" + path + ");"; //create the reading file code in interperted form that is read into a tempoary variable
            }


            return processVariableIndependant(line,line_num,strgVar);
        }

        public static string processRows(string line, int line_num, ROWS rowsVar)
        {
        

            if (line.StartsWith("ROWSPLIT")) //rowsplit command
            {
                GroupCollection gc = new Regex(@"ROWSPLIT ([^>]*) BY \[(.*)\]").Match(line).Groups;
                return rowsVar.name + " = ROWSPLIT_F(" + gc[1].Value + "," + gc[2].Value + ");"; //interperted code
            }

            if (line.StartsWith("SELECT")) //rowsplit command
            {
                return rowsVar.name + " = " + selectBuilder(line); //interperted code
            }



            if (line.StartsWith("GETLINES")) //getlines command
            {
                GroupCollection gc = new Regex(@"GETLINES ([^>]*) \[(.*)\]").Match(line).Groups;
                return rowsVar.name + " = GETLINES_F(" + gc[1].Value + "," + serializeNumericRange(gc[2].Value) + ");"; //interperted code
            }
            /*
            if (line.StartsWith("ADD")) //rowsplit command
            {
                GroupCollection gc = new Regex(@"\[(.*)\] TO ([^>]*)").Match(line).Groups;
                return rowsVar.name + " = ADD_F(" + gc[2].Value + ",new object[] {" + gc[1].Value + "});"; //interperted code
            }
            */

            if (line.StartsWith("EXTRACT")) //extract command
            {
                GroupCollection gc = new Regex(@"EXTRACT COLLUM\[([^>]*)\] FROM (.*?) SPLIT BY \[(.*)\]").Match(line).Groups;

                return rowsVar.name + " = EXTRACT_F(" + gc[2].Value + "," + gc[3].Value + "," + serializeNumericString(gc[1].Value) + ");"; //interperted code
            }

            if (line.StartsWith("COMBINE")) //extract command
            {
                GroupCollection gc = new Regex(@"COMBINE\[([^>]*?)\] WITH \[(.*)\]").Match(line).Groups;
                return rowsVar.name + " = COMBINE_F(new string[][] {" + gc[1].Value + "}," + gc[2].Value + ");"; //interperted code
            }

            return processVariableIndependant(line, line_num, rowsVar);
        }

        public static string processNumb(string line, int line_num, NUMB numbVar)
        {
            return processVariableIndependant(line, line_num, numbVar);
        }

        public static string processVariableIndependant(string line, int line_num, Variable inpVar)
        {

            string cast = inpVar is STRG ? "string" : "string[]";

            if (line.StartsWith("GETBETWEEN")) //filter command
            {
                GroupCollection gc = new Regex(@"GETBETWEEN (.*?) \[(.*)\] AND \[(.*)\]").Match(line).Groups;
                return inpVar.name + " = (" + cast + ")GETBETWEEN_F(" + gc[1].Value + "," + gc[2].Value + "," + gc[3].Value + ");"; //interperted code
            }

            if (line.StartsWith("GETRANGE")) //getrange command
            {
                GroupCollection gc = new Regex(@"GETRANGE (.*?) FROM \[(.*)\] TO \[(.*)\]").Match(line).Groups;
                return inpVar.name + " = (" + cast + ")GETRANGE_F(" + gc[1].Value + "," + serializeNumericString(gc[2].Value) + "," + serializeNumericString(gc[3].Value) + ");"; //interperted code
            }

            if (line.StartsWith("REPLACE"))
            {
                GroupCollection gc = new Regex(@"\[(.*)\] IN ([^>]*)").Match(line).Groups;
                return inpVar.name + " = (" + cast + ")REPLACE_F(" + gc[2].Value + "," + gc[1].Value + ");";
            }
            
            
            if (inpVar is STRG)
            {
                return inpVar.name + " = (string)ADD_F(new object[]{" + sanitizeEvaluation(line) + "},typeof(string));";
            }

            if (inpVar is ROWS)
            {      
                return inpVar.name + " = (string[])ADD_F(new object[]{" + sanitizeEvaluation(line) + "},typeof(string[]));";
            }

            if (inpVar is NUMB)
            {
                    return inpVar.name + " = " + serializeNumericString(line) + ";";
            }

            debug.throwException("Line: " + line_num + " invalid command", debug.importance.Fatal);

            return ""; //this never happens
        }

        #endregion

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
                        return "output += SHOW_F(" + line + ") + System.Environment.NewLine;";
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
                        returnedCode += typeShow("STRG") + "\n" + typeShow("ROWS");
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
                toReturn += "output += \"Listing " + ver.name + ":\" ;";
                toReturn += "output += SHOW_F(" + ver.name + ") + System.Environment.NewLine;"; //call SHOW_F() on any type of variable the users wants to display
            }
            return toReturn;
        }

        #endregion
        

        #region NOTF command
        public static string processNotification(string line, int line_num)
        {
            return "NOTIFY_F(" + line + ");";
        }
        #endregion

        #region DEBG command
        public static string processDebug(string line, int line_num)
        {
            return "DEBG_F(" + line + "," + line_num + ");";
        }
        #endregion
        #region OUTP command
        public static string processOutput(string line, int line_num)
        {
            GroupCollection gc = new Regex(@"(.*) HERE\[(.*)\]").Match(line).Groups;
            return "OUTP_F(" + gc[2].Value + ", " + gc[1].Value + ");"; //outputs to file
        }

        #endregion

        public static string processLine(string line, int line_num)
        {
            if (line.isLogic())
                return processOperators(line, line_num);


            string[] splitline = line.Split(new char[] { '=' }, 2); //splitting the line of "variable = evaluated string later to be parsed
            string varname = splitline[0].Substring(4).Trim(); //the first 4 characters will always be the variable type ex: strg,rows
            string start_argument = splitline[0].Substring(0, 4);
            splitline[1] = splitline[1].Substring(1);

            if (start_argument == "STRG")
                return processStrg(splitline[1], line_num, new STRG(varname));


            if (start_argument == "ROWS")
                return processRows(splitline[1], line_num, new ROWS(varname));


            if (start_argument == "NUMB")
                return processNumb(splitline[1], line_num, new NUMB(varname));


            if (start_argument == "SHOW")
                return processShow(splitline[1], line_num);


            if (start_argument == "OUTP")
                return processOutput(splitline[1], line_num);


            if (start_argument == "DEBG")
                return processDebug(splitline[1], line_num);


            if (start_argument == "NOTF")
                return processNotification(splitline[1], line_num);

            debug.throwException("Line: " + line + " could not be interpeted", debug.importance.Fatal);
            return "";

        }

        #region processingFunctions

        public static string removePreWhitespace(string line)
        {
            int firstCharIndex = 0;

            foreach (char c in line)
            {
                if (c == ' ')
                    firstCharIndex++;
                else
                    break;
            }
            return line.Substring(firstCharIndex);
        }

        public static string removeAfterWhitespace(string line)
        {
            while (line.Substring(line.Length - 1) == " ")
                line = line.Substring(0, line.Length - 1);

            return line;
        }

        public static string removePreEqualsAndAfter(string line)
        {
            int firstEqualsIndex = line.IndexOf("=");

            string part_1 = line.Substring(0, firstEqualsIndex);

            string part_2 = line.Substring(firstEqualsIndex + 1);

            part_1 = removeAfterWhitespace(part_1);
            part_2 = removePreWhitespace(part_2);

            return part_1 + " = " + part_2;

        }

        public static bool isLogic(this string line)
        {
            if (line.Length < 1)
                return false;


            return !Char.IsLetter(line.Substring(0, 1).ToCharArray()[0]);

        }

        public static string sanitizeEvaluation(string input)
        {
            string pattern = @"""(?:[^""\\]*(?:\\.)?)*""";
            Match[] mc = Regex.Matches(input, pattern).Cast<Match>().ToArray();
            Array.Reverse(mc);
            string[] replacementStrings = mc.Select(m => m.Value).ToArray();

            mc.ToList().ForEach(n => input = input.replaceStringRange(n.Index, n.Length, "[" + mc.ToList().IndexOf(n) + "]"));
            input = input.Replace("{", "").Replace("}", "").Replace(",", "+").Replace(" ", "");
            string[] splitExpression = input.Split('+');

            for (int i = 0; i < splitExpression.Length; i++)
            {
                for (int j = 0; j < mc.Length; j++)
                {
                    if (splitExpression[i] == "[" + j + "]")
                    {
                        splitExpression[i] = splitExpression[i].Replace("[" + j + "]", mc[j].Value);
                        break;
                    }
                }
            }
            return String.Join(" , ", splitExpression);
        }

        public static string replaceStringRange(this string input, int startIndex, int length, string replacement)
        {
            return input.Substring(0, startIndex) + replacement + input.Substring(startIndex + length);
        }
        #endregion



        #region queryFunctions
        #region if
        public static string ifBuilder(string line)
        {
            line = new Regex(@"IF (.*)").Match(line).Groups[1].Value;
            GroupCollection gc = new Regex(@"(.*)(ISOVER|ISUNDER|ISEQUALOVER|ISEQUALUNDER|ISEQUAL|ISNOT|IS|CONTAINSNOT|CONTAINS)(.*)").Match(line).Groups;
            Tuple<string, string> variables = getVarnames2(line);
            Tuple<string, string> sides = new Tuple<string, string>(gc[1].Value, gc[3].Value);
            switch (gc[2].Value)
            {
                case "ISOVER":
                case "ISUNDER":
                case "ISEQUAL":
                case "ISEQUALOVER":
                case "ISEQUALUNDER":
                    return numericIf(variables, sides, baseDefinitions.operatorConversion[gc[2].Value]);

                case "CONTAINSNOT":
                case "CONTAINS":
                    return containIf(variables, sides, baseDefinitions.operatorConversion[gc[2].Value]);

                case "IS":
                case "ISNOT":
                    return equallityIf(variables, sides, baseDefinitions.operatorConversion[gc[2].Value]);
            }
            debug.throwException("if mode does not exist", debug.importance.Fatal);
            return "";

        }
        
        public static string numericIf(Tuple<string, string> variables, Tuple<string, string> line, string operation)
        {
            return serializeNumericString(line.Item1) + operation + serializeNumericString(line.Item2);
        }

        public static string containIf(Tuple<string, string> variables, Tuple<string, string> line, string operation)
        {
            return operation + variables.Item1 + ".Contains(" + variables.Item2 + ")";
        }

        public static string equallityIf(Tuple<string, string> variables, Tuple<string, string> line, string operation)
        {
            if (variables.Item1.ofVarType("ROWS") && variables.Item2.ofVarType("ROWS"))
                return operation=="=="?variables.Item1 + ".SequenceEqual(" + variables.Item2 + ")" : "!" + variables.Item1 + ".SequenceEqual(" + variables.Item2 + ")";

            return variables.Item1 + operation + variables.Item2;
        }
        #endregion

        #region select
        public static string selectBuilder(string line)
        {
            string var1 = new Regex(@"FROM (.*) WHERE").Match(line).Groups[1].Value;
            line = new Regex(@"WHERE\[(.*)\]").Match(line).Groups[1].Value;
            GroupCollection gc = new Regex(@"(.*)(ISOVER|ISUNDER|ISEQUALOVER|ISEQUALUNDER|ISEQUAL|ISNOT|IS|CONTAINSNOT|CONTAINS)(.*)").Match(line).Groups;
            Tuple<string, string> variables = getVarnames(line, var1);
            Tuple<string, string> sides = new Tuple<string, string>(gc[1].Value, gc[3].Value);
            switch (gc[2].Value)
            {
                case "ISOVER":
                case "ISUNDER":
                case "ISEQUAL":
                case "ISEQUALOVER":
                case "ISEQUALUNDER":
                    return numericSelect(variables, sides, baseDefinitions.operatorConversion[gc[2].Value]);

                case "CONTAINSNOT":
                case "CONTAINS":
                    return containSelect(variables, sides, baseDefinitions.operatorConversion[gc[2].Value]);

                case "IS":
                case "ISNOT":
                    return equallitySelect(variables, sides, baseDefinitions.operatorConversion[gc[2].Value]);
            }
            debug.throwException("select mode does not exist", debug.importance.Fatal);
            return "";


        }

        public static string numericSelect(Tuple<string, string> variables, Tuple<string, string> line, string operation)
        {
            if (line.Item2.Contains("EVERY"))
                return variables.Item1 + ".Where(temp => returnLength(temp) " + operation + " " + variables.Item2 + ".Max(temp_2 => temp_2.Length)).ToArray();";

            if (line.Item2.Contains("ANY"))
                return variables.Item1 + ".Where(temp => returnLength(temp) " + operation + " " + variables.Item2 + ".Min(temp_2 => temp_2.Length)).ToArray();";

            return variables.Item1 + ".Where(temp => returnLength(temp) " + operation + " " +serializeNumericString(line.Item2) + ").ToArray();";
        }

        public static string containSelect(Tuple<string, string> variables, Tuple<string, string> line, string operation)
        {
            if (line.Item2.Contains("EVERY"))
                return variables.Item1 + ".Where(temp => " + variables.Item2 + ".Where(temp_2 => " + operation + "temp_2.Contains(temp)).ToArray().Length == " + variables.Item2 + ".Length).ToArray();";

            if (line.Item2.Contains("ANY"))
                return variables.Item1 + ".Where(temp => " + variables.Item2 + ".Where(temp_2 => " + operation + "temp_2.Contains(temp)).ToArray().Length > 0).ToArray();";

            return variables.Item1 + ".Where(temp => " + operation + "temp.Contains(" + variables.Item2 + ")).ToArray();";
        }

        public static string equallitySelect(Tuple<string, string> variables, Tuple<string, string> line, string operation)
        {
            bool positive = (operation == "==");
            bool enclusive = line.Item2.StartsWith(" EVERY");

            if (enclusive || line.Item2.StartsWith(" ANY"))
            {

                if (positive == enclusive)
                    return variables.Item1 + ".Where(temp => " + variables.Item2 + ".Where(temp_2 => temp_2 " + operation + " temp).ToArray().Length == " + variables.Item2 + ".Length).ToArray();";

                if (!enclusive)
                    return variables.Item1 + ".Where(temp => " + variables.Item2 + ".Where(temp_2 => temp_2 " + operation + " temp).ToArray().Length > 0).ToArray();";
            }

            return variables.Item1 + ".Where(temp => temp " + operation + " " + variables.Item2 + ").ToArray();";
        }

        public static Tuple<string, string> getVarnames(string inp, string var1)
        {
            string literal = "";
            if (inp.Contains("\""))
            {
                literal = new Regex("\"(.*)\"").Match(inp).Groups[0].Value;
                inp = inp.Replace(" " + literal, "");
            }

            string[] t = inp.Split(' ').Where(temp => !new string[] { "ANY", "EVERY", "LENGTH", "IN", "STRG", "IS", "ISNOT", "ISUNDER", "ISOVER", "ISEQUAL" ,"CONTAINS", "CONTAINSNOT" }.Contains(temp)).ToArray();
            if (t.Length == 0)
                return new Tuple<string, string>(var1, literal);
            return new Tuple<string, string>(var1, t[0]);
        }

        public static Tuple<string, string> getVarnames2(string inp)
        {
            string literal = "";
            if (inp.Contains("\""))
            {
                literal = new Regex("\"(.*)\"").Match(inp).Groups[0].Value;
                inp = inp.Replace(" " + literal, "");
            }

            string[] t = inp.Split(' ').Where(temp => !new string[] { "ANY", "EVERY", "LENGTH", "IN", "STRG", "IS", "ISNOT", "ISUNDER", "ISOVER", "ISEQUAL", "CONTAINS", "CONTAINSNOT" }.Contains(temp)).ToArray();
            if (t.Length == 1)
                return new Tuple<string, string>(t[0], literal);
            return new Tuple<string, string>(t[0], t[1]);
        }

        public static string serializeNumericString(string input)
        {
            foreach (Match m in Regex.Matches(input, @"(\w+) LENGTH"))
                input = input.Replace(m.Groups[0].Value, "returnLength(" + m.Groups[1].Value + ")");

            return input;
        }

        public static string serializeNumericRange(string input)
        {
            string[] rangeElements = input.Split(',');
            string query = "new List<IEnumerable<int>>() {";

            for (int i = 0; i<rangeElements.Length; i++)
            {
                string element = rangeElements[i];
                string[] splitElement = Regex.Split(element, " TO ");
                splitElement = splitElement.Select(n => serializeNumericString(n)).ToArray();

                query += element.Contains(" TO ") ? "EdgeRange(" + splitElement[0] + "," + splitElement[1] + ")" : "EdgeRange(" + splitElement[0] + "," + splitElement[0] + ")";
                if (i != rangeElements.Length - 1)
                    query += ",";
            }


            return query + "}.SelectMany(n => n).ToList()";
        }
        #endregion
        #endregion
    }
}
