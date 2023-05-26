using RoseLib.CSPath.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.CSPath.Exceptions
{
    public class PathNotSupportedExeption: Exception
    {
        public PathNotSupportedExeption(PathPart part) : base($"Stuck at part: {part}") { }
    }
}
