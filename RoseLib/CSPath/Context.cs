using RoseLib.CSPath.Model;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.CSPath
{
    internal class Context
    {
        internal IStatefulVisitor Visitor { get; set; }
        internal PathPart PathPart { get; set; }
        internal Context(IStatefulVisitor visitor, PathPart pathPart) 
        { 
            Visitor= visitor;
            PathPart= pathPart;
        }
    }
}
