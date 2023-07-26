using RoseLib.Traversal.Selectors.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Navigators
{
    public class BlockNavigator: BaseNavigator, IBodySelector
    {
        public BlockNavigator() { }
        internal BlockNavigator(BaseNavigator? parent): base(parent)
        {
        }
    }
}
