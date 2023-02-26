using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Composers;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace RoseLib.Tests
{
    public class InterfaceCompositionTests
    {
        [Test]
        public void EmptyInterface()
        {
            var newNamespace = "RoseLib.Tests";
            Regex testRegexNS = new Regex(newNamespace);

            var newUsing = "System.Linq";
            Regex testRegexU = new Regex(newUsing);

            var newInterface = "ITestInterface";
            Regex testRegexI = new Regex(newInterface);

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
                .AddInterface(new Model.InterfaceProperties
                {
                    InterfaceName = newInterface
                });

            var code = composer.GetCode();
            Assert.IsTrue(testRegexNS.IsMatch(code));
            Assert.IsTrue(testRegexU.IsMatch(code));
            Assert.IsTrue(testRegexI.IsMatch(code));
        }

        [Test]
        public void PublicInterfaceWithAttributesAndBaseList()
        {
            var newNamespace = "RoseLib.Tests";
            Regex testRegexNS = new Regex(newNamespace);

            var newInterface = "ITestInterface";
            Regex testRegexI = new Regex(newInterface);

            var testRegexPublic = new Regex("public");

            var baseType1 = $"IEquatable<{newInterface}>";
            var testRegexBaseTypes = new Regex($": {baseType1}");

            var attribute1 = "CustomAttribute";
            var testRegexAttribute1 = new Regex($"[{attribute1}]");

            CompilationUnitComposer composer = new CompilationUnitComposer();
            composer
                .AddUsingDirectives(
                    "System"
                )
                .AddNamespace(newNamespace)
                .EnterNamespace()
                .AddInterface(new Model.InterfaceProperties
                {
                    AccessModifier = Enums.AccessModifierTypes.PUBLIC,
                    InterfaceName = newInterface,
                    BaseTypes = new List<string>() { baseType1 },
                    Attributes = new List<Model.AttributeProperties> {
                        new Model.AttributeProperties(){ Name = attribute1},
                    }
                });

            var code = composer.GetCode();
            Assert.IsTrue(testRegexNS.IsMatch(code));
            Assert.IsTrue(testRegexI.IsMatch(code));
            Assert.IsTrue(testRegexPublic.IsMatch(code));
            Assert.IsTrue(testRegexBaseTypes.IsMatch(code));
            Assert.IsTrue(testRegexAttribute1.IsMatch(code));
        }
    }
}