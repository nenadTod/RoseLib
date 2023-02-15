using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Exceptions;
using RoseLib.Model;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RoseLib.Composers
{
    public abstract class BaseComposer
    {
        public IStatefulVisitor Visitor { get; protected set; }

        protected BaseComposer(IStatefulVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException("Cannot create a composer without a navigator.");
            }

            Visitor = visitor;
        }

        protected BaseComposer(SyntaxNode? node, IStatefulVisitor visitor)
        {
            if (node == null)
            {
                throw new ArgumentNullException("Cannot add null as navigator state.");
            }
            if(visitor == null)
            {
                throw new ArgumentNullException("Cannot create a composer without a navigator.");
            }

            Visitor = visitor;
            Visitor.State.Push(new SelectedObject(node));
        }

        protected BaseComposer(List<SyntaxNode> nodes, IStatefulVisitor visitor)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException("Cannot add null as navigator state.");
            }
            if (visitor == null)
            {
                throw new ArgumentNullException("Cannot create a composer without a navigator.");
            }
            Visitor = visitor;
            Visitor.State.Push(new SelectedObject(nodes));
        }

        // TODO, izmesti ovo GetCode, nije mu samo tu mesto. Napravi nekog writer-a ili nešto

        /// <summary>
        /// Generates a textual representation of a syntax tree.
        /// Does not alter the state of the composer.
        /// </summary>
        /// <returns>syntax tree as a string</returns>
        public string GetCode()
        {
            var compilationUnit = Visitor.State
               .Where(so => so.CurrentNode is CompilationUnitSyntax)
               .Select(so => so.CurrentNode)
               .FirstOrDefault();

            if (compilationUnit == null)
            {
                throw new InvalidActionForStateException("Cannot generate textual representation if there is no compilation unit");
            }

            return compilationUnit
                .NormalizeWhitespace()
                .ToFullString();
        }
    }
}
