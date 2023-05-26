using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.CSPath.Model
{
    public class Descend
    {
        public string Tokens { get; set; }
        public Descend(string tokens)
        {
            Tokens = tokens;
        }

        public override string ToString()
        {
            return Tokens;
        }
    }
}
