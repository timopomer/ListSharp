using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ListSharp
{
    public static class launchArguments
    {
        public static Dictionary<string, object> flags = new Dictionary<string, object>()
        {
            {"silent",false},
            {"downloadretry",false},
            {"createbinary",false},
            {"downloadtries",1}
        };
        public static void processFlags(IEnumerable<string> flaginp)
        {
            foreach (string flag in flaginp)
            {
                if (flag.StartsWith("-s"))
                    flags["silent"] = true;

                if (flag.StartsWith("-r"))
                    flags["downloadretry"] = true;

                if (flag.StartsWith("-b"))
                    flags["createbinary"] = true;
            }
        }
        public static string flagsAsString()
        {
            return String.Join(Environment.NewLine, flags.Select(n => $"{n.Key}:{n.Value}"));
        }
    }
}
