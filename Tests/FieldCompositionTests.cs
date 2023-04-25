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
    public class FieldCompositionTests
    {
        [Test]
        public void EditField()
        {
            var oldFieldName = "field1";
            Regex testRegexOFN = new Regex(oldFieldName);

            var newFieldName = "testF";
            Regex testRegexFN = new Regex(newFieldName);

            var newFieldType = "int";
            Regex testRegexModifiers = new Regex($"protected {newFieldType}");

            var newAttributeName = "TestAtt";
            Regex testRegexAttribute = new Regex(newAttributeName);

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectFieldDeclaration(oldFieldName)
                    .StartComposing<FieldComposer>()
                    .SetAccessModifier(Enums.AccessModifierTypes.PROTECTED)
                    .SetType(newFieldType)
                    .Rename(newFieldName)
                    .SetAttributes(new List<Model.AttributeProperties>() { new AttributeProperties() { Name = newAttributeName } })
                    .GetCode();

                Assert.IsFalse(testRegexOFN.IsMatch(code));
                Assert.IsTrue(testRegexFN.IsMatch(code));
                Assert.IsTrue(testRegexModifiers.IsMatch(code));
                Assert.IsTrue(testRegexAttribute.IsMatch(code));
            }
        }
    }
}
