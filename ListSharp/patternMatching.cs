using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ListSharp
{
    public static class patternMatching
    {
        public static bool notRecursing(string line)
        {
            return baseDefinitions.regexPatterns.Select(n => n.Value.Item1).ToList().Where(n => n.IsMatch(line)).Count() == 0;
            //return baseDefinitions.commandPattern.Matches(line).Count == 0;
        }
        public static string evaluateMatch(string line, string command)
        {
            for (int i = 0; i < line.Length; i++)
            {
                string workingLine = line.Substring(i);
                Match m = baseDefinitions.regexPatterns[command].Item1.Match(workingLine);
                GroupCollection gc = m.Groups;

                if (m.Success)
                {
                    if (gc.Cast<Group>().Skip(1).ToArray().Where(n => notRecursing(n.Value)).Count() == gc.Count - 1)
                    {
                        return line.Replace(gc[0].Value, baseDefinitions.regexPatterns[command].Item2.Invoke(gc));
                    }
                }
            }
            return line;

        }
        public static string evaluateAllMatches(string line)
        {
            baseDefinitions.regexPatterns.Select(n => n.Key).ToList().ForEach(m => line = evaluateMatch(line, m));
            return line;
        }
    }
}
