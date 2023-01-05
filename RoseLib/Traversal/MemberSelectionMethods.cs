using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Traversal.Navigators;
using RoseLib.Traversal.Selectors.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal
{
    public static class MemberSelectionMethods
    {
        #region Field selection
        
        public static FieldNavigator SelectFieldDeclaration<T>(this T navigator, string fieldName) where T: IMemberSelector
        {
            NavigationGuard.CurrentNodeNotNull(navigator.CurrentNode);
            NavigationGuard.NameNotNull(fieldName);

            FieldDeclarationSyntax? foundDeclaration = navigator.CurrentNode?
                .DescendantNodes()
                .OfType<FieldDeclarationSyntax>()
                .Where(
                    fd => fd.DescendantNodes()
                            .OfType<VariableDeclaratorSyntax>()
                            .Where(vd => vd.Identifier.ValueText == fieldName)
                            .Any()
                )
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            navigator.NextStep(foundDeclaration);

            return navigator.ToFieldNavigator();
        }

        public static FieldNavigator SelectLastFieldDeclaration<T>(this T navigator) where T : IMemberSelector
        {
            NavigationGuard.CurrentNodeNotNull(navigator.CurrentNode);
            SyntaxNode? lastFieldDeclaration = navigator.CurrentNode?
                .DescendantNodes()
                .OfType<FieldDeclarationSyntax>()
                .GetClosestDepthwise()
                ?.LastOrDefault();

            navigator.NextStep(lastFieldDeclaration);

            return navigator.ToFieldNavigator();
        }

        #endregion

        #region Property selection
        public static PropertyNavigator SelectPropertyDeclaration<T>(this T visitor, string propertyName) where T : IMemberSelector
        {
            NavigationGuard.CurrentNodeNotNull(visitor.CurrentNode);
            NavigationGuard.NameNotNull(propertyName);

            var result = visitor.CurrentNode?
                .DescendantNodes()
                .OfType<PropertyDeclarationSyntax>()
                .Where(p => p.Identifier.ValueText == propertyName)
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(result);
            return visitor.ToPropertyNavigator();
        }


        public static PropertyNavigator SelectLastPropertyDeclaration<T>(this T visitor) where T : IMemberSelector
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

        public static MethodNavigator SelectOverloadedMethodDeclarations<T>(this T visitor, string methodName) where T : IMemberSelector
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

        public static MethodNavigator SelectMethodDeclaration<T>(this T visitor, string methodName) where T : IMemberSelector
        {
            NavigationGuard.CurrentNodeNotNull(visitor.CurrentNode);
            NavigationGuard.NameNotNull(methodName);

            var methodDeclaration = visitor.CurrentNode?
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(p => p.Identifier.ValueText == methodName)
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(methodDeclaration);
            return visitor.ToMethodNavigator();
        }

        public static MethodNavigator SelectMethodDeclaration<T>(this T navigator, string methodName, params string[] parameterTypes) where T : IMemberSelector
        {
            MethodDeclarationSyntax? foundMethod = null;
            NavigationGuard.NameNotNull(methodName);

            var overloadedMethods = navigator.CurrentNode?
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(p => p.Identifier.ValueText == methodName)
                .GetClosestDepthwise()
                ?.ToList();

            if(overloadedMethods != null && overloadedMethods.Any())
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

        public static MethodNavigator SelectLastMethodDeclaration<T>(this T navigator) where T : IMemberSelector
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

        #region Constructor selection

        public static ConstructorNavigator SelectOverloadedConstructorDeclarations<T>(this T navigator) where T : IMemberSelector
        {
            var constructors = navigator
                .CurrentNode?
                .DescendantNodes()
                .OfType<ConstructorDeclarationSyntax>()
                .GetClosestDepthwise()
                ?.ToList<SyntaxNode>();

            navigator.NextStep(constructors);
            return navigator.ToConstructorNavigator();
        }

        public static ConstructorNavigator SelectConstructorDeclaration<T>(this T navigator, params string[] parameterTypes) where T : IMemberSelector
        {
            ConstructorDeclarationSyntax? foundConstructor = null;
            var constructors = navigator
                .CurrentNode?
                .DescendantNodes()
                .OfType<ConstructorDeclarationSyntax>()
                ?.ToList();

            if(constructors != null && constructors.Count() > 0)
            {
                List<ConstructorDeclarationSyntax> ctorsWithSameParams = new List<ConstructorDeclarationSyntax>();
                constructors.ForEach(ctor =>
                {
                    bool areSame = CompareParameterTypes(ctor, parameterTypes);
                    if (areSame)
                    {
                        ctorsWithSameParams.Add(ctor);
                    }
                });

                foundConstructor = ctorsWithSameParams
                    .GetClosestDepthwise()?
                    .FirstOrDefault();
            }

            navigator.NextStep(foundConstructor);
            return navigator.ToConstructorNavigator();
        }

        public static ConstructorNavigator SelectLastConstructorDeclaration<T>(this T navigator) where T : IMemberSelector
        {
            var lastConstructor = navigator
             .CurrentNode?
             .DescendantNodes()
             .OfType<ConstructorDeclarationSyntax>()
             .GetClosestDepthwise()
             ?.LastOrDefault();
            
            navigator.NextStep(lastConstructor);
            return navigator.ToConstructorNavigator();
        }

        #endregion

        #region Destructor selection

        public static DestructorNavigator SelectDestructorDeclaration<T>(this T navigator) where T: IMemberSelector
        {
            var destructor = navigator
                .CurrentNode?
                .DescendantNodes()
                .OfType<DestructorDeclarationSyntax>()
                .GetClosestDepthwise()
                ?.FirstOrDefault();
            navigator.NextStep(destructor);
            return navigator.ToDestructorNavigator();
        }

        #endregion

        #region Conversion operator selection

        public static ConversionOperatorNavigator SelectConversionOperatorDeclaration<T>(this T navigator, string parameterType) where T: IMemberSelector
        {
            ConversionOperatorDeclarationSyntax? foundCO = null;

            var conversionOperators = navigator
                .CurrentNode?
                .DescendantNodes()
                .OfType<ConversionOperatorDeclarationSyntax>()
                .ToList();

            
            if(conversionOperators != null && conversionOperators.Count() > 0)
            {
                List<ConversionOperatorDeclarationSyntax> conversionOperatorsWithSameParams = new List<ConversionOperatorDeclarationSyntax>();
                conversionOperators.ForEach(co =>
                {
                    bool areSame = CompareParameterTypes(co, parameterType);

                    if (areSame)
                    {
                        conversionOperatorsWithSameParams.Add(co);
                    }
                });

                foundCO = conversionOperatorsWithSameParams
                    .GetClosestDepthwise()?
                    .FirstOrDefault();

            }
            
            navigator.NextStep(foundCO);
            return navigator.ToConversionOperatorNavigator();
        }

        #endregion

        #region Operator selection

        private const short PARAM_NUM_OF_UNARY = 1;
        private const short PARAM_NUM_OF_BINARY = 2;

        public static OperatorNavigator SelectUnaryOperatorDeclaration<T>(this T navigator, string operatorToken) where T: IMemberSelector
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

        public static OperatorNavigator SelectBinaryOperatorDeclaration<T>(this T navigator, string operatorToken, string firstParameterType, string secondParameterType) where T: IMemberSelector
        {
            OperatorDeclarationSyntax? foundOperator = null;

            var binaryOperators = navigator.CurrentNode
                ?.DescendantNodes()
                .OfType<OperatorDeclarationSyntax>()
               .Where(op => op.OperatorToken.ToString() == operatorToken
                    && op.ParameterList.Parameters.Count == PARAM_NUM_OF_BINARY)
               .ToList();

            if(binaryOperators != null && binaryOperators.Count() > 0)
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
        public static OperatorNavigator SelectOverloadedBinaryOperatorDeclarations<T>(this T navigator, string operatorToken) where T : IMemberSelector
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
            if(node == null)
            {
                throw new Exception("Can't compare parameters when not provided with method declaration.");
            }

            var foundParameters = node.ParameterList.Parameters;

            if(foundParameters.Count != 0 && parameterTypes == null)
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
