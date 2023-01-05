using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Exceptions
{
    public class InvalidNavigatorHierarchyException: Exception
    {
        public InvalidNavigatorHierarchyException(): base(){}

        public InvalidNavigatorHierarchyException(string message):base(message){}
    }
}
