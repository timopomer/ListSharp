using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ListSharp
{
    class IO
    {
        public static string filename = "", scriptfile = "", currentdir = "";
        public static void setScriptFile(string arg)
        {
            if (!File.Exists(arg))
            {
                debug.throwException("Initializing error, invalid script", "reason: Script file doesnt exist", debug.importance.Fatal);
            }
            if (Path.GetExtension(arg) != ".ls")
            {
                debug.throwException("Initializing error, invalid script", "reason: Script file not from .ls type", debug.importance.Fatal);
            }
            scriptfile = arg;
        }

        public static void setScriptLocation()
        {
            currentdir = Path.GetDirectoryName(scriptfile);
        }

        public static string getFullCode()
        {
            return File.ReadAllText(scriptfile);
        }

        public static void setFileName()
        {
            filename = Path.GetFileName(scriptfile);
        }

        public static string getExePath()
        {
            return scriptfile.Substring(0,scriptfile.Length-2)+"exe";
        }

    }
}
