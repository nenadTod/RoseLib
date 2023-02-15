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
        private NamespaceNavigator() { }
        internal NamespaceNavigator(BaseNavigator parentNavigator) : base(parentNavigator)
        {
        }
        internal static NamespaceNavigator CreateTempNavigator(IStatefulVisitor visitor)
        {
            var navigator = new NamespaceNavigator();
            navigator.AsVisitor.State = visitor.State;
            return navigator;
        }

        NamespaceNavigator INamespaceSelector.ToNamespaceNavigator()
        {
            return new NamespaceNavigator(this);
        }
    }
}
