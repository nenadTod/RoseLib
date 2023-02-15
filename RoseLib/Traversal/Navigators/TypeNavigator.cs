using RoseLib.Traversal.Selectors.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Navigators
{
    public class TypeNavigator: BaseNavigator, ITypeSelector, ITypeMemberSelector
    {
        internal TypeNavigator(BaseNavigator? parentNavigator): base(parentNavigator)
        {
        }

    }
}
