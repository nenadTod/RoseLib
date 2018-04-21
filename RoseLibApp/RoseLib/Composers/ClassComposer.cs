using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLibApp.RoseLib.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLibApp.RoseLib.Composers
{
    public class ClassComposer: ClassStructSelector<ClassComposer>, IComposer
    {
        public IComposer ParentComposer { get; set; }
        
        public ClassComposer(ClassDeclarationSyntax @class, IComposer parentComposer) : base(@class)
        {
            Composer = this;
            ParentComposer = parentComposer;
        }

        public ClassComposer(ClassDeclarationSyntax @class):base(@class)
        {
            Composer = this;
        }
        
        public ClassComposer Rename(string newName)
        {
            if(!(CurrentNode is ClassDeclarationSyntax))
            {
                throw new Exception("Rename can only be called if class node is selected! Call Reset if you want to select the class node.");
            }

            var id = SyntaxFactory.Identifier(newName);
            var newNode = (CurrentNode as ClassDeclarationSyntax).WithIdentifier(id);
            newNode = RenameConstuctors(newNode, id) as ClassDeclarationSyntax;
            Replace(CurrentNode, newNode, null);

            return this;
        }

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
                if (!(oldNode is ClassDeclarationSyntax))
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
               
        private SyntaxNode RenameConstuctors(SyntaxNode root, SyntaxToken identifier)
        {
            var constructorCount = root.DescendantNodes().OfType<ConstructorDeclarationSyntax>().Count();
            var newRoot = root;

            for (var current = 0; current < constructorCount; current++)
            {
                var constructors = newRoot.DescendantNodes().OfType<ConstructorDeclarationSyntax>();
                var ctor = constructors.ElementAt(current);

                var newCtor = ctor.WithIdentifier(identifier);
                newRoot = newRoot.ReplaceNode(ctor, newCtor);
            }

            return newRoot;
        }
    }
}
