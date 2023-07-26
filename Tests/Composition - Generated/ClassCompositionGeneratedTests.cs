using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Composers;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace Tests.Composition
{
    public class ClassCompositionGeneratedTests
    {
        [Test]
        public void AddingGetOneMethodBeneathAnotherMember()
        {
            var referenceFieldName = "field2";
            Regex testRegexRF = new Regex(referenceFieldName);

            var newMethodName = "GetOne";
            Regex testRegexM = new Regex(newMethodName);

            var newMethodType = "Country";


            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectFieldDeclaration(referenceFieldName)
                    .StartComposing<ClassComposer>()
                    .AddGetOneMethod(newMethodName, newMethodType)
                    .GetCode();

                Assert.IsTrue(testRegexM.IsMatch(code));
                var codeParts = testRegexM.Split(code);
                Assert.IsTrue(testRegexRF.IsMatch(codeParts[0]));
            }
        }

    }
}