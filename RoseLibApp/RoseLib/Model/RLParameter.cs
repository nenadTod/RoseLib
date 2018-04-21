using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLibApp.RoseLib.Model
{
    public class RLParameter
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object DefaultValue { get; set; }

        public RLParameter()
        {

        }

        public RLParameter(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}
