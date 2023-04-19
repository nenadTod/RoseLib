using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Selectors.Interfaces
{
    public interface ICSRTypeMemberSelector: IBaseSelector
    {
        public FieldNavigator ToFieldNavigator()
        {
            return new FieldNavigator(this as BaseNavigator);
        }

        public ConstructorNavigator ToConstructorNavigator()
        {
            return new ConstructorNavigator(this as BaseNavigator);
        }

        public DestructorNavigator ToDestructorNavigator()
        {
            return new DestructorNavigator(this as BaseNavigator);
        }

        public ConversionOperatorNavigator ToConversionOperatorNavigator()
        {
            return new ConversionOperatorNavigator(this as BaseNavigator);
        }
    }
}
