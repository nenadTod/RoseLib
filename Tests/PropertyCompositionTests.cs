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

namespace RoseLib.Tests
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
                    .SetAccessModifier(Enums.AccessModifiers.PROTECTED)
                    .SetType(newPropertyType)
                    .Rename(newPropertyName)
                    .SetAttributes(new List<Model.AttributeProps>() { new AttributeProps() { Name = newAttributeName } })
                    .GetCode();

                Assert.IsFalse(testRegexOPN.IsMatch(code));
                Assert.IsTrue(testRegexPN.IsMatch(code));
                Assert.IsTrue(testRegexModifiers.IsMatch(code));
                Assert.IsTrue(testRegexAttribute.IsMatch(code));
            }
        }
    }
}
