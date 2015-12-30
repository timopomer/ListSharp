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
using System.Windows;
using System.Windows.Forms;
namespace ListSharp
{
    class Program
    {
        public static string[] allcommands = { "ROWSPLIT", "REPLACE", "READ", "EXTRACT", "COMBINE", "GETLINES","ADD" };
        [STAThread]
        static void Main(string[] args)
        {
            System.Drawing.Icon theicon = Properties.Resources.Untitled; //the application icon

            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ListSharp"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ListSharp");
            string dpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ListSharp\\theIcon.ico";
            File.WriteAllBytes(dpath, IconToBytes(theicon)); //writing the icon from resources to a pleace in appdata to refference it later


            

            Boolean debugmode = true;
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            Console.BackgroundColor = ConsoleColor.White;

            Console.Title = "ListSharp Console";
            Console.Clear();
            
            Regex _regex;
            Match match;

            string scriptfile = "";
            foreach (string s in args)
            {
                scriptfile = s;
            }


            if (args.Length == 0)
            {
                Console.WriteLine("No Script file was provided");
                Console.WriteLine("type 1 to open one or 2 to associate .ls files to this console");

                int choice = Convert.ToInt32(Console.ReadLine());

                if (choice == 1)
                {
                        Console.WriteLine("Opening file selection");
                        OpenFileDialog dialog1 = new OpenFileDialog();
                        dialog1.Filter = "ListSharp Scripts|*.ls";
                        DialogResult result = dialog1.ShowDialog();
                        if (result == DialogResult.OK) // Test result.
                        {
                            scriptfile = dialog1.FileName;
                        }
                        else
                        {
                            Console.WriteLine("Closed Dialog");
                            while (true)
                            {
                                Thread.Sleep(1000); //sleep forever
                            }
                        }


                }


                
                if (choice == 2)
                {
                    if (!IsAdministrator())
                    {
                        Console.WriteLine("Run as admin to associate .ls files");
                        while (true)
                        {
                            Thread.Sleep(1000); //sleep forever
                        }
                    }
                    SetAssociation(".ls", "ListSharp", Environment.CurrentDirectory + "\\listsharp.exe", "ListSharp Script", dpath); //.ls association
                    Console.WriteLine("Associated .ls files to this console");
                    while (true)
                    {
                        Thread.Sleep(1000); //sleep forever
                    }


                }
            }



            if (Path.GetExtension(scriptfile) != ".ls")
            {
                Console.WriteLine("Script file not from .ls type");
                while (true)
                {
                    Thread.Sleep(1000); //sleep forever
                }
            }
            
            
            Console.WriteLine("Script file");
            Console.WriteLine(scriptfile);

            string currentdir = Path.GetDirectoryName(scriptfile);
            Console.WriteLine("\n");

            string allcode = System.IO.File.ReadAllText(scriptfile);
            allcode = allcode.Replace("<here>", currentdir);

            List<string> maincode = allcode.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            maincode.RemoveAll(string.IsNullOrWhiteSpace);
            List<string> alllists = new List<string>();
            string code = "public class MainClass" + Environment.NewLine + "{ " + Environment.NewLine + "public string Execute()" + Environment.NewLine + "{" + Environment.NewLine + "string temp_contents = \"\";" + Environment.NewLine + "string output = \"\";" + Environment.NewLine;
            //variables initialization starts here:
            List<string> allRowsVariables = new List<string>();
            List<string> allStrgVariables = new List<string>();
            foreach (string singleline in maincode) {



                //rows variable
                if (singleline.Substring(0, 4) == "ROWS")
                {
                    _regex = new Regex(@"ROWS([^=]*)");
                    match = _regex.Match(singleline);
                    alllists.Add("string[] " + match.Groups[1].Value.Trim() + ";" + Environment.NewLine);
                    allRowsVariables.Add(match.Groups[1].Value.Trim());
                }

                //strg variable
                if (singleline.Substring(0, 4) == "STRG")
                {
                    _regex = new Regex(@"STRG([^=]*)");
                    match = _regex.Match(singleline);
                    alllists.Add("string " + match.Groups[1].Value.Trim() + " = \"\";" + Environment.NewLine);
                    allStrgVariables.Add(match.Groups[1].Value.Trim());
                }

                if (singleline.Substring(0, 1) == "#") //to see if the code is commented out so it does net get into the final code (replaced with //skipped for debugging porpuses
                {
                    if (singleline.Contains("ShowDebuggingInformation"))
                    {
                        if (singleline.Substring(singleline.Length - 4, 4) == "true")
                            debugmode = true;
                        else
                            debugmode = false;
                    }
                    continue;
                }

            }

            if (debugmode == true)
            {
                Console.WriteLine("Original Code \n-----------------------");
                Console.WriteLine(allcode);
                Console.WriteLine("-----------------------");
                Console.WriteLine(Environment.NewLine);
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
                

                if (splitline[0].Substring(0, 2) == "//") //to see if the code is commented out so it does net get into the final code (replaced with //skipped for debugging porpuses
                {
                    code += "//skipped";
                    code += Environment.NewLine;
                    continue;
                }

                if (splitline[0].Substring(0, 1) == "#") //to see if the code is commented out so it does net get into the final code (replaced with //skipped for debugging porpuses
                {
                    code += "//command executed: " + splitline[0];
                    code += Environment.NewLine;
                    continue;
                }

                splitline[1] = splitline[1].Substring(1);
                if (splitline[0].Length < 4) //checking that there is not too short/impossible line to break the interperter
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

                    if (splitline[1].Substring(0, 1) == "\"" && splitline[1].Substring(splitline[1].Length-1, 1) == "\"") //check if string simply assigned;
                    {
                        code += varname + " = @" + splitline[1] + ";"; //set variable to tempoary variable
                        code += Environment.NewLine;
                        continue;
                    }


                    if (isCommand(splitline[1]))
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
                        
                        code += "temp_contents = System.IO.File.ReadAllText(@" + path + ");"; //create the reading file code in interperted form that is read into a tempoary variable
                        code += Environment.NewLine;
                        code += varname + " = temp_contents;"; //set variable to tempoary variable
                    
                    }


                }

                //rows variable type
                if (splitline[0].Substring(0, 4) == "ROWS")
                {

                    if (splitline[1].Substring(0, 1) == "{" && splitline[1].Substring(splitline[1].Length - 1, 1) == "}") //check if string simply assigned;
                    {
                        code += varname + " = new string[]" + splitline[1] + ";"; //set variable to tempoary variable
                        code += Environment.NewLine;
                        continue;
                    }



                    if (isCommand(splitline[1]))
                    if (splitline[1].Substring(0, 8) == "ROWSPLIT") //rowsplit command
                    {
                        _regex = new Regex(@"ROWSPLIT([^>]*)BY"); //this finds what variable is to be split
                        match = _regex.Match(splitline[1]);
                        string invar = match.Groups[1].Value.Trim();

                        _regex = new Regex(@"\[(.*)\]"); //this finds by what string to split the variable
                        match = _regex.Match(splitline[1]);
                        string bywhat = match.Groups[1].Value.Trim();

                        bywhat = bywhat.Replace("<newline>", "System.Environment.NewLine"); //so you can split by newline by saying the string is <newline>

                        code += varname + " = SplitRows(" + invar + "," + bywhat + ");" ; //interperted code
                    }

                    if (isCommand(splitline[1]))
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

                    if (isCommand(splitline[1]))
                        if (splitline[1].Substring(0, 3) == "ADD") //rowsplit command
                        {
                            _regex = new Regex(@"TO([^>]*)"); //this finds what variable is to be split
                            match = _regex.Match(splitline[1]);
                            string invar = match.Groups[1].Value.Trim();

                            _regex = new Regex(@"\[(.*)\]"); //this finds what lines of the array to get
                            match = _regex.Match(splitline[1]);
                            string bywhat = match.Groups[1].Value.Trim();

                            code += varname + " = Add(" + invar + ",new object[] {" + bywhat + "});"; //interperted code
                        }

                    if (isCommand(splitline[1]))
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


                    if (isCommand(splitline[1]))
                    if (splitline[1].Substring(0, 7) == "COMBINE") //extract command
                    {

                        _regex = new Regex(@"COMBINE\[([^>]*?)\]"); //everything between the square brackets "[array,array,array]"
                        match = _regex.Match(splitline[1]);
                        string combinearrays = @match.Groups[1].Value.Trim();

                        _regex = new Regex(@"WITH \[(.*)\]"); //this finds by what string to extract the variable
                        match = _regex.Match(splitline[1]);
                        string withwhat = match.Groups[1].Value;

                        code += varname + " = Combine(new string[][] {" + combinearrays + "}," + withwhat + ");"; //interperted code
                    }



                }


                if (splitline[0].Substring(0, 4) == "STRG" || splitline[0].Substring(0, 4) == "ROWS")
                {
                    
                    if (!isCommand(splitline[1]))
                    {
                        code += varname + " = " + splitline[1] + ";"; //set variable to tempoary variable
                        code += Environment.NewLine;
                        continue;
                    }
                        


                    string restring = "";
                    if (splitline[0].Substring(0, 4) == "STRG")
                        restring = "string";
                    if (splitline[0].Substring(0, 4) == "ROWS")
                        restring = "string[]";
                    if (splitline[1].Substring(0, 7) == "REPLACE")
                    {

                        _regex = new Regex(@"\[(.*)\]");
                        match = _regex.Match(splitline[1]);
                        string withwhat = match.Groups[1].Value.Trim();
                        _regex = new Regex(@"IN([^>]*)"); //this finds by what string to extract the variable
                        match = _regex.Match(splitline[1]);
                        string whatvar = match.Groups[1].Value.Trim();

                        code += varname + " = (" + restring + ")replacestrg(" + whatvar + "," + withwhat + ");";
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
                        foreach (string ver in allStrgVariables)
                        {
                            code += "output += \"Listing " + ver + "\";";
                            code += Environment.NewLine;

                            code += "output = makeOutput(" + ver + " , output);"; //call makeOutput() on any type of variable the users wants to display
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
                        foreach (string ver in allRowsVariables)
                        {
                            code += "output += \"Listing " + ver + "\";";
                            code += Environment.NewLine;

                            code += "output = makeOutput(" + ver + " , output);"; //call makeOutput() on any type of variable the users wants to display
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
                            foreach (string ver in allStrgVariables)
                            {
                                code += "output += \"Listing " + ver + "\";";
                                code += Environment.NewLine;

                                code += "output = makeOutput(" + ver + " , output);"; //call makeOutput() on any type of variable the users wants to display
                                code += Environment.NewLine;
                            }

                            foreach (string ver in allRowsVariables)
                            {
                                code += "output += \"Listing " + ver + "\";";
                                code += Environment.NewLine;

                                code += "output = makeOutput(" + ver + " , output);"; //call makeOutput() on any type of variable the users wants to display
                                code += Environment.NewLine;
                            }
                        }
                        else
                        {
                            code += "output = makeOutput(" + splitline[1] + " , output);"; //call makeOutput() on any type of variable the users wants to display
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

                    code += "Write(@" + path + ", " + thevar + ");"; //output the rows to file
                    code += Environment.NewLine;
                }

                code += Environment.NewLine;
            }

            code += Environment.NewLine + "return output;";
            
            
            code += Environment.NewLine + "}" + Environment.NewLine;



            code += Properties.Resources.externalFunctions + "}"; //here we add all function depndecies
            

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


        public static bool isCommand(string inp)
        {
            for (int i = 0; i<allcommands.Count(); i++)
            {
                if (inp.Length >= allcommands[i].Length)
                    if (inp.Substring(0, allcommands[i].Length) == allcommands[i])
                        return true;
            }
            return false;
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


