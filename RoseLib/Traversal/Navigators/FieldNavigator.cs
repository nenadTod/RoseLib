using RoseLib.Composers;
using RoseLib.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Navigators
{
    public class FieldNavigator: BaseNavigator
    {
        internal FieldNavigator(BaseNavigator? parentNavigator): base(parentNavigator)
        {
        }
    }
}
