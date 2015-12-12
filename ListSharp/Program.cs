using ListSharp.Properties;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Deployment;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Associations;

namespace ListSharp
{
    class Program
    {


        static void Main(string[] args)
        {
            System.Drawing.Icon theicon = Properties.Resources.Untitled; //the application icon

            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ListSharp"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ListSharp");
            string dpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ListSharp\\theIcon.ico";
            File.WriteAllBytes(dpath, IconToBytes(theicon)); //writing the icon from resources to a pleace in appdata to refference it later



            if (IsAdministrator() && args.Length == 0) //if administrator and no .ls file was sent to be interperted
            SetAssociation(".ls", "ListSharp", Environment.CurrentDirectory + "\\listsharp.exe","ListSharp Script",dpath); //.ls association


            Boolean debugmode = true;
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            Console.BackgroundColor = ConsoleColor.White;

            Console.Title = "ListSharp Console";
            Console.Clear();
            
            Regex _regex;
            Match match;

            if (!IsAdministrator() && args.Length == 0)
            {
                Console.WriteLine("Run as admin to associate .ls files");
                while (true)
                {
                    Thread.Sleep(1000); //sleep forever
                }
            }


            if (args.Length == 0)
            {
                Console.WriteLine("Added .ls file extention");
                while (true)
                {
                    Thread.Sleep(1000); //sleep forever
                }
            }
            string currentdir = Environment.CurrentDirectory;
            Console.WriteLine("Script file");
            foreach (string s in args)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("\n");
            
            string allcode = System.IO.File.ReadAllText(currentdir + "\\whatever.ls");
            allcode = allcode.Replace("<here>", currentdir);
            if (debugmode == true)
            {
                Console.WriteLine("Original Code \n-----------------------");
                Console.WriteLine(allcode);
                Console.WriteLine("-----------------------");
                Console.WriteLine(Environment.NewLine);
            }
            List<string> maincode = allcode.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            maincode.RemoveAll(string.IsNullOrWhiteSpace);
            List<string> alllists = new List<string>();
            string code = "public class MainClass" + Environment.NewLine + "{ " + Environment.NewLine + "public string Execute()" + Environment.NewLine + "{" + Environment.NewLine + "string temp_contents = \"\";" + Environment.NewLine + "string[][] tempstrarr;" + Environment.NewLine + "string output = \"\";" + Environment.NewLine;
            //variables initialization starts here:
            foreach (string singleline in maincode) {



                //rows variable
                if (singleline.Substring(0, 4) == "ROWS")
                {
                    _regex = new Regex(@"ROWS([^>]*)=");
                    match = _regex.Match(singleline);
                    alllists.Add("string[] " + match.Groups[1].Value.Trim() + ";" + Environment.NewLine);
                }

                //strg variable
                if (singleline.Substring(0, 4) == "STRG")
                {
                    _regex = new Regex(@"STRG([^>]*)=");
                    match = _regex.Match(singleline);
                    alllists.Add("string " + match.Groups[1].Value.Trim() + ";" + Environment.NewLine);
                }

            }

            /*
             * since it creates the appropiate code for each variable even if it occurs twice
             * i am making sure there are no duplicate variable initalizations which would break the code
             */
            alllists = alllists.Distinct().ToList<string>();
            foreach (string temp_string in alllists)
            {
                code += temp_string;
            }

            int line = 0;
            foreach (string singleline in maincode)
            {


                line++; //to know what line number we are currently processing


                string[] splitline = singleline.Split(new char[] { '=' }, 2); //splitting the line of "variable = evaluated string later to be parsed
                splitline[1] = splitline[1].Substring(1);

                if (splitline[0].Substring(0, 2) == "//") //to see if the code is commented out so it does net get into the final code (replaced with //skipped for debugging porpuses
                {
                    code += "//skipped";
                    code += Environment.NewLine;
                    continue;
                }


                if (splitline[0].Length < 4 || splitline[1].Length < 3) //checking that there is not too short/impossible line to break the interperter
                {
                    Console.WriteLine("ListSharp exception: Line: " + line + " invalid");
                    while(true)
                    {
                        Thread.Sleep(1000);
                    }
                    
                }


                string varname = splitline[0].Substring(4).Trim(); //the first 4 characters will always be the variable type ex: strg,rows,list

                //strg variable type
                if (splitline[0].Substring(0, 4) == "STRG")
                {



                    if (splitline[1].Substring(0, 4) == "READ") //read text file into code command is called "read"
                    {
                        _regex = new Regex(@"READ\[([^>]*)\]"); //everything between the square brackets "[path]"
                        match = _regex.Match(splitline[1]);
                        string path = @match.Groups[1].Value.Trim();

                        if (!File.Exists(path)) //checking that the file is readable
                        {
                            Console.WriteLine("ListSharp exception: File does not exist, aborting operation\nadditional information: File:\"" + path + "\"");

                            while (true)
                            {
                                Thread.Sleep(1000); //sleep forever
                            }
                        }
                        code += "temp_contents = System.IO.File.ReadAllText(@\"" + path + "\");"; //create the reading file code in interperted form that is read into a tempoary variable
                        code += Environment.NewLine;

                    }

                    code += varname + " = temp_contents;"; //set variable to tempoary variable
                    
                }

                //rows variable type
                if (splitline[0].Substring(0, 4) == "ROWS")
                {
                    if (splitline[1].Substring(0, 8) == "ROWSPLIT") //rowsplit command
                    {
                        _regex = new Regex(@"ROWSPLIT([^>]*)BY"); //this finds what variable is to be split
                        match = _regex.Match(splitline[1]);
                        string invar = match.Groups[1].Value.Trim();

                        _regex = new Regex(@"\[(.*)\]"); //this finds by what string to split the variable
                        match = _regex.Match(splitline[1]);
                        string bywhat = match.Groups[1].Value.Trim();

                        bywhat = bywhat.Replace("<newline>", "System.Environment.NewLine"); //so you can split by newline by saying the string is <newline>

                        code += varname + " = " + invar + ".Split(new string[] { " + bywhat + " }, System.StringSplitOptions.None);"; //interperted code
                    }

                    if (splitline[1].Substring(0, 8) == "GETLINES") //rowsplit command
                    {
                        _regex = new Regex(@"GETLINES([^>]*)\["); //this finds what variable is to be split
                        match = _regex.Match(splitline[1]);
                        string invar = match.Groups[1].Value.Trim();

                        _regex = new Regex(@"\[(.*)\]"); //this finds what lines of the array to get
                        match = _regex.Match(splitline[1]);
                        string bywhat = match.Groups[1].Value.Trim();

                        code += varname + " = getlines(" + invar + ",\"" + bywhat + "\");"; //interperted code
                    }

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


                        code += varname + " = Extract(" + whatvar + "," + bywhat + "," + collumnum + ");"; //interperted code
                    }



                    if (splitline[1].Substring(0, 7) == "COMBINE") //extract command
                    {

                        _regex = new Regex(@"COMBINE\[([^>]*)\]"); //everything between the square brackets "[array,array,array]"
                        match = _regex.Match(splitline[1]);
                        string combinearrays = @match.Groups[1].Value.Trim();

                        _regex = new Regex(@"WITH \[(.*)\]"); //this finds by what string to extract the variable
                        match = _regex.Match(splitline[1]);
                        string withwhat = match.Groups[1].Value;

                        code += "tempstrarr = new string[][] {" + combinearrays + "};";
                        code += Environment.NewLine;
                        code += varname + " = Combine(tempstrarr," + withwhat + ");"; //interperted code
                    }



                }




                if (splitline[0].Substring(0, 4) == "SHOW") //show a variable to debug your program
                {
                        _regex = new Regex(@"([^>]*);");
                        match = _regex.Match(splitline[1]);
                        code += "output = makeOutput(" + match.Groups[1].Value.Trim() + " , output);"; //call makeOutput() on any type of variable the users wants to display
                        code += Environment.NewLine;
                }

                if (splitline[0].Substring(0, 4) == "OUTP") //output command
                {
                    _regex = new Regex(@"([^>]*)HERE"); //everything between the square brackets "[path]"
                    match = _regex.Match(splitline[1]);
                    string thevar = @match.Groups[1].Value.Trim();


                    _regex = new Regex(@"HERE\[([^>]*)\]"); //everything between the square brackets "[path]"
                    match = _regex.Match(splitline[1]);
                    string path = @match.Groups[1].Value.Trim();

                    code += "Write(@\"" + path + "\", " + thevar + ");"; //output the rows to file
                    code += Environment.NewLine;
                }

                code += Environment.NewLine;
            }

            code += Environment.NewLine + "return output;";
            
            
            code += Environment.NewLine + "}" + Environment.NewLine;



            code += Properties.Resources.externalFunctions; //here we add all function depndecies
            

            if (debugmode == true)
            {
                Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine);
                Console.WriteLine("Interperted Code \n-----------------------");
            }
            int index = 1;
            char[] delimiterChars = { '\n' };
            string[] allLines = code.Split(delimiterChars);
            

            foreach (string currentLine in allLines)
            {
                if (debugmode == true)
                System.Console.WriteLine("Line: " + index + "    " +currentLine);
                index++;
            }


            if (debugmode == true)
            {
                Console.WriteLine("-----------------------");
                Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            }
            Console.WriteLine("Initializing ListSharp");


            //compiling starts here

            string[] sources = { code };
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;


            
            using (Microsoft.CSharp.CSharpCodeProvider CodeProv =
            new Microsoft.CSharp.CSharpCodeProvider())
            {
                CompilerResults results = CodeProv.CompileAssemblyFromSource(parameters,sources);
                foreach (CompilerError er in results.Errors)
                {
                    Console.WriteLine(er.ToString());
                }
                if (results.Errors.HasErrors)
                {
                    Console.WriteLine(Environment.NewLine + "Got compiling errors,aborting ListSharp");
                    while (true)
                    {
                        Thread.Sleep(1000);
                    }
                }


                 
                 var type = results.CompiledAssembly.GetType("MainClass");

                 var obj = Activator.CreateInstance(type);

                 var output = type.GetMethod("Execute").Invoke(obj, new object[] { });

                 Console.WriteLine(output);
            }

            while (true)
            {
                Thread.Sleep(1000); //sleep forever
            }
        }




        public static byte[] IconToBytes(System.Drawing.Icon icon)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                icon.Save(ms);
                return ms.ToArray();
            }
        }

        public static void SetAssociation(string Extension, string KeyName, string OpenWith, string FileDescription,string IconPath)
        {
            AF_FileAssociator assoc = new AF_FileAssociator(Extension);

            if (assoc.Exists)
            assoc.Delete();
            // Creates a new file association for the .ABC file extension. 
            // Data is overwritten if it already exists.
            assoc.Create(KeyName,
                FileDescription,
                new ProgramIcon(IconPath),
                new ExecApplication(OpenWith),
                new OpenWithList(new string[] { KeyName }));


            
        }



        public static bool IsAdministrator()
        {
            WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(windowsIdentity);

            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }

    }
}


