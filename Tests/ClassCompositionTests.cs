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
        public void ClassWithAField()
        {
            var newNamespace = "RoseLib.Tests";
            Regex testRegexNS = new Regex(newNamespace);

            var newUsing = "System.Linq";
            Regex testRegexU = new Regex(newUsing);

            var newClass = "TestClass";
            Regex testRegexC = new Regex(newClass);

            var newFieldName = "newField";
            Regex testRegexF = new Regex(newFieldName);

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
                .AddClass(new Model.ClassOptions { ClassName = newClass })
                .EnterClass()
                .AddField(new Model.FieldOptions { 
                    AccessModifier = Enums.AccessModifierTypes.PRIVATE,
                    FieldType = "string",
                    FieldName = newFieldName
                });

            var code = composer.GetCode();
            Assert.IsTrue(testRegexNS.IsMatch(code));
            Assert.IsTrue(testRegexU.IsMatch(code));
            Assert.IsTrue(testRegexC.IsMatch(code));
            Assert.IsTrue(testRegexF.IsMatch(code));
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
                    .StartComposing()
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
    }
}