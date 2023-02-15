using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Navigators
{
    public class StatementNavigator: BaseNavigator
    {
        internal StatementNavigator(BaseNavigator? parentNavigator) : base(parentNavigator)
        {
        }
    }
}
