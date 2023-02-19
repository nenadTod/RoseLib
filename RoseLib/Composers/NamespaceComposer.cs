using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
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

namespace RoseLib.Composers
{
    public class NamespaceComposer : BaseComposer
    {
        public NamespaceComposer(IStatefulVisitor visitor) : base(visitor)
        {
        }
        public NamespaceComposer(NamespaceDeclarationSyntax? namespaceDeclaration, NamespaceNavigator navigator) : base(namespaceDeclaration, navigator)
        {
        }

        public static new bool CanProcessCurrentNode(IStatefulVisitor statefulVisitor)
        {
            return GenericCanProcessCurrentSelectionCheck(statefulVisitor, typeof(NamespaceDeclarationSyntax), SupporedScope.IMMEDIATE_OR_PARENT);
        }

        public NamespaceComposer AddClass(ClassOptions options)
        {
            Visitor.PopUntil(typeof(NamespaceDeclarationSyntax));
            var @namespace = (Visitor.CurrentNode as NamespaceDeclarationSyntax)!;


            var template = new CreateClass() { Options = options };
            var code = template.TransformText();
            var cu = SyntaxFactory.ParseCompilationUnit(code).NormalizeWhitespace();
            var newClass = cu.DescendantNodes().OfType<ClassDeclarationSyntax>().First();

            var newNamespaceVersion = @namespace.AddMembers(newClass); // Should I track this class and select it without a navigator?
            Visitor.ReplaceNodeAndAdjustState(@namespace, newNamespaceVersion);

            BaseNavigator.CreateTempNavigator<NamespaceNavigator>(Visitor)
                .SelectClassDeclaration(options.ClassName);

            return this;
        }


        /// <summary>
        /// Creates a namespace composer positioned at the last selected or added namespace, available at the
        /// current navigator.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidActionForStateException"></exception>
        public ClassComposer EnterClass()
        {
            var @class = Visitor.State.Peek().CurrentNode as ClassDeclarationSyntax;
            if (@class == null)
            {
                throw new InvalidActionForStateException("Entering classes only possible when positioned on a class declaration syntax instance.");
            }

            return new ClassComposer(Visitor);
        }
    }
}
