using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLibApp.RoseLib.Validation_Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RoseLibApp.RoseLib.Selectors
{
    public class MethodSelector : BaseSelector
    {
        #region Constructors

        public MethodSelector(StreamReader reader) : base(reader)
        {
        }

        public MethodSelector(SyntaxNode node) : base(node)
        {
        }

        public MethodSelector(List<SyntaxNode> nodes) : base(nodes)
        {
        }

        #endregion

        #region

        /// <summary>
		/// Finds a method invocation of the given name which is a descendant of the provided root
		/// </summary>
		/// <param name="root">Root node</param>
		/// <param name="methodName">Method's name</param>
		/// <returns>True if found, false otherwise</returns>
		public bool FindMethodInvocationByMethodName([NotBlank] string methodName)
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
                    CurrentNode = invocation;
                    return true;
                }
            }
            CurrentNode = null;
            return false;
        }

        /// <summary>
		/// Finds all method invocations of the given name which are a descendant of the provided root
		/// </summary>
		/// <param name="methodName">Method's name</param>
		/// <returns>True if found, false otherwise</returns>
		public bool FindAllMethodInvocationByMethodName([NotBlank] string methodName)
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
                CurrentNodesList = found;
                return true;
            }

            CurrentNodesList = null;
            return false;
        }

        /// <summary>
		///Finds variable declaration based on the variable's name if it exists within the specified root.
		/// </summary>
		/// <param name="variableName">Name of the variable</param>
		/// <returns>True if found, false otherwise</returns>
		public bool FindVariableDeclaration([NotBlank] string variableName)
        {
            var declarator = CurrentNode?.DescendantNodes().OfType<VariableDeclaratorSyntax>()
                .Where(v => v.Identifier.ValueText == variableName).FirstOrDefault();
            if (declarator == null)
            {
                CurrentNode = null;
                return false;
            }
            else
            {
                CurrentNode = declarator.Parent;
                return true;
            }
        }

        /// <summary>
		/// Finds the last statement contained by the given syntax node
		/// </summary>
		/// <returns>True if found, false otherwise</returns>
		public bool FindLastStatement()
        {
            List<StatementSyntax> statements = CurrentNode?.DescendantNodes().OfType<StatementSyntax>().ToList();
            if (statements.Count() > 0)
            {
                CurrentNode = statements.Last();
                return true;
            }
            else
            {
                CurrentNode = null;
                return false;
            }

        }
        #endregion

    }
}
