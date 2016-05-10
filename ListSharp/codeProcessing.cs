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
        public static List<string> preProcessCode(this string rawCode)
        {
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
                foreach (replacePair replacer in baseDefinitions.replacePairs)
                    alllines[i] = alllines[i].Replace(replacer.name,replacer.value);

            return alllines.ToList();

        }

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
                GroupCollection gc = new Regex(@"IN (.*)AS (.*)").Match(line).Groups;
                return "foreach (string " + gc[2].Value + " in " + gc[1].Value + ")";
            }

            debug.throwException("Line: " + line_num + " invalid Expression", debug.importance.Fatal);

            return "";
        }

        public static string processStrg(string line,int line_num,STRG strgVar)
        {

            if (line.StartsWith("\"") && line.EndsWith("\"")) //check if string simply assigned;
            {
                return strgVar.name + " = @" + line + ";";
            }



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


            if (line.StartsWith("DOWNLOAD")) //read text file into code command is called "read"
            {
                string path = new Regex(@"DOWNLOAD\[([^>]*)\]").Match(line).Groups[1].Value.Trim(); //everything between the square brackets "[path]"
                return strgVar.name +  " = DOWNLOAD_F(" + path + ");"; //create the reading file code in interperted form that is read into a tempoary variable
            }


            return processVariableIndependant(line,line_num,strgVar);
        }

        public static string processRows(string line, int line_num, ROWS rowsVar)
        {
        
            if (line.StartsWith("{") && line.EndsWith("}")) //check if string simply assigned;
            {
                return rowsVar.name + " = new string[]" + line + ";";
            }

            if (line.StartsWith("ROWSPLIT")) //rowsplit command
            {
                GroupCollection gc = new Regex(@"ROWSPLIT ([^>]*) BY \[(.*)\]").Match(line).Groups;
                return rowsVar.name + " = ROWSPLIT_F(" + gc[1].Value + "," + gc[2].Value + ");"; //interperted code
            }

            if (line.StartsWith("FILTER")) //filter command
            {
                GroupCollection gc = new Regex(@"FILTER ([^>]*) IF (.*?) \[(.*)\]").Match(line).Groups;

                if (!new string[] {"CONTAINS","CONTAINSNOT","IS","ISNOT"}.Contains(gc[2].Value))
                {
                     debug.throwException("Filter mode \"" + gc[2].Value + "\" does not exist", debug.importance.Fatal);
                }

                return rowsVar.name + " = FILTER_F(" + gc[1].Value + ",\"" + gc[2].Value + "\"," + gc[3].Value + ");"; //interperted code
            }


            if (line.StartsWith("GETLINES")) //getlines command
            {
                GroupCollection gc = new Regex(@"GETLINES ([^>]*) \[(.*)\]").Match(line).Groups;
                return rowsVar.name + " = GETLINES_F(" + gc[1].Value + ",\"" + gc[2].Value + "\");"; //interperted code
            }

            if (line.StartsWith("ADD")) //rowsplit command
            {
                GroupCollection gc = new Regex(@"\[(.*)\] TO ([^>]*)").Match(line).Groups;
                return rowsVar.name + " = ADD_F(" + gc[2].Value + ",new object[] {" + gc[1].Value + "});"; //interperted code
            }

            if (line.StartsWith("EXTRACT")) //extract command
            {
                GroupCollection gc = new Regex(@"EXTRACT COLLUM\[([^>]*)\] FROM (.*?) SPLIT BY \[(.*)\]").Match(line).Groups;
                int collumnum = 0;
                try
                {
                    collumnum = Convert.ToInt32(gc[1].Value);
                }
                catch
                {
                    debug.throwException("Could not turn \"" + gc[1].Value + "\" into number in extract command", debug.importance.Fatal);
                }
                return rowsVar.name + " = EXTRACT_F(" + gc[2].Value + "," + gc[3].Value + "," + collumnum + ");"; //interperted code
            }

            if (line.StartsWith("COMBINE")) //extract command
            {
                GroupCollection gc = new Regex(@"COMBINE\[([^>]*?)\] WITH \[(.*)\]").Match(line).Groups;
                return rowsVar.name + " = COMBINE_F(new string[][] {" + gc[1].Value + "}," + gc[2].Value + ");"; //interperted code
            }

            return processVariableIndependant(line, line_num, rowsVar);
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
                return inpVar.name + " = (" + cast + ")GETRANGE_F(" + gc[1].Value + "," + gc[2].Value + "," + gc[3].Value + ");"; //interperted code
            }

            if (line.StartsWith("REPLACE"))
            {
                GroupCollection gc = new Regex(@"\[(.*)\] IN ([^>]*)").Match(line).Groups;
                return inpVar.name + " = (" + cast + ")REPLACE_F(" + gc[2].Value + "," + gc[1].Value + ");";
            }
            
            
            if (inpVar is STRG)
            {
                if (memory.variables["STRG"].Where(p => p.name == line).ToArray().Length > 0)
                    return inpVar.name + " = " + line + ";";
                else
                    debug.throwException("Variable: " + line + " wasnt defined yet", debug.importance.Fatal);

            }
            if (inpVar is ROWS)
            {
                if (memory.variables["ROWS"].Where(p => p.name == line).ToArray().Length > 0)
                    return inpVar.name + " = " + line + ";";
                else
                    debug.throwException("Variable: " + line + " wasnt defined yet", debug.importance.Fatal);
            }

            debug.throwException("Line: " + line_num + " invalid command", debug.importance.Fatal);

            return ""; //this never happens
        }


        public static string processLine(string line, int line_num)
        {
            if (line.isLogic())
                return processOperators(line, line_num);


            string[] splitline = line.Split(new char[] { '=' }, 2); //splitting the line of "variable = evaluated string later to be parsed
            string varname = splitline[0].Substring(4).Trim(); //the first 4 characters will always be the variable type ex: strg,rows
            string vartype = splitline[0].Substring(0, 4);
            splitline[1] = splitline[1].Substring(1);

            if (vartype == "STRG")
                return processStrg(splitline[1], line_num, new STRG(varname));


            if (vartype == "ROWS")
                return processRows(splitline[1], line_num, new ROWS(varname));


            Regex _regex;
            Match match;
            string code = "";










                if (splitline[0].Substring(0, 4) == "SHOW") //show a variable to debug your program
                {
                    if (splitline[1] == "STRG")
                    {
                        code += Environment.NewLine;
                        code += "output += System.Environment.NewLine;";
                        code += "output += \"Listing all STRG variables\";";
                        code += Environment.NewLine;
                        code += "output += System.Environment.NewLine;";
                        code += Environment.NewLine;
                        foreach (Variable ver in memory.variables["STRG"])
                        {
                            code += "output += \"Listing " + ver.name + "\";";
                            code += Environment.NewLine;

                            code += "output = SHOW_F(" + ver.name + " , output);"; //call makeOutput() on any type of variable the users wants to display
                            code += Environment.NewLine;
                        }
                    }
                    else
                    if (splitline[1] == "ROWS")
                    {
                        code += Environment.NewLine;
                        code += "output += System.Environment.NewLine;";
                        code += "output += \"Listing all ROWS variables\";";
                        code += Environment.NewLine;
                        code += "output += System.Environment.NewLine;";
                        code += Environment.NewLine;
                        foreach (Variable ver in memory.variables["ROWS"])
                        {
                            code += "output += \"Listing " + ver.name + "\";";
                            code += Environment.NewLine;

                            code += "output = SHOW_F(" + ver.name + " , output);"; //call makeOutput() on any type of variable the users wants to display
                            code += Environment.NewLine;
                        }
                    }
                    else
                        if (splitline[1] == "ALL")
                    {
                        code += Environment.NewLine;
                        code += "output += System.Environment.NewLine;";
                        code += "output += \"Listing all variables\";";
                        code += Environment.NewLine;
                        code += "output += System.Environment.NewLine;";
                        code += Environment.NewLine;
                        foreach (Variable ver in memory.variables["STRG"])
                        {
                            code += "output += \"Listing " + ver.name + "\";";
                            code += Environment.NewLine;

                            code += "output = SHOW_F(" + ver.name + " , output);"; //call makeOutput() on any type of variable the users wants to display
                            code += Environment.NewLine;
                        }

                        foreach (Variable ver in memory.variables["ROWS"])
                        {
                            code += "output += \"Listing " + ver.name + "\";";
                            code += Environment.NewLine;

                            code += "output = SHOW_F(" + ver.name + " , output);"; //call makeOutput() on any type of variable the users wants to display
                            code += Environment.NewLine;
                        }
                    }
                    else
                    {
                        code += "output = SHOW_F(" + splitline[1] + " , output);"; //call makeOutput() on any type of variable the users wants to display
                        code += Environment.NewLine;
                    }
                }

                if (splitline[0].Substring(0, 4) == "OUTP") //output command
                {
                    _regex = new Regex(@"([^>]*)HERE"); //everything between the square brackets "[path]"
                    match = _regex.Match(splitline[1]);
                    string thevar = @match.Groups[1].Value.Trim();


                    _regex = new Regex(@"HERE\[([^>]*)\]"); //everything between the square brackets "[path]"
                    match = _regex.Match(splitline[1]);
                    string path = @match.Groups[1].Value.Trim();

                    code += "OUTP_F(" + path + ", " + thevar + ");"; //output the rows to file
                    code += Environment.NewLine;
                }

            
            return code;
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
        /*
        public static bool startsWith(this string line,string value)
        {
            if (line.Length < value.Length)
                return false;

            return line.Substring(0, value.Length) == value;

        }
        */

        public static bool isLogic(this string line)
        {
            if (line.Length < 1)
                return false;


            return !Char.IsLetter(line.Substring(0, 1).ToCharArray()[0]);

        }

        #endregion
    }
}
