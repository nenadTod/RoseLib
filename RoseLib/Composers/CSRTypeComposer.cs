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
using RoseLib.Exceptions;

namespace RoseLib.Composers
{
    public abstract class CSRTypeComposer : TypeComposer
    {
        internal CSRTypeComposer(IStatefulVisitor visitor, bool pivotOnParent) : base(visitor, pivotOnParent)
        {
        }

        protected override bool CanHaveBodylessMethod()
        {
            return false;
        }

        public abstract CSRTypeComposer AddField(FieldProps options);

        protected CSRTypeComposer AddFieldToNodeOfType<T>(FieldProps options) where T : TypeDeclarationSyntax
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

            var referenceNode = TryGetReferenceAndPopToPivot();
            var newEnclosingNode = AddMemberToCurrentNode(fieldDeclaration, referenceNode);

            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, newEnclosingNode);
            var navigator = BaseNavigator.CreateTempNavigator<CSRTypeNavigator>(Visitor);
            navigator.SelectFieldDeclaration(options.FieldName);

            return this;
        }
        public FieldComposer EnterField()
        {
            var field = Visitor.State.Peek().CurrentNode as FieldDeclarationSyntax;
            if (field == null)
            {
                throw new InvalidActionForStateException("Entering fields only possible when positioned on a field declaration syntax instance.");
            }

            return new FieldComposer(Visitor);
        }
    }
}
