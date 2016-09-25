using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListSharp
{
    public class Variable
    {
        public string name;
        protected Variable(string name)
        {
            this.name = name;
        }
    }
    public class NUMB : Variable
    {
        public NUMB(string name) : base(name) { }
    }
    public class ROWS : Variable
    {
        public ROWS(string name) : base(name) { }
    }
    public class STRG : Variable
    {
        public STRG(string name) : base(name) { }
    }
}
