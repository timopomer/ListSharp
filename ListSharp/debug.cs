using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ListSharp
{
    public static class debug
    {
        public enum importance
        {
            Regular,
            Fatal
        }

        public static void throwException(string title,string message,importance e)
        {

            Console.WriteLine(e.ToString() + " ListSharp exception: " + title);
            Console.WriteLine(message);
            if (e == importance.Fatal)
            {
                while (true)
                    Thread.Sleep(1000);
            }
            
        }
    }
}
