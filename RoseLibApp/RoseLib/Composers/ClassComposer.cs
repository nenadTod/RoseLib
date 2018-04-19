using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLibApp.RoseLib.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLibApp.RoseLib.Composers
{
    public class ClassComposer: ClassStructSelector<ClassComposer>, IComposer
    {
        public IComposer ParentComposer { get; set; }
        private ClassDeclarationSyntax Root { get; set; }

        public ClassComposer(ClassDeclarationSyntax @class, IComposer parentComposer) : base(@class)
        {
            NextStep(@class);
            Root = @class;
            Composer = this;
            ParentComposer = parentComposer;
        }

        public ClassComposer(ClassDeclarationSyntax @class):base(@class)
        {
            NextStep(@class);
            Composer = this;
        }
        
        public ClassComposer Rename(string newName)
        {
            if (!(CurrentNode is ClassDeclarationSyntax))
            {
                throw new Exception("Pera");
            }

            var id = SyntaxFactory.Identifier(newName);
            var newNode = (CurrentNode as ClassDeclarationSyntax).WithIdentifier(id);
            newNode = RenameConstuctors(newNode, id) as ClassDeclarationSyntax;
            Replace(Root, newNode);

            return this;
        }

        public void Replace(SyntaxNode oldNode, SyntaxNode newNode)
        {
            if(ParentComposer != null)
            {
                ParentComposer.Replace(oldNode, newNode);
            }

            Root = newNode as ClassDeclarationSyntax;
        }

        private SyntaxNode RenameConstuctors(SyntaxNode root, SyntaxToken identifier)
        {
            var constructorCount = root.DescendantNodes().OfType<ConstructorDeclarationSyntax>().Count();
            var newRoot = root;

            for (var current = 0; current < constructorCount; current++)
            {
                var constructors = newRoot.DescendantNodes().OfType<ConstructorDeclarationSyntax>();
                var ctor = constructors.ElementAt(current);

                var newCtor = ctor.WithIdentifier(identifier);
                newRoot = newRoot.ReplaceNode(ctor, newCtor);
            }

            return newRoot;
        }
    }
}
