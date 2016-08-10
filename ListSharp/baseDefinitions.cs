using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListSharp
{
    public static class baseDefinitions
    {

        public static Dictionary<String,String> constantPairs;
        public static Dictionary<String, String> operatorConversion;

        public static void initialize()
        {
            constantPairs = new Dictionary<String, String>();
            operatorConversion = new Dictionary<String, String>();

            #region contstants
            constantPairs.Add("<scriptname>", "@\"" + IO.filename + "\"");
            constantPairs.Add("<here>", "@\"" + IO.currentdir + "\"");
            constantPairs.Add("<newline>", "\"\\n\"");
            #endregion

            #region operators
            operatorConversion.Add("ISOVER", ">");
            operatorConversion.Add("ISUNDER", "<");
            operatorConversion.Add("ISEQUALOVER", "=>");
            operatorConversion.Add("ISEQUALUNDER", "=<");
            operatorConversion.Add("ISEQUAL", "==");
            operatorConversion.Add("ISDIFF", "!=");
            operatorConversion.Add("IS", "==");
            operatorConversion.Add("ISNOT", "!=");
            operatorConversion.Add("CONTAINS", "");
            operatorConversion.Add("CONTAINSNOT", "!");
            #endregion

        }

    }
}
