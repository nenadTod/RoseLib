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

        [Test]
        public void InterfaceWithAPropertyAndMethod()
        {
            var newNamespace = "RoseLib.Tests";
            Regex testRegexNS = new Regex(newNamespace);

            var newUsing = "System.Linq";
            Regex testRegexU = new Regex(newUsing);

            var newInterface = "ITestInterface";
            Regex testRegexI = new Regex(newInterface);

            var newPropertyName = "NewProperty";
            Regex testRegexP = new Regex(newPropertyName);

            var newMethodName = "NewMethod";
            Regex testRegexM = new Regex(newMethodName);

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
                    InterfaceName = newInterface,
                })
                .EnterInterface()
                .AddProperty(new Model.PropertyProperties
                {
                    AccessModifier = Enums.AccessModifierTypes.PUBLIC,
                    PropertyName = newPropertyName,
                    PropertyType = "string",
                })
                .AddMethod(new Model.MethodProperties
                {
                    AccessModifier = Enums.AccessModifierTypes.PUBLIC,
                    MethodName = newMethodName,
                    ReturnType = "string",
                });

            var code = composer.GetCode();
            Assert.IsTrue(testRegexNS.IsMatch(code));
            Assert.IsTrue(testRegexU.IsMatch(code));
            Assert.IsTrue(testRegexI.IsMatch(code));
            Assert.IsTrue(testRegexP.IsMatch(code));
            Assert.IsTrue(testRegexM.IsMatch(code));
        }

        [Test]
        public void SettingAttributes()
        {
            var attribute1Name = "Serializable";
            Regex testRegexA1 = new Regex(attribute1Name);

            var attribute2Name = "TestAttribute";
            Regex testRegexA2 = new Regex(attribute2Name);
            var attribute2Argument1 = "FakeEnum.Member1";
            Regex testRegexA2A1 = new Regex(attribute2Argument1);
            var attribute2Argument2 = "FakeEnum.Member2";
            Regex testRegexA2A2 = new Regex(attribute2Argument2);

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Interface1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectInterfaceDeclaration("Interface1")
                    .StartComposing<InterfaceComposer>()
                    .SetAttributes(
                        new List<Model.AttributeProperties>()
                        {
                            new Model.AttributeProperties { Name = attribute1Name },
                            new Model.AttributeProperties
                            {
                                Name = attribute2Name,
                                AttributeArgumentList = new List<string>
                                {
                                    attribute2Argument1,
                                    attribute2Argument2
                                }
                            }
                        })
                    .GetCode();

                Assert.IsTrue(testRegexA1.IsMatch(code));
                Assert.IsTrue(testRegexA2.IsMatch(code));
                Assert.IsTrue(testRegexA2A1.IsMatch(code));
                Assert.IsTrue(testRegexA2A2.IsMatch(code));
            }
        }

        [Test]
        public void DeletingNestedInterface()
        {
            var nestedInterface1 = "INestedInterface1";
            Regex testRegexNI1 = new Regex(nestedInterface1);

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Interface1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectInterfaceDeclaration(nestedInterface1)
                    .StartComposing<InterfaceComposer>(pivotOnParent: true)
                    .Delete()
                    .GetCode();

                Assert.IsFalse(testRegexNI1.IsMatch(code));
            }
        }

    }
}