using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Selectors.Interfaces
{
    public interface ITypeMemberSelector: IBaseSelector
    {
        public PropertyNavigator ToPropertyNavigator()
        {
            return new PropertyNavigator(this as BaseNavigator);
        }

        public MethodNavigator ToMethodNavigator()
        {
            return new MethodNavigator(this as BaseNavigator);
        }

        public OperatorNavigator ToOperatorNavigator()
        {
            return new OperatorNavigator(this as BaseNavigator);
        }
    }
}
