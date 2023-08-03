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
    public partial class MethodComposer : MemberComposer
    {
        public MethodComposer(IStatefulVisitor visitor, bool pivotOnParent = false) : base(visitor, pivotOnParent)
        {
        }

        #region Transition methods
        public static new bool CanProcessCurrentSelection(IStatefulVisitor statefulVisitor, bool pivotOnParent)
        {
            if (pivotOnParent)
            {
                throw new NotSupportedException("Metod does not have descendants which composer can handle. It's body does.");
            }
            return GenericCanProcessCurrentSelectionCheck(statefulVisitor, typeof(MethodDeclarationSyntax), SupportedScope.IMMEDIATE);
        }

        protected override void PrepareStateAndSetStatePivot(bool pivotOnParent)
        {
            if (pivotOnParent)
            {
                throw new NotSupportedException("Metod does not have descendants which composer can handle. It's body does.");
            }
            
            GenericPrepareStateAndSetStatePivot(typeof(MethodDeclarationSyntax), SupportedScope.IMMEDIATE);
        }
        #endregion

        #region Method change methods
        public MethodComposer Rename(string newName)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(MethodDeclarationSyntax));

            var identifier = SyntaxFactory.Identifier(newName);
            var renamedMethod = (Visitor.CurrentNode as MethodDeclarationSyntax)!.WithIdentifier(identifier);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, renamedMethod);

            return this;
        }

        public MethodComposer SetReturnType(string type)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(MethodDeclarationSyntax));

            var identifier = SyntaxFactory.IdentifierName(type);
            var method = (Visitor.CurrentNode as MethodDeclarationSyntax)!.WithReturnType(identifier);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, method);

            return this;
        }

        public MethodComposer Parameters(params ParamProps[] parameters)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(MethodDeclarationSyntax));

            var @params = SyntaxFactory.ParameterList();
            foreach (var param in parameters)
            {
                var type = SyntaxFactory.IdentifierName(param.Type);
                var name = SyntaxFactory.Identifier(param.Name);
                var paramSyntax = SyntaxFactory
                    .Parameter(new SyntaxList<AttributeListSyntax>(), SyntaxFactory.TokenList(), type, name, null);
                @params = @params.AddParameters(paramSyntax);
            }
            @params = @params.NormalizeWhitespace();
            var method = (Visitor.CurrentNode as MethodDeclarationSyntax)!.WithParameterList(@params);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, method);

            return this;
        }

        public MethodComposer AppendParameters(params ParamProps[] parameters)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(MethodDeclarationSyntax));

            var existingParams = (Visitor.CurrentNode as MethodDeclarationSyntax)!.ParameterList;

            foreach (var param in parameters)
            {
                var type = SyntaxFactory.IdentifierName(param.Type);
                var name = SyntaxFactory.Identifier(param.Name);
                var paramSyntax = SyntaxFactory
                    .Parameter(new SyntaxList<AttributeListSyntax>(), SyntaxFactory.TokenList(), type, name, null);

                existingParams = existingParams.AddParameters(paramSyntax);
            }
            existingParams = existingParams.NormalizeWhitespace();

            var method = (Visitor.CurrentNode as MethodDeclarationSyntax)!.WithParameterList(existingParams);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, method);

            return this;
        }

        public MethodComposer SetAccessModifier(AccessModifiers newType)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(MethodDeclarationSyntax));

            var method = (Visitor.CurrentNode as MethodDeclarationSyntax)!;
            SyntaxTokenList modifiers = method.Modifiers;
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

            SyntaxNode withSetModifiers = method.WithModifiers(modifiers);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, withSetModifiers);

            return this;
        }

        public MethodComposer MakeStatic()
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(MethodDeclarationSyntax));

            var method = (Visitor.CurrentNode as MethodDeclarationSyntax)!;

            SyntaxTokenList modifiers = method.Modifiers;

            if (modifiers.Where(m => m.IsKind(SyntaxKind.StaticKeyword)).Any())
            {
                return this;
            }

            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            SyntaxNode madeStatic = method.WithModifiers(modifiers);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, madeStatic);

            return this;
        }

        public MethodComposer MakeNonStatic()
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(MethodDeclarationSyntax));

            var method = (Visitor.CurrentNode as MethodDeclarationSyntax)!;
            SyntaxTokenList modifiers = method.Modifiers;
            for (int i = modifiers.Count - 1; i >= 0; i--)
            {
                var m = modifiers.ElementAt(i);
                if (m.IsKind(SyntaxKind.StaticKeyword))
                {
                    modifiers = modifiers.RemoveAt(i);
                    break;
                }
            }

            SyntaxNode madeNonStatic = method.WithModifiers(modifiers);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, madeNonStatic);

            return this;
        }

        public MethodComposer MakePartial()
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(MethodDeclarationSyntax));

            var method = (Visitor.CurrentNode as MethodDeclarationSyntax)!;

            SyntaxTokenList modifiers = method.Modifiers;

            if (modifiers.Where(m => m.IsKind(SyntaxKind.PartialKeyword)).Any())
            {
                return this;
            }

            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.PartialKeyword));
            SyntaxNode madeStatic = method.WithModifiers(modifiers);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, madeStatic);

            return this;
        }

        public MethodComposer MakeNonPartial()
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(MethodDeclarationSyntax));

            var method = (Visitor.CurrentNode as MethodDeclarationSyntax)!;
            SyntaxTokenList modifiers = method.Modifiers;
            for (int i = modifiers.Count - 1; i >= 0; i--)
            {
                var m = modifiers.ElementAt(i);
                if (m.IsKind(SyntaxKind.PartialKeyword))
                {
                    modifiers = modifiers.RemoveAt(i);
                    break;
                }
            }

            SyntaxNode madeNonStatic = method.WithModifiers(modifiers);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, madeNonStatic);

            return this;
        }
        public override MethodComposer SetAttributes(List<AttributeProps> modelAttributeList)
        {
            base.SetAttributes(modelAttributeList);

            return this;
        }
        #endregion

        public BlockComposer EnterBody()
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(MethodDeclarationSyntax));
            
            var method = Visitor.State.Peek().CurrentNode as MethodDeclarationSyntax;
            if (method == null)
            {
                throw new InvalidActionForStateException("Entering body possible when positioned on a method declaration syntax instance.");
            }

            if(method.Body == null)
            {
                throw new InvalidOperationException("Cannot enter a body of a bodyless method.");
            }

            Visitor.NextStep(new SelectedObject(method.Body));

            return new BlockComposer(Visitor);
        }
    }
}
