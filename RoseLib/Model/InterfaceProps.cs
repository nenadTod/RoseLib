using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Model
{
    public class InterfaceProps: AccessModifierProps 
    {
        public string InterfaceName { get; set; } = "";
        public List<string>? BaseTypes { get; set; }
        public List<Model.AttributeProps> Attributes { get; set; } = new List<Model.AttributeProps>();
    }
}
