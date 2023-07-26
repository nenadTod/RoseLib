using RoseLib.Composers;
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
    public class NamespaceCompositionGeneratedTests
    {
        [Test]
        public void AddingControllerToNamespace()
        {
            var referenceNamespaceName = "Tests.TestFiles";

            var resourceName = "Country";
            var resourceNamePlural = "Countries";
            var resourcePath = "countries";

            Regex testRegexRN = new Regex(resourceName);


            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectNamespace(referenceNamespaceName)
                    .StartComposing<NamespaceComposer>()
                    .AddController(resourcePath, resourceName, resourceNamePlural)
                    .GetCode();

                Assert.IsTrue(testRegexRN.IsMatch(code));
            }
        }
    }
}
