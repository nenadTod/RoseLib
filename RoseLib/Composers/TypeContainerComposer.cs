using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using RoseLib.Exceptions;
using RoseLib.Model;
using RoseLib.Templates;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoseLib.Guards;

namespace RoseLib.Composers
{
    public abstract class TypeContainerComposer : BaseComposer
    {
        protected TypeContainerComposer(IStatefulVisitor visitor) : base(visitor)
        {
        }

        public abstract TypeContainerComposer AddClass(ClassProperties options);
        
        /// <summary>
        /// Use this method only if the concrete type has AddMember method.
        /// The problem is that Roslyn's hierarchy does have AddMember methods with same declarations,
        /// but in different subtrees, so polymorphisms is not applicable.
        /// For example, both NamespaceDeclarationSyntax and TypeDeclarationSyntax have it,
        /// but the code can't be unified.
        /// So, this method exists to unify it behind the dynamic type (to skip design time checks).
        /// </summary>
        /// <typeparam name="T">Descendant of MemberDeclarationSyntax, but should also have AddMembers method</typeparam>
        /// <param name="options">Desired class options</param>
        /// <returns>this</returns>
        protected virtual TypeContainerComposer AddClassToNodeOfType<T>(ClassProperties options) where T : MemberDeclarationSyntax
        {
            CompositionGuard.ImmediateOrParentOfNodeIs(Visitor.CurrentNode, typeof(T));

            var template = new EmptyClassTemplate() { Properties = options };
            var code = template.TransformText();
            var cu = SyntaxFactory.ParseCompilationUnit(code).NormalizeWhitespace();
            var newClass = cu.DescendantNodes().OfType<ClassDeclarationSyntax>().First();


            var referenceNode = TryGetReferenceAndPopToPivot();
            var newEnclosingNode = AddMemberToCurrentNode(newClass, referenceNode);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, newEnclosingNode);

            BaseNavigator.CreateTempNavigator<NamespaceNavigator>(Visitor)
                .SelectClassDeclaration(options.ClassName);

            return this;
        }

        public ClassComposer EnterClass()
        {
            var @class = Visitor.State.Peek().CurrentNode as ClassDeclarationSyntax;
            if (@class == null)
            {
                throw new InvalidActionForStateException("Entering classes only possible when positioned on a class declaration syntax instance.");
            }

            return new ClassComposer(Visitor);
        }

        public abstract TypeContainerComposer AddInterface(InterfaceProperties properties);

        /// <summary>
        /// Use this method only if the concrete type has AddMember method.
        /// The problem is that Roslyn's hierarchy does have AddMember methods with same declarations,
        /// but in different subtrees, so polymorphisms is not applicable.
        /// For example, both NamespaceDeclarationSyntax and TypeDeclarationSyntax have it,
        /// but the code can't be unified.
        /// So, this method exists to unify it behind the dynamic type (to skip design time checks).
        /// </summary>
        /// <typeparam name="T">Descendant of MemberDeclarationSyntax, but should also have AddMembers method</typeparam>
        /// <param name="properties">Desired interface properties</param>
        /// <returns>this</returns>
        protected virtual TypeContainerComposer AddInterfaceToNodeOfType<T>(InterfaceProperties properties) where T : MemberDeclarationSyntax
        {
            CompositionGuard.ImmediateOrParentOfNodeIs(Visitor.CurrentNode, typeof(T));

            var template = new EmptyInterfaceTemplate() { Properties = properties };
            var code = template.TransformText();
            var cu = SyntaxFactory.ParseCompilationUnit(code).NormalizeWhitespace();
            var newInterface = cu.DescendantNodes().OfType<InterfaceDeclarationSyntax>().First();

            var referenceNode = TryGetReferenceAndPopToPivot();
            var newEnclosingNode = AddMemberToCurrentNode(newInterface, referenceNode);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, newEnclosingNode);

            BaseNavigator.CreateTempNavigator<NamespaceNavigator>(Visitor)
                .SelectInterfaceDeclaration(properties.InterfaceName);

            return this;
        }

        protected MemberDeclarationSyntax? TryGetReferenceAndPopToPivot()
        {
            var enclosingNode = Visitor.GetNodeAtIndex((int)StatePivotIndex!);
            var isAtBase = enclosingNode == Visitor.CurrentNode;
            var referenceNode = isAtBase ? null : Visitor.CurrentNode as MemberDeclarationSyntax;

            Visitor.PopToIndex((int)StatePivotIndex);
            return referenceNode;
        }

        protected SyntaxNode AddMemberToCurrentNode(MemberDeclarationSyntax member, MemberDeclarationSyntax? referenceNode = null)
        {
            SyntaxNode newEnclosingNode;
            var dynamicNode = (Visitor.CurrentNode as dynamic)!;
            if (referenceNode == null)
            {
                newEnclosingNode = dynamicNode.AddMembers(member);
            }
            else
            {
                var currentSelection = referenceNode!;
                var currentMembers = (SyntaxList<MemberDeclarationSyntax>)dynamicNode.Members;
                var indexOfSelected = currentMembers.IndexOf(currentSelection);
                if (indexOfSelected == -1)
                {
                    throw new InvalidStateException("For some reason, reference node not found in members.");
                }
                var updatedMembers = currentMembers.Insert(indexOfSelected + 1, member);
                newEnclosingNode = dynamicNode.WithMembers(updatedMembers);
            }

            return newEnclosingNode;
        }
    }
}
