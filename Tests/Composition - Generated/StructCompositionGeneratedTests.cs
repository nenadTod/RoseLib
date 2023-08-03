using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Composers;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace Tests.Composition
{
    public class StructCompositionGeneratedTests
    {
        [Test]
        public void AddingGetOneMethodBeneathAnotherMember()
        {
            var referenceFieldName = "field1";
            Regex testRegexRF = new Regex(referenceFieldName);

            var newMethodName = "Method123";
            Regex testRegexM = new Regex(newMethodName);

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Struct1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectFieldDeclaration(referenceFieldName)
                    .StartComposing<StructComposer>()
                    .AddSimpleMethod(newMethodName)
                    .GetCode();

                Assert.IsTrue(testRegexM.IsMatch(code));
                var codeParts = testRegexM.Split(code);
                Assert.IsTrue(testRegexRF.IsMatch(codeParts[0]));
            }
        }

    }
}