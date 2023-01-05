using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Exceptions
{
    public class InvalidActionForStateException : Exception
    {
        public InvalidActionForStateException() : base("Action can't be performed, state not appropriate.")
        {
        }

        public InvalidActionForStateException(string message) : base(message)
        {
        }
    }
}
