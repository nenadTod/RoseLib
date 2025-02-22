using Microsoft.CodeAnalysis.CSharp;
using RoseLib.Composers;
using RoseLib.Exceptions;
using RoseLib.Model;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tests.Composition
{
    public class PropertyCompositionTests
    {
        [Test]
        public void EditProperty()
        {
            var oldPropertyName = "Prop1";
            Regex testRegexOPN = new Regex(oldPropertyName);

            var newPropertyName = "TestP";
            Regex testRegexPN = new Regex(newPropertyName);

            var newPropertyType = "bool";
            Regex testRegexModifiers = new Regex($"protected {newPropertyType}");

            var newAttributeName = "TestAtt";
            Regex testRegexAttribute = new Regex(newAttributeName);

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectPropertyDeclaration(oldPropertyName)
                    .StartComposing<PropertyComposer>()
                    .SetAccessModifier(RoseLib.Enums.AccessModifiers.PROTECTED)
                    .SetType(newPropertyType)
                    .Rename(newPropertyName)
                    .SetAttributes(new List<RoseLib.Model.AttributeProps>() { new AttributeProps() { Name = newAttributeName } })
                    .GetCode();

                Assert.IsFalse(testRegexOPN.IsMatch(code));
                Assert.IsTrue(testRegexPN.IsMatch(code));
                Assert.IsTrue(testRegexModifiers.IsMatch(code));
                Assert.IsTrue(testRegexAttribute.IsMatch(code));

                var newCodeNavigator = new CompilationUnitNavigator(SyntaxFactory.ParseCompilationUnit(code));
                var propertyHash = newCodeNavigator
                    .SelectPropertyDeclaration(newPropertyName)
                    .GetSubtreeHashCode();
                Assert.IsNotEmpty(propertyHash);

            }
        }

        [Test]
        public void AddPropertyWithGet()
        {
            var existingPropertyName = "Prop1";

            var newPropertyType = "bool";
            Regex testRegexType = new Regex($"{newPropertyType}");

            var newPropertyName = "Prop2";
            Regex testRegexName = new Regex($"{newPropertyName}");

            var newPropertyGetStatement = "throw new Exception(\"Not Implemented\");";
            Regex testStatement = new Regex($"throw new Exception");

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectPropertyDeclaration(existingPropertyName)
                    .StartComposing<ClassComposer>()
                    .AddProperty(new PropertyProps()
                    {
                        PropertyName = newPropertyName,
                        PropertyType = newPropertyType,
                        AccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC,
                        PropertyWith = PropertyWith.Get,
                        InitializeEmptyAccessorBodies = true
                    }
                    )
                    .EnterProperty()
                    .EnterGetBody()
                    .InsertStatements(newPropertyGetStatement)
                    .GetCode();

                Assert.IsTrue(testRegexType.IsMatch(code));
                Assert.IsTrue(testRegexName.IsMatch(code));
                Assert.IsTrue(testStatement.IsMatch(code));
            }
        }
    }
}
