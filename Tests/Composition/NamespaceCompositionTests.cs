using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Composers;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace Tests.Composition
{
    public class NamespaceCompositionTests
    {

        [Test]
        public void DefaultVSClass()
        {
            var newNamespace = "RoseLib.Tests";
            Regex testRegexNS = new Regex(newNamespace);

            var newUsing = "System.Linq";
            Regex testRegexU = new Regex(newUsing);

            var newClass = "TestClass";
            Regex testRegexC = new Regex(newClass);

            CompilationUnitComposer composer = new CompilationUnitComposer();
            composer
                .AddUsingDirectives(
                    "System",
                    "System.Collections.Generic",
                    "System.Linq",
                    "System.Text",
                    "System.Threading.Tasks"
                )
                .AddNamespace(newNamespace)
                .EnterNamespace()
                .AddClass(new RoseLib.Model.ClassProps { ClassName = newClass });

            var code = composer.GetCode();
            Assert.IsTrue(testRegexNS.IsMatch(code));
            Assert.IsTrue(testRegexU.IsMatch(code));
            Assert.IsTrue(testRegexC.IsMatch(code));
        }



        /// <summary>
        /// Important for testing if all nodes in the state remain from the same tree 
        /// </summary>
        [Test]
        public void MultipleClassesInANamespace()
        {
            var newNamespace = "RoseLib.Tests";
            Regex testRegexNS = new Regex(newNamespace);

            var newUsing = "System.Linq";
            Regex testRegexU = new Regex(newUsing);

            var newClass1 = "TestClass1";
            Regex testRegexC1 = new Regex(newClass1);


            var newClass2 = "TestClass2";
            Regex testRegexC2 = new Regex(newClass2);

            CompilationUnitComposer composer = new CompilationUnitComposer();
            composer
                .AddUsingDirectives(
                    "System",
                    "System.Collections.Generic",
                    "System.Linq",
                    "System.Text",
                    "System.Threading.Tasks"
                )
                .AddNamespace(newNamespace)
                .EnterNamespace()
                .AddClass(new RoseLib.Model.ClassProps { ClassName = newClass1 })
                .AddClass(new RoseLib.Model.ClassProps { ClassName = newClass2 });

            var code = composer.GetCode();
            Assert.IsTrue(testRegexNS.IsMatch(code));
            Assert.IsTrue(testRegexU.IsMatch(code));
            Assert.IsTrue(testRegexC1.IsMatch(code));
            Assert.IsTrue(testRegexC2.IsMatch(code));
        }

        [Test]
        public void DeletingAnInterface()
        {
            var @interface = "Interface1";
            Regex testRegexI = new Regex(@interface);

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Interface1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectInterfaceDeclaration(@interface)
                    .StartComposing<NamespaceComposer>()
                    .Delete()
                    .GetCode();

                Assert.IsFalse(testRegexI.IsMatch(code));

            }
        }
    }
}