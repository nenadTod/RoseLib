using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System;
using System.Collections.Generic;
using RoseLibApp.RoseLib.Validation_Attributes;
using System.IO;
using System.Runtime.CompilerServices;

namespace RoseLibApp.RoseLib.Selectors
{
    public class ClassStructSelector : NamespaceSelector
    {
        #region Constructors

        public ClassStructSelector(StreamReader reader) : base(reader)
        {
            // TODO: Only files containing class declaration should be accepted?
        }

        public ClassStructSelector(ClassDeclarationSyntax node) : base(node)
        {
        }
        
        #endregion

        #region Converters

        public MethodSelector ToMethodSelector() //TODO: Solve the situation between Constructors, Coverter Operators, Operators.
        {
            if(CurrentNode != null && CurrentNode is BaseMethodDeclarationSyntax)
            {
                return new MethodSelector(CurrentNode as MethodDeclarationSyntax);
            }

            if(CurrentNodesList != null && CurrentNodesList.Any())
            {
                if(CurrentNodesList.First() is BaseMethodDeclarationSyntax)
                {
                    return new MethodSelector(CurrentNodesList.Cast<MethodDeclarationSyntax>().ToList());
                }
            }

            return null;
        }

        #endregion

        #region Select field declarations

        /// <summary>
        /// Selects a field declaration of a given name (if such exists) and makes it the current node.
        /// </summary>
        /// <param name="fieldName">Name of the variable being declared</param>
        /// <returns>True if found and made current, false otherwise.</returns>
        public bool SelectFieldDeclaration([NotBlank] string fieldName)
        {
            var fieldDeclarations = CurrentNode?.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();
            foreach (var fieldDeclaration in fieldDeclarations)
            {
                var declaratorExists = fieldDeclaration.DescendantNodes().OfType<VariableDeclaratorSyntax>().
                    Where(d => d.Identifier.ValueText == fieldName).Any();

                if (declaratorExists)
                {
                    return NextStep(fieldDeclaration);
                }
            }

            return false;
        }



        /// <summary>
        /// Finds the last field declaration of a given name (if such exists) and it makes it the current node.
        /// </summary>
        /// <returns>True if found and made current, false otherwise.</returns>
        public bool SelectLastFieldDeclaration()
        {
            var result = CurrentNode?.DescendantNodes().OfType<FieldDeclarationSyntax>().LastOrDefault();
            return NextStep(result);
        }

        #endregion

        #region Select property declarations

        /// <summary>
        /// Finds a property declaration of a given name (if such exists) and makes it the current node.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        ///  <returns>True if found and made current, false otherwise.</returns>
        public bool SelectPropertyDeclaration([NotBlank] string propertyName)
        {
            var result = CurrentNode?.DescendantNodes().OfType<PropertyDeclarationSyntax>().
                Where(p => p.Identifier.ValueText == propertyName).FirstOrDefault();

            return NextStep(result);
        }

        /// <summary>
        /// Finds the last property declaration of a given name (if such exists) and makes it the current node.
        /// </summary>
        /// <returns>True if found and made current, false otherwise.</returns>
        public bool SelectLastPropertyDeclaration()
        {
            var result = CurrentNode?.DescendantNodes().OfType<PropertyDeclarationSyntax>().LastOrDefault();
            return NextStep(result);
        }

        #endregion

        #region Select method declarations

        /// <summary>
        /// Finds occurances of (possibly overloaded) methods with a specified name, if such exist, and makes them current.
        /// </summary>
        /// <param name="methodName">Method's name</param>
        /// <returns>True if any found and made current, false otherwise.</returns>
        public bool SelectOverloadedMethodDeclarations([NotBlank] string methodName)
        {
            var allMethods = CurrentNode?.DescendantNodes().OfType<MethodDeclarationSyntax>();

            var result = allMethods?.Where(p => p.Identifier.ValueText == methodName).Cast<SyntaxNode>().ToList();
            return NextStep(result);
        }

        /// <summary>
        /// Finds a method with a specified name (if such exists) and makes it the current node. If there is method overloading the first one is made current. 
        /// </summary>
        /// <param name="methodName">Method's name</param>
        /// <returns>True if found and made current, false otherwise.</returns>
        public bool SelectMethodDeclaration([NotBlank] string methodName)
        {
            SelectOverloadedMethodDeclarations(methodName);

            var result = CurrentNodesList?.FirstOrDefault();
            return NextStep(result);
        }

        /// <summary>
        /// Finds a method with a specified name and parameter types, if such exists, and makes it the current node.
        /// </summary>
        /// <param name="methodName">Method's name</param>
        /// <param name="parameterTypes">Array of strings representing parameters' types.</param>
        /// <returns>True if found and made current, false otherwise.</returns>
        public bool SelectMethodDeclaration([NotBlank] string methodName, params string[] parameterTypes)
        {
            SelectOverloadedMethodDeclarations(methodName);

            foreach (var methodDeclaration in CurrentNodesList)
            {
                bool areSame = CompareParameterTypes(methodDeclaration as MethodDeclarationSyntax, parameterTypes);

                if (areSame)
                {
                    return NextStep(methodDeclaration);
                }
            }

            return false;
        }

        /// <summary>
        /// Finds the last method declaration and makes it the current node.
        /// </summary>
        /// <returns>True if found and made current, false otherwise.</returns>
        public bool SelectLastMethodDeclaration()
        {
            var result = CurrentNode?.DescendantNodes().OfType<FieldDeclarationSyntax>().LastOrDefault();
            return NextStep(result);
        }

        #endregion

        #region Select constructor declarations

        /// <summary>
        /// Finds all constructors of a class, and makes them current.
        /// </summary>
        /// <returns>True if found and made current, false otherwise.</returns>
        public bool SelectOverloadedConstructorDeclarations()
        {
            var result = CurrentNode?.DescendantNodes().OfType<ConstructorDeclarationSyntax>().Cast<SyntaxNode>().ToList();
            return NextStep(result);
        }

        /// <summary>
        /// Finds a constructor with parameters' types and makes it the current node, if such exists.
        /// </summary>
        /// <returns>True if found and made current, false otherwise.</returns>
        public bool SelectConstructorDeclaration(params string[] parameterTypes)
        {
            SelectOverloadedConstructorDeclarations();

            foreach (var constructorDeclaration in CurrentNodesList)
            {
                bool areSame = CompareParameterTypes(constructorDeclaration as ConstructorDeclarationSyntax, parameterTypes);

                if (areSame)
                {
                    return NextStep(constructorDeclaration);
                }
            }

            return false;
        }

        /// <summary>
        /// Find last declared constructor and makes it the current node.
        /// </summary>
        /// <returns>True if found and made current, false otherwise.</returns>
        public bool SelectLastConstructorDeclaration()
        {
            SelectOverloadedConstructorDeclarations();
            var result = CurrentNodesList?.LastOrDefault();
            return NextStep(result);
        }
        #endregion

        #region Select destructor declaration

        /// <summary>
        /// A method that find a destructor and makes it the current node.
        /// </summary>
        /// <returns>True if found and made current, false otherwise.</returns>
        public bool SelectDestructorDeclaration()
        {
            var result = CurrentNode?.DescendantNodes().OfType<DestructorDeclarationSyntax>().FirstOrDefault();
            return NextStep(result);
        }

        #endregion

        #region Select conversion operator

        public bool SelectConversionOperatorDeclaration([NotBlank] string parameterType)
        {
            var conversionOperators = CurrentNode?.DescendantNodes().OfType<ConversionOperatorDeclarationSyntax>();
            foreach (var co in conversionOperators)
            {
                if(CompareParameterTypes(co, parameterType))
                {
                    return NextStep(co);
                }
            }

            return false;
        }

        #endregion

        #region Select operator

        const short PARAM_NUM_OF_UNARY = 1;
        const short PARAM_NUM_OF_BINARY = 2;


        public bool SelectUnaryOperatorDeclaration(string operatorToken)
        {
            var resultingOperator = CurrentNode?.DescendantNodes().OfType<OperatorDeclarationSyntax>()
                .Where(op => op.ParameterList.Parameters.Count == PARAM_NUM_OF_UNARY && op.OperatorToken.ToString() == operatorToken)
                .FirstOrDefault();

            return NextStep(resultingOperator);
        }

        public bool SelectAllUnaryOperatorDeclarations()
        {
            var resultingOperators = CurrentNode?.DescendantNodes().OfType<OperatorDeclarationSyntax>()
                .Where(op => op.ParameterList.Parameters.Count == PARAM_NUM_OF_UNARY).ToList<SyntaxNode>();

            return NextStep(resultingOperators);
        }

        public bool SelectBinaryOperatorDeclaration(string operatorToken, string firstParameterType, string secondParameterType)
        {
            var resultingOperators = GetBinaryOperators(operatorToken);

            foreach(var op in resultingOperators)
            {
                if(CompareParameterTypes(op, firstParameterType, secondParameterType))
                {
                    return NextStep(op);
                }
            }

            return false;
        }

        public bool SelectOverloadedBinaryOperatorDeclarations(string operatorToken)
        {

            var resultingOperators = GetBinaryOperators(operatorToken).ToList<SyntaxNode>();

            return NextStep(resultingOperators);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<OperatorDeclarationSyntax> GetBinaryOperators(string operatorToken)
        {
            return CurrentNode?.DescendantNodes().OfType<OperatorDeclarationSyntax>()
               .Where(op => op.OperatorToken.ToString() == operatorToken && op.ParameterList.Parameters.Count == PARAM_NUM_OF_BINARY);
        }

        #endregion

        #region Common functionalities

        /// <summary>
        /// A method that compares types of parameters of a given method, with expected values.
        /// </summary>
        /// <param name="node">A base method node. (It is in inheritance tree of constructors and ordinary methods)</param>
        /// <param name="parameterTypes">Expected parameters.</param>
        /// <returns>Returns true if parameters match, false otherwise.</returns>
        private bool CompareParameterTypes([NotNull] BaseMethodDeclarationSyntax node, params string[] parameterTypes)
        {
            var foundParameters = node.ParameterList.Parameters;

            if (foundParameters.Count() != parameterTypes.Count())
            {
                return false;   
            }

            for (int i = 0; i < foundParameters.Count(); i++)
            {
                string foundType = foundParameters[i].Type.ToString();
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
