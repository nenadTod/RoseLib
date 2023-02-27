using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Exceptions
{
    public class InvalidUsageException: Exception
    {
        public InvalidUsageException() : base() { }

        public InvalidUsageException(string message) : base(message) { } 
    }
}
