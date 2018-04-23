using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLibApp.RoseLib.Composers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLibApp.RoseLib.Selectors
{
    public class CompilationUnitSelector<T>: BaseSelector<T> where T:IComposer
    {
        protected CompilationUnitSelector()
        {
        }

        public CompilationUnitSelector(StreamReader sr):base(sr)
        {
        }

        public NamespaceComposer ToNamespaceComposer()
        {
            if (CurrentNode != null && CurrentNode is NamespaceDeclarationSyntax)
            {
                return new NamespaceComposer(CurrentNode as NamespaceDeclarationSyntax, Composer);
            }

            return null;
        }

        public T SelectNamespace()
        {
            var @namespace = CurrentNode?
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault();

            if (@namespace != null)
            {
                NextStep(@namespace);
            }
           
            return Composer;
        }
    }
}