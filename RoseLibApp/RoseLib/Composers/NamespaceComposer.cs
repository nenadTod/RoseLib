using RoseLibApp.RoseLib.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace RoseLibApp.RoseLib.Composers
{
    class NamespaceComposer : NamespaceSelector<NamespaceComposer>, IComposer
    {
        public NamespaceDeclarationSyntax Root { get; set; }

        public NamespaceComposer(NamespaceDeclarationSyntax @namespace, IComposer parent):base(@namespace)
        {
            NextStep(@namespace);
            Root = @namespace;
            Composer = this;
            ParentComposer = parent;
        }

        public IComposer ParentComposer { get; set; }

        public void Replace(SyntaxNode oldNode, SyntaxNode newNode)
        {
            var newRoot = Root.ReplaceNode(oldNode, newNode);

            if (ParentComposer != null)
            {
                ParentComposer.Replace(Root, newRoot);
            }
            Root = newRoot;
        }
    }
}
