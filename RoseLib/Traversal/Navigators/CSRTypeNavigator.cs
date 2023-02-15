using RoseLib.Traversal.Selectors.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Navigators
{
    public class CSRTypeNavigator: BaseNavigator, ITypeSelector, ITypeMemberSelector, ICSRTypeMemberSelector
    {
        internal CSRTypeNavigator(BaseNavigator? parentNavigator): base(parentNavigator)
        {
        }

        CSRTypeNavigator ITypeSelector.ToCSRTypeNavigator()
        {
            return this;
        }

    }
}
