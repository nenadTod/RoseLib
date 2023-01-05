using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Selectors.Interfaces
{
    public interface ICSRTypeSelector: IBaseSelector
    {
        internal CSRTypeNavigator ToCSRTypeNavigator()
        {
            return new CSRTypeNavigator(this as BaseNavigator);
        }
    }
}
