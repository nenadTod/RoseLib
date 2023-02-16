using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Selectors.Interfaces
{
    public interface ITypeSelector: IBaseSelector
    {
        internal CSRTypeNavigator ToCSRTypeNavigator()
        {
            return new CSRTypeNavigator(this as BaseNavigator);
        }

        internal TypeNavigator ToTypeNavigator()
        {
            return new TypeNavigator(this as BaseNavigator);
        }

        internal EnumNavigator ToEnumNavigator()
        {
            return new EnumNavigator(this as BaseNavigator);
        }

    }
}
