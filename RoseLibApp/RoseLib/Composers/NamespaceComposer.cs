using RoseLibApp.RoseLib.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using RoseLibApp.RoseLib.Model;
using RoseLibApp.RoseLib.Templates;

namespace RoseLibApp.RoseLib.Composers
{
    public class NamespaceComposer : NamespaceSelector<NamespaceComposer>, IComposer
    {
        public NamespaceComposer(NamespaceDeclarationSyntax @namespace, IComposer parent):base(@namespace)
        {
            Composer = this;
            ParentComposer = parent;
        }

        public IComposer ParentComposer { get; set; }

        public List<SyntaxNode> Replace(SyntaxNode oldNode, SyntaxNode newNode, List<SyntaxNode> nodesToTrack)
        {
            if (oldNode.GetType() != newNode.GetType())
            {
                throw new Exception("Old and new node must be of the same type");
            }

            var trackedNodes = new List<SyntaxNode>();

            if (nodesToTrack != null)
            {
                trackedNodes.AddRange(nodesToTrack);
            }

            Reset();

            var newRoot = CurrentNode;

            if (ParentComposer != null)
            {
                trackedNodes.Add(CurrentNode);
                trackedNodes = ParentComposer.Replace(oldNode, newNode, trackedNodes);
                var tempNode = trackedNodes.LastOrDefault();

                if (tempNode != null)
                {
                    newRoot = tempNode;
                    trackedNodes.Remove(newRoot);
                }
            }
            else
            {
                if (!(oldNode is NamespaceDeclarationSyntax))
                {
                    newRoot = newRoot.ReplaceNode(oldNode, newNode);
                }
                else
                {
                    newRoot = newNode;
                }
            }

            ReplaceHead(newRoot);
            return trackedNodes;
        }

        public NamespaceComposer Delete()
        {
            var nodeForRemoval = CurrentNode;
            Reset();

            var @namespace = CurrentNode;

            if (@namespace == nodeForRemoval)
            {
                throw new Exception("Root of the composer cannot be deleted. Deletion can be done using parent selector.");
            }
            if (nodeForRemoval == null)
            {
                throw new Exception("You cannot perform delete operation when the value of the current node is null.");

            }

            var newClass = @namespace.RemoveNode(nodeForRemoval, SyntaxRemoveOptions.KeepExteriorTrivia);
            Replace(@namespace, newClass, null);

            return this;
        }

        public NamespaceComposer AddClass(ClassOptions options)
        {
            if (!IsAtRoot())
            {
                throw new Exception("The namespace must be selected (which is root to the composer) to add a class to it.");
            }

            var template = new CreateClass() { Options = options };
            var code = template.TransformText();
            var cu = SyntaxFactory.ParseCompilationUnit(code).NormalizeWhitespace();
            var newClass = cu.DescendantNodes().OfType<ClassDeclarationSyntax>().First();

            var @namespace = CurrentNode as NamespaceDeclarationSyntax;
            @namespace = @namespace.AddMembers(newClass);

            Replace(CurrentNode, @namespace, null);

            return this;
        }
    }
}
