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
        const string NODE_ANNOTATION_KIND = "RoseLibNewNode";
        public IComposer ParentComposer { get; set; }
        
        public CompilationUnitComposer(StreamReader sr):base(sr)
        {
            Composer = this;
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
