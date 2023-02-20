using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Model
{
    public class AttributeProperties
    {
        public string Name { get; set; } = "";
        public List<string>? AttributeArgumentList { get; set; }

        public string? AttributeArgumentsAsString
        {
            get
            {
                if (AttributeArgumentList == null)
                {
                    return null;
                }
                else
                {
                    return "(" + string.Join(", ", AttributeArgumentList) + ")";
                }
            }
        }
    }
}
