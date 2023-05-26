using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.CSPath.Model
{
    public class Concept
    {
        public string Name { get; set; }
        public Predicate? Predicate { get; set; }
        public Concept(string name, Predicate? predicate)
        {
            Name = name;
            Predicate = predicate;
        }

        public override string ToString() { return $"{Name}{Predicate?.ToString() ?? ""}"; }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is not Concept other) return false;

            if (!other.Name.Equals(Name)) return false;
            if (Predicate == null && other.Predicate != null) return false;

            if (Predicate == null && other.Predicate == null) return true;

            return Predicate!.Equals(other.Predicate);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + (Predicate?.GetHashCode() ?? 0);
        }
    }
}
