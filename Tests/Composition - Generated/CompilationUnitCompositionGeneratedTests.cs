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
    public class CompilationUnitCompositionGeneratedTests
    {
        [Test]
        public void AddingNamespaceWithAClassToCU()
        {

            var namespaceName = "Tests.Composition";
            var className = "Class2";

            Regex testRegexRN = new Regex(namespaceName);


            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .StartComposing<CompilationUnitComposer>()
                    .AddVSClass(namespaceName, className)
                    .GetCode();

                Assert.IsTrue(testRegexRN.IsMatch(code));
            }
        }
    }
}
