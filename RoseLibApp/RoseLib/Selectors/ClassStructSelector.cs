using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System;
using System.Collections.Generic;
using RoseLibApp.RoseLib.Validation_Attributes;
using System.IO;
using System.Runtime.CompilerServices;
using RoseLibApp.RoseLib.Composers;

namespace RoseLibApp.RoseLib.Selectors
{
    public class ClassStructSelector<T> : NamespaceSelector<T> where T:IComposer
    {
        #region Constructors

        protected ClassStructSelector()
        {
        }

        public ClassStructSelector(StreamReader reader) : base(reader)
        {
            // TODO: Only files containing class declaration should be accepted?
        }

        public ClassStructSelector(ClassDeclarationSyntax node) : base(node)
        {
        }

        #endregion

        #region Converters

        public MethodComposer ToMethodComposer() //TODO: Solve the situation between Constructors, Coverter Operators, Operators.
        {
            if (CurrentNode != null && CurrentNode is BaseMethodDeclarationSyntax)
            {
                return new MethodComposer(CurrentNode as MethodDeclarationSyntax, Composer);
            }

            if (CurrentNodesList != null && CurrentNodesList.Any())
            {
                if (CurrentNodesList.First() is BaseMethodDeclarationSyntax)
                {
                    return new MethodComposer(CurrentNodesList.Cast<MethodDeclarationSyntax>().ToList());
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
        public T SelectFieldDeclaration([NotBlank] string fieldName)
        {
            var fieldDeclarations = CurrentNode?.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();
            foreach (var fieldDeclaration in fieldDeclarations)
            {
                var declaratorExists = fieldDeclaration.DescendantNodes().OfType<VariableDeclaratorSyntax>().
                    Where(d => d.Identifier.ValueText == fieldName).Any();

                if (declaratorExists)
                {
                    NextStep(fieldDeclaration);
                }
            }

            return Composer;
        }



        /// <summary>
        /// Finds the last field declaration of a given name (if such exists) and it makes it the current node.
        /// </summary>
        /// <returns>True if found and made current, false otherwise.</returns>
        public T SelectLastFieldDeclaration()
        {
            var result = CurrentNode?.DescendantNodes().OfType<FieldDeclarationSyntax>().LastOrDefault();
            NextStep(result);

            return Composer;
        }

        #endregion

        #region Select property declarations

        /// <summary>
        /// Finds a property declaration of a given name (if such exists) and makes it the current node.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        ///  <returns>True if found and made current, false otherwise.</returns>
        public T SelectPropertyDeclaration([NotBlank] string propertyName)
        {
            var result = CurrentNode?.DescendantNodes().OfType<PropertyDeclarationSyntax>().
                Where(p => p.Identifier.ValueText == propertyName).FirstOrDefault();

            NextStep(result);
            return Composer;
        }

        /// <summary>
        /// Finds the last property declaration of a given name (if such exists) and makes it the current node.
        /// </summary>
        /// <returns>True if found and made current, false otherwise.</returns>
        public T SelectLastPropertyDeclaration()
        {
            var result = CurrentNode?.DescendantNodes().OfType<PropertyDeclarationSyntax>().LastOrDefault();
            NextStep(result);
            return Composer;
        }

        #endregion

        #region Select method declarations

        /// <summary>
        /// Finds occurances of (possibly overloaded) methods with a specified name, if such exist, and makes them current.
        /// </summary>
        /// <param name="methodName">Method's name</param>
        /// <returns>True if any found and made current, false otherwise.</returns>
        public T SelectOverloadedMethodDeclarations([NotBlank] string methodName)
        {
            var allMethods = CurrentNode?.DescendantNodes().OfType<MethodDeclarationSyntax>();

            var result = allMethods?.Where(p => p.Identifier.ValueText == methodName).Cast<SyntaxNode>().ToList();
            NextStep(result);
            return Composer;
        }

        /// <summary>
        /// Finds a method with a specified name (if such exists) and makes it the current node. If there is method overloading the first one is made current. 
        /// </summary>
        /// <param name="methodName">Method's name</param>
        /// <returns>True if found and made current, false otherwise.</returns>
        public T SelectMethodDeclaration([NotBlank] string methodName)
        {
            SelectOverloadedMethodDeclarations(methodName);

            var result = CurrentNodesList?.FirstOrDefault();
            NextStep(result);
            return Composer;
        }

        /// <summary>
        /// Finds a method with a specified name and parameter types, if such exists, and makes it the current node.
        /// </summary>
        /// <param name="methodName">Method's name</param>
        /// <param name="parameterTypes">Array of strings representing parameters' types.</param>
        /// <returns>True if found and made current, false otherwise.</returns>
        public T SelectMethodDeclaration([NotBlank] string methodName, params string[] parameterTypes)
        {
            SelectOverloadedMethodDeclarations(methodName);

            foreach (var methodDeclaration in CurrentNodesList)
            {
                bool areSame = CompareParameterTypes(methodDeclaration as MethodDeclarationSyntax, parameterTypes);

                if (areSame)
                {
                    NextStep(methodDeclaration);
                }
            }

            return Composer;
        }

        /// <summary>
        /// Finds the last method declaration and makes it the current node.
        /// </summary>
        /// <returns>True if found and made current, false otherwise.</returns>
        public T SelectLastMethodDeclaration()
        {
            var result = CurrentNode?.DescendantNodes().OfType<FieldDeclarationSyntax>().LastOrDefault();
            NextStep(result);
            return Composer;
        }

        #endregion

        #region Select constructor declarations

        /// <summary>
        /// Finds all constructors of a class, and makes them current.
        /// </summary>
        /// <returns>True if found and made current, false otherwise.</returns>
        public T SelectOverloadedConstructorDeclarations()
        {
            var result = CurrentNode?.DescendantNodes().OfType<ConstructorDeclarationSyntax>().Cast<SyntaxNode>().ToList();
            NextStep(result);
            return Composer;
        }

        /// <summary>
        /// Finds a constructor with parameters' types and makes it the current node, if such exists.
        /// </summary>
        /// <returns>True if found and made current, false otherwise.</returns>
        public T SelectConstructorDeclaration(params string[] parameterTypes)
        {
            SelectOverloadedConstructorDeclarations();

            foreach (var constructorDeclaration in CurrentNodesList)
            {
                bool areSame = CompareParameterTypes(constructorDeclaration as ConstructorDeclarationSyntax, parameterTypes);

                if (areSame)
                {
                    NextStep(constructorDeclaration);
                }
            }

            return Composer;
        }

        /// <summary>
        /// Find last declared constructor and makes it the current node.
        /// </summary>
        /// <returns>True if found and made current, false otherwise.</returns>
        public T SelectLastConstructorDeclaration()
        {
            SelectOverloadedConstructorDeclarations();
            var result = CurrentNodesList?.LastOrDefault();
            NextStep(result);
            return Composer;
        }
        #endregion

        #region Select destructor declaration

        /// <summary>
        /// A method that find a destructor and makes it the current node.
        /// </summary>
        /// <returns>True if found and made current, false otherwise.</returns>
        public T SelectDestructorDeclaration()
        {
            var result = CurrentNode?.DescendantNodes().OfType<DestructorDeclarationSyntax>().FirstOrDefault();
            NextStep(result);
            return Composer;
        }

        #endregion

        #region Select conversion operator

        public T SelectConversionOperatorDeclaration([NotBlank] string parameterType)
        {
            var conversionOperators = CurrentNode?.DescendantNodes().OfType<ConversionOperatorDeclarationSyntax>();
            foreach (var co in conversionOperators)
            {
                if(CompareParameterTypes(co, parameterType))
                {
                    NextStep(co);
                }
            }

            return Composer;
        }

        #endregion

        #region Select operator

        const short PARAM_NUM_OF_UNARY = 1;
        const short PARAM_NUM_OF_BINARY = 2;


        public T SelectUnaryOperatorDeclaration(string operatorToken)
        {
            var resultingOperator = CurrentNode?.DescendantNodes().OfType<OperatorDeclarationSyntax>()
                .Where(op => op.ParameterList.Parameters.Count == PARAM_NUM_OF_UNARY && op.OperatorToken.ToString() == operatorToken)
                .FirstOrDefault();

            NextStep(resultingOperator);
            return Composer;
        }

        public T SelectAllUnaryOperatorDeclarations()
        {
            var resultingOperators = CurrentNode?.DescendantNodes().OfType<OperatorDeclarationSyntax>()
                .Where(op => op.ParameterList.Parameters.Count == PARAM_NUM_OF_UNARY).ToList<SyntaxNode>();

            NextStep(resultingOperators);
            return Composer;
        }

        public T SelectBinaryOperatorDeclaration(string operatorToken, string firstParameterType, string secondParameterType)
        {
            var resultingOperators = GetBinaryOperators(operatorToken);

            foreach(var op in resultingOperators)
            {
                if(CompareParameterTypes(op, firstParameterType, secondParameterType))
                {
                    NextStep(op);
                }
            }

            return Composer;
        }

        public T SelectOverloadedBinaryOperatorDeclarations(string operatorToken)
        {

            var resultingOperators = GetBinaryOperators(operatorToken).ToList<SyntaxNode>();

            NextStep(resultingOperators);
            return Composer;
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
