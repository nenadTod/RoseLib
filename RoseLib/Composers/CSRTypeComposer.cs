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

namespace RoseLib.Composers
{
    public abstract class CSRTypeComposer : TypeComposer
    {
        public CSRTypeComposer(IStatefulVisitor visitor) : base(visitor)
        {
        }

        public virtual CSRTypeComposer AddField(FieldProperties options)
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
    }
}
