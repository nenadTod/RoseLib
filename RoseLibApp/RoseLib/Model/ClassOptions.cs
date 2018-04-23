using RoseLibApp.RoseLib.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLibApp.RoseLib.Model
{
    public class ClassOptions
    {
        public string ClassName { get; set; }
        public AccessModifierTypes AccessModifier { get; set; }
        public bool IsPartial { get; set; }
        public bool IsStatic { get; set; }

        public string ModifiersToString()
        {
            switch (AccessModifier)
            {
                case AccessModifierTypes.PRIVATE:
                    return "private";
                case AccessModifierTypes.PROTECTED:
                    return "protected";
                case AccessModifierTypes.PRIVATE_PROTECTED:
                    return "private protected";
                case AccessModifierTypes.INTERNAL:
                    return "internal";
                case AccessModifierTypes.PROTECTED_INTERNAL:
                    return "protected internal";
                case AccessModifierTypes.PUBLIC:
                    return "public";
                default:
                    return "";
            }
        }
    }
}
