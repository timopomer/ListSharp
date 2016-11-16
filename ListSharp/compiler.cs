using System;
using System.CodeDom.Compiler;
using System.Linq;

namespace ListSharp
{
	public static class compiler
	{
		public static void createAssembly(string code)
		{

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
		}
	}
}
