using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoseLib.CSPath.Engine;
using RoseLib.Traversal.Selectors.Interfaces;

namespace RoseLib.Traversal.Navigators
{

    public class EnumNavigator : BaseNavigator, IEnumMemberSelector
    {
        public EnumNavigator() { }

        internal EnumNavigator(BaseNavigator? parent) : base(parent)
        {
        }
    }
}
