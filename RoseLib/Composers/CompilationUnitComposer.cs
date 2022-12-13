using RoseLib.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace RoseLib.Composers
{
    class CompilationUnitComposer : CompilationUnitSelector<CompilationUnitComposer>, IComposer
    {
        const string NODE_ANNOTATION_KIND = "RoseLibNewNode";
        public IComposer ParentComposer { get; set; }
        
        public CompilationUnitComposer(StreamReader sr):base(sr)
        {
            Composer = this;
        }

        public CompilationUnitComposer()
        {
            Composer = this;

            CompilationUnitSyntax cu = SyntaxFactory.CompilationUnit();
            SetHead(cu);
        }

        public CompilationUnitComposer AddUsing(string @namespace)
        {
            if (!IsAtRoot())
            {
                throw new Exception("You must be positioned at a compilation unit (which is root to the composer) to add a using to it.");
            }

            var cu = CurrentNode as CompilationUnitSyntax;
            var newCu = cu.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(@namespace)));

            Replace(cu, newCu, null);
            return this;
        }

        public CompilationUnitComposer AddNamespace(string @namespace)
        {
            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(@namespace));

            if (!IsAtRoot())
            {
                throw new Exception("You must be positioned at a compilation unit (which is root to the composer) to add a namespace to it.");
            }

            var cu = CurrentNode as CompilationUnitSyntax;

            var newCu = cu.AddMembers(namespaceDeclaration);
            Replace(cu, newCu, null);

            return this;
        }

        public CompilationUnitComposer Delete()
        {
            var nodeForRemoval = CurrentNode;
            Reset();

            var cu = CurrentNode;

            if (cu == nodeForRemoval)
            {
                throw new Exception("Root of the composer cannot be deleted. Deletion can be done using parent selector.");
            }
            if (nodeForRemoval == null)
            {
                throw new Exception("You cannot perform delete operation when the value of the current node is null.");

            }

            var newCU = cu.RemoveNode(nodeForRemoval, SyntaxRemoveOptions.KeepExteriorTrivia);
            Replace(cu, newCU, null);

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

            trackedNodes.Add(oldNode);
            
            var newRoot = CurrentNode.TrackNodes(trackedNodes);
            trackedNodes.Remove(oldNode);

            string customId = null;
            SyntaxAnnotation annotation = null;

            if (!newNode.HasAnnotations(NODE_ANNOTATION_KIND))
            {
                customId = Guid.NewGuid().ToString();
                annotation = new SyntaxAnnotation(NODE_ANNOTATION_KIND, customId);
                newNode = newNode.WithAdditionalAnnotations(annotation);
            }
            else
            {
                annotation = newNode.GetAnnotations(NODE_ANNOTATION_KIND).FirstOrDefault();
                customId = annotation.Data;
            }
          
            newRoot = newRoot.ReplaceNode(newRoot.GetCurrentNode(oldNode), newNode);    
            var annotatedNode = newRoot.GetAnnotatedNodes(annotation).First();
            
            ReplaceHead(newRoot);
            var currentNodes = newRoot.GetCurrentNodes<SyntaxNode>(trackedNodes).ToList();
            currentNodes.Insert(0, annotatedNode);

            return currentNodes;
        }
    }
}
