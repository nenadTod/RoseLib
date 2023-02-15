using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Exceptions
{
    internal class InvalidCastException : Exception
    {
        public InvalidCastException() : base("Cannot cast to provided type")
        {
        }

        public InvalidCastException(string message) : base(message)
        {
        }
    }
}
