using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Model
{
    public class FieldProps: AccessModifierProps
    {
        public string FieldName { get; set; } = "";
        public string FieldType { get; set; } = "";
    }
}
