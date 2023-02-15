using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Model;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoseLib.Exceptions;

namespace RoseLib.Composers
{
    public class ClassComposer: BaseComposer
    {
        public ClassComposer(BaseNavigator navigator) : base(navigator)
        {
        }
        public ClassComposer(ClassDeclarationSyntax? classDeclaration, BaseNavigator navigator) : base(classDeclaration, navigator)
        {
        }

        
        public ClassComposer AddField(FieldOptions options)
        {
            Navigator.AsVisitor.PopUntil(typeof(ClassDeclarationSyntax));
            var @class = (Navigator.AsVisitor.CurrentNode as ClassDeclarationSyntax)!;


            TypeSyntax type = SyntaxFactory.ParseTypeName(options.FieldType);
            var declaration = SyntaxFactory.VariableDeclaration(type,
                    SyntaxFactory.SeparatedList(new[]
                        {
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(options.FieldName))
                        }
                    )
                );

            var fieldDeclaration = SyntaxFactory.FieldDeclaration(new SyntaxList<AttributeListSyntax> { }, options.ModifiersToTokenList(), declaration);

            var newClassNode = @class.AddMembers(fieldDeclaration);
            Navigator.AsVisitor.ReplaceNodeAndAdjustState(@class, newClassNode);

            return this;
        }

        public ClassComposer UpdateField(FieldOptions options)
        {
            
            var existingFieldDeclaration = Navigator.AsVisitor.CurrentNode as FieldDeclarationSyntax;

            if(existingFieldDeclaration == null)
            {
                throw new InvalidActionForStateException("A field must be selected to update it");
            }

            TypeSyntax type = SyntaxFactory.ParseTypeName(options.FieldType);
            var declaration = SyntaxFactory.VariableDeclaration(type,
                    SyntaxFactory.SeparatedList(new[]
                        {
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(options.FieldName))
                        }
                    )
                );

            var newFieldDeclaration = SyntaxFactory.FieldDeclaration(new SyntaxList<AttributeListSyntax> { }, options.ModifiersToTokenList(), declaration);

            Navigator.AsVisitor.ReplaceNodeAndAdjustState(existingFieldDeclaration, newFieldDeclaration);

            return this;
        }

        public ClassComposer Delete()
        {

            if (Navigator.AsVisitor.CurrentNode != null)
            {
                DeleteSingleMember(Navigator.AsVisitor.CurrentNode);
            }
            else if(Navigator.AsVisitor.CurrentNodesList != null)
            {
                DeleteMultipleMembers(Navigator.AsVisitor.CurrentNodesList);
            }
            else
            {
                throw new InvalidStateException("Nothing in the state, nothing to delete");
            }


            return this;
        }

        private void DeleteSingleMember(SyntaxNode member)
        {
            Navigator.AsVisitor.State.Pop();

            var stateStepBefore = Navigator.AsVisitor.CurrentNode;
            var classParent = member.Parent as ClassDeclarationSyntax;
            if (classParent == null)
            {
                throw new InvalidActionForStateException("Cannot delete a member when not in a class.");
            }
            if (stateStepBefore == null)
            {
                throw new InvalidStateException("For some reason, only selected member was in the state");
            }
            if (stateStepBefore != classParent)
            {
                Navigator.AsVisitor.NextStep(classParent);
            }


            var newClassVersion = classParent!.RemoveNode(member, SyntaxRemoveOptions.KeepNoTrivia);
            Navigator.AsVisitor.ReplaceNodeAndAdjustState(classParent!, newClassVersion!);
        }
        private void DeleteMultipleMembers(List<SyntaxNode> members)
        {
            if(members == null || members.Count == 0)
            {
                throw new InvalidStateException("List of members in the state is empty");
            }

            Navigator.AsVisitor.State.Pop();

            var stateStepBefore = Navigator.AsVisitor.CurrentNode;


            var classParent = members[0].Parent as ClassDeclarationSyntax;
            
            if (classParent == null)
            {
                throw new InvalidActionForStateException("Cannot delete a member when not in a class.");
            }

            foreach (var member in members)
            {
                if(member.Parent != classParent)
                {
                    throw new InvalidStateException("For some reason, not all members have the same parent");
                }
            }

            if (stateStepBefore == null)
            {
                throw new InvalidStateException("For some reason, only selected field was in the state");
            }
            if (stateStepBefore != classParent)
            {
                Navigator.AsVisitor.NextStep(classParent);
            }


            var newClassVersion = classParent!.RemoveNodes(members, SyntaxRemoveOptions.KeepNoTrivia);
            Navigator.AsVisitor.ReplaceNodeAndAdjustState(classParent!, newClassVersion!);
        }
    }
}
