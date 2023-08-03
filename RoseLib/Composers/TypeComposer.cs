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
    public abstract class TypeComposer : MemberComposer
    {
        internal TypeComposer(IStatefulVisitor visitor, bool pivotOnParent) : base(visitor, pivotOnParent)
        {
        }

        public abstract TypeComposer AddMethod(MethodProps options);
        protected abstract bool CanHaveBodylessMethod();
        public virtual TypeComposer AddMethodToType<T>(MethodProps props) where T : TypeDeclarationSyntax
        {
            CompositionGuard.NodeOrParentIs(Visitor.CurrentNode, typeof(T));

            List<AttributeSyntax> attributeSyntaxList = new List<AttributeSyntax>();
            foreach (var attribute in props.Attributes)
            {
                AttributeArgumentListSyntax? attributeArgumentListSyntax = null;
                if (attribute.AttributeArgumentsAsString != null)
                {
                    var parameterToBePassed = attribute.AttributeArgumentsAsString;
                    attributeArgumentListSyntax = SyntaxFactory.ParseAttributeArgumentList(parameterToBePassed, 0, CSharpParseOptions.Default, false);
                }

                var attributeSyntax = SyntaxFactory.Attribute(SyntaxFactory.ParseName(attribute.Name), attributeArgumentListSyntax);
                attributeSyntaxList.Add(attributeSyntax);
            }

            var attributeList = SyntaxFactory.AttributeList(new SeparatedSyntaxList<AttributeSyntax>().AddRange(attributeSyntaxList));

            TypeSyntax returnType = SyntaxFactory.ParseTypeName(props.ReturnType);
            var method = SyntaxFactory.MethodDeclaration(returnType, props.MethodName)
                .WithModifiers(props.ModifiersToTokenList());

            if (props.Attributes != null &&  props.Attributes.Count > 0)
            {
                method = method.AddAttributeLists(attributeList);
            }

            var @params = SyntaxFactory.ParameterList();
            foreach (var param in props.Params)
            {
                var type = SyntaxFactory.IdentifierName(param.Type);
                var name = SyntaxFactory.Identifier(param.Name);
                var paramSyntax = SyntaxFactory
                    .Parameter(new SyntaxList<AttributeListSyntax>(), SyntaxFactory.TokenList(), type, name, null);
                @params = @params.AddParameters(paramSyntax);
            }
            @params = @params.NormalizeWhitespace();
            method = method.WithParameterList(@params);

            if (!props.BodylessMethod)
            {
                method = method.WithBody(SyntaxFactory.Block());
            }
            else if(CanHaveBodylessMethod())
            {
                method = method.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            }
            else
            {
                throw new NotSupportedException("Cannot create a method without a body.");
            }

            var referenceNode = TryGetReferenceAndPopToPivot();
            var newEnclosingNode = AddMemberToCurrentNode(method, referenceNode);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, newEnclosingNode);

            var navigator = BaseNavigator.CreateTempNavigator<CSRTypeNavigator>(Visitor);
            navigator.SelectMethodDeclaration(props.MethodName);

            return this;
        }

        public abstract TypeComposer AddProperty(PropertyProps options);

        public virtual TypeComposer AddPropertyToType<T>(PropertyProps options) where T: TypeDeclarationSyntax
        {
            CompositionGuard.NodeOrParentIs(Visitor.CurrentNode, typeof(T));

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

            var referenceNode = TryGetReferenceAndPopToPivot();
            var newEnclosingNode = AddMemberToCurrentNode(@property, referenceNode);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, newEnclosingNode);

            var navigator = BaseNavigator.CreateTempNavigator<CSRTypeNavigator>(Visitor);
            navigator.SelectPropertyDeclaration(options.PropertyName);

            return this;
        }

        protected SyntaxNode AddMemberToCurrentNode(MemberDeclarationSyntax member, MemberDeclarationSyntax? referenceNode = null)
        {
            SyntaxNode newEnclosingNode;
            var typeNode = (Visitor.CurrentNode as TypeDeclarationSyntax)!;
            if (referenceNode == null)
            {
                newEnclosingNode = typeNode.AddMembers(member);
            }
            else
            {
                var currentSelection = referenceNode!;
                var currentMembers = typeNode.Members;
                var indexOfSelected = currentMembers.IndexOf(currentSelection);
                if (indexOfSelected == -1)
                {
                    throw new InvalidStateException("For some reason, reference node not found in members.");
                }
                var updatedMembers = currentMembers.Insert(indexOfSelected + 1, member);
                newEnclosingNode = typeNode.WithMembers(updatedMembers);
            }

            return newEnclosingNode;
        }

        #region Child navigation methods
        public MethodComposer EnterMethod()
        {
            var method = Visitor.State.Peek().CurrentNode as MethodDeclarationSyntax;
            if (method == null)
            {
                throw new InvalidActionForStateException("Entering methods only possible when positioned on a method declaration syntax instance.");
            }

            return new MethodComposer(Visitor);
        }

        public PropertyComposer EnterProperty()
        {
            var method = Visitor.State.Peek().CurrentNode as PropertyDeclarationSyntax;
            if (method == null)
            {
                throw new InvalidActionForStateException("Entering properties only possible when positioned on a property declaration syntax instance.");
            }

            return new PropertyComposer(Visitor);
        }
        #endregion
    }
}
