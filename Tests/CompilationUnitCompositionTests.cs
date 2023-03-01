using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Composers;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace RoseLib.Tests
{
    public class CompilationUnitCompositionTests
    {
        

        [Test]
        public void AddNamespace()
        {
            var newNamespace = "RoseLib.Composers";
            Regex testRegex = new Regex(newNamespace);

            CompilationUnitComposer composer = new CompilationUnitComposer();
            composer.AddNamespace(newNamespace);

            var code = composer.GetCode();
            Assert.IsTrue(testRegex.IsMatch(code));

        }

        [Test]
        public void AddUsing()
        {
            var newUsing = "Roselib.Composers";
            Regex testRegex = new Regex(newUsing);

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .StartComposing()
                    .AddUsingDirectives(newUsing)
                    .GetCode();
                
                Assert.IsTrue(testRegex.IsMatch(code));
            }
        }

        [Test]
        public void AddNoneUsing()
        {
            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                try
                {
                    var returnedComposer = navigator
                    .StartComposing()
                    .AddUsingDirectives();
                    Assert.NotNull(returnedComposer);
                }
                catch (Exception e)
                {
                    Assert.Fail($"Shouldn't have failed if none added. Exception: {e}");
                }
            }
        }

        [Test]
        public void DeletingANamespace()
        {
            var @namespace = "Tests.TestFiles";
            Regex testRegexN = new Regex(@namespace);

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Interface1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectNamespace(@namespace)
                    .StartComposing<CompilationUnitComposer>()
                    .Delete()
                    .GetCode();

                Assert.IsFalse(testRegexN.IsMatch(code));
            }
        }
    }
}