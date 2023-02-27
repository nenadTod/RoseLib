using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using RoseLib.Model;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoseLib.Exceptions;
using RoseLib.Guards;

namespace RoseLib.Composers
{
    public abstract class TypeComposer : TypeContainerComposer
    {
        public TypeComposer(IStatefulVisitor visitor) : base(visitor)
        {
        }

        public abstract TypeComposer AddMethod(MethodProperties options);

        public virtual TypeComposer AddMethodToType<T>(MethodProperties options) where T : TypeDeclarationSyntax
        {
            CompositionGuard.ImmediateOrParentOfNodeIs(Visitor.CurrentNode, typeof(T));

            TypeSyntax returnType = SyntaxFactory.ParseTypeName(options.ReturnType);
            var method = SyntaxFactory.MethodDeclaration(returnType, options.MethodName).WithModifiers(options.ModifiersToTokenList());

            var @params = SyntaxFactory.ParameterList();
            foreach (var param in options.Parameters)
            {
                var type = SyntaxFactory.IdentifierName(param.Type);
                var name = SyntaxFactory.Identifier(param.Name);
                var paramSyntax = SyntaxFactory
                    .Parameter(new SyntaxList<AttributeListSyntax>(), SyntaxFactory.TokenList(), type, name, null);
                @params = @params.AddParameters(paramSyntax);
            }
            @params = @params.NormalizeWhitespace();
            method = method.WithParameterList(@params);

            method = method.WithBody(SyntaxFactory.Block());

            var earn = PopToEnclosingNodeOfType<T>();
            var newEnclosingNode = AddMemberToCurrentNode(method, earn.referenceNode);
            Visitor.ReplaceNodeAndAdjustState(earn.enclosingNode, newEnclosingNode);

            var navigator = BaseNavigator.CreateTempNavigator<CSRTypeNavigator>(Visitor);
            navigator.SelectMethodDeclaration(options.MethodName);

            return this;
        }

        public abstract TypeComposer AddProperty(PropertyProperties options);

        public virtual TypeComposer AddPropertyToType<T>(PropertyProperties options) where T: TypeDeclarationSyntax
        {
            CompositionGuard.ImmediateOrParentOfNodeIs(Visitor.CurrentNode, typeof(T));

            PropertyDeclarationSyntax @property = SyntaxFactory
                .PropertyDeclaration(SyntaxFactory.ParseTypeName(options.PropertyType), options.PropertyName)
                .WithModifiers(options.ModifiersToTokenList());

            @property = @property.AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                    ));
            @property = @property.AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                ));

            var earn = PopToEnclosingNodeOfType<T>();
            var newEnclosingNode = AddMemberToCurrentNode(@property, earn.referenceNode);
            Visitor.ReplaceNodeAndAdjustState(earn.enclosingNode, newEnclosingNode);

            var navigator = BaseNavigator.CreateTempNavigator<CSRTypeNavigator>(Visitor);
            navigator.SelectPropertyDeclaration(options.PropertyName);

            return this;
        }

        protected (T enclosingNode, MemberDeclarationSyntax? referenceNode) PopToEnclosingNodeOfType<T>() where T : TypeDeclarationSyntax
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
                if (enclosingNode != Visitor.State.Peek().CurrentNode)
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
                if (indexOfSelected == -1)
                {
                    throw new InvalidStateException("For some reason, reference node not found in members.");
                }
                var updatedMembers = currentMembers.Insert(indexOfSelected + 1, member);
                newEnclosingNode = dynamicNode.WithMembers(updatedMembers);
            }

            return newEnclosingNode;
        }
    }
}
