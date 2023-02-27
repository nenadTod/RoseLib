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
            Visitor.PopUntil(typeof(T));
            var typeNode = (Visitor.CurrentNode as dynamic)!;


            var template = new EmptyClassTemplate() { Properties = options };
            var code = template.TransformText();
            var cu = SyntaxFactory.ParseCompilationUnit(code).NormalizeWhitespace();
            var newClass = cu.DescendantNodes().OfType<ClassDeclarationSyntax>().First();

            var newTypeNodeVersion = typeNode.AddMembers(newClass); // Should I track this class and select it without a navigator?
            Visitor.ReplaceNodeAndAdjustState(typeNode, newTypeNodeVersion);

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
            Visitor.PopUntil(typeof(T));
            var typeNode = (Visitor.CurrentNode as dynamic)!;


            var template = new EmptyInterfaceTemplate() { Properties = properties };
            var code = template.TransformText();
            var cu = SyntaxFactory.ParseCompilationUnit(code).NormalizeWhitespace();
            var newInterface = cu.DescendantNodes().OfType<InterfaceDeclarationSyntax>().First();

            var newTypeNodeVersion = typeNode.AddMembers(newInterface); // Should I track this interface and select it without a navigator?
            Visitor.ReplaceNodeAndAdjustState(typeNode, newTypeNodeVersion);

            BaseNavigator.CreateTempNavigator<NamespaceNavigator>(Visitor)
                .SelectInterfaceDeclaration(properties.InterfaceName);

            return this;
        }
    }
}
