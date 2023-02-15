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
    public static class CSRTypeMemberSelectionExtensions
    {
        #region Field selection
        
        public static FieldNavigator SelectFieldDeclaration<T>(this T navigator, string fieldName) where T: ICSRTypeMemberSelector
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

        public static FieldNavigator SelectLastFieldDeclaration<T>(this T navigator) where T : ICSRTypeMemberSelector
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

        #region Constructor selection

        public static ConstructorNavigator SelectOverloadedConstructorDeclarations<T>(this T navigator) where T : ICSRTypeMemberSelector
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

        public static ConstructorNavigator SelectConstructorDeclaration<T>(this T navigator, params string[] parameterTypes) where T : ICSRTypeMemberSelector
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

        public static ConstructorNavigator SelectLastConstructorDeclaration<T>(this T navigator) where T : ICSRTypeMemberSelector
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

        public static DestructorNavigator SelectDestructorDeclaration<T>(this T navigator) where T: ICSRTypeMemberSelector
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

        public static ConversionOperatorNavigator SelectConversionOperatorDeclaration<T>(this T navigator, string parameterType) where T: ICSRTypeMemberSelector
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
