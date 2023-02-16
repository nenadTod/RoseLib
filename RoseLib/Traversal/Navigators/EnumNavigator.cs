using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoseLib.Traversal.Selectors.Interfaces;

namespace RoseLib.Traversal.Navigators
{
    public class EnumNavigator : BaseNavigator, IEnumMemberSelector
    {
        internal EnumNavigator(BaseNavigator? parent) : base(parent)
        {
        }
    }
}
