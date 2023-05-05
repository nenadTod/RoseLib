using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Model
{
    public class PropertyProps : AccessModifierProps
    {
        public string PropertyName { get; set; } = "";
        public string PropertyType { get; set; } = "";
    }
}
