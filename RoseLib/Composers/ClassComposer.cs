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
using RoseLib.Traversal;

namespace RoseLib.Composers
{
    public class ClassComposer: BaseComposer
    {
        public ClassComposer(IStatefulVisitor visitor) : base(visitor)
        {
        }
        public ClassComposer(ClassDeclarationSyntax? classDeclaration, IStatefulVisitor visitor) : base(classDeclaration, visitor)
        {
        }
        
        public ClassComposer AddField(FieldOptions options)
        {
            Visitor.PopUntil(typeof(ClassDeclarationSyntax));
            var @class = (Visitor.CurrentNode as ClassDeclarationSyntax)!;


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
            Visitor.ReplaceNodeAndAdjustState(@class, newClassNode);

            return this;
        }

        public ClassComposer UpdateField(FieldOptions options)
        {
            
            var existingFieldDeclaration = Visitor.CurrentNode as FieldDeclarationSyntax;

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

            Visitor.ReplaceNodeAndAdjustState(existingFieldDeclaration, newFieldDeclaration);

            return this;
        }

        public ClassComposer Delete()
        {

            if (Visitor.CurrentNode != null)
            {
                DeleteSingleMember(Visitor.CurrentNode);
            }
            else if(Visitor.CurrentNodesList != null)
            {
                DeleteMultipleMembers(Visitor.CurrentNodesList);
            }
            else
            {
                throw new InvalidStateException("Nothing in the state, nothing to delete");
            }


            return this;
        }

        private void DeleteSingleMember(SyntaxNode member)
        {
            Visitor.State.Pop();

            var stateStepBefore = Visitor.CurrentNode;
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
                Visitor.NextStep(classParent);
            }


            var newClassVersion = classParent!.RemoveNode(member, SyntaxRemoveOptions.KeepNoTrivia);
            Visitor.ReplaceNodeAndAdjustState(classParent!, newClassVersion!);
        }
        private void DeleteMultipleMembers(List<SyntaxNode> members)
        {
            if(members == null || members.Count == 0)
            {
                throw new InvalidStateException("List of members in the state is empty");
            }

            Visitor.State.Pop();

            var stateStepBefore =Visitor.CurrentNode;


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
                Visitor.NextStep(classParent);
            }


            var newClassVersion = classParent!.RemoveNodes(members, SyntaxRemoveOptions.KeepNoTrivia);
            Visitor.ReplaceNodeAndAdjustState(classParent!, newClassVersion!);
        }
    }
}
