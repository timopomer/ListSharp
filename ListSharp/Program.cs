using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Security.Principal;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq;

namespace ListSharp
{
    class Program
    {
        
        
        [STAThread]
        static void Main(string[] args)
        {
            #region defining variables
            //System.Drawing.Icon theicon = Properties.Resources.Untitled; //the application icon

            //will be moved to IDE
            /*
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (!Directory.Exists(Path.Combine(appdata, "ListSharp")))
                Directory.CreateDirectory(Path.Combine(appdata, "ListSharp"));
            string dpath = Path.Combine(appdata, "ListSharp", "theIcon.ico");
            File.WriteAllBytes(dpath, IconToBytes(theicon)); //writing the icon from resources to a pleace in appdata to refference it later
            */

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Title = "ListSharp Console";
            Console.Clear();
            #endregion

            #region setting up workspace
            if (args.Length == 0)
            {
                debug.throwException("Initializing error, invalid parameters","reason: 0 paramters", debug.importance.Fatal);
            }
            Console.WriteLine(String.Join(" ", args));

            IO.setScriptFile(args[0]);
            IO.setScriptLocation();
            IO.setFileName();
            /* script file location intialized and validity checked */
            launchArguments.processFlags(args.Skip(1));
            baseDefinitions.initialize();
            #endregion

            #region getting and cleaning up code
            /* cleaning up code */
            string rawCodeFromFile = IO.getFullCode();
            string workedOnCode = rawCodeFromFile;

            workedOnCode = workedOnCode.replaceConstants();
            workedOnCode = workedOnCode.initialCleanup();
            List<string> initializers = memory.InitializeVariables(workedOnCode);
            workedOnCode = workedOnCode.sanitizeStrings();
            workedOnCode = workedOnCode.sanitizeCode();
            workedOnCode = workedOnCode.sanitizeVars();
            workedOnCode = workedOnCode.preProcessCode();

			List<string> codeAsLines = new List<string>(Regex.Split(workedOnCode, Environment.NewLine));
            #endregion

            #region codepart
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
static void Main(string[] args){
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.BackgroundColor = ConsoleColor.White;
Console.Title = ""ListSharp Console"";
Console.Clear();
Console.WriteLine(""Initializing ListSharp"");
Console.WriteLine(Execute());
while(true);
}
public static string Execute()
{
string output = """";
";
            #endregion

            code += String.Join(Environment.NewLine, initializers);
			code += Environment.NewLine;

            /* initializing all variables the script needs */
            code += String.Join(Environment.NewLine, codeAsLines.Select((line, line_num) => codeParsing.processLine(line, line_num + 1)));

            code += Environment.NewLine;
            code += "return output;";
            code += Environment.NewLine;
            code += "}";
            code += Environment.NewLine;

			int initialCodeLen = Regex.Split(code, Environment.NewLine).Length-1;
            code += Properties.Resources.externalFunctions;
            code += "}";

            code = code.returnStringArr();
            code = code.desanitizeVars();
            code = code.deSanitizeCode();
            code = code.deSanitizeStrings();

            if (!(bool)launchArguments.flags["silent"])
            {
                Console.WriteLine("Flags");
                Console.WriteLine("-----------------------");
                Console.WriteLine(launchArguments.flagsAsString());
                Console.WriteLine("-----------------------");
                Console.WriteLine(Environment.NewLine);

                Console.WriteLine("Original Code");
                Console.WriteLine("-----------------------");
                Console.WriteLine(rawCodeFromFile);
                Console.WriteLine("-----------------------");
                Console.WriteLine(Environment.NewLine);

                Console.WriteLine("Original Code Tokenized");
                Console.WriteLine("-----------------------");
                Console.WriteLine(String.Join(Environment.NewLine, codeAsLines));
                Console.WriteLine("-----------------------");
                Console.WriteLine(Environment.NewLine);

                Console.WriteLine("Interperted Code De-tokenized");
                Console.WriteLine("-----------------------");
				Console.WriteLine(String.Join(Environment.NewLine, Regex.Split(code, Environment.NewLine).Take(initialCodeLen).Select((line, line_num) => $"Line:   {line_num}        {line}")));
                Console.WriteLine("-----------------------");
                Console.WriteLine(Environment.NewLine);
            }

            //compiling starts here

            string[] sources = { code };
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = (bool)launchArguments.flags["createbinary"];
            if ((bool)launchArguments.flags["createbinary"])
            parameters.OutputAssembly = IO.getExePath();

            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            parameters.ReferencedAssemblies.Add("System.Drawing.dll");
            
            using (Microsoft.CSharp.CSharpCodeProvider CodeProv =
            new Microsoft.CSharp.CSharpCodeProvider())
            {
                CompilerResults results = CodeProv.CompileAssemblyFromSource(parameters, sources);

                if (results.Errors.HasErrors)
                    debug.throwException("Compilation error", String.Join(Environment.NewLine, results.Errors.Cast<CompilerError>().Select(n => n.ToString())), debug.importance.Fatal);
                
                if (!(bool)launchArguments.flags["createbinary"])
                {
                    Console.WriteLine("Initializing ListSharp");

                    var type = results.CompiledAssembly.GetType("MainClass");
                    var obj = Activator.CreateInstance(type);
                    var output = type.GetMethod("Execute").Invoke(obj, new object[] { });
                    Console.WriteLine(output);
                }
            }
            while (true){Thread.Sleep(1000);}
        }
    }
}


