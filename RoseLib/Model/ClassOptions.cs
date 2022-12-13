using RoseLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Model
{
    public class ClassOptions : AccessModifierOptions
    {
        public string ClassName { get; set; }
        public bool IsPartial { get; set; }
        public bool IsStatic { get; set; }

        
    }
}
