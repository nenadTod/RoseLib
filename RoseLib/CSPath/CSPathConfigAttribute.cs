using RoseLib.CSPath.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.CSPath
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CSPathConfigAttribute: Attribute
    {
        public string Concept { get; set; }
        public string? Attribute { get; set; }
        public string? Function { get; set; }

        public Concept ToCSPathModel()
        {
            Predicate? predicate = null;
            if(Attribute != null)
            {
                predicate = new Predicate(Attribute, null);
            }
            return new Concept(Concept, predicate);
        }
    }
}
