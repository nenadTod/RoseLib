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

                CUNavigator navigator = new CUNavigator(reader);

                var code = navigator
                    .SelectClassDeclaration("WorkOrder")
                    .SelectFieldDeclaration("OrderId")
                    .StartComposing<FieldComposer>()
                    .SetType(newFieldType)
                    .GetCode();


                CPathEngine cSPathEngine = new CPathEngine();
                var navigator = cSPathEngine.Evaluate(
                    reader,
                    "/Class[name='WorkOrder]/Field[name='OrderId']'"
                    );

                var hashValue = navigator.GetSubtreeHashCode();
                if (!hashValue.Equals(previousHashValue))
                {
                    throw Exception("Code was changed!")
                }

                var output = navigator.StartComposing<FieldComposer>()
                .SetType("String")
                .GetCode();

                Assert.IsFalse(testRegexOFN.IsMatch(code));
                Assert.IsTrue(testRegexFN.IsMatch(code));
                Assert.IsTrue(testRegexModifiers.IsMatch(code));
                Assert.IsTrue(testRegexAttribute.IsMatch(code));
            }
        }
    }
}
