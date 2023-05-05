using RoseLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Model
{
    public class ClassProps : AccessModifierProps
    {
        public bool IsPartial { get; set; }
        public bool IsStatic { get; set; }
        public bool IsAbstract { get; set; }
        public string ClassName { get; set; } = "";
        public List<string>? BaseTypes { get; set; }
        public List<Model.AttributeProps> Attributes { get; set; } = new List<Model.AttributeProps>();
    }
}
