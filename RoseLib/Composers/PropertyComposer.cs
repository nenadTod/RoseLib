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
    public partial class PropertyComposer : MemberComposer
    {
        public PropertyComposer(IStatefulVisitor visitor, bool pivotOnParent = false) : base(visitor, pivotOnParent)
        {
        }

        #region Transition methods
        public static new bool CanProcessCurrentSelection(IStatefulVisitor statefulVisitor, bool pivotOnParent)
        {
            if (pivotOnParent)
            {
                throw new NotSupportedException("Property does not have descendants which composer can handle.");
            }
            return GenericCanProcessCurrentSelectionCheck(statefulVisitor, typeof(PropertyDeclarationSyntax), SupportedScope.IMMEDIATE);
        }

        protected override void PrepareStateAndSetStatePivot(bool pivotOnParent)
        {
            if (pivotOnParent)
            {
                throw new NotSupportedException("Property does not have descendants which composer can handle.");
            }
            
            GenericPrepareStateAndSetStatePivot(typeof(PropertyDeclarationSyntax), SupportedScope.IMMEDIATE);
        }
        #endregion

        #region Property change methods
        public PropertyComposer Rename(string newName)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(PropertyDeclarationSyntax));

            var identifier = SyntaxFactory.Identifier(newName);
            var renamedProperty = (Visitor.CurrentNode as PropertyDeclarationSyntax)!.WithIdentifier(identifier);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, renamedProperty);

            return this;
        }

        public PropertyComposer SetType(string type)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(PropertyDeclarationSyntax));

            var identifier = SyntaxFactory.IdentifierName(type);
            var property = (Visitor.CurrentNode as PropertyDeclarationSyntax)!.WithType(identifier);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, property);

            return this;
        }

        public PropertyComposer SetAccessModifier(AccessModifiers newType)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(PropertyDeclarationSyntax));

            var property = (Visitor.CurrentNode as PropertyDeclarationSyntax)!;
            SyntaxTokenList modifiers = property.Modifiers;
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

            SyntaxNode withSetModifiers = property.WithModifiers(modifiers);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, withSetModifiers);

            return this;
        }

        public PropertyComposer MakeStatic()
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(PropertyDeclarationSyntax));

            var property = (Visitor.CurrentNode as PropertyDeclarationSyntax)!;

            SyntaxTokenList modifiers = property.Modifiers;

            if (modifiers.Where(m => m.IsKind(SyntaxKind.StaticKeyword)).Any())
            {
                return this;
            }

            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            SyntaxNode madeStatic = property.WithModifiers(modifiers);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, madeStatic);

            return this;
        }

        public PropertyComposer MakeNonStatic()
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(PropertyDeclarationSyntax));

            var property = (Visitor.CurrentNode as PropertyDeclarationSyntax)!;
            SyntaxTokenList modifiers = property.Modifiers;
            for (int i = modifiers.Count - 1; i >= 0; i--)
            {
                var m = modifiers.ElementAt(i);
                if (m.IsKind(SyntaxKind.StaticKeyword))
                {
                    modifiers = modifiers.RemoveAt(i);
                    break;
                }
            }

            SyntaxNode madeNonStatic = property.WithModifiers(modifiers);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, madeNonStatic);

            return this;
        }

        public override PropertyComposer SetAttributes(List<AttributeProps> modelAttributeList)
        {
            base.SetAttributes(modelAttributeList);

            return this;
        }

        #endregion

        #region Child navigation

        public BlockComposer EnterGetBody()
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(PropertyDeclarationSyntax));

            var property = Visitor.State.Peek().CurrentNode as PropertyDeclarationSyntax;
            if (property == null)
            {
                throw new InvalidActionForStateException("Entering body possible when positioned on a method declaration syntax instance.");
            }

            var getAccessor = property
                .DescendantNodes()
                .Where(node => node.Kind() == SyntaxKind.GetAccessorDeclaration)
                .FirstOrDefault() as AccessorDeclarationSyntax;

            if (getAccessor == null)
            {
                throw new InvalidOperationException("Cannot enter the get accessor.");
            }

            if (getAccessor.Body == null)
            {
                throw new InvalidOperationException("Cannot enter a body of a bodyless get accessor.");
            }

            Visitor.NextStep(new SelectedObject(getAccessor.Body));

            return new BlockComposer(Visitor);
        }

        public BlockComposer EnterSetBody()
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(PropertyDeclarationSyntax));

            var property = Visitor.State.Peek().CurrentNode as PropertyDeclarationSyntax;
            if (property == null)
            {
                throw new InvalidActionForStateException("Entering body possible when positioned on a method declaration syntax instance.");
            }

            var setAccessor = property
                .DescendantNodes()
                .Where(node => node.Kind() == SyntaxKind.SetAccessorDeclaration)
                .FirstOrDefault() as AccessorDeclarationSyntax;

            if (setAccessor == null)
            {
                throw new InvalidOperationException("Cannot enter the get accessor.");
            }

            if (setAccessor.Body == null)
            {
                throw new InvalidOperationException("Cannot enter a body of a bodyless set accessor.");
            }

            Visitor.NextStep(new SelectedObject(setAccessor.Body));

            return new BlockComposer(Visitor);
        }

        #endregion
    }
}
