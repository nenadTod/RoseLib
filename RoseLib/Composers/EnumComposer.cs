using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Scripting.Interpreter;
using RoseLib.Enums;
using RoseLib.Exceptions;
using RoseLib.Guards;
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

    public partial class EnumComposer : MemberComposer
    {
        public EnumComposer(IStatefulVisitor visitor, bool pivotOnParent = false) : base(visitor, pivotOnParent)
        {
        }

        #region Transition methods
        public static new bool CanProcessCurrentSelection(IStatefulVisitor statefulVisitor, bool pivotOnParent)
        {
            if (!pivotOnParent)
            {
                return GenericCanProcessCurrentSelectionCheck(statefulVisitor, typeof(EnumDeclarationSyntax), SupportedScope.IMMEDIATE_OR_PARENT);
            }
            else
            {
                return GenericCanProcessCurrentSelectionParentCheck(statefulVisitor, typeof(EnumDeclarationSyntax));
            }
        }

        protected override void PrepareStateAndSetStatePivot(bool pivotOnParent)
        {
            if (!pivotOnParent)
            {
                GenericPrepareStateAndSetStatePivot(typeof(EnumDeclarationSyntax), SupportedScope.IMMEDIATE_OR_PARENT);
            }
            else
            {
                GenericPrepareStateAndSetParentAsStatePivot(typeof(EnumDeclarationSyntax));
            }
        }
        #endregion

        #region Enum change methods
        public EnumComposer Rename(string newName)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(EnumDeclarationSyntax));

            var identifier = SyntaxFactory.Identifier(newName);
            var @enum = (Visitor.CurrentNode as EnumDeclarationSyntax)!;
            var renamedEnum = @enum.WithIdentifier(identifier);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, renamedEnum);

            return this;
        }

        public EnumComposer SetAccessModifier(AccessModifiers newType)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(EnumDeclarationSyntax));

            var @enum = (Visitor.CurrentNode as EnumDeclarationSyntax)!;
            SyntaxTokenList modifiers = @enum.Modifiers;
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
                    throw new NotSupportedException($"Setting {newType} as an access modifier of ae enum not supported");
            }

            SyntaxNode withSetModifiers = @enum.WithModifiers(modifiers);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, withSetModifiers);

            return this;
        }

        #endregion

        public EnumComposer AddEnumMember(string identifier)
        {
            CompositionGuard.NodeOrParentIs(Visitor.CurrentNode, typeof(EnumDeclarationSyntax));
            var newEnumMember = SyntaxFactory.EnumMemberDeclaration(identifier);

            var referenceNode = (EnumMemberDeclarationSyntax) TryGetReferenceAndPopToPivot()!;
            var newEnclosingNode = AddEnumMember(newEnumMember, referenceNode);

            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, newEnclosingNode);
            var navigator = BaseNavigator.CreateTempNavigator<EnumNavigator>(Visitor);
            navigator.SelectEnumMemberDeclaration(identifier);

            return this;
        }

        protected SyntaxNode AddEnumMember(EnumMemberDeclarationSyntax newEnumMember, EnumMemberDeclarationSyntax? referenceEnumMember= null)
        {
            SyntaxNode newEnumNode;
            var enumNode = (Visitor.CurrentNode as EnumDeclarationSyntax)!;
            if (referenceEnumMember == null)
            {
                newEnumNode = enumNode.AddMembers(newEnumMember);
            }
            else
            {
                var currentSelection = referenceEnumMember!;
                var currentMembers = enumNode.Members;
                var indexOfSelected = currentMembers.IndexOf(currentSelection);
                if (indexOfSelected == -1)
                {
                    throw new InvalidStateException("For some reason, reference node not found in members.");
                }
                var updatedMembers = currentMembers.Insert(indexOfSelected + 1, newEnumMember);
                newEnumNode = enumNode.WithMembers(updatedMembers);
            }

            return newEnumNode;
        }

        public EnumComposer Delete()
        {
            base.DeleteForParentNodeOfType<EnumDeclarationSyntax>();
            return this;
        }
    }
}
