using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.ComposersOLD;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RoseLib.Selectors
{
    public class CompilationUnitSelector<T>: BaseSelector<T> where T:IComposer
    {
        protected CompilationUnitSelector()
        {
        }

        public CompilationUnitSelector(string path) : base(path)
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
            else
            {
                throw new Exception("Can't convert to namespace converter if not at namespace");
            }
        }

        public ClassComposer ToClassComposer()
        {
            if (CurrentNode != null && CurrentNode is ClassDeclarationSyntax)
            {
                return new ClassComposer(CurrentNode as ClassDeclarationSyntax, Composer);
            }
            else
            {
                throw new Exception("Can't convert to namespace converter if not at class");
            }
        }

        /// <summary>
        /// Selects the first found namespace.
        /// </summary>
        /// <returns>Composer</returns>
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

        public T SelectNamespace(string name)
        {
            var @namespace = CurrentNode?
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .Where((nds) => nds.Name.ToFullString().Trim().Equals(name))
                .FirstOrDefault();

            if (@namespace != null)
            {
                NextStep(@namespace);
            }

            return Composer;
        }

        public T SelectNamespace(Regex regex)
        {
            var @namespace = CurrentNode?
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .Where((nds) => regex.IsMatch(nds.Name.ToFullString()))
                .FirstOrDefault();

            if (@namespace != null)
            {
                NextStep(@namespace);
            }

            return Composer;
        }

        /// <summary>
        /// Selects the first found class.
        /// </summary>
        /// <returns>Composer</returns>
        public T SelectClass()
        {
            var @class = CurrentNode?
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault();

            if (@class != null)
            {
                NextStep(@class);
            }

            return Composer;
        }

        public T SelectClass(string name)
        {
            var @class = CurrentNode?
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where((cds) => cds.Identifier.ToString().Equals(name))
                .FirstOrDefault();

            if (@class != null)
            {
                NextStep(@class);
            }

            return Composer;
        }
    }
}