using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Exceptions
{
    public class CodeHasErrorsException : Exception
    {
        public CodeHasErrorsException() { }

        public CodeHasErrorsException(string message) : base(message) { }
    }
}
