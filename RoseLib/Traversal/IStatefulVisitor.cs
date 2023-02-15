using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Exceptions;
using RoseLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RoseLib.Traversal
{
    public interface IStatefulVisitor
    {
        const string ANNOTATION = "RoseLibAnnotation";
        public FileInfo? SourceFile { get; set; }
        public Stack<SelectedObject> State { get; protected set; }
        public SyntaxNode? CurrentNode => State.Peek()?.CurrentNode;
        public List<SyntaxNode>? CurrentNodesList => State.Peek()?.CurrentNodesList;

        internal void NextStep(SyntaxNode? node)
        {
            if (node == null)
            {
                throw new InvalidOperationException($"{GetType()}: Selection failed!");
            }

            State.Push(new SelectedObject(node));
        }

        internal void NextStep(List<SyntaxNode>? nodes)
        {
            if (nodes == null)
            {
                throw new InvalidOperationException($"{GetType()}: Selection failed!");
            }

            State.Push(new SelectedObject(nodes));
        }

        internal void SetHead(SyntaxNode node)
        {
            State.Clear();
            State.Push(new SelectedObject(node));
        }

        internal void PopUntil(Type T)
        {
            var typeInState = State
                .Where(so => so.CurrentNode != null)
                .Select(so => so.CurrentNode)
                .Where(node => node?.GetType() == T)
                .Any();

            if (!typeInState)
            {
                throw new InvalidOperationException($"Can't pop to node of type {T}, not in state");
            }
            
            while(State.Peek().CurrentNode?.GetType() != T)
            {
                State.Pop();
            }
        }


        internal CompilationUnitSyntax? GetRoot()
        {
            return State.ToList()
                .Select(so => so.CurrentNode)
                .Where(node => node is CompilationUnitSyntax)
                .Cast<CompilationUnitSyntax>()
                .FirstOrDefault();
        }

        internal List<SyntaxNode> GetAllSelectedSyntaxNodes()
        {
            List<SyntaxNode> selectedNodes = new List<SyntaxNode>();

            var allSelectedObjects = State.ToList();
            foreach(var @object in allSelectedObjects)
            {
                if(@object.CurrentNode != null)
                {
                    selectedNodes.Add(@object.CurrentNode);
                }
                else if(@object.CurrentNodesList != null)
                {
                    selectedNodes.AddRange(@object.CurrentNodesList);
                }
            }

            return selectedNodes;
        }

        internal List<SelectedObject> GetAllSelectedObjects()
        {
            var allOldSelectedObjects = State.ToList();
            allOldSelectedObjects.Reverse();
            return allOldSelectedObjects;
        }
       
        // Track all the nodes in the state, to be able to recreate the state based on the new tree version
        // Annotate the node to be replaced, to be able to find it after replacement (replacement destroys tracking info)
        // Recreate the state based on track and annotation data
        // Works even when replacing compilation unit with another compilation unit
        internal void ReplaceNodeAndAdjustState(SyntaxNode oldNode, SyntaxNode newNode)
        {
            var root = GetRoot();

            if (root == null)
            {
                throw new InvalidStateException("State not valid, compilation unit not present");
            }

            var nodesToTrack = GetAllSelectedSyntaxNodes();
            // Creates a new tree, keep that in mind.
            var TrackedRoot = root.TrackNodes(nodesToTrack);

            if (State.Peek().CurrentNode != oldNode)
            {
                throw new InvalidStateException("Updates only possible to the currently selected node");

            }

            if (oldNode.GetType() != newNode.GetType())
            {
                throw new Exception("Old and new node must be of the same type");
            }

            // Creates a new annotated node, keep that in mind.
            SyntaxAnnotation? annotation = AnnotateNode(ref newNode);

            var trackedOldNode = TrackedRoot.GetCurrentNode(oldNode)!;

            // Creates a new tree by replacing, keep that in mind.
            var newRoot = TrackedRoot.ReplaceNode(trackedOldNode, newNode);

            var newState = new Stack<SelectedObject>();

            List<SelectedObject> allOldSelectedObjects = GetAllSelectedObjects();
            foreach (var oldSelectedObject in allOldSelectedObjects)
            {
                if (oldSelectedObject.CurrentNode != null)
                {
                    if (oldSelectedObject.CurrentNode == oldNode)
                    {
                        var insertedNewNode = newRoot.GetAnnotatedNodes(annotation).First();
                        newState.Push(new SelectedObject(insertedNewNode));
                    }
                    else
                    {
                        var freshNode = newRoot.GetCurrentNode(oldSelectedObject.CurrentNode);
                        if (freshNode != null)
                        {
                            newState.Push(new SelectedObject(freshNode));
                        }
                    }

                }
                else if (oldSelectedObject.CurrentNodesList != null)
                {
                    var freshNodes = newRoot.GetCurrentNodes(oldSelectedObject.CurrentNodesList);
                    newState.Push(new SelectedObject(freshNodes.ToList()));
                }
            }

            State = newState;
        }

        SyntaxAnnotation AnnotateNode(ref SyntaxNode newNode)
        {
            string? customId;
            SyntaxAnnotation? annotation;

            if (!newNode.HasAnnotations(ANNOTATION))
            {
                customId = Guid.NewGuid().ToString();
                annotation = new SyntaxAnnotation(ANNOTATION, customId)!;
                newNode = newNode.WithAdditionalAnnotations(annotation);
            }
            else
            {
                annotation = newNode.GetAnnotations(ANNOTATION).First()!;
            }

            return annotation;
        }
    }
}
