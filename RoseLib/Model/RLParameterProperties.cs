using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Model
{
    public class RLParameterProperties
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object DefaultValue { get; set; }

        public RLParameterProperties()
        {

        }

        public RLParameterProperties(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}
