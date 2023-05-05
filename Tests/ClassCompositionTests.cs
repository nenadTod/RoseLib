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
                .AddClass(new Model.ClassProps {  
                    ClassName = newClass
                })
                .EnterClass()
                .AddField(new Model.FieldProps { 
                    AccessModifier = Enums.AccessModifierTypes.PRIVATE,
                    FieldType = "string",
                    FieldName = newFieldName
                })
                .AddProperty(new Model.PropertyProps
                {
                    AccessModifier = Enums.AccessModifierTypes.PUBLIC,
                    PropertyName = newPropertyName,
                    PropertyType= "string",
                })
                .AddMethod(new Model.MethodProps {
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
        public void PublicClassWithAttributesAndBaseList()
        {
            var newNamespace = "RoseLib.Tests";
            Regex testRegexNS = new Regex(newNamespace);

            var newClass = "TestClass";
            Regex testRegexC = new Regex(newClass);

            var testRegexPublic = new Regex("public");

            var baseType1 = "object";
            var baseType2 = $"IEquatable<{newClass}>";
            var testRegexBaseTypes = new Regex($": {baseType1}, {baseType2}");

            var attribute1 = "Serializable";
            var attribute2 = "DllImport";
            var testRegexAttribute1 = new Regex($"[{attribute1}]");
            var testRegexAttribute2 = new Regex($"[{attribute2}(]");

            CompilationUnitComposer composer = new CompilationUnitComposer();
            composer
                .AddUsingDirectives(
                    "System"
                )
                .AddNamespace(newNamespace)
                .EnterNamespace()
                .AddClass(new Model.ClassProps
                {
                    AccessModifier = Enums.AccessModifierTypes.PUBLIC,
                    ClassName = newClass,
                    BaseTypes = new List<string>() { baseType1, baseType2 },
                    Attributes = new List<Model.AttributeProps> {
                        new Model.AttributeProps(){ Name = attribute1},
                        new Model.AttributeProps()
                        {
                            Name = attribute2,
                            AttributeArgs = new List<string>
                            {
                                "user32.dll", "SetLastError=false", "ExactSpelling=false"
                            }
                        }
                    }
                });

            var code = composer.GetCode();
            Assert.IsTrue(testRegexNS.IsMatch(code));
            Assert.IsTrue(testRegexC.IsMatch(code));
            Assert.IsTrue(testRegexPublic.IsMatch(code));
            Assert.IsTrue(testRegexBaseTypes.IsMatch(code));
            Assert.IsTrue(testRegexAttribute1.IsMatch(code));
            Assert.IsTrue(testRegexAttribute2.IsMatch(code));
        }

        [Test]
        public void RenamingClass()
        {
            var newClassName = "RenamedClass";
            Regex testRegexC1 = new Regex(newClassName);


            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectClassDeclaration("Class1")
                    .StartComposing<ClassComposer>()
                    .Rename(newClassName)
                    .GetCode();

                Assert.That(testRegexC1.Matches(code).Count, Is.EqualTo(3));
            }
        }

        [Test]
        public void SettingBaseTypes()
        {
            var baseType1 = "object";
            var baseType2 = $"IEquatable<{"Class1"}>";
            var testRegexBaseTypes = new Regex($": {baseType1}, {baseType2}");


            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectClassDeclaration("Class1")
                    .StartComposing<ClassComposer>()
                    .SetBaseTypes(new List<string> { baseType1, baseType2 })
                    .GetCode();

                Assert.IsTrue(testRegexBaseTypes.IsMatch(code));
            }

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectClassDeclaration("Class1")
                    .StartComposing<ClassComposer>()
                    .SetBaseTypes(new List<string> { baseType1, baseType2 })
                    .SetBaseTypes(null)
                    .GetCode();

                Assert.IsFalse(testRegexBaseTypes.IsMatch(code));
            }
        }

        [Test]
        public void MakingClassPublic()
        {
            var classWithPublic = "public class";
            Regex testRegexC1 = new Regex(classWithPublic);


            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectClassDeclaration("Class1")
                    .StartComposing<ClassComposer>()
                    .SetAccessModifier(Enums.AccessModifierTypes.PUBLIC)
                    .GetCode();

                Assert.IsTrue(testRegexC1.IsMatch(code));
            }
        }

        [Test]
        public void MakingClassStatic()
        {
            var @static = "static";
            Regex testRegexC1 = new Regex(@static);


            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectClassDeclaration("Class1")
                    .StartComposing<ClassComposer>()
                    .MakeStatic()
                    .GetCode();

                Assert.IsTrue(testRegexC1.IsMatch(code));
            }
        }

        [Test]
        public void MakingClassPartialAndNonPartial()
        {
            var partial = "partial";
            Regex testRegexC1 = new Regex(partial);


            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectClassDeclaration("Class1")
                    .StartComposing<ClassComposer>()
                    .MakePartial()
                    .GetCode();

                Assert.IsTrue(testRegexC1.IsMatch(code));
            }

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectClassDeclaration("Class1")
                    .StartComposing<ClassComposer>()
                    .MakePartial()
                    .MakeNonPartial()
                    .GetCode();

                Assert.IsFalse(testRegexC1.IsMatch(code));
            }
        }

        [Test]
        public void AddingFieldBeneathAnotherOne()
        {
            var referenceFieldName = "field2";
            Regex testRegexRF = new Regex(referenceFieldName);

            var newFieldName = "newField25";
            Regex testRegexF = new Regex(newFieldName);

            var newFieldType = "string";
            var newFieldAccessModifier = Enums.AccessModifierTypes.PRIVATE;


            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectFieldDeclaration(referenceFieldName)
                    .StartComposing<ClassComposer>()
                    .AddField(new Model.FieldProps()
                    {
                        FieldName = newFieldName,
                        FieldType = newFieldType,
                        AccessModifier = newFieldAccessModifier
                    })
                    .GetCode();

                Assert.IsTrue(testRegexF.IsMatch(code));
                var codeParts = testRegexF.Split(code);
                Assert.IsTrue(testRegexRF.IsMatch(codeParts[0]));
            }
        }

        [Test]
        public void AddingPropertyBeneathAnotherMember()
        {
            var referenceFieldName = "field2";
            Regex testRegexRF = new Regex(referenceFieldName);

            var newPropertyName = "NewProperty";
            Regex testRegexP = new Regex(newPropertyName);

            var newPropertyType = "string";
            var newPropertyAccessModifier = Enums.AccessModifierTypes.PUBLIC;


            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectFieldDeclaration(referenceFieldName)
                    .StartComposing<ClassComposer>()
                    .AddProperty(new Model.PropertyProps()
                    {
                        PropertyName = newPropertyName,
                        PropertyType = newPropertyType,
                        AccessModifier = newPropertyAccessModifier
                    })
                    .GetCode();

                Assert.IsTrue(testRegexP.IsMatch(code));
                var codeParts = testRegexP.Split(code);
                Assert.IsTrue(testRegexRF.IsMatch(codeParts[0]));
            }
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
                    .StartComposing<FieldComposer>()
                    .Rename(newFieldName)
                    .SetType(newFieldType)
                    .SetAccessModifier(newFieldAccessModifier)
                    .GetCode();

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




            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectClassDeclaration("Class1")
                    .StartComposing<ClassComposer>()
                    .SetAttributes(
                        new List<Model.AttributeProps>() 
                        { 
                            new Model.AttributeProps { Name = attribute1Name },
                            new Model.AttributeProps 
                            { 
                                Name = attribute2Name,
                                AttributeArgs = new List<string>
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