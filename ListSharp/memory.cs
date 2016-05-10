using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ListSharp
{
    public static class memory
    {
        public static Dictionary<string, Variable[]> variables;

        public static List<string> InitializeVariables(List<string> maincode)
        {

            variables = new Dictionary<string, Variable[]>();
            Regex rowsRegex = new Regex(@"ROWS([^=]*)");
            Regex strgRegex = new Regex(@"STRG([^=]*)");
            List<string> variableInitializers = new List<string>();

            List<ROWS> tempRows = new List<ROWS>();
            List<STRG> tempStrg = new List<STRG>();

            Match match;
            foreach (string singleline in maincode)
            {



                //rows variable
                if (singleline.StartsWith("ROWS"))
                {
                    match = rowsRegex.Match(singleline);
                    variableInitializers.Add("string[] " + match.Groups[1].Value.Trim() + " = { };");
                    tempRows.Add(new ROWS(match.Groups[1].Value.Trim()));
                }

                //strg variable
                if (singleline.StartsWith("STRG"))
                {      
                    match = strgRegex.Match(singleline);
                    variableInitializers.Add("string " + match.Groups[1].Value.Trim() + " = \"\";");
                    tempStrg.Add(new STRG(match.Groups[1].Value.Trim()));
                }
            }
            variables.Add("STRG", tempStrg.ToArray().Distinct().ToArray());
            variables.Add("ROWS", tempRows.ToArray().Distinct().ToArray());

            return variableInitializers.Distinct().ToList<string>();

        }
    }
}
