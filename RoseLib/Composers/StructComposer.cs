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
using RoseLib.Enums;
using RoseLib.Guards;

namespace RoseLib.Composers
{
    public partial class StructComposer: CSRTypeComposer
    {
        internal StructComposer(IStatefulVisitor visitor, bool pivotOnParent = false) : base(visitor, pivotOnParent)
        {
        }

        #region Transition methods
        public static new bool CanProcessCurrentSelection(IStatefulVisitor statefulVisitor, bool pivotOnParent)
        {
            if(!pivotOnParent) 
            {
                return GenericCanProcessCurrentSelectionCheck(statefulVisitor, typeof(StructDeclarationSyntax), SupportedScope.IMMEDIATE_OR_PARENT);
            }
            else
            {
                return GenericCanProcessCurrentSelectionParentCheck(statefulVisitor, typeof(StructDeclarationSyntax));
            }
        }

        protected override void PrepareStateAndSetStatePivot(bool pivotOnParent)
        {
            if (!pivotOnParent)
            {
                GenericPrepareStateAndSetStatePivot(typeof(StructDeclarationSyntax), SupportedScope.IMMEDIATE_OR_PARENT);
            }
            else
            {
                GenericPrepareStateAndSetParentAsStatePivot(typeof(StructDeclarationSyntax));
            }
        }
        #endregion

        #region Addition methods
        public override StructComposer AddField(FieldProps options)
        {
            return (base.AddFieldToNodeOfType<StructDeclarationSyntax>(options) as StructComposer)!;
        }

        public override StructComposer AddProperty(PropertyProps options)
        {
            return (base.AddPropertyToType<StructDeclarationSyntax>(options) as StructComposer)!;
        }
        public override StructComposer AddMethod(MethodProps options)
        {
            return (base.AddMethodToType<StructDeclarationSyntax>(options) as StructComposer)!;
        }

        #endregion

        #region Struct change methods
        public StructComposer Rename(string newName)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(StructDeclarationSyntax));

            var identifier = SyntaxFactory.Identifier(newName);
            var renamedStruct = (Visitor.CurrentNode as StructDeclarationSyntax)!.WithIdentifier(identifier);
            var withAdjustedConstructors = RenameConstuctors(renamedStruct, identifier) as StructDeclarationSyntax;
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, withAdjustedConstructors!);

            return this;
        }
        private SyntaxNode RenameConstuctors(SyntaxNode @struct, SyntaxToken identifier)
        {
            // TODO: Rely on selector methods to find all the constructors.
            var constructorCount = @struct.DescendantNodes().OfType<ConstructorDeclarationSyntax>().Count();
            var newRoot = @struct;

            for (var current = 0; current < constructorCount; current++)
            {
                var constructors = newRoot.DescendantNodes().OfType<ConstructorDeclarationSyntax>();
                var ctor = constructors.ElementAt(current);

                var newCtor = ctor.WithIdentifier(identifier);
                newRoot = newRoot.ReplaceNode(ctor, newCtor);
            }

            return newRoot;
        }

        public StructComposer SetAccessModifier(AccessModifiers newType)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(StructDeclarationSyntax));

            var @class = (Visitor.CurrentNode as StructDeclarationSyntax)!;
            SyntaxTokenList modifiers = @class.Modifiers;
            for (int i = modifiers.Count - 1; i >= 0; i--)
            {
                var m = modifiers.ElementAt(i);
                switch (m.Kind())
                {
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.PublicKeyword:
                        modifiers = modifiers.RemoveAt(i);
                        break;
                }
            }

            switch (newType)
            {
                case AccessModifiers.PUBLIC:
                    modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                    break;
                case AccessModifiers.INTERNAL:
                    modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.InternalKeyword));
                    break;
                case AccessModifiers.NONE:
                    break;
                case AccessModifiers.PRIVATE:
                case AccessModifiers.PROTECTED:
                case AccessModifiers.PRIVATE_PROTECTED:
                case AccessModifiers.PROTECTED_INTERNAL:
                    throw new NotSupportedException($"Setting {newType} as an access modifier of a class not supported");
            }

            SyntaxNode withSetModifiers = @class.WithModifiers(modifiers);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, withSetModifiers);

            return this;
        }


        #endregion

        public StructComposer Delete()
        {
            base.DeleteForParentNodeOfType<StructDeclarationSyntax>();
            return this;
        }

    }
}
