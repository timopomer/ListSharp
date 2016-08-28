using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ListSharp
{
    public static class codeProcessing
    {
        public static Tuple<string, string>[] replacementStrings;
        public static Tuple<string, string>[] replacementCode;
        public static Tuple<string, string>[] replacementVars;

        #region preprocessingFunctions
        public static string initialCleanup(this string rawCode)
        {
            rawCode = Regex.Replace(rawCode, @"\/\*(.*?)\*/", "", RegexOptions.Singleline);
            string[] codeLines = Regex.Split(rawCode, "\r\n").Where(n=>!string.IsNullOrWhiteSpace(n)).ToArray();

            for (int i = 0; i < codeLines.Length; i++)
            {
                codeLines[i] = removePreWhitespace(codeLines[i]);
                codeLines[i] = makeEqualsEvenlySpaced(codeLines[i]);
            }
            codeLines = codeLines.Where(n => !n.StartsWith(@"//")).ToArray();
            return String.Join("\r\n",codeLines);
        }
        public static string preProcessCode(this string rawCode)
        {

            string[] codeLines = Regex.Split(rawCode, "\r\n");

            for (int i = 0; i < codeLines.Length; i++)
            {
                codeLines[i] = removeArrayBrackets(codeLines[i]);
                codeLines[i] = makeAdditionEvenlySpaced(codeLines[i]);
                codeLines[i] = replaceAddition(codeLines[i]);
            }
            

            return String.Join("\r\n", codeLines);
        }
        public static string replaceConstants(this string inputCode)
        {

            foreach (KeyValuePair<String,String> replacer in baseDefinitions.constantPairs)
                inputCode = inputCode.Replace(replacer.Key,replacer.Value);

            return inputCode;
        }
        #endregion


        #region processingFunctions
        public static string removePreWhitespace(string line)
        {
            return new Regex(@"^\s*(.*)").Match(line).Groups[1].Value;
        }
        public static string removeAfterWhitespace(string line)
        {
            return new Regex(@"(.*)\s*$").Match(line).Groups[1].Value;
        }
        public static string makeEqualsEvenlySpaced(string line)
        {
            
            return line.Contains("=")?line.Replace(new Regex(@"\s*=\s*").Match(line).Groups[0].Value," = "):line;

        }
        public static string makeAdditionEvenlySpaced(string line)
        {
            MatchCollection mc = new Regex(@"\s*\+\s*").Matches(line);
            foreach (Match m in mc)
            {
                line = line.Replace(m.Value, "+");
            }
            return line;
        }
        public static string removeArrayBrackets(string line)
        {
            return line.Replace("{", "").Replace("}", "");
        }
        public static string replaceAddition(string line)
        {
            return line.Replace("+",",");
            /*
            string oB = "(" + String.Join("|", new string[] { "«", "<", "(" }.Select(n => Regex.Escape(n))) + ")";
            string cB = "(" + String.Join("|", new string[] { "»", ">", ")" }.Select(n => Regex.Escape(n))) + ")";
            Regex r = new Regex(oB + @"\d-\w{10}" + cB + @"?\+?" + oB + @"\d-\w{10}" + cB + "?");
            MatchCollection mc = r.Matches(line);
            foreach (Match m in mc)
            {
                line = line.Replace(m.Value, String.Join(",", m.Value.Split('+')));
            }
            return r.Matches(line).Count == 0 ? line : modifyVariableAddition(line);
            */
        }
        public static string replaceStringRange(this string input, int startIndex, int lengthOfReplacedText, string replacementText)
        {
            return input.Substring(0, startIndex) + replacementText + input.Substring(startIndex + lengthOfReplacedText);
        }
        public static string returnStringArr(this string inputCode)
        {
            return inputCode.Replace("stringarr", "string[]");
        }
        #endregion

        #region sanitizationFunctions
        public static string sanitizeStrings(this string inputCode)
        {
            string pattern = @"""(?:[^""\\]*(?:\\.)?)*""";
            Match[] mc = Regex.Matches(inputCode, pattern,RegexOptions.Singleline).Cast<Match>().ToArray();
            Array.Reverse(mc);
            replacementStrings = mc.Select((m, i) => new Tuple<string, string>(m.Value, createHash(i))).ToArray();
            for (int i = 0; i < mc.Length; i++)
            {
                inputCode = inputCode.replaceStringRange(mc[i].Index, mc[i].Length, "«" + replacementStrings[i].Item2 + "»");
            }
            Array.Reverse(replacementStrings);
            return inputCode;
        }
        public static string deSanitizeStrings(this string inputCode)
        {
            for (int i = 0; i < replacementStrings.Length; i++)
            {
                inputCode = inputCode.Replace("«" + replacementStrings[i].Item2 + "»", replacementStrings[i].Item1);
            }
            return inputCode;
        }
        public static string sanitizeCode(this string inputCode)
        {
            string pattern = @"<c#(.*?)c#>";
            Match[] mc = Regex.Matches(inputCode, pattern, RegexOptions.Singleline).Cast<Match>().ToArray();
            Array.Reverse(mc);
            replacementCode = mc.Select((m, i) => new Tuple<string, string>(m.Groups[1].Value, createHash(i))).ToArray();
            for (int i = 0; i < mc.Length; i++)
            {
                inputCode = inputCode.replaceStringRange(mc[i].Index, mc[i].Length, "<" + replacementCode[i].Item2 + ">");
            }
            Array.Reverse(replacementCode);
            return inputCode;
        }
        public static string deSanitizeCode(this string inputCode)
        {
            for (int i = 0; i < replacementCode.Length; i++)
            {
                inputCode = inputCode.Replace("<" + replacementCode[i].Item2 + ">",replacementCode[i].Item1);
            }
            return inputCode;
        }
        public static string sanitizeVars(this string inputCode)
        {

            replacementVars = memory.getVariableNames().Select((m, i) => new Tuple<string, string>(m, createHash(i))).ToArray();
            replacementVars = replacementVars.OrderByDescending(n => n.Item1.Length).ToArray();
            for (int i = 0; i < replacementVars.Length; i++)
            {
                inputCode = inputCode.Replace(replacementVars[i].Item1, "(" + replacementVars[i].Item2 + ")");
            }
            Array.Reverse(replacementVars);
            return inputCode;
        }
        public static string desanitizeVars(this string inputCode)
        {
            for (int i = 0; i < replacementVars.Length; i++)
            {
                inputCode = inputCode.Replace("(" + replacementVars[i].Item2 + ")", replacementVars[i].Item1);
            }
            return inputCode;
        }
        #endregion


        #region hashingFunctions
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public static string createHash(int index)
        {
            var now = DateTime.Now;
            return index + "-" + CreateMD5(index + now.Hour + now.Minute + now.Second + now.Millisecond + "").Substring(0,10);
        }
        #endregion

    }
}
