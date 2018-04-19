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
        public NamespaceComposer(NamespaceDeclarationSyntax @namespace, IComposer parent):base(@namespace)
        {
            Composer = this;
            ParentComposer = parent;
        }

        public IComposer ParentComposer { get; set; }

        public void Replace(SyntaxNode oldNode, SyntaxNode newNode)
        {
            if (oldNode.GetType() != newNode.GetType())
            {
                throw new Exception("Old and new node must be of the same type");
            }

            Reset();

            var newRoot = CurrentNode;

            if (!(oldNode is NamespaceDeclarationSyntax))
            {
                newRoot = CurrentNode.ReplaceNode(oldNode, newNode);
            }
            else
            {
                newRoot = newNode;
            }

            if (ParentComposer != null)
            {
                ParentComposer.Replace(CurrentNode, newRoot);
            }

            ReplaceHead(newRoot);
        }
    }
}
