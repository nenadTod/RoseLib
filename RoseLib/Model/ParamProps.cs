using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Model
{
    public class ParamProps
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public object? DefaultValue { get; set; } = null;

        public ParamProps()
        {

        }

        public ParamProps(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}
