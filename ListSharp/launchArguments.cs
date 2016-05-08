using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListSharp
{
    public static class launchArguments
    {
        public static bool debugmode = true;
        public static void initializeArguments(List<string> maincode)
        {
            foreach (string singleline in maincode)
            {
                if (singleline.Length < 1)
                    continue;

                if (singleline.Substring(0, 1) == "#") //to see if the code is commented out so it does net get into the final code (replaced with //skipped for debugging porpuses
                {
                    if (singleline.Contains("ShowDebuggingInformation"))
                        debugmode = singleline.Substring(singleline.Length - 4, 4) == "true";
                }

            }
        }
    }
}
