using RoseLib.CSPath.Engine;
using RoseLib.Traversal.Selectors.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Navigators
{
    public class NamespaceNavigator : BaseNavigator, INamespaceSelector, ITypeSelector, ITypeMemberSelector, ICSRTypeMemberSelector
    {
        public NamespaceNavigator() { }
        internal NamespaceNavigator(BaseNavigator parentNavigator) : base(parentNavigator)
        {
        }

        NamespaceNavigator INamespaceSelector.ToNamespaceNavigator()
        {
            return new NamespaceNavigator(this);
        }
    }
}
