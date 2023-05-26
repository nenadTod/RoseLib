using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.CSPath.Model
{
    public class PathPart
    {
        public Descend Descend { get; set; }
        public Concept Concept { get; set; }
        public PathPart(Descend descend, Concept concept)
        {
            Descend = descend;
            Concept = concept;
        }

        public override string ToString()
        {
            return $"{Descend.ToString()}{Concept.ToString()}";
        }
    }
}
