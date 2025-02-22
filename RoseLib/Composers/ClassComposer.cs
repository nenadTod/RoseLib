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
    public partial class ClassComposer: CSRTypeComposer
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
        public override ClassComposer AddField(FieldProps options)
        {
            return (base.AddFieldToNodeOfType<ClassDeclarationSyntax>(options) as ClassComposer)!;
        }

        public override ClassComposer AddProperty(PropertyProps options)
        {
            return (base.AddPropertyToType<ClassDeclarationSyntax>(options) as ClassComposer)!;
        }
        public override ClassComposer AddMethod(MethodProps options)
        {
            return (base.AddMethodToType<ClassDeclarationSyntax>(options) as ClassComposer)!;
        }

        public override ClassComposer SetAttributes(List<AttributeProps> modelAttributeList)
        {
            base.SetAttributes(modelAttributeList);

            return this;
        }

        public ClassComposer AddConstructor(ConstructorProps options)
        {
            var constructor = SyntaxFactory.ConstructorDeclaration(
                SyntaxFactory.Identifier(options.ClassName)
            );

            constructor = constructor.WithModifiers(options.ModifiersToTokenList());

            var @params = SyntaxFactory.ParameterList();
            foreach (var param in options.Params)
            {
                var type = SyntaxFactory.IdentifierName(param.Type);
                var name = SyntaxFactory.Identifier(param.Name);
                var paramSyntax = SyntaxFactory
                    .Parameter(new SyntaxList<AttributeListSyntax>(), SyntaxFactory.TokenList(), type, name, null);
                @params = @params.AddParameters(paramSyntax);
            }
            @params = @params.NormalizeWhitespace();
            constructor = constructor.WithParameterList(@params);
            
            if(options.BaseArguments.Count > 0)
            {
                var @arguments = SyntaxFactory.ArgumentList();
                foreach(var baseArgument in options.BaseArguments)
                {
                    var @argument = SyntaxFactory.Argument(SyntaxFactory.IdentifierName(baseArgument));
                    @arguments = arguments.AddArguments(argument);
                }

                @arguments.NormalizeWhitespace();
                constructor = constructor.WithInitializer(
                    SyntaxFactory.ConstructorInitializer
                    (
                        SyntaxKind.BaseConstructorInitializer,
                        @arguments
                    )
                );
            }

            constructor = constructor.WithBody(SyntaxFactory.Block());

            var referenceNode = TryGetReferenceAndPopToPivot();
            var newEnclosingNode = AddMemberToCurrentNode(constructor, referenceNode);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, newEnclosingNode);

            var navigator = BaseNavigator.CreateTempNavigator<CSRTypeNavigator>(Visitor);
            navigator.SelectConstructorDeclaration(options.Params.Select(p => p.Type).ToArray()); 

            return this;
        }
        #endregion


        public ConstructorComposer EnterConstructor()
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(ConstructorDeclarationSyntax));

            var constructor = Visitor.State.Peek().CurrentNode as ConstructorDeclarationSyntax;
            if (constructor == null)
            {
                throw new InvalidActionForStateException("Entering body possible when positioned on a method declaration syntax instance.");
            }

            Visitor.NextStep(new SelectedObject(constructor));

            return new ConstructorComposer(Visitor);
        }


        #region Class change methods
        public ClassComposer Rename(string newName)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(ClassDeclarationSyntax));

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

        public ClassComposer SetBaseTypes(List<string>? baseTypes)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(ClassDeclarationSyntax));
            var @class = (Visitor.CurrentNode as ClassDeclarationSyntax)!;

            ClassDeclarationSyntax? alteredClass;
            if(baseTypes == null || baseTypes.Count() == 0)
            {
                alteredClass = @class.WithBaseList(null);
            }
            else
            {
                List<BaseTypeSyntax> parsedBaseTypes = new List<BaseTypeSyntax>();
                foreach(var baseType in baseTypes)
                {
                    var type = SyntaxFactory.ParseTypeName(baseType);
                    var parsedbaseType = SyntaxFactory.SimpleBaseType(type);
                    parsedBaseTypes.Add(parsedbaseType);
                }
                var syntaxList = SyntaxFactory.SeparatedList(parsedBaseTypes);
                var baseTypeList = SyntaxFactory.BaseList(syntaxList);
                alteredClass = @class.WithBaseList(baseTypeList);
            }

            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, alteredClass);

            return this;
        }

        public ClassComposer SetAccessModifier(AccessModifiers newType)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(ClassDeclarationSyntax));

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

        public ClassComposer MakeStatic()
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(ClassDeclarationSyntax));

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
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(ClassDeclarationSyntax));

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

        public ClassComposer MakePartial()
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(ClassDeclarationSyntax));

            var @class = (Visitor.CurrentNode as ClassDeclarationSyntax)!;

            SyntaxTokenList modifiers = @class.Modifiers;

            if (modifiers.Where(m => m.IsKind(SyntaxKind.PartialKeyword)).Any())
            {
                return this;
            }

            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
            SyntaxNode madeStatic = @class.WithModifiers(modifiers);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, madeStatic);

            return this;
        }

        public ClassComposer MakeNonPartial()
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(ClassDeclarationSyntax));

            var @class = (Visitor.CurrentNode as ClassDeclarationSyntax)!;
            SyntaxTokenList modifiers = @class.Modifiers;
            for (int i = modifiers.Count - 1; i >= 0; i--)
            {
                var m = modifiers.ElementAt(i);
                if (m.IsKind(SyntaxKind.PartialKeyword))
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

    }
}
