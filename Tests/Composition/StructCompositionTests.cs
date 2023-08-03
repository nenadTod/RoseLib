using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Composers;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace Tests.Composition
{
    public class StructCompositionTests
    {
        [Test]
        public void StructWithAFieldPropertyAndMethod()
        {
            var newNamespace = "RoseLib.Tests";
            Regex testRegexNS = new Regex(newNamespace);

            var newUsing = "System.Linq";
            Regex testRegexU = new Regex(newUsing);

            var newStruct = "TestStruct";
            Regex testRegexC = new Regex(newStruct);

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
                .AddStruct(new RoseLib.Model.StructProps
                {
                    StructName = newStruct
                })
                .EnterStruct()
                .AddField(new RoseLib.Model.FieldProps
                {
                    AccessModifier = RoseLib.Enums.AccessModifiers.PRIVATE,
                    FieldType = "string",
                    FieldName = newFieldName
                })
                .AddProperty(new RoseLib.Model.PropertyProps
                {
                    AccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC,
                    PropertyName = newPropertyName,
                    PropertyType = "string",
                })
                .AddMethod(new RoseLib.Model.MethodProps
                {
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
        public void RenamingStruct()
        {
            var newStructName = "RenamedStruct";
            Regex testRegexS1 = new Regex(newStructName);


            using (StreamReader reader = new StreamReader(".\\TestFiles\\Struct1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectStructDeclaration("Struct1")
                    .StartComposing<StructComposer>()
                    .Rename(newStructName)
                    .GetCode();

                Assert.That(testRegexS1.Matches(code).Count, Is.EqualTo(2));
            }
        }

        [Test]
        public void MakingStructPublic()
        {
            var structWithPublic = "public struct";
            Regex testRegexC1 = new Regex(structWithPublic);


            using (StreamReader reader = new StreamReader(".\\TestFiles\\Struct1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectStructDeclaration("Struct1")
                    .StartComposing<StructComposer>()
                    .SetAccessModifier(RoseLib.Enums.AccessModifiers.PUBLIC)
                    .GetCode();

                Assert.IsTrue(testRegexC1.IsMatch(code));
            }
        }


        [Test]
        public void AddingFieldBeneathAnotherOne()
        {
            var referenceFieldName = "field1";
            Regex testRegexRF = new Regex(referenceFieldName);

            var newFieldName = "newField15";
            Regex testRegexF = new Regex(newFieldName);

            var newFieldType = "string";
            var newFieldAccessModifier = RoseLib.Enums.AccessModifiers.PRIVATE;


            using (StreamReader reader = new StreamReader(".\\TestFiles\\Struct1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectFieldDeclaration(referenceFieldName)
                    .StartComposing<StructComposer>()
                    .AddField(new RoseLib.Model.FieldProps()
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
            var referenceFieldName = "field1";
            Regex testRegexRF = new Regex(referenceFieldName);

            var newPropertyName = "NewProperty";
            Regex testRegexP = new Regex(newPropertyName);

            var newPropertyType = "string";
            var newPropertyAccessModifier = RoseLib.Enums.AccessModifiers.PUBLIC;


            using (StreamReader reader = new StreamReader(".\\TestFiles\\Struct1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectFieldDeclaration(referenceFieldName)
                    .StartComposing<StructComposer>()
                    .AddProperty(new RoseLib.Model.PropertyProps()
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
        public void DeleteField()
        {
            var fieldForDeletionName = "field1";
            Regex testRegexF = new Regex(fieldForDeletionName);

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Struct1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var code = navigator
                    .SelectFieldDeclaration(fieldForDeletionName)
                    .StartComposing<StructComposer>()
                    .Delete()
                    .GetCode();

                Assert.IsFalse(testRegexF.IsMatch(code));
            }
        }


    }
}