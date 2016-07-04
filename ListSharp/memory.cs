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
            Regex numbRegex = new Regex(@"NUMB([^=]*)");
            List<string> variableInitializers = new List<string>();

            List<ROWS> tempRows = new List<ROWS>();
            List<STRG> tempStrg = new List<STRG>();
            List<NUMB> tempNumb = new List<NUMB>();

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

                //numb variable
                if (singleline.StartsWith("NUMB"))
                {
                    match = numbRegex.Match(singleline);
                    variableInitializers.Add("int " + match.Groups[1].Value.Trim() + " = 0;");
                    tempNumb.Add(new NUMB(match.Groups[1].Value.Trim()));
                }

            }
            variables.Add("STRG", tempStrg.ToArray().Distinct().ToArray());
            variables.Add("ROWS", tempRows.ToArray().Distinct().ToArray());
            variables.Add("NUMB", tempNumb.ToArray().Distinct().ToArray());


            return variableInitializers.Distinct().ToList<string>();

        }

        public static bool ofVarType(this string varname,string type)
        {
            return variables[type].Where(p => p.name == varname).ToArray().Length > 0;
        }
    }
}
