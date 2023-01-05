using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal.Selectors.Interfaces
{
    public interface INamespaceSelector: IBaseSelector
    {
        internal NamespaceNavigator ToNamespaceNavigator();
    }
}
