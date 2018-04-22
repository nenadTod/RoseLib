using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System;
using System.Collections.Generic;
using RoseLibApp.RoseLib.Validation_Attributes;

namespace RoseLibApp.RoseLib.Selectors.Test
{
    protected internal static class ClassSelector : BaseSelector
    {
        #region Constructors

        public ClassSelector() : base()
        {
        }

		public ClassSelector(int num) : base()
        {
        }

		public ClassSelector(SyntaxNode node, int num) : base(node)
        {
        }

        public ClassSelector(SyntaxNode node) : base(node)
        {
        }

        public ClassSelector(List<SyntaxNode> nodes) : base(nodes)
        {
        }

        #endregion

		#region
		public int Dummy {get;set;}

		#endregion

        #region Finding field declarations

        /// <summary>
        /// Finds a field declaration of a given name, if such exists.
        /// </summary>
        /// <param name="root">Root node</param>
        /// <param name="fieldName">Name of the variable being declared</param>
        public void FindFieldDeclaration([NotNull] SyntaxNode root, [NotBlank] string fieldName)
        {
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName");
            }

            var fieldDeclarations = root.DescendantNodes().OfType<FieldDeclarationSyntax>().ToList();
            foreach (var fieldDeclaration in fieldDeclarations)
            {
                var declaratorExists = fieldDeclaration.DescendantNodes().OfType<VariableDeclaratorSyntax>().
                    Where(d => d.Identifier.ValueText == fieldName).Any();

                if (declaratorExists)
                {
                    CurrentNode = fieldDeclaration;
                }
            }

            CurrentNode = null;
        }

        /// <summary>
        /// Finds the last field declaration, if such exists.
        /// </summary>
        /// <param name="root">Root node</param>
        public void FindLastFieldDeclaration([NotNull] SyntaxNode root)
        {
            CurrentNode = root.DescendantNodes().OfType<FieldDeclarationSyntax>().LastOrDefault();
        }

        #endregion

        #region Finding property declarations

        /// <summary>
        /// Finds a property declaration of a given name.
        /// </summary>
        /// <param name="root">Root node</param>
        /// <param name="propertyName">Name of the property.</param>
        public void FindPropertyDeclaration([NotNull] SyntaxNode root, [NotBlank] string propertyName)
        {
            CurrentNode = root.DescendantNodes().OfType<PropertyDeclarationSyntax>().
                Where(p => p.Identifier.ValueText == propertyName).FirstOrDefault();
        }

        /// <summary>
        /// Finds the last property declaration.
        /// </summary>
        /// <param name="root">Root node</param>
        public void FindLastPropertyDeclaration([NotNull] SyntaxNode root)
        {
            CurrentNode = root.DescendantNodes().OfType<PropertyDeclarationSyntax>().LastOrDefault();
        }

        #endregion

        #region Finding method declarations

        /// <summary>
        /// Finds and returns occurances of (possibly overloaded) methods with a specified name, if such exist.
        /// </summary>
        /// <param name="root">Root node</param>
        /// <param name="methodName">Method's name</param>
        public void FindOverloadedMethodDeclarations([NotNull] SyntaxNode root, [NotBlank] string methodName)
        {
            var allMethods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

            CurrentNodesList = allMethods.Where(p => p.Identifier.ValueText == methodName).Cast<SyntaxNode>().ToList();
        }

        /// <summary>
        /// Finds and returns a method with a specified name if such exists. If there is method overloading, returns the first one.
        /// </summary>
        /// <param name="root">Root node</param>
        /// <param name="methodName">Method's name</param>
        public void FindMethodDeclaration([NotNull] SyntaxNode root, [NotBlank] string methodName)
        {
            FindOverloadedMethodDeclarations(root, methodName);

            CurrentNode = CurrentNodesList.FirstOrDefault();
        }

        /// <summary>
        /// Finds and returns a method with a specified name and parameter types, if such exists.
        /// </summary>
        /// <param name="root">Root node</param>
        /// <param name="methodName">Method's name</param>
        public void FindMethodDeclaration([NotNull] SyntaxNode root, [NotBlank] string methodName, params string[] parameterTypes)
        {
            FindOverloadedMethodDeclarations(root, methodName);

            if (CurrentNodesList.Count == 0)
            {
                CurrentNode = null;
            }

            foreach (var methodDeclaration in CurrentNodesList)
            {
                bool areSame = CompareParameterTypes(methodDeclaration as MethodDeclarationSyntax, parameterTypes);

                if (areSame)
                {
                    CurrentNode = methodDeclaration;
                }
            }

            CurrentNode = null;
        }

        /// <summary>
        /// Finds the last method declaration.
        /// </summary>
        /// <param name="root">Root node</param>
        public void FindLastMethodDeclaration([NotNull] SyntaxNode root)
        {
            CurrentNode = root.DescendantNodes().OfType<FieldDeclarationSyntax>().LastOrDefault();
        }

        #endregion

        #region Finding constructor declarations

        /// <summary>
        /// Finds all constructors of a class
        /// <param name="root">Root node</param>
        public void FindOverloadedConstructorDeclarations([NotNull] SyntaxNode root)
        {
            CurrentNodesList = root.DescendantNodes().OfType<ConstructorDeclarationSyntax>().Cast<SyntaxNode>().ToList();
        }

        /// <summary>
		/// Finds a parameterless constructor of a class. 
		/// </summary>
		/// <param name="root">Root node</param>
		public void FindParameterlessConstructorDeclaration([NotNull] SyntaxNode root)
        {
            FindOverloadedConstructorDeclarations(root);

            var constructors = CurrentNodesList
                .Where(n => 
                {
                    var c = n as ConstructorDeclarationSyntax;
                    return c.ParameterList.DescendantNodes().Count() == 0;
                })
                .ToList();

            CurrentNode = constructors.FirstOrDefault();
        }

        /// <summary>
        /// Finds and returns a constructor with parameters' types, if such exists.
        /// </summary>
        /// <param name="root">Root node</param>
        public void FindConstructorDeclaration([NotNull] SyntaxNode root, params string[] parameterTypes)
        {
            FindOverloadedConstructorDeclarations(root);

            if (CurrentNodesList.Count == 0)
            {
                CurrentNode = null;
            }

            foreach (var constructorDeclaration in CurrentNodesList)
            {
                bool areSame = CompareParameterTypes(constructorDeclaration as ConstructorDeclarationSyntax, parameterTypes);

                if (areSame)
                {
                    CurrentNode = constructorDeclaration;
                }
            }

            CurrentNode = null;
        }

        #endregion

        #region Finding destructor declaration

        /// <summary>
        /// A method that find a destructor.
        /// </summary>
        /// <param name="root">Root node</param>
        public void FindDestructor([NotNull] SyntaxNode root)
        {
            CurrentNode = root.DescendantNodes().OfType<DestructorDeclarationSyntax>().FirstOrDefault();
        }

        #endregion

        #region Common functionalities

        /// <summary>
        /// A method that compares types of a parameters of a given method, with expected values.
        /// </summary>
        /// <param name="node">A base method node. (It is in inheritance tree of constructors and ordinary methods)</param>
        /// <param name="parameterTypes">Expected parameters.</param>
        /// <returns></returns>
        private bool CompareParameterTypes([NotNull] BaseMethodDeclarationSyntax node, params string[] parameterTypes)
        {
            var foundParameters = node.ParameterList.Parameters;

            if (foundParameters.Count() != parameterTypes.Count())
            {
                return false;   
            }

            for (int i = 0; i < foundParameters.Count(); i++)
            {
                string foundType = (foundParameters[i].Type as ITypeSymbol).ToDisplayString();
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
