using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using RoseLib.Model;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoseLib.Guards;

namespace RoseLib.Composers
{
    public abstract class CSRTypeComposer : TypeComposer
    {
        public CSRTypeComposer(IStatefulVisitor visitor) : base(visitor)
        {
        }

        public abstract CSRTypeComposer AddField(FieldProperties options);

        protected CSRTypeComposer AddFieldToNodeOfType<T>(FieldProperties options) where T : TypeDeclarationSyntax
        {
            CompositionGuard.ImmediateOrParentOfNodeIs(Visitor.CurrentNode, typeof(T));

            TypeSyntax type = SyntaxFactory.ParseTypeName(options.FieldType);
            var declaration = SyntaxFactory.VariableDeclaration(type,
                    SyntaxFactory.SeparatedList(new[]
                        {
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(options.FieldName))
                        }
                    )
                );

            var fieldDeclaration = SyntaxFactory.FieldDeclaration(new SyntaxList<AttributeListSyntax> { }, options.ModifiersToTokenList(), declaration).NormalizeWhitespace();

            var earn = PopToEnclosingNodeOfType<T>();
            var newEnclosingNode = AddMemberToCurrentNode(fieldDeclaration, earn.referenceNode);

            Visitor.ReplaceNodeAndAdjustState(earn.enclosingNode, newEnclosingNode);
            var navigator = BaseNavigator.CreateTempNavigator<CSRTypeNavigator>(Visitor);
            navigator.SelectFieldDeclaration(options.FieldName);

            return this;
        }

        protected (T enclosingNode, MemberDeclarationSyntax? referenceNode) PopToEnclosingNodeOfType<T>() where T: TypeDeclarationSyntax
        {
            var enclosingNode = Visitor.CurrentNode!.GetType() == typeof(T) ?
            (T)Visitor.CurrentNode : (T)Visitor.CurrentNode.Parent!;
            var isAtBase = enclosingNode == Visitor.CurrentNode;
            var referenceNode = isAtBase ? null : Visitor.CurrentNode as MemberDeclarationSyntax;

            // If not at base
            if (!isAtBase)
            {
                // Pop the reference node
                Visitor.State.Pop();
                // If still not at base (Because, some other selection...)
                if(enclosingNode != Visitor.State.Peek().CurrentNode)
                {
                    // Set base as current node
                    Visitor.NextStep(enclosingNode);
                }
            }

            return (enclosingNode, referenceNode);
        }

        protected SyntaxNode AddMemberToCurrentNode(MemberDeclarationSyntax member, MemberDeclarationSyntax? referenceNode = null)
        {
            SyntaxNode newEnclosingNode;
            var dynamicNode = (Visitor.CurrentNode as dynamic)!;
            if (referenceNode == null)
            {
                newEnclosingNode = dynamicNode.AddMembers(member);
            }
            else
            {
                var currentSelection = referenceNode!;
                var currentMembers = (SyntaxList<MemberDeclarationSyntax>)dynamicNode.Members;
                var indexOfSelected = currentMembers.IndexOf(currentSelection);
                var updatedMembers = currentMembers.Insert(indexOfSelected+1, member);
                newEnclosingNode = dynamicNode.WithMembers(updatedMembers);
            }

            return newEnclosingNode;
        }
    }
}
