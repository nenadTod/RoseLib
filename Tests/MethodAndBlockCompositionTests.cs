using RoseLib.Composers;
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
    public class MethodAndBlockCompositionTests
    {
        [Test]
        public void MethodWithConsoleLog()
        {
            var newMethodName = "TestM";
            Regex testRegexMN = new Regex(newMethodName);

            var newMethodType = "bool";
            Regex testRegexMT = new Regex(newMethodType);

            var newStatement = "Console.WriteLine(\"It works!\");";
            Regex testRegexS = new Regex("Console\\.WriteLine");

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectClassDeclaration("Class1")
                    .StartComposing<ClassComposer>()
                    .AddMethod(new RoseLib.Model.MethodProperties() { MethodName = newMethodName, ReturnType = newMethodType })
                    .EnterMethod()
                    .SetAttributes(new List<Model.AttributeProperties>() { new AttributeProperties() { Name = "TestAtt" } })
                    .EnterBody()
                    .InsertStatements(newStatement)
                    .GetCode();

                Assert.IsTrue(testRegexMN.IsMatch(code));
                Assert.IsTrue(testRegexMT.IsMatch(code));
                Assert.IsTrue(testRegexS.IsMatch(code));

            }
        }
    }
}
