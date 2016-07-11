using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Security.Principal;
using Associations;
using System.Windows.Forms;
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
            baseDefinitions.initialize();
            /* script file location intialized and validity checked */

            #endregion

            #region getting and cleaning up code




            Console.WriteLine("Script file");
            Console.WriteLine(IO.scriptfile);
            Console.WriteLine("\n");



            string allcode = IO.getFullCode();

            /* cleaning up code */
            maincode = allcode.preProcessCode();
            maincode = maincode.replaceConstants();
            maincode.RemoveAll(string.IsNullOrWhiteSpace);

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


public class MainClass
{ 

public string Execute()
{

string temp_contents = """";
string output = """";
";
            //variables initialization starts here:
            



            /*
             * since it creates the appropiate code for each variable even if it occurs twice
             * i am making sure there are no duplicate variable initalizations which would break the code
             */

            launchArguments.initializeArguments(maincode);
            List<string> initializers = memory.InitializeVariables(maincode);



            if (launchArguments.debugmode)
            {
                Console.WriteLine("Original Code \n-----------------------");
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

                code += codeProcessing.processLine(singleline, line_num);
                code += Environment.NewLine;
                line_num++; //to know what line number we are currently processing

            }

            code += "return output;";
            code += Environment.NewLine + "}" + Environment.NewLine;
            code += Properties.Resources.externalFunctions + "}"; //here we add all function depndecies


            if (launchArguments.debugmode)
            {
                Console.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine);
                Console.WriteLine("Interperted Code \n-----------------------");
            }
            int index = 1;
            char[] delimiterChars = { '\n' };
            string[] allLines = code.Split(delimiterChars);


            foreach (string currentLine in allLines)
            {
                if (launchArguments.debugmode)
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


