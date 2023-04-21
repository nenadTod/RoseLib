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

    [Serializable]
    public class ClassComposer: CSRTypeComposer
    {
        internal ClassComposer(IStatefulVisitor visitor, bool pivotOnParent = false) : base(visitor, pivotOnParent)
        {
        }

        #region Transition methods
        public static new bool CanProcessCurrentSelection(IStatefulVisitor statefulVisitor, bool pivotOnParent)
        {
            if(!pivotOnParent) 
            {
                return GenericCanProcessCurrentSelectionCheck(statefulVisitor, typeof(ClassDeclarationSyntax), SupportedScope.IMMEDIATE_OR_PARENT);
            }
            else
            {
                return GenericCanProcessCurrentSelectionParentCheck(statefulVisitor, typeof(ClassDeclarationSyntax));
            }
        }

        protected override void PrepareStateAndSetStatePivot(bool pivotOnParent)
        {
            if (!pivotOnParent)
            {
                GenericPrepareStateAndSetStatePivot(typeof(ClassDeclarationSyntax), SupportedScope.IMMEDIATE_OR_PARENT);
            }
            else
            {
                GenericPrepareStateAndSetParentAsStatePivot(typeof(ClassDeclarationSyntax));
            }
        }
        #endregion

        #region Addition methods
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
        #endregion

        #region Class change methods
        public ClassComposer Rename(string newName)
        {
            CompositionGuard.ImmediateNodeIs(Visitor.CurrentNode, typeof(ClassDeclarationSyntax));

            var identifier = SyntaxFactory.Identifier(newName);
            var renamedClass = (Visitor.CurrentNode as ClassDeclarationSyntax)!.WithIdentifier(identifier);
            var withAdjustedConstructors = RenameConstuctors(renamedClass, identifier) as ClassDeclarationSyntax;
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, withAdjustedConstructors!);

            return this;
        }
        private SyntaxNode RenameConstuctors(SyntaxNode @class, SyntaxToken identifier)
        {
            // TODO: Rely on selector methods to find all the constructors.
            var constructorCount = @class.DescendantNodes().OfType<ConstructorDeclarationSyntax>().Count();
            var newRoot = @class;

            for (var current = 0; current < constructorCount; current++)
            {
                var constructors = newRoot.DescendantNodes().OfType<ConstructorDeclarationSyntax>();
                var ctor = constructors.ElementAt(current);

                var newCtor = ctor.WithIdentifier(identifier);
                newRoot = newRoot.ReplaceNode(ctor, newCtor);
            }

            return newRoot;
        }

        public ClassComposer SetAccessModifier(AccessModifierTypes newType)
        {
            CompositionGuard.ImmediateNodeIs(Visitor.CurrentNode, typeof(ClassDeclarationSyntax));

            var @class = (Visitor.CurrentNode as ClassDeclarationSyntax)!;
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
                case AccessModifierTypes.PUBLIC:
                    modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                    break;
                case AccessModifierTypes.INTERNAL:
                    modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.InternalKeyword));
                    break;
                case AccessModifierTypes.NONE:
                    break;
                case AccessModifierTypes.PRIVATE:
                case AccessModifierTypes.PROTECTED:
                case AccessModifierTypes.PRIVATE_PROTECTED:
                case AccessModifierTypes.PROTECTED_INTERNAL:
                    throw new NotSupportedException($"Setting {newType} as an access modifier not supported");
            }

            SyntaxNode withSetModifiers = @class.WithModifiers(modifiers);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, withSetModifiers);

            return this;
        }

        public ClassComposer MakeStatic()
        {
            CompositionGuard.ImmediateNodeIs(Visitor.CurrentNode, typeof(ClassDeclarationSyntax));

            var @class = (Visitor.CurrentNode as ClassDeclarationSyntax)!;

            SyntaxTokenList modifiers = @class.Modifiers;

            if (modifiers.Where(m => m.IsKind(SyntaxKind.StaticKeyword)).Any())
            {
                return this;
            }

            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            SyntaxNode madeStatic = @class.WithModifiers(modifiers);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, madeStatic);

            return this;
        }

        public ClassComposer MakeNonStatic()
        {
            CompositionGuard.ImmediateNodeIs(Visitor.CurrentNode, typeof(ClassDeclarationSyntax));

            var @class = (Visitor.CurrentNode as ClassDeclarationSyntax)!;
            SyntaxTokenList modifiers = @class.Modifiers;
            for (int i = modifiers.Count - 1; i >= 0; i--)
            {
                var m = modifiers.ElementAt(i);
                if (m.IsKind(SyntaxKind.StaticKeyword))
                {
                    modifiers = modifiers.RemoveAt(i);
                    break;
                }
            }

            SyntaxNode madeNonStatic = @class.WithModifiers(modifiers);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, madeNonStatic);

            return this;
        }
        #endregion

        public ClassComposer Delete()
        {
            base.DeleteForParentNodeOfType<ClassDeclarationSyntax>();
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
    }
}
