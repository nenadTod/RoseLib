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
        public ClassComposer(CSRTypeNavigator navigator) : base(navigator)
        {
        }
        public ClassComposer(ClassDeclarationSyntax? classDeclaration, CSRTypeNavigator navigator) : base(classDeclaration, navigator)
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

            Navigator.AsVisitor.PrepareForTreeUpdate();
            var newClassNode = @class.AddMembers(fieldDeclaration);
            Navigator.AsVisitor.ReplaceAndAdjustState(@class, newClassNode);

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

            Navigator.AsVisitor.PrepareForTreeUpdate();
            Navigator.AsVisitor.ReplaceAndAdjustState(existingFieldDeclaration, newFieldDeclaration);

            return this;
        }

    }
}
