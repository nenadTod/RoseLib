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
    internal class CompilationUnitHandler : BaseHandler
    {
        public CompilationUnitHandler() { InitializeForType(typeof(CompilationUnitNavigator)); }
        internal override void HandleDescent(Context context)
        {
            if(context == null) { throw new ArgumentNullException("context"); }

            if(!(context.Visitor is CompilationUnitNavigator))
            {
                if(NextHandler!= null) { NextHandler.HandleDescent(context); }
                else { throw new PathNotSupportedExeption(context.PathPart); }
            }
            else
            {
                Descend(context.Visitor, typeof(CompilationUnitNavigator), context);
            }
        }
    }
}
