using RoseLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Model
{
    public class ConstructorProps : AccessModifierProps
    {
        public string ClassName { get; set; } = "";
        public List<ParamProps> Params { get; set; } = new List<ParamProps>();

        public List<string> BaseArguments { get; set; } = new List<string>();
    }
}
