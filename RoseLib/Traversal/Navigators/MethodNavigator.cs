using RoseLib.Traversal.Selectors.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Navigators
{
    public class MethodNavigator: BaseNavigator, IBodySelector
    {
        public MethodNavigator(BaseNavigator? parent): base(parent)
        {
        }
    }
}
