using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.TestFiles
{
    internal struct Struct1
    {
        private bool field1;
        public int Prop1 { get; set; } // Comment

        public Struct1() { }

        public void Method1() 
        {
            var variable = true; 
        }
    }
}
