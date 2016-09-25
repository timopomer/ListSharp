using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Security.Principal;
using Associations;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ListSharp
{
    class Program
    {
        
        
        [STAThread]
        static void Main(string[] args)
        {
            #region defining variables

            System.Drawing.Icon theicon = Properties.Resources.Untitled; //the application icon

            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ListSharp"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ListSharp");
            string dpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ListSharp\\theIcon.ico";
            File.WriteAllBytes(dpath, IconToBytes(theicon)); //writing the icon from resources to a pleace in appdata to refference it later


            List<string> maincode;

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Title = "ListSharp Console";
            Console.Clear();



            #endregion


            #region setting up workspace
            IO.setScriptFile(args);
            if (IO.scriptfile == "")
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
                        IO.scriptfile = dialog1.FileName;
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



            if (Path.GetExtension(IO.scriptfile) != ".ls")
            {
                Console.WriteLine("Script file not from .ls type");
                while (true)
                {
                    Thread.Sleep(1000); //sleep forever
                }
            }

            IO.setScriptLocation();
            IO.setFileName();
            baseDefinitions.initialize();
            /* script file location intialized and validity checked */

            #endregion

            #region getting and cleaning up code

            Console.WriteLine("Script file");
            Console.WriteLine(IO.scriptfile);
            Console.WriteLine("\n");



            string allcode = IO.getFullCode();
            if (launchArguments.debugmode)
            {
                Console.WriteLine("Original Code \n-----------------------");
                Console.WriteLine(allcode);
                Console.WriteLine("-----------------------");
                Console.WriteLine(Environment.NewLine);
            }
            /* cleaning up code */


            allcode = allcode.replaceConstants();
            allcode = allcode.initialCleanup();
            List<string> initializers = memory.InitializeVariables(allcode);
            allcode = allcode.sanitizeStrings();
            allcode = allcode.sanitizeCode();
            allcode = allcode.sanitizeVars();
            allcode = allcode.preProcessCode();
            maincode = new List<string>(Regex.Split(allcode, "\r\n"));

            #endregion



string code =
@"using System.Net;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Text.RegularExpressions;

public class MainClass
{ 
[DllImport(""kernel32.dll"")]
static extern IntPtr GetConsoleWindow();

[DllImport(""user32.dll"")]
static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

const int SW_HIDE = 0;
const int SW_SHOW = 5;

public string Execute()
{
var handle = GetConsoleWindow();
string output = """";
";


            //variables initialization starts here:
            

            /*
             * since it creates the appropiate code for each variable even if it occurs twice
             * i am making sure there are no duplicate variable initalizations which would break the code
             */

            launchArguments.initializeArguments(maincode);
            

            if (launchArguments.debugmode)
            {
                Console.WriteLine("Original Code Tokenized \n-----------------------");
                foreach (string l in maincode)
                    Console.WriteLine(l);
                Console.WriteLine("-----------------------");
                Console.WriteLine(Environment.NewLine);
            }



            foreach (string temp_string in initializers)
            {
                code += temp_string + Environment.NewLine;
            }
            /* initializing all variables the script needs */
            int line_num = 1;
            foreach (string singleline in maincode)
            {
                code += codeParsing.processLine(singleline, line_num);
                code += Environment.NewLine;
                line_num++; //to know what line number we are currently processing
            }

            code += "ShowWindow(handle, SW_SHOW);\r\nreturn output;";
            code += Environment.NewLine + "}" + Environment.NewLine;
            int initialCodeLen = Regex.Split(code, "\r\n").Length;
            code += Properties.Resources.externalFunctions + "}"; //here we add all function depndecies

            code = code.returnStringArr();
            code = code.desanitizeVars();
            code = code.deSanitizeCode();
            code = code.deSanitizeStrings();

            if (launchArguments.debugmode)
            {
                Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine);
                Console.WriteLine("Interperted Code De-tokenized \n-----------------------");
            }
            int index = 1;

            string[] allLines = Regex.Split(code,"\r\n");


            foreach (string currentLine in allLines)
            {
                if (launchArguments.debugmode && index <= initialCodeLen)
                    System.Console.WriteLine("Line: " + index + "    " + currentLine);
                index++;
            }


            if (launchArguments.debugmode)
            {
                Console.WriteLine("-----------------------");
                Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            }
            Console.WriteLine("Initializing ListSharp");


            //compiling starts here


            string[] sources = { code };
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;

            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            parameters.ReferencedAssemblies.Add("System.Drawing.dll");
            
            using (Microsoft.CSharp.CSharpCodeProvider CodeProv =
            new Microsoft.CSharp.CSharpCodeProvider())
            {
                CompilerResults results = CodeProv.CompileAssemblyFromSource(parameters, sources);
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

        public static void SetAssociation(string Extension, string KeyName, string OpenWith, string FileDescription, string IconPath)
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


