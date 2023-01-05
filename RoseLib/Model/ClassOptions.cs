using RoseLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Model
{
    // TODO: Added a couple of new class attributes, need to extend logic to support them
    public class ClassOptions : AccessModifierOptions
    {
        public bool IsPartial { get; set; }
        public bool IsStatic { get; set; }
        public bool IsAbstract { get; set; }
        public AccessModifierOptions? AccessModifierOptions {get;set;}
        public string ClassName { get; set; } = "";
        public List<string>? BaseTypes { get; set; }
    }
}
