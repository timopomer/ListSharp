using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListSharp
{
    public static class codeProcessing
    {
        public static List<string> preProcessCode(this string rawCode)
        {
            string[] codeLines = rawCode.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 0; i < codeLines.Length; i++)
            {
                codeLines[i] = removePreWhitespace(codeLines[i]);

                if (codeLines[i].Contains('='))
                    codeLines[i] = removePreEqualsAndAfter(codeLines[i]);

            }

            return codeLines.ToList();
        }


        public static List<string> replaceConstants(this List<string> inputLines)
        {
            string[] alllines = inputLines.ToArray();

            for (int i = 0; i < alllines.Length; i++)
                foreach (replacePair replacer in baseDefinitions.replacePairs)
                    alllines[i] = alllines[i].Replace(replacer.name,replacer.value);

            return alllines.ToList();

        }




        #region processingFunctions

        public static string removePreWhitespace(string line)
        {
            int firstCharIndex = 0;

            foreach (char c in line)
            {
                if (c == ' ')
                    firstCharIndex++;
                else
                    break;
            }
            return line.Substring(firstCharIndex);
        }

        public static string removeAfterWhitespace(string line)
        {
            while (line.Substring(line.Length - 1) == " ")
                line = line.Substring(0, line.Length - 1);

            return line;
        }

        public static string removePreEqualsAndAfter(string line)
        {
            int firstEqualsIndex = line.IndexOf("=");

            string part_1 = line.Substring(0, firstEqualsIndex);

            string part_2 = line.Substring(firstEqualsIndex + 1);

            part_1 = removeAfterWhitespace(part_1);
            part_2 = removePreWhitespace(part_2);

            return part_1 + " = " + part_2;

        }

        #endregion
    }
}
