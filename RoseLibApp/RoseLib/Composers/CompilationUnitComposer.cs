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
        
        public CompilationUnitComposer(StreamReader sr):base(sr)
        {
            Composer = this;
        }

        public void Replace(SyntaxNode oldNode, SyntaxNode newNode)
        {
            if (oldNode.GetType() != newNode.GetType())
            {
                throw new Exception("Old and new node must be of the same type");
            }

            var newRoot = CurrentNode.ReplaceNode(oldNode, newNode);
            ResetAndReplaceHead(newRoot);
        }
    }
}
