using RoseLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Model
{
    public class MethodProperties : AccessModifierProperties
    {
        public string MethodName { get; set; } = "";

        public string ReturnType { get; set; } = "";
        public List<RLParameterProperties> Parameters { get; set; } = new List<RLParameterProperties>();
    }
}
