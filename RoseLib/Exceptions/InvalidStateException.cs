using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Exceptions
{
    public class InvalidStateException: Exception
    {
        public InvalidStateException() : base() { }

        public InvalidStateException(string message) : base(message) { } 
    }
}
