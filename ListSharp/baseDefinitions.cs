using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListSharp
{
    public static class baseDefinitions
    {

        public static replacePair[] replacePairs;
        public static void initialize()
        {
            replacePairs = new replacePair[]{ new replacePair("<here>", "@\"" + IO.currentdir + "\""), new replacePair("<newline>", "\"\\n\"") };
        }
        
    }
}
