using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Navigators
{
    public class PropertyNavigator: BaseNavigator
    {
        internal PropertyNavigator(BaseNavigator? parentNavigator) : base(parentNavigator)
        {
        }
    }
}
