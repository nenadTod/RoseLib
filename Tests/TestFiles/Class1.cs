using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.TestFiles
{
    internal class Class1
    {
        public Class1() { }

        public Class1(bool param1) { }

        private bool field1;

        internal class InnerClass1
        {
            private int field2;
            private int field3;

            internal class InnerClass2
            {
                private int field4;
                private int field5;
            }
        }
    }
}
