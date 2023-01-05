using RoseLib.Composers;
using RoseLib.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Navigators
{
    public class FieldNavigator: BaseNavigator
    {
        internal FieldNavigator(BaseNavigator? parentNavigator): base(parentNavigator)
        {
        }

        public ClassComposer StartComposing()
        {
            if(ParentNavigator == null || ParentNavigator.GetType() != typeof(CSRTypeNavigator))
            {
                throw new InvalidNavigatorHierarchyException("Field navigator must have class navigator as a parent.");
            }

            return new ClassComposer((ParentNavigator as CSRTypeNavigator)!);
        }
    }
}
