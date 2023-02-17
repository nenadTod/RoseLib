using RoseLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Model
{
    public class MethodOptions : AccessModifierOptions
    {
        public string MethodName { get; set; } = "";

        public string ReturnType { get; set; } = "";
        public List<RLParameter> Parameters { get; set; } = new List<RLParameter>();
    }
}
