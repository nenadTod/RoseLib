using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Exceptions;
using RoseLib.Model;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RoseLib.Composers
{
    public abstract class BaseComposer
    {
        public IStatefulVisitor Visitor { get; protected set; }
        public int? StatePivotIndex { get; protected set; } = -1;

        protected BaseComposer(IStatefulVisitor visitor, bool pivotOnParent)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException("Cannot create a composer without a navigator.");
            }

            Visitor = visitor;

            PrepareStateAndSetStatePivot(pivotOnParent);
        }
        
        protected abstract void PrepareStateAndSetStatePivot(bool pivotOnParent);
        
        protected void GenericPrepareStateAndSetStatePivot(Type pivotType, SupportedScope scope)
        {
            var isListSelected = false;
            SyntaxNode testNode;
            if (Visitor.CurrentNode != null)
            {
                testNode = Visitor.CurrentNode;
            }
            else if (Visitor.CurrentNodesList != null && Visitor.CurrentNodesList.Count > 0)
            {
                testNode = Visitor.CurrentNodesList[0];
                isListSelected = true;
            }
            else
            {
                throw new InvalidStateException("No selected nodes in the state!");
            }

            var testNodeType = testNode.GetType();
            if (testNodeType == pivotType && !isListSelected)
            {
                StatePivotIndex = Visitor.State.Count() - 1; // Head
            }
            else if (scope == SupportedScope.IMMEDIATE_OR_PARENT && testNode.Parent != null)
            {
                var parentNodeType = testNode.Parent.GetType();
                if (parentNodeType == pivotType)
                {
                    var indexOfParent = Visitor.GetIndexOfSelectedNode(testNode.Parent);
                    if (indexOfParent == -1)
                    {
                        Visitor.InsertBeforeHead(testNode.Parent);
                    }
                    StatePivotIndex = Visitor.State.Count() - 2; // Behind the head
                }
            }
        }

        protected void GenericPrepareStateAndSetParentAsStatePivot(Type pivotType)
        {
            SyntaxNode childNode;
            if (Visitor.CurrentNode != null)
            {
                childNode = Visitor.CurrentNode;
            }
            else if (Visitor.CurrentNodesList != null && Visitor.CurrentNodesList.Count > 0)
            {
                childNode = Visitor.CurrentNodesList[0];
            }
            else
            {
                throw new InvalidStateException("No selected nodes in the state!");
            }

            var testNode = childNode.Parent!;
            var testNodeType = testNode.GetType();
            if (testNodeType == pivotType)
            {
                var indexOfParent = Visitor.GetIndexOfSelectedNode(testNode);
                if (indexOfParent == -1)
                {
                    Visitor.InsertBeforeHead(testNode);
                }
                StatePivotIndex = Visitor.State.Count() - 2; // Behind the head
            }
            else
            {
                throw new InvalidStateException($"Can't set parent as pivot if it is not of {pivotType}");
            }
        }

        internal static bool CanProcessCurrentSelection(IStatefulVisitor statefulVisitor, bool pivotOnParent)
        {
            return false;
        }

        protected static bool GenericCanProcessCurrentSelectionCheck(IStatefulVisitor statefulVisitor, Type supportedSyntaxNodeType, SupportedScope scope)
        {
            SyntaxNode testNode;
            if (statefulVisitor.CurrentNode != null)
            {
                testNode = statefulVisitor.CurrentNode;
            }
            else if (statefulVisitor.CurrentNodesList != null && statefulVisitor.CurrentNodesList.Count > 0)
            {
                testNode = statefulVisitor.CurrentNodesList[0];
            }
            else
            {
                throw new InvalidStateException("No selected nodes in the state!");
            }

            var testNodeType = testNode.GetType();
            if (testNodeType == supportedSyntaxNodeType)
            {
                return true;
            }
            else if (scope == SupportedScope.IMMEDIATE_OR_PARENT && testNode.Parent != null)
            {
                var parentNodeType = testNode.Parent.GetType();
                if(parentNodeType == supportedSyntaxNodeType)
                {
                    return true;
                }
            }

            return false;
        }

        protected static bool GenericCanProcessCurrentSelectionParentCheck(IStatefulVisitor statefulVisitor, Type supportedSyntaxNodeType)
        {
            SyntaxNode childNode;
            if (statefulVisitor.CurrentNode != null)
            {
                childNode = statefulVisitor.CurrentNode;
            }
            else if (statefulVisitor.CurrentNodesList != null && statefulVisitor.CurrentNodesList.Count > 0)
            {
                childNode = statefulVisitor.CurrentNodesList[0];
            }
            else
            {
                throw new InvalidStateException("No selected nodes in the state!");
            }

            var testNode = childNode.Parent!;
            var testNodeType = testNode.GetType();
            if (testNodeType == supportedSyntaxNodeType)
            {
                return true;
            }

            return false;
        }
        protected enum SupportedScope
        {
            IMMEDIATE,
            IMMEDIATE_OR_PARENT,
        }

        protected void DeleteForParentNodeOfType<T>()
        {

            if (Visitor.CurrentNode != null)
            {
                DeleteSingleMember<T>();
            }
            else if (Visitor.CurrentNodesList != null)
            {
                DeleteMultipleMembers<T>();
            }
            else
            {
                throw new InvalidStateException("Nothing in the state, nothing to delete");
            }
        }

        private void DeleteSingleMember<T>()
        {
            SyntaxNode forDeletion = Visitor.CurrentNode!;
            Visitor.State.Pop();

            var stateStepBefore = Visitor.CurrentNode;
            var parent = forDeletion.Parent;
            if (parent == null)
            {
                throw new InvalidActionForStateException("Syntax node to be deleted does not have a parent.");
            }
            if (parent.GetType() != typeof(T))
            {
                throw new InvalidActionForStateException($"Cannot delete a member when parent is not of a {typeof(T)} type.");
            }
            if (stateStepBefore == null)
            {
                throw new InvalidStateException("For some reason, only selected member was in the state");
            }

            // Moving to the parent node, if not already there
            if (stateStepBefore != parent)
            {
                Visitor.NextStep(parent);
            }


            var newParentVersion = parent!.RemoveNode(forDeletion, SyntaxRemoveOptions.KeepNoTrivia);
            Visitor.ReplaceNodeAndAdjustState(parent!, newParentVersion!);
        }
        private void DeleteMultipleMembers<T>()
        {
            List<SyntaxNode> members = Visitor.CurrentNodesList!;
            if (members == null || members.Count == 0)
            {
                throw new InvalidStateException("List of members in the state is empty");
            }

            Visitor.State.Pop();

            var stateStepBefore = Visitor.CurrentNode;


            var parent = members[0].Parent as ClassDeclarationSyntax;

            if (parent == null)
            {
                throw new InvalidActionForStateException("Syntax node to be deleted does not have a parent.");
            }
            if (parent.GetType() != typeof(T))
            {
                throw new InvalidActionForStateException($"Cannot delete a member when parent is not of a {typeof(T)} type.");
            }
            foreach (var member in members)
            {
                if (member.Parent != parent)
                {
                    throw new InvalidStateException("For some reason, not all members have the same parent");
                }
            }
            if (stateStepBefore == null)
            {
                throw new InvalidStateException("For some reason, only selected field was in the state");
            }


            // Moving to the parent node, if not already there
            if (stateStepBefore != parent)
            {
                Visitor.NextStep(parent);
            }


            var newParentVersion = parent!.RemoveNodes(members, SyntaxRemoveOptions.KeepNoTrivia);
            Visitor.ReplaceNodeAndAdjustState(parent!, newParentVersion!);
        }

        public CompilationUnitNavigator StartNavigating()
        {
            Visitor.PopUntil(typeof(CompilationUnitSyntax));
            return new CompilationUnitNavigator((Visitor.CurrentNode as CompilationUnitSyntax)!);
        }

        /// <summary>
        /// Generates a textual representation of a syntax tree.
        /// Does not alter the state of the composer.
        /// </summary>
        /// <returns>syntax tree as a string</returns>
        public string GetCode()
        {
            var compilationUnit = Visitor.State
               .Where(so => so.CurrentNode is CompilationUnitSyntax)
               .Select(so => so.CurrentNode)
               .FirstOrDefault();

            if (compilationUnit == null)
            {
                throw new InvalidActionForStateException("Cannot generate textual representation if there is no compilation unit");
            }

            return compilationUnit
                .NormalizeWhitespace()
                .ToFullString();
        }

        public string GetSubtreeHashCode()
        {
            return Visitor.GetSubtreeHashCode();            
        }
        public string GetCSPath()
        {
            return Visitor.GetCSPath();
        }
    }
}
