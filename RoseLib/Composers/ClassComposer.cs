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

    [Serializable]
    public class ClassComposer: BaseComposer
    {
        public ClassComposer(IStatefulVisitor visitor) : base(visitor)
        {
        }
        public ClassComposer(ClassDeclarationSyntax? classDeclaration, IStatefulVisitor visitor) : base(classDeclaration, visitor)
        {
        }

        public static new bool CanProcessCurrentNode(IStatefulVisitor statefulVisitor)
        {
            SyntaxNode testNode;
            if(statefulVisitor.CurrentNode != null)
            {
                testNode = statefulVisitor.CurrentNode;
            }
            else if(statefulVisitor.CurrentNodesList != null && statefulVisitor.CurrentNodesList.Count > 0)
            {
                testNode = statefulVisitor.CurrentNodesList[0];
            }
            else
            {
                throw new InvalidStateException("No selected nodes in the state!");
            }

            if(testNode is ClassDeclarationSyntax || testNode.Parent is ClassDeclarationSyntax)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ClassComposer SetClassAttributes(List<Model.Attribute> modelAttributeList)
        {
            List<AttributeSyntax> attributeSyntaxList = new List<AttributeSyntax>();
            foreach (var attribute in modelAttributeList)
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

            Visitor.PopUntil(typeof(ClassDeclarationSyntax));
            var @class = (Visitor.CurrentNode as ClassDeclarationSyntax)!;

            
            var newClassNode = @class.AddAttributeLists(attributeList);
            Visitor.ReplaceNodeAndAdjustState(@class, newClassNode);

            return this;
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

            var navigator = BaseNavigator.CreateTempNavigator<CSRTypeNavigator>(Visitor);
            navigator.SelectFieldDeclaration(options.FieldName);

            return this;
        }

        public ClassComposer AddMethod(MethodOptions options)
        {
            Visitor.PopUntil(typeof(ClassDeclarationSyntax));
            var @class = (Visitor.CurrentNode as ClassDeclarationSyntax)!;

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

            var newClassNode = @class.AddMembers(method);
            Visitor.ReplaceNodeAndAdjustState(@class, newClassNode);

            var navigator = BaseNavigator.CreateTempNavigator<CSRTypeNavigator>(Visitor);
            navigator.SelectMethodDeclaration(options.MethodName);

            return this;
        }

        public ClassComposer AddProperty(PropertyOptions options)
        {
            Visitor.PopUntil(typeof(ClassDeclarationSyntax));
            var @class = (Visitor.CurrentNode as ClassDeclarationSyntax)!;

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

            var newClassNode = @class.AddMembers(@property);
            Visitor.ReplaceNodeAndAdjustState(@class, newClassNode);

            var navigator = BaseNavigator.CreateTempNavigator<CSRTypeNavigator>(Visitor);
            navigator.SelectPropertyDeclaration(options.PropertyName);

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
