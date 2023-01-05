using RoseLib.Composers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Navigators
{
    public class ConstructorNavigator: BaseNavigator
    {
        internal ConstructorNavigator(BaseNavigator? parent): base(parent)
        {
        }
    }
}
