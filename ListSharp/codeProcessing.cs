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

            debug.throwException("Line: " + line_num + " invalid", debug.importance.Fatal);

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

            debug.throwException("Line: " + line_num + " invalid", debug.importance.Fatal);

            return "";
        }


        public static string processLine(string line, int line_num)
        {
            if (line.isLogic())
                return processOperators(line, line_num);





            Regex _regex;
            Match match;
            string code = "";
            string[] splitline = line.Split(new char[] { '=' }, 2); //splitting the line of "variable = evaluated string later to be parsed








            string varname = splitline[0].Substring(4).Trim(); //the first 4 characters will always be the variable type ex: strg,rows
            string vartype = splitline[0].Substring(0, 4);










            if (line.Contains("="))
            {

                splitline[1] = splitline[1].Substring(1);
                //strg variable type
                if (vartype == "STRG")
                {

                    if (splitline[1].Substring(0, 1) == "\"" && splitline[1].Substring(splitline[1].Length - 1, 1) == "\"") //check if string simply assigned;
                    {
                        code += varname + " = @" + splitline[1] + ";"; //set variable to tempoary variable
                        code += Environment.NewLine;
                        return code;
                    }


                    if (specifications.isCommand(splitline[1]))
                        if (splitline[1].Substring(0, 4) == "READ") //read text file into code command is called "read"
                        {
                            _regex = new Regex(@"READ\[([^>]*)\]"); //everything between the square brackets "[path]"
                            match = _regex.Match(splitline[1]);
                            string path = @match.Groups[1].Value.Trim();


                            if (path.Substring(0, 1) == "\"" && path.Substring(path.Length - 1, 1) == "\"")
                            {

                                if (!File.Exists(path.Substring(1, path.Length - 2))) //checking that the file is readable
                                {
                                    Console.WriteLine("ListSharp exception: File does not exist, aborting operation\nadditional information: File:\"" + path + "\"");

                                    while (true)
                                    {
                                        Thread.Sleep(1000); //sleep forever
                                    }
                                }
                            }


                            code += "temp_contents = System.IO.File.ReadAllText(" + path + ");"; //create the reading file code in interperted form that is read into a tempoary variable
                            code += Environment.NewLine;
                            code += varname + " = temp_contents;"; //set variable to tempoary variable

                        }

                    if (specifications.isCommand(splitline[1]))
                        if (splitline[1].Substring(0, 4) == "READ") //read text file into code command is called "read"
                        {
                            _regex = new Regex(@"READ\[([^>]*)\]"); //everything between the square brackets "[path]"
                            match = _regex.Match(splitline[1]);
                            string path = @match.Groups[1].Value.Trim();


                            if (path.Substring(0, 1) == "\"" && path.Substring(path.Length - 1, 1) == "\"")
                            {
                                  
                                if (!File.Exists(path.Substring(1, path.Length - 2))) //checking that the file is readable
                                {
                                    Console.WriteLine("ListSharp exception: File does not exist, aborting operation\nadditional information: File:\"" + path + "\"");

                                    while (true)
                                    {
                                        Thread.Sleep(1000); //sleep forever
                                    }
                                }
                            }


                            code += "temp_contents = System.IO.File.ReadAllText(" + path + ");"; //create the reading file code in interperted form that is read into a tempoary variable
                            code += Environment.NewLine;
                            code += varname + " = temp_contents;"; //set variable to tempoary variable

                        }


                    if (specifications.isCommand(splitline[1]))
                        if (splitline[1].Substring(0, 8) == "DOWNLOAD") //read text file into code command is called "read"
                        {
                            _regex = new Regex(@"DOWNLOAD\[([^>]*)\]"); //everything between the square brackets "[path]"
                            match = _regex.Match(splitline[1]);
                            string path = @match.Groups[1].Value.Trim();



                            code += "temp_contents = DOWNLOAD_F(" + path + ");"; //create the reading file code in interperted form that is read into a tempoary variable
                            code += Environment.NewLine;
                            code += varname + " = temp_contents;"; //set variable to tempoary variable

                        }




                }

                //rows variable type
                if (vartype == "ROWS")
                {

                    if (splitline[1].Substring(0, 1) == "{" && splitline[1].Substring(splitline[1].Length - 1, 1) == "}") //check if string simply assigned;
                    {
                        code += varname + " = new string[]" + splitline[1] + ";"; //set variable to tempoary variable
                        code += Environment.NewLine;
                        return code;
                    }



                    if (specifications.isCommand(splitline[1]))
                        if (splitline[1].Substring(0, 8) == "ROWSPLIT") //rowsplit command
                        {
                            _regex = new Regex(@"ROWSPLIT([^>]*)BY"); //this finds what variable is to be split
                            match = _regex.Match(splitline[1]);
                            string invar = match.Groups[1].Value.Trim();

                            _regex = new Regex(@"\[(.*)\]"); //this finds by what string to split the variable
                            match = _regex.Match(splitline[1]);
                            string bywhat = match.Groups[1].Value.Trim();


                            code += varname + " = ROWSPLIT_F(" + invar + "," + bywhat + ");"; //interperted code
                        }

                    if (specifications.isCommand(splitline[1]))
                        if (splitline[1].Substring(0, 6) == "FILTER") //filter command
                        {
                            _regex = new Regex(@"FILTER([^>]*)IF"); //this finds what variable is to be split
                            match = _regex.Match(splitline[1]);
                            string invar = match.Groups[1].Value.Trim();


                            _regex = new Regex(@"IF(.*?)\["); //this finds what variable is to be split
                            match = _regex.Match(splitline[1]);
                            string mode = match.Groups[1].Value.Trim();


                            _regex = new Regex(@"\[(.*)\]"); //this finds by what string to split the variable
                            match = _regex.Match(splitline[1]);
                            string param = match.Groups[1].Value.Trim();

                            if (mode != "CONTAINS" && mode != "CONTAINSNOT" && mode != "IS" && mode != "ISNOT")
                            {
                                Console.WriteLine("ListSharp exception: Filter mode does not exist, aborting operation\nadditional information: mode:\"" + mode + "\"");

                                while (true)
                                {
                                    Thread.Sleep(1000); //sleep forever
                                }
                            }
                            code += varname + " = FILTER_F(" + invar + ",\"" + mode + "\"," + param + ");"; //interperted code
                        }



                    if (specifications.isCommand(splitline[1]))
                        if (splitline[1].Substring(0, 8) == "GETLINES") //rowsplit command
                        {
                            _regex = new Regex(@"GETLINES([^>]*)\["); //this finds what variable is to be split
                            match = _regex.Match(splitline[1]);
                            string invar = match.Groups[1].Value.Trim();

                            _regex = new Regex(@"\[(.*)\]"); //this finds what lines of the array to get
                            match = _regex.Match(splitline[1]);
                            string bywhat = match.Groups[1].Value.Trim();

                            code += varname + " = GETLINES_F(" + invar + ",\"" + bywhat + "\");"; //interperted code
                        }

                    if (specifications.isCommand(splitline[1]))
                        if (splitline[1].Substring(0, 3) == "ADD") //rowsplit command
                        {
                            _regex = new Regex(@"TO([^>]*)"); //this finds what variable is to be split
                            match = _regex.Match(splitline[1]);
                            string invar = match.Groups[1].Value.Trim();

                            _regex = new Regex(@"\[(.*)\]"); //this finds what lines of the array to get
                            match = _regex.Match(splitline[1]);
                            string bywhat = match.Groups[1].Value.Trim();

                            code += varname + " = ADD_F(" + invar + ",new object[] {" + bywhat + "});"; //interperted code
                        }

                    if (specifications.isCommand(splitline[1]))
                        if (splitline[1].Substring(0, 7) == "EXTRACT") //extract command
                        {
                            _regex = new Regex(@"EXTRACT([^>]*)FROM");
                            match = _regex.Match(splitline[1]);
                            string collumvar = match.Groups[1].Value.Trim();

                            _regex = new Regex(@"\[(.*)\]"); //this finds what collum is to be extracted
                            match = _regex.Match(collumvar);
                            int collumnum = Convert.ToInt32(match.Groups[1].Value.Trim());

                            _regex = new Regex(@"FROM([^>]*)SPLIT"); //this finds what variable is to be extracted from
                            match = _regex.Match(splitline[1]);
                            string whatvar = match.Groups[1].Value.Trim();

                            _regex = new Regex(@"BY \[(.*)\]"); //this finds by what string to extract the variable
                            match = _regex.Match(splitline[1]);
                            string bywhat = match.Groups[1].Value;


                            code += varname + " = EXTRACT_F(" + whatvar + "," + bywhat + "," + collumnum + ");"; //interperted code
                        }


                    if (specifications.isCommand(splitline[1]))
                        if (splitline[1].Substring(0, 7) == "COMBINE") //extract command
                        {

                            _regex = new Regex(@"COMBINE\[([^>]*?)\]"); //everything between the square brackets "[array,array,array]"
                            match = _regex.Match(splitline[1]);
                            string combinearrays = @match.Groups[1].Value.Trim();

                            _regex = new Regex(@"WITH \[(.*)\]"); //this finds by what string to extract the variable
                            match = _regex.Match(splitline[1]);
                            string withwhat = match.Groups[1].Value;

                            code += varname + " = COMBINE_F(new string[][] {" + combinearrays + "}," + withwhat + ");"; //interperted code
                        }



                }


                if (splitline[0].Substring(0, 4) == "STRG" || splitline[0].Substring(0, 4) == "ROWS")
                {

                    if (!specifications.isCommand(splitline[1]))
                    {
                        code += varname + " = " + splitline[1] + ";"; //set variable to tempoary variable
                        code += Environment.NewLine;
                        return code;
                    }



                    string restring = "";
                    if (splitline[0].Substring(0, 4) == "STRG")
                        restring = "string";
                    if (splitline[0].Substring(0, 4) == "ROWS")
                        restring = "string[]";



                    if (splitline[1].Substring(0, 10) == "GETBETWEEN") //filter command
                    {
                        _regex = new Regex(@"GETBETWEEN(.*?)\["); //this finds what variable is to be split
                        match = _regex.Match(splitline[1]);
                        string invar = match.Groups[1].Value.Trim();


                        _regex = new Regex(@"\[(.*)\] AND"); //this finds what variable is to be split
                        match = _regex.Match(splitline[1]);
                        string start = match.Groups[1].Value.Trim();


                        _regex = new Regex(@"AND \[(.*)\]"); //this finds by what string to split the variable
                        match = _regex.Match(splitline[1]);
                        string end = match.Groups[1].Value.Trim();


                        code += varname + " = (" + restring + ")GETBETWEEN_F(" + invar + "," + start + "," + end + ");"; //interperted code
                    }

                    if (splitline[1].Substring(0, 8) == "GETRANGE") //getrange command
                    {
                        _regex = new Regex(@"GETRANGE(.*?)FROM \["); //this finds what variable is to be split
                        match = _regex.Match(splitline[1]);
                        string invar = match.Groups[1].Value.Trim();


                        _regex = new Regex(@"FROM \[(.*)\] TO"); //find starting index
                        match = _regex.Match(splitline[1]);
                        string start = match.Groups[1].Value.Trim();


                        _regex = new Regex(@"TO \[(.*)\]"); //find end index
                        match = _regex.Match(splitline[1]);
                        string end = match.Groups[1].Value.Trim();


                        code += varname + " = (" + restring + ")GETRANGE_F(" + invar + "," + start + "," + end + ");"; //interperted code
                    }

                    if (splitline[1].Substring(0, 7) == "REPLACE")
                    {

                        _regex = new Regex(@"\[(.*)\]");
                        match = _regex.Match(splitline[1]);
                        string withwhat = match.Groups[1].Value.Trim();
                        _regex = new Regex(@"IN([^>]*)"); //this finds by what string to extract the variable
                        match = _regex.Match(splitline[1]);
                        string whatvar = match.Groups[1].Value.Trim();

                        code += varname + " = (" + restring + ")REPLACE_F(" + whatvar + "," + withwhat + ");";
                    }
                }

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
