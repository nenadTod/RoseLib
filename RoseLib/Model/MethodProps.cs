using RoseLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Model
{
    public class MethodProps : AccessModifierProps
    {
        public string MethodName { get; set; } = "";
        public string ReturnType { get; set; } = "";
        public List<ParamProps> Params { get; set; } = new List<ParamProps>();
        public bool BodylessMethod { get; set; } = false;
    }
}
