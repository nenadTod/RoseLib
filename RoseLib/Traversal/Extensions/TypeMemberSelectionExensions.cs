using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using RoseLib.Traversal.Navigators;
using RoseLib.Traversal.Selectors.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoseLib.Guards;
using RoseLib.CSPath.Model;
using RoseLib.CSPath;
using System.Runtime.CompilerServices;
using RoseLib.Model;
using RoseLib.Traversal.Extensions;
using System.Reflection;

namespace RoseLib.Traversal
{
    public static class TypeMemberSelectionExensions
    {
        #region Property selection

        [MethodImpl(MethodImplOptions.NoInlining)]
        [CSPathConfig(Concept = "Property", Attribute = "name")]
        public static PropertyNavigator SelectPropertyDeclaration<T>(this T visitor, string name) where T : ITypeMemberSelector
        {
            NavigationGuard.CurrentNodeNotNull(visitor.CurrentNode);
            NavigationGuard.NameNotNull(name);

            var result = visitor.CurrentNode?
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .Where(p => p.Identifier.ValueText == name)
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(new SelectedObject(
                result,
                ExtensionsHelper.GetPathPartForMethodAndValue(MethodBase.GetCurrentMethod()!, name)
                )
            );
            return visitor.ToPropertyNavigator();
        }


        public static PropertyNavigator SelectLastPropertyDeclaration<T>(this T visitor) where T : ITypeMemberSelector
        {
            NavigationGuard.CurrentNodeNotNull(visitor.CurrentNode);
            SyntaxNode? lastPropertyDeclaration = visitor.CurrentNode?
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .GetClosestDepthwise()
                ?.LastOrDefault();

            visitor.NextStep(lastPropertyDeclaration);
            return visitor.ToPropertyNavigator();
        }

        #endregion

        #region Method selection

        public static MethodNavigator SelectOverloadedMethodDeclarations<T>(this T visitor, string methodName) where T : ITypeMemberSelector
        {
            NavigationGuard.CurrentNodeNotNull(visitor.CurrentNode);
            NavigationGuard.NameNotNull(methodName);

            var overloadedMethods = visitor.CurrentNode?
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(p => p.Identifier.ValueText == methodName)
                .GetClosestDepthwise()
                ?.ToList<SyntaxNode>();

            visitor.NextStep(overloadedMethods);
            return visitor.ToMethodNavigator();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        [CSPathConfig(Concept = "Method", Attribute = "name")]
        public static MethodNavigator SelectMethodDeclaration<T>(this T visitor, string name) where T : ITypeMemberSelector
        {
            NavigationGuard.CurrentNodeNotNull(visitor.CurrentNode);
            NavigationGuard.NameNotNull(name);

            var methodDeclaration = visitor.CurrentNode?
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(p => p.Identifier.ValueText == name)
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(new SelectedObject(
                methodDeclaration,
                ExtensionsHelper.GetPathPartForMethodAndValue(MethodBase.GetCurrentMethod()!, name)
                )
            );
            return visitor.ToMethodNavigator();
        }

        public static MethodNavigator SelectMethodDeclaration<T>(this T navigator, string methodName, params string[] parameterTypes) where T : ITypeMemberSelector
        {
            MethodDeclarationSyntax? foundMethod = null;
            NavigationGuard.NameNotNull(methodName);

            var overloadedMethods = navigator.CurrentNode?
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(p => p.Identifier.ValueText == methodName)
                .GetClosestDepthwise()
                ?.ToList();

            if (overloadedMethods != null && overloadedMethods.Any())
            {
                foreach (var methodDeclaration in overloadedMethods)
                {
                    bool areSame = CompareParameterTypes(methodDeclaration, parameterTypes);

                    if (areSame)
                    {
                        foundMethod = methodDeclaration;
                        break;
                    }
                }
            }

            navigator.NextStep(foundMethod);
            return navigator.ToMethodNavigator();
        }

        public static MethodNavigator SelectLastMethodDeclaration<T>(this T navigator) where T : ITypeMemberSelector
        {
            NavigationGuard.CurrentNodeNotNull(navigator.CurrentNode);

            MethodDeclarationSyntax? lastMethod = null;

            lastMethod = navigator.CurrentNode?
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .GetClosestDepthwise()
                ?.LastOrDefault();

            navigator.NextStep(lastMethod);
            return navigator.ToMethodNavigator();
        }

        #endregion

        #region Operator selection

        private const short PARAM_NUM_OF_UNARY = 1;
        private const short PARAM_NUM_OF_BINARY = 2;

        public static OperatorNavigator SelectUnaryOperatorDeclaration<T>(this T navigator, string operatorToken) where T : ITypeMemberSelector
        {
            var foundOperator = navigator
                .CurrentNode
                ?.DescendantNodes()
                .OfType<OperatorDeclarationSyntax>()
                .Where(op => op.ParameterList.Parameters.Count == PARAM_NUM_OF_UNARY && op.OperatorToken.ToString() == operatorToken)
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            navigator.NextStep(foundOperator);
            return navigator.ToOperatorNavigator();
        }

        public static OperatorNavigator SelectBinaryOperatorDeclaration<T>(this T navigator, string operatorToken, string firstParameterType, string secondParameterType) where T : ITypeMemberSelector
        {
            OperatorDeclarationSyntax? foundOperator = null;

            var binaryOperators = navigator.CurrentNode
                ?.DescendantNodes()
                .OfType<OperatorDeclarationSyntax>()
               .Where(op => op.OperatorToken.ToString() == operatorToken
                    && op.ParameterList.Parameters.Count == PARAM_NUM_OF_BINARY)
               .ToList();

            if (binaryOperators != null && binaryOperators.Count() > 0)
            {
                List<OperatorDeclarationSyntax> binaryOperatorsWithSame = new List<OperatorDeclarationSyntax>();
                binaryOperators.ForEach(bo =>
                {
                    bool areSame = CompareParameterTypes(bo, firstParameterType, secondParameterType);
                    if (areSame)
                    {
                        binaryOperatorsWithSame.Add(bo);
                    }
                    foundOperator = binaryOperatorsWithSame
                        .GetClosestDepthwise()?
                        .FirstOrDefault();
                });
            }

            navigator.NextStep(foundOperator);
            return navigator.ToOperatorNavigator();
        }
        public static OperatorNavigator SelectOverloadedBinaryOperatorDeclarations<T>(this T navigator, string operatorToken) where T : ITypeMemberSelector
        {
            var foundOperators = navigator.CurrentNode
                ?.DescendantNodes()
                .OfType<OperatorDeclarationSyntax>()
               .Where(op => op.OperatorToken.ToString() == operatorToken
                    && op.ParameterList.Parameters.Count == PARAM_NUM_OF_BINARY)
               .ToList<SyntaxNode>();

            navigator.NextStep(foundOperators);
            return navigator.ToOperatorNavigator();
        }


        #endregion

        #region Common functionalities

        /// <summary>
        /// A method that compares types of parameters of a given method, with expected values.
        /// </summary>
        /// <param name="node">A base method node. (It is in inheritance tree of constructors and ordinary methods)</param>
        /// <param name="parameterTypes">Expected parameters.</param>
        /// <returns>Returns true if parameters match, false otherwise.</returns>
        private static bool CompareParameterTypes(BaseMethodDeclarationSyntax? node, params string[] parameterTypes)
        {
            if (node == null)
            {
                throw new Exception("Can't compare parameters when not provided with method declaration.");
            }

            var foundParameters = node.ParameterList.Parameters;

            if (foundParameters.Count != 0 && parameterTypes == null)
            {
                return false;
            }

            if (foundParameters.Count() != parameterTypes.Count())
            {
                return false;
            }

            for (int i = 0; i < foundParameters.Count(); i++)
            {
                string? foundType = foundParameters[i].Type?.ToString();
                string expectedType = parameterTypes[i];
                if (foundType != expectedType)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
