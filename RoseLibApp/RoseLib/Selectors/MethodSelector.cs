using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLibApp.RoseLib.Composers;
using RoseLibApp.RoseLib.Validation_Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RoseLibApp.RoseLib.Selectors
{
    public class MethodSelector<T> : BaseSelector<T> where T:IComposer
    {
        #region Constructors

        public MethodSelector(StreamReader reader) : base(reader)
        {
        }

        public MethodSelector(MethodDeclarationSyntax node) : base(node)
        {
        }

        public MethodSelector(List<MethodDeclarationSyntax> nodes) : base(nodes.Cast<SyntaxNode>().ToList())
        {
        }

        #endregion

        #region

        /// <summary>
		/// Finds an invocation of a method of the given name which is a descendant of the provided root, and makes it the current node.
		/// </summary>
		/// <param name="root">Root node</param>
		/// <param name="methodName">Method's name</param>
		/// <returns>True if found and made current, false otherwise</returns>
		public bool SelectMethodInvocation([NotBlank] string methodName)
        {
            var invocations = CurrentNode?.DescendantNodes().OfType<InvocationExpressionSyntax>().ToList();

            foreach (var invocation in invocations)
            {
                var identifierNames = invocation.DescendantNodes().OfType<IdentifierNameSyntax>().ToList();
                bool found = (from name in identifierNames
                              where name.Identifier.ValueText == methodName
                              select name).Any();
                if (found)
                {
                    //return NextStep(invocation);
                }
            }

            return false;
        }

        /// <summary>
		/// Finds all method invocations of the given name which are a descendant of the provided root, and makes them the current standing point.
		/// </summary>
		/// <param name="methodName">Method's name</param>
		/// <returns>True if found and made current, false otherwise</returns>
		public bool SelectAllMethodInvocationByMethodName([NotBlank] string methodName)
        {
            var invocations = CurrentNode?.DescendantNodes().OfType<ExpressionStatementSyntax>().ToList();
            List<SyntaxNode> found = new List<SyntaxNode>();

            foreach (var invocation in invocations)
            {
                var identifierNames = invocation.DescendantNodes().OfType<IdentifierNameSyntax>().ToList();
                if ((from name in identifierNames
                     where name.Identifier.ValueText == methodName
                     select name).Any())
                {
                    found.Add(invocation);
                }

            }

            if (found.Any())
            {
                //return NextStep(found);
            }

            return false;
        }

        /// <summary>
		///Finds variable declaration based on the variable's name if it exists within the specified root, and is made current.
		/// </summary>
		/// <param name="variableName">Name of the variable</param>
		/// <returns>True if found and made current, false otherwise</returns>
		public bool SelectVariableDeclaration([NotBlank] string variableName)
        {
            var declarator = CurrentNode?.DescendantNodes().OfType<VariableDeclaratorSyntax>()
                .Where(v => v.Identifier.ValueText == variableName).FirstOrDefault();
            if (declarator != null)
            {
                //return NextStep(declarator.Parent);
            }

            return false;
        }

        /// <summary>
		/// Finds the last statement contained by the given syntax node, and makes it current.
		/// </summary>
		/// <returns>True if found made current, false otherwise</returns>
		public T SelectLastStatement()
        {
            List<StatementSyntax> statements = CurrentNode?.DescendantNodes().OfType<StatementSyntax>().ToList();
            if (statements.Count() > 0)
            {
                NextStep(statements.Last());
            }
            
            return Composer;
        }
        #endregion

    }
}
