using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Navigators
{
    public class DestructorNavigator: BaseNavigator
    {
        public DestructorNavigator(BaseNavigator? parent): base(parent)
        {
        }
    }
}
