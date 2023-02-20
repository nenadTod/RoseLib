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
        protected virtual TypeContainerComposer AddClassToType<T>(ClassProperties options) where T : TypeDeclarationSyntax
        {
            Visitor.PopUntil(typeof(T));
            var typeNode = (Visitor.CurrentNode as T)!;


            var template = new CreateClass() { Options = options };
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
    }
}
