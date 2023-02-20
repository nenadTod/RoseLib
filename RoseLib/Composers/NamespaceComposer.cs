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
    public class NamespaceComposer : TypeContainerComposer
    {
        public NamespaceComposer(IStatefulVisitor visitor) : base(visitor)
        {
        }
   
        public static new bool CanProcessCurrentSelection(IStatefulVisitor statefulVisitor)
        {
            return GenericCanProcessCurrentSelectionCheck(statefulVisitor, typeof(NamespaceDeclarationSyntax), SupporedScope.IMMEDIATE_OR_PARENT);
        }

        public override NamespaceComposer AddClass(ClassProperties options)
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

    }
}
