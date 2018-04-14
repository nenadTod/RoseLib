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
        }

        public ClassSelector(SyntaxNode node) : base(node)
        {
        }

        public ClassSelector(List<SyntaxNode> nodes) : base(nodes)
        {
        }

        #endregion

        #region Converters

        public MethodSelector ToMethodSelector()
        {
            if(CurrentNode != null && CurrentNode is BaseMethodDeclarationSyntax)
            {
                return new MethodSelector(CurrentNode);
            }

            if(CurrentNodesList != null && CurrentNodesList.Any())
            {
                if(CurrentNodesList.First() is BaseMethodDeclarationSyntax)
                {
                    return new MethodSelector(CurrentNodesList);
                }
            }

            return null;
        }

        #endregion

        #region Finding field declarations

        #region Experimental
        /// <summary>
        /// Experimental
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public bool TryFindFieldDeclaration([NotBlank] string fieldName)
        {
            return FindFieldDeclarationNew(fieldName) != null;
        }

        /// <summary>
        /// Experimental
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public bool GoToFieldDeclaration([NotBlank] string fieldName)
        {
            CurrentNode = FindFieldDeclarationNew(fieldName);
            return CurrentNode != null;
        }
        
        /// <summary>
        /// Finds a field declaration of a given name, if such exists.
        /// </summary>
        /// <param name="fieldName">Name of the variable being declared</param>
        /// <returns>Field declaration if found, null otherwise.</returns>
        private FieldDeclarationSyntax FindFieldDeclarationNew([NotBlank] string fieldName)
        {
            var fieldDeclarations = CurrentNode?.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();
            foreach (var fieldDeclaration in fieldDeclarations)
            {
                var declaratorExists = fieldDeclaration.DescendantNodes().OfType<VariableDeclaratorSyntax>().
                    Where(d => d.Identifier.ValueText == fieldName).Any();

                if (declaratorExists)
                {
                    return fieldDeclaration;
                }
            }

            return null;
        }
        #endregion

        /// <summary>
        /// Finds a field declaration of a given name, if such exists.
        /// </summary>
        /// <param name="fieldName">Name of the variable being declared</param>
        /// <returns>True if found, false otherwise.</returns>
        public FieldDeclarationSyntax FindFieldDeclaration([NotBlank] string fieldName)
        {
            var fieldDeclarations = CurrentNode?.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();
            foreach (var fieldDeclaration in fieldDeclarations)
            {
                var declaratorExists = fieldDeclaration.DescendantNodes().OfType<VariableDeclaratorSyntax>().
                    Where(d => d.Identifier.ValueText == fieldName).Any();

                if (declaratorExists)
                {
                    CurrentNode = fieldDeclaration;
                    return true;
                }
            }

            CurrentNode = null;
            return null;
        }

        #region Experimental

        /// <summary>
        /// Experimental
        /// </summary>
        /// <returns>True if found, false otherwise.</returns>
        public bool TryFindLastFieldDeclaration()
        {
            return FindLastFieldDeclarationNew() != null;
        }

        /// <summary>
        /// Experimental
        /// </summary>
        /// <returns></returns>
        public bool GoToLastFieldDeclartion()
        {
            CurrentNode = FindLastFieldDeclarationNew();
            return CurrentNode != null;
        }

        /// <summary>
        /// Finds the last field declaration, if such exists.
        /// </summary>
        /// <returns>Last field declaration if any, null otherwise.</returns>
        private FieldDeclarationSyntax FindLastFieldDeclarationNew()
        {
            return CurrentNode?.DescendantNodes().OfType<FieldDeclarationSyntax>().LastOrDefault();
        }

        #endregion

        /// <summary>
        /// Finds the last field declaration, if such exists.
        /// </summary>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindLastFieldDeclaration()
        {
            CurrentNode = CurrentNode?.DescendantNodes().OfType<FieldDeclarationSyntax>().LastOrDefault();
            return CurrentNode != null;
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
            CurrentNode = CurrentNode?.DescendantNodes().OfType<PropertyDeclarationSyntax>().
                Where(p => p.Identifier.ValueText == propertyName).FirstOrDefault();
            return CurrentNode != null; 
        }

        /// <summary>
        /// Finds the last property declaration.
        /// </summary>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindLastPropertyDeclaration()
        {
            CurrentNode = CurrentNode?.DescendantNodes().OfType<PropertyDeclarationSyntax>().LastOrDefault();
            return CurrentNode != null;
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

            CurrentNodesList = allMethods?.Where(p => p.Identifier.ValueText == methodName).Cast<SyntaxNode>().ToList();
            return CurrentNodesList != null;
        }

        /// <summary>
        /// Finds and returns a method with a specified name if such exists. If there is method overloading, returns the first one.
        /// </summary>
        /// <param name="methodName">Method's name</param>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindMethodDeclaration([NotBlank] string methodName)
        {
            FindOverloadedMethodDeclarations(methodName);

            CurrentNode = CurrentNodesList?.FirstOrDefault();
            return CurrentNode != null;
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
                    CurrentNode = methodDeclaration;
                    return true;
                }
            }

            CurrentNode = null;
            return false;
        }

        /// <summary>
        /// Finds the last method declaration.
        /// </summary>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindLastMethodDeclaration()
        {
            CurrentNode = CurrentNode?.DescendantNodes().OfType<FieldDeclarationSyntax>().LastOrDefault();
            return CurrentNode != null;
        }

        #endregion

        #region Finding constructor declarations

        /// <summary>
        /// Finds all constructors of a class
        /// </summary>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindOverloadedConstructorDeclarations()
        {
            CurrentNodesList = CurrentNode?.DescendantNodes().OfType<ConstructorDeclarationSyntax>().Cast<SyntaxNode>().ToList();
            return CurrentNodesList != null;
        }

        /// <summary>
		/// Finds a parameterless constructor of a class. 
		/// </summary>
        /// <returns>True if found, false otherwise.</returns>
		public bool FindParameterlessConstructorDeclaration()
        {
            FindOverloadedConstructorDeclarations();

            var constructors = CurrentNodesList
                ?.Where(n => 
                {
                    var c = n as ConstructorDeclarationSyntax;
                    return c.ParameterList.DescendantNodes().Count() == 0;
                })
                .ToList();

            CurrentNode = constructors?.FirstOrDefault();
            return CurrentNode != null;
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
                    CurrentNode = constructorDeclaration;
                    return true;
                }
            }

            CurrentNode = null;
            return false;
        }

        /// <summary>
        /// Find last declared constructor
        /// </summary>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindLastConstructorDeclaration()
        {
            FindOverloadedConstructorDeclarations();
            CurrentNode = CurrentNodesList?.LastOrDefault();
            return CurrentNode != null;
        }
        #endregion

        #region Finding destructor declaration

        /// <summary>
        /// A method that find a destructor.
        /// </summary>
        /// <returns>True if found, false otherwise.</returns>
        public bool FindDestructor()
        {
            CurrentNode = CurrentNode?.DescendantNodes().OfType<DestructorDeclarationSyntax>().FirstOrDefault();
            return CurrentNode != null;
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
