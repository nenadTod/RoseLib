using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.CSPath.Exceptions;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.CSPath.Engine.CoR
{
    internal class NamespaceHandler : BaseHandler
    {
        internal NamespaceHandler() { 
            InitializeForType(typeof(NamespaceNavigator)); 
        }
        internal override void HandleDescent(Context context)
        {
            HandleDescendForNavigator<NamespaceNavigator>(context);
        }
    }
}
