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
    public class ClassComposer: CSRTypeComposer
    {
        public ClassComposer(IStatefulVisitor visitor) : base(visitor)
        {
        }

        public static new bool CanProcessCurrentSelection(IStatefulVisitor statefulVisitor)
        {
            return GenericCanProcessCurrentSelectionCheck(statefulVisitor, typeof(ClassDeclarationSyntax), SupporedScope.IMMEDIATE_OR_PARENT);
        }

        public override ClassComposer AddField(FieldProperties options)
        {
            return (base.AddFieldToNodeOfType<ClassDeclarationSyntax>(options) as ClassComposer)!;
        }

        public override ClassComposer AddProperty(PropertyProperties options)
        {
            return (base.AddPropertyToType<ClassDeclarationSyntax>(options) as ClassComposer)!;
        }
        public override ClassComposer AddMethod(MethodProperties options)
        {
            return (base.AddMethodToType<ClassDeclarationSyntax>(options) as ClassComposer)!;
        }

        public override ClassComposer AddClass(ClassProperties options)
        {
            return (base.AddClassToNodeOfType<ClassDeclarationSyntax>(options) as ClassComposer)!;
        }

        public override ClassComposer AddInterface(InterfaceProperties properties)
        {
            return (base.AddInterfaceToNodeOfType<ClassDeclarationSyntax>(properties) as ClassComposer)!;
        }

        public ClassComposer SetClassAttributes(List<Model.AttributeProperties> modelAttributeList)
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


        public ClassComposer UpdateField(FieldProperties options)
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
            base.DeleteForParentType(typeof(ClassDeclarationSyntax));
            return this;
        }
    }
}
