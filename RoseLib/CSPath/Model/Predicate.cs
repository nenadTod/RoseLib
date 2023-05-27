using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IronPython.Modules._ast;

namespace RoseLib.CSPath.Model
{
    public class Predicate
    {
        public string Attribute { get; set; }
        public string? Value { get; set; }

        public Predicate(string attribute, string? value) 
        { 
            Attribute = attribute;
            Value = value;
        }

        public override string ToString()
        {
            return $"[{Attribute}={(Value != null ? "'" + Value + "'" : "?")}]";
        }

        // Only Attribute name is significant here
        // Revise when introducing other types of predicates
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is not Predicate other) return false;

            if (!other.Attribute.Equals(Attribute)) return false;

            return true;
        }

        // Only Attribute name is significant here
        // Revise when introducing other types of predicates
        public override int GetHashCode()
        {
            return Attribute.GetHashCode();
        }
    }
}
