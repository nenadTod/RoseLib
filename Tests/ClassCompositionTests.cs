using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Composers;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace RoseLib.Tests
{
    public class ClassCompositionTests
    {
        [Test]
        public void ClassWithAFieldPropertyAndMethod()
        {
            var newNamespace = "RoseLib.Tests";
            Regex testRegexNS = new Regex(newNamespace);

            var newUsing = "System.Linq";
            Regex testRegexU = new Regex(newUsing);

            var newClass = "TestClass";
            Regex testRegexC = new Regex(newClass);

            var newFieldName = "newField";
            Regex testRegexF = new Regex(newFieldName);

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
                .AddClass(new Model.ClassOptions {  
                    AccessModifier = Enums.AccessModifierTypes.PUBLIC,
                    ClassName = newClass,
                    BaseTypes = new List<string>() { "object", $"IEquatable<{newClass}>" }, 
                    Attributes = new List<Model.Attribute> { 
                        new Model.Attribute(){ Name = "Serializable"},
                        new Model.Attribute()
                        {
                            Name = "DllImport",
                            AttributeArgumentList = new List<string>
                            {
                                "user32.dll", "SetLastError=false", "ExactSpelling=false"
                            }
                        }
                    }
                })
                .EnterClass()
                .AddField(new Model.FieldOptions { 
                    AccessModifier = Enums.AccessModifierTypes.PRIVATE,
                    FieldType = "string",
                    FieldName = newFieldName
                })
                .AddProperty(new Model.PropertyOptions
                {
                    AccessModifier = Enums.AccessModifierTypes.PUBLIC,
                    PropertyName = newPropertyName,
                    PropertyType= "string",
                })
                .AddMethod(new Model.MethodOptions {
                    AccessModifier= Enums.AccessModifierTypes.PUBLIC,
                    MethodName = newMethodName,
                    ReturnType = "string",
                });

            var code = composer.GetCode();
            Assert.IsTrue(testRegexNS.IsMatch(code));
            Assert.IsTrue(testRegexU.IsMatch(code));
            Assert.IsTrue(testRegexC.IsMatch(code));
            Assert.IsTrue(testRegexF.IsMatch(code));
            Assert.IsTrue(testRegexP.IsMatch(code));
            Assert.IsTrue(testRegexM.IsMatch(code));
        }


        [Test]
        public void UpdateField()
        {
            var newFieldName = "newField";
            Regex testRegexF = new Regex(newFieldName);

            var newFieldType = "string";
            var newFieldAccessModifier = Enums.AccessModifierTypes.PRIVATE;


            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);
                
                var code = navigator
                    .SelectFieldDeclaration("field1")
                    .StartComposing<ClassComposer>()
                    .UpdateField(new Model.FieldOptions()
                    {
                        FieldName = newFieldName,
                        FieldType = newFieldType,
                        AccessModifier = newFieldAccessModifier
                    }
                ).GetCode();

                Assert.IsTrue(testRegexF.IsMatch(code));
            }
        }

        [Test]
        public void DeleteField()
        {
            var fieldForDeletionName = "field5";
            Regex testRegexF = new Regex(fieldForDeletionName);

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectFieldDeclaration(fieldForDeletionName)
                    .StartComposing<ClassComposer>()
                    .Delete()
                    .GetCode();

                Assert.IsFalse(testRegexF.IsMatch(code));
            }
        }

        [Test]
        public void AddingAttributes()
        {
            var attribute1Name = "Serializable";
            Regex testRegexA1 = new Regex(attribute1Name);

            var attribute2Name = "TestAttribute";
            Regex testRegexA2 = new Regex(attribute2Name);
            var attribute2Argument1 = "FakeEnum.Member1";
            Regex testRegexA2A1 = new Regex(attribute2Argument1);
            var attribute2Argument2 = "FakeEnum.Member2";
            Regex testRegexA2A2 = new Regex(attribute2Argument2);




            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectClassDeclaration("Class1")
                    .StartComposing<ClassComposer>()
                    .SetClassAttributes(
                        new List<Model.Attribute>() 
                        { 
                            new Model.Attribute { Name = attribute1Name },
                            new Model.Attribute 
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
    }
}