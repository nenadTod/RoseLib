using RoseLibApp.RoseLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLibApp.RoseLib.Model
{
    public class MethodOptions : AccessModifierOptions
    {
        public string MethodName { get; set; }

        public string ReturnType { get; set; }
        public List<RLParameter> Parameters { get; set; }
    }
}
