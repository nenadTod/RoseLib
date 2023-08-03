using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Scripting.Interpreter;
using RoseLib.Enums;
using RoseLib.Exceptions;
using RoseLib.Guards;
using RoseLib.Model;
using RoseLib.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Composers
{
    public class FieldComposer : MemberComposer
    {
        public FieldComposer(IStatefulVisitor visitor, bool pivotOnParent = false) : base(visitor, pivotOnParent)
        {
        }

        #region Transition methods
        public static new bool CanProcessCurrentSelection(IStatefulVisitor statefulVisitor, bool pivotOnParent)
        {
            if (pivotOnParent)
            {
                throw new NotSupportedException("Field does not have descendants which composer can handle.");
            }
            return GenericCanProcessCurrentSelectionCheck(statefulVisitor, typeof(FieldDeclarationSyntax), SupportedScope.IMMEDIATE);
        }

        protected override void PrepareStateAndSetStatePivot(bool pivotOnParent)
        {
            if (pivotOnParent)
            {
                throw new NotSupportedException("Property does not have descendants which composer can handle.");
            }
            
            GenericPrepareStateAndSetStatePivot(typeof(FieldDeclarationSyntax), SupportedScope.IMMEDIATE);
        }
        #endregion

        #region Field change methods
        public FieldComposer Rename(string newName)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(FieldDeclarationSyntax));

            var identifier = SyntaxFactory.Identifier(newName);
            var variableDeclarator = (Visitor.CurrentNode as FieldDeclarationSyntax)!
                .DescendantNodes()
                .OfType<VariableDeclaratorSyntax>()
                .First();
            Visitor.NextStep(new SelectedObject(variableDeclarator));
            var renamedDeclarator = variableDeclarator.WithIdentifier(identifier);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, renamedDeclarator);
            Visitor.PopUntil(typeof(FieldDeclarationSyntax));

            return this;
        }

        public FieldComposer SetType(string type)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(FieldDeclarationSyntax));

            var identifier = SyntaxFactory.IdentifierName(type);
            var variableDeclaration = (Visitor.CurrentNode as FieldDeclarationSyntax)!
                .DescendantNodes()
                .OfType<VariableDeclarationSyntax>()
                .First();
            Visitor.NextStep(new SelectedObject(variableDeclaration));
            var retypedVariable = variableDeclaration.WithType(identifier);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, retypedVariable);
            Visitor.PopUntil(typeof(FieldDeclarationSyntax));

            return this;
        }

        public FieldComposer SetAccessModifier(AccessModifiers newType)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(FieldDeclarationSyntax));

            var field = (Visitor.CurrentNode as FieldDeclarationSyntax)!;
            SyntaxTokenList modifiers = field.Modifiers;
            for (int i = modifiers.Count - 1; i >= 0; i--)
            {
                var m = modifiers.ElementAt(i);
                switch (m.Kind())
                {
                    case SyntaxKind.PrivateKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.PublicKeyword:
                        modifiers = modifiers.RemoveAt(i);
                        break;
                }
            }

            switch (newType)
            {
                case AccessModifiers.NONE:
                    break;
                case AccessModifiers.PUBLIC:
                    modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                    break;
                case AccessModifiers.INTERNAL:
                    modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.InternalKeyword));
                    break;
                case AccessModifiers.PRIVATE:
                    modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
                    break;
                case AccessModifiers.PROTECTED:
                    modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
                    break;
                case AccessModifiers.PRIVATE_PROTECTED:
                    modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
                    modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
                    break;
                case AccessModifiers.PROTECTED_INTERNAL:
                    modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
                    modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.InternalKeyword));
                    break;
                default:
                    throw new NotSupportedException($"Setting {newType} as an access modifier of a method not supported");
            }

            SyntaxNode withSetModifiers = field.WithModifiers(modifiers);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, withSetModifiers);

            return this;
        }

        public FieldComposer MakeStatic()
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(FieldDeclarationSyntax));

            var field = (Visitor.CurrentNode as FieldDeclarationSyntax)!;

            SyntaxTokenList modifiers = field.Modifiers;

            if (modifiers.Where(m => m.IsKind(SyntaxKind.StaticKeyword)).Any())
            {
                return this;
            }

            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            SyntaxNode madeStatic = field.WithModifiers(modifiers);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, madeStatic);

            return this;
        }

        public FieldComposer MakeNonStatic()
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(FieldDeclarationSyntax));

            var field = (Visitor.CurrentNode as FieldDeclarationSyntax)!;
            SyntaxTokenList modifiers = field.Modifiers;
            for (int i = modifiers.Count - 1; i >= 0; i--)
            {
                var m = modifiers.ElementAt(i);
                if (m.IsKind(SyntaxKind.StaticKeyword))
                {
                    modifiers = modifiers.RemoveAt(i);
                    break;
                }
            }

            SyntaxNode madeNonStatic = field.WithModifiers(modifiers);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, madeNonStatic);

            return this;
        }

        public override FieldComposer SetAttributes(List<AttributeProps> modelAttributeList)
        {
            base.SetAttributes(modelAttributeList);

            return this;
        }

        #endregion
    }
}
