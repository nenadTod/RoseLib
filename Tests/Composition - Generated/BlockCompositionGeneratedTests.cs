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
    public class BlockCompositionGeneratedTests
    {
        [Test]
        public void AddingIfClauseToMethod()
        {
            var referenceMethodName = "Method1";

            var str = "\"Hello World\"";
            Regex testRegexIC = new Regex(str);


            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectMethodDeclaration(referenceMethodName)
                    .StartComposing<MethodComposer>()
                    .EnterBody()
                    .AddIf(str)
                    .GetCode();

                Assert.IsTrue(testRegexIC.IsMatch(code));
            }
        }
    }
}
