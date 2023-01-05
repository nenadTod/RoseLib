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
        public NamespaceComposer(NamespaceNavigator navigator) : base(navigator)
        {
        }
        public NamespaceComposer(NamespaceDeclarationSyntax? namespaceDeclaration, NamespaceNavigator navigator) : base(namespaceDeclaration, navigator)
        {
        }

        // TODO: Dodatno razmisliti o selekciji klase nakon kreirana.
        public NamespaceComposer AddClass(ClassOptions options)
        {
            Navigator.AsVisitor.PopUntil(typeof(NamespaceDeclarationSyntax));
            var @namespace = (Navigator.AsVisitor.CurrentNode as NamespaceDeclarationSyntax)!;


            var template = new CreateClass() { Options = options };
            var code = template.TransformText();
            var cu = SyntaxFactory.ParseCompilationUnit(code).NormalizeWhitespace();
            var newClass = cu.DescendantNodes().OfType<ClassDeclarationSyntax>().First();

            (Navigator as IStatefulVisitor).PrepareForTreeUpdate();
            var newNamespaceVersion = @namespace.AddMembers(newClass); // Da li bi ovu instancu klase mogao da upotrebiš za selekciju?
            (Navigator as IStatefulVisitor).AfterUpdateStateAdjustment(@namespace, newNamespaceVersion);

            (Navigator as NamespaceNavigator)?.SelectClassDeclaration(options.ClassName);

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
            var @class = Navigator.State.Peek().CurrentNode as ClassDeclarationSyntax;
            if (@class == null)
            {
                throw new InvalidActionForStateException("Entering classes only possible when positioned on a class declaration syntax instance.");
            }

            CSRTypeNavigator classNavigator = new CSRTypeNavigator(Navigator as BaseNavigator);

            return new ClassComposer(classNavigator);
        }
    }
}
