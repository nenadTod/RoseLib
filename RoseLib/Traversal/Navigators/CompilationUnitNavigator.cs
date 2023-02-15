using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoseLib.Composers;
using RoseLib.Traversal.Selectors.Interfaces;

namespace RoseLib.Traversal.Navigators
{
    public class CompilationUnitNavigator : BaseNavigator, INamespaceSelector, ICSRTypeSelector, IMemberSelector
    {
        public CompilationUnitNavigator() : base()
        {
            CompilationUnitSyntax cu = SyntaxFactory.CompilationUnit();
            AsVisitor.SetHead(cu);
        }
        public CompilationUnitNavigator(string path) : base(path)
        {
        }

        public CompilationUnitNavigator(StreamReader sr) : base(sr)
        {
        }

        internal static CompilationUnitNavigator CreateTempNavigator(IStatefulVisitor visitor)
        {
            var navigator = new CompilationUnitNavigator();
            navigator.AsVisitor.State = visitor.State;
            return navigator;
        }


        NamespaceNavigator INamespaceSelector.ToNamespaceNavigator()
        {
            return new NamespaceNavigator(this);
        }

        public CompilationUnitComposer StartComposing()
        {
            return new CompilationUnitComposer(this);
        }
    }
}
