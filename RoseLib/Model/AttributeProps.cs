using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Model
{
    public class AttributeProps
    {
        public string Name { get; set; } = "";
        public List<string>? AttributeArgs { get; set; }

        public string? AttributeArgumentsAsString
        {
            get
            {
                if (AttributeArgs == null)
                {
                    return null;
                }
                else
                {
                    return "(" + string.Join(", ", AttributeArgs) + ")";
                }
            }
        }
    }
}
