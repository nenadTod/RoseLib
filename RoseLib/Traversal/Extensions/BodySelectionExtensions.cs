using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Guards;
using RoseLib.Traversal.Navigators;
using RoseLib.Traversal.Selectors.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal
{
    public static class BodySelectionExtensions
    {


        /// <summary>
        ///Finds variable declaration based on the variable's name if it exists within the specified root, and is made current.
        /// </summary>
        /// <param name="name">Name of the variable</param>
        /// <returns>Statement navigator</returns>
        public static StatementNavigator SelectVariableDeclaration<T>(this T navigator, string name) where T : IBodySelector
        {
            NavigationGuard.NameNotNull(name);

            var declarator = navigator.CurrentNode?.DescendantNodes().OfType<VariableDeclaratorSyntax>()
                .Where(v => v.Identifier.ValueText == name).FirstOrDefault();
            
            navigator.NextStep(declarator);

            return navigator.ToStatementNavigator();
        }
    }
}
