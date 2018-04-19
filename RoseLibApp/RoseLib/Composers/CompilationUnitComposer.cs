using RoseLibApp.RoseLib.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLibApp.RoseLib.Composers
{
    class CompilationUnitComposer : CompilationUnitSelector<CompilationUnitComposer>, IComposer
    {
        public IComposer ParentComposer { get; set; }
        private CompilationUnitSyntax Root { get; set; }
        
        public CompilationUnitComposer(StreamReader sr):base(sr)
        {
            Root = CurrentNode as CompilationUnitSyntax;
            Composer = this;
        }

        public void Replace(SyntaxNode oldNode, SyntaxNode newNode)
        {
            Root = Root.ReplaceNode(oldNode, newNode);
        }
    }
}
