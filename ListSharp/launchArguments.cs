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
        public static bool debugmode = true;
        public static int downloadtries = 1;
        public static void initializeArguments(List<string> maincode)
        {
            foreach (string singleline in maincode)
            {
                if (singleline.Length < 1)
                    continue;

                if (singleline.StartsWith("#")) //to see if the code is commented out so it does net get into the final code (replaced with //skipped for debugging porpuses
                {
                    if (singleline.Contains("ShowDebuggingInformation"))
                        debugmode = singleline.EndsWith("true");


                    if (singleline.Contains("DownloadMaxTries"))
                        downloadtries = int.Parse(Regex.Split(singleline, " ")[1]);
                }


                

            }
        }
    }
}
