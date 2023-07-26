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
        public static StatementNavigator SelectLastStatementDeclaration<T>(this T navigator) where T : IBodySelector
        {
            if (navigator.CurrentNode is BlockSyntax)
            {
                var lastStatement = navigator
                                .CurrentNode
                                ?.DescendantNodes()
                                .OfType<StatementSyntax>()
                                .GetClosestDepthwise()
                                ?.LastOrDefault();

                navigator.NextStep(lastStatement);

                return navigator.ToStatementNavigator();
            }
            else if(navigator.CurrentNode is MethodDeclarationSyntax)
            {
                var method = navigator.CurrentNode as MethodDeclarationSyntax;
                var methodsBody = method.Body;

                var lastStatement = methodsBody
                               ?.DescendantNodes()
                               .OfType<StatementSyntax>()
                               .GetClosestDepthwise()
                               ?.LastOrDefault();

                navigator.NextStep(lastStatement);

                return navigator.ToStatementNavigator();
            } else { 
                throw new NotSupportedException("Not able to select the last statement - current node type not supported."); 
            }
            
        }
    }
}
