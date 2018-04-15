using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System;
using System.Collections.Generic;
using RoseLibApp.RoseLib.Validation_Attributes;
using System.IO;

namespace RoseLibApp.RoseLib.Selectors
{
    public class ClassSelector : BaseSelector
    {
        #region Constructors

        public ClassSelector(StreamReader reader) : base(reader)
        {
            // TODO: Only files containing class declaration should be accepted?
        }

        public ClassSelector(ClassDeclarationSyntax node) : base(node)
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

        #region Finding field declarations

        /// <summary>
        /// Finds a field declaration of a given name, if such exists.
        /// </summary>
        /// <param name="fieldName">Name of the variable being declared</param>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindFieldDeclaration([NotBlank] string fieldName)
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
        /// Finds the last field declaration, if such exists.
        /// </summary>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindLastFieldDeclaration()
        {
            var result = CurrentNode?.DescendantNodes().OfType<FieldDeclarationSyntax>().LastOrDefault();
            return NextStep(result);
        }

        #endregion

        #region Finding property declarations

        /// <summary>
        /// Finds a property declaration of a given name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        ///  <returns>True if found, false otherwise.</returns>
        public bool FindPropertyDeclaration([NotBlank] string propertyName)
        {
            var result = CurrentNode?.DescendantNodes().OfType<PropertyDeclarationSyntax>().
                Where(p => p.Identifier.ValueText == propertyName).FirstOrDefault();

            return NextStep(result);
        }

        /// <summary>
        /// Finds the last property declaration.
        /// </summary>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindLastPropertyDeclaration()
        {
            var result = CurrentNode?.DescendantNodes().OfType<PropertyDeclarationSyntax>().LastOrDefault();
            return NextStep(result);
        }

        #endregion

        #region Finding method declarations

        /// <summary>
        /// Finds and returns occurances of (possibly overloaded) methods with a specified name, if such exist.
        /// </summary>
        /// <param name="methodName">Method's name</param>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindOverloadedMethodDeclarations([NotBlank] string methodName)
        {
            var allMethods = CurrentNode?.DescendantNodes().OfType<MethodDeclarationSyntax>();

            var result = allMethods?.Where(p => p.Identifier.ValueText == methodName).Cast<SyntaxNode>().ToList();
            return NextStep(result);
        }

        /// <summary>
        /// Finds and returns a method with a specified name if such exists. If there is method overloading, returns the first one.
        /// </summary>
        /// <param name="methodName">Method's name</param>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindMethodDeclaration([NotBlank] string methodName)
        {
            FindOverloadedMethodDeclarations(methodName);

            var result = CurrentNodesList?.FirstOrDefault();
            return NextStep(result);
        }

        /// <summary>
        /// Finds and returns a method with a specified name and parameter types, if such exists.
        /// </summary>
        /// <param name="methodName">Method's name</param>
        /// <param name="parameterTypes">Array of strings representing parameters' types.</param>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindMethodDeclaration([NotBlank] string methodName, params string[] parameterTypes)
        {
            FindOverloadedMethodDeclarations(methodName);

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
        /// Finds the last method declaration.
        /// </summary>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindLastMethodDeclaration()
        {
            var result = CurrentNode?.DescendantNodes().OfType<FieldDeclarationSyntax>().LastOrDefault();
            return NextStep(result);
        }

        #endregion

        #region Finding constructor declarations

        /// <summary>
        /// Finds all constructors of a class
        /// </summary>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindOverloadedConstructorDeclarations()
        {
            var result = CurrentNode?.DescendantNodes().OfType<ConstructorDeclarationSyntax>().Cast<SyntaxNode>().ToList();
            return NextStep(result);
        }

        /// <summary>
        /// Finds and returns a constructor with parameters' types, if such exists.
        /// </summary>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindConstructorDeclaration(params string[] parameterTypes)
        {
            FindOverloadedConstructorDeclarations();

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
        /// Find last declared constructor
        /// </summary>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindLastConstructorDeclaration()
        {
            FindOverloadedConstructorDeclarations();
            var result = CurrentNodesList?.LastOrDefault();
            return NextStep(result);
        }
        #endregion

        #region Finding destructor declaration

        /// <summary>
        /// A method that find a destructor.
        /// </summary>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindDestructor()
        {
            var result = CurrentNode?.DescendantNodes().OfType<DestructorDeclarationSyntax>().FirstOrDefault();
            return NextStep(result);
        }

        #endregion

        #region Common functionalities

        /// <summary>
        /// A method that compares types of a parameters of a given method, with expected values.
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
