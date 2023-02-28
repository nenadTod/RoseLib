using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using RoseLib.Traversal.Navigators;
using RoseLib.Exceptions;
using RoseLib.Traversal;

namespace RoseLib.Composers
{
    public class CompilationUnitComposer : BaseComposer
    {
        public CompilationUnitComposer(): base(new CompilationUnitNavigator())
        {
        }

        internal CompilationUnitComposer(IStatefulVisitor visitor) : base(visitor)
        {
        }

        protected override void PrepareStateAndSetStatePivotIndex()
        {
            GenericPrepareStateAndSetStatePivotIndex(typeof(CompilationUnitSyntax), SupportedScope.IMMEDIATE_OR_PARENT);
        }

        public static new bool CanProcessCurrentSelection(IStatefulVisitor statefulVisitor)
        {
            return GenericCanProcessCurrentSelectionCheck(statefulVisitor, typeof(CompilationUnitSyntax), SupportedScope.IMMEDIATE_OR_PARENT);
        }

        public CompilationUnitComposer AddUsingDirectives(params string[] usings)
        {
            Visitor.PopUntil(typeof(CompilationUnitSyntax));
            var compilationUnit = (Visitor.CurrentNode as CompilationUnitSyntax)!;

            if(usings.Length == 0)
            {
                return this;
            }

            List<UsingDirectiveSyntax> usingDirectives = new List<UsingDirectiveSyntax>();
            foreach (var @using in usings)
            {
                var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(@using));
                usingDirectives.Add(usingDirective);
            }
            CompilationUnitSyntax newCompilationUnit = compilationUnit.AddUsings(usingDirectives.ToArray());
            Visitor.SetHead(newCompilationUnit);

            return this;
        }

        public CompilationUnitComposer AddNamespace(string namespaceName)
        {
            Visitor.PopUntil(typeof(CompilationUnitSyntax));
            var compilationUnit = (Visitor.CurrentNode as CompilationUnitSyntax)!;

            if(namespaceName == null)
            {
                throw new ArgumentNullException("Namespace to add cannot be null");
            }

            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(namespaceName));
            CompilationUnitSyntax newCompilationUnit = compilationUnit.AddMembers(namespaceDeclaration);

            Visitor.SetHead(newCompilationUnit);
            CompilationUnitNavigator.CreateTempNavigator(Visitor).SelectNamespace(namespaceName);

            return this;
        }

        /// <summary>
        /// Creates a namespace composer positioned at the last selected or added namespace, available at the
        /// current navigator.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidActionForStateException"></exception>
        public NamespaceComposer EnterNamespace()
        {
            var @namespace = Visitor.State.Peek().CurrentNode as NamespaceDeclarationSyntax;
            if(@namespace == null)
            {
                throw new InvalidActionForStateException("Entering namespaces only possible when positioned on a namespace declaration syntax instance.");
            }

            return new NamespaceComposer(Visitor);
        }
    }
}