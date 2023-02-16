using IronPython.Compiler.Ast;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Composers;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System.Text.RegularExpressions;

namespace RoseLib.Tests
{
    public class NamespaceAndTypeNavigationTests
    {
    [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SelectFieldByName()
        {
            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);
                navigator.SelectFieldDeclaration("field1");

                var selection = navigator.State.Peek();
                var fieldDeclaration = selection.CurrentNode as FieldDeclarationSyntax;

                Assert.NotNull(fieldDeclaration);
            }
        }

        [Test]
        public void SelectLastFieldClosestDepthwise()
        {
            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);
                navigator.SelectLastFieldDeclaration();
                
                var selection = navigator.State.Peek();
                var fieldDeclaration = selection.CurrentNode as FieldDeclarationSyntax;
                var variableDeclaratorWithName = fieldDeclaration?.DescendantNodes()
                    .OfType<VariableDeclaratorSyntax>()
                    .Where(vd => vd.Identifier.ValueText == "field1")
                    .FirstOrDefault();

                Assert.NotNull(variableDeclaratorWithName);
            }
        }

        [Test]
        public void SelectLastFieldInInnerClass()
        {
            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);
                navigator.SelectClassDeclaration("InnerClass1").SelectLastFieldDeclaration();

                var selection = navigator.State.Peek();
                var fieldDeclaration = selection.CurrentNode as FieldDeclarationSyntax;
                var variableDeclaratorWithName = fieldDeclaration?.DescendantNodes()
                    .OfType<VariableDeclaratorSyntax>()
                    .Where(vd => vd.Identifier.ValueText == "field3")
                    .FirstOrDefault();

                Assert.NotNull(variableDeclaratorWithName);
            }
        }


        [Test]
        public void SelectLastFieldInFirstInnerClass()
        {
            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                Regex regex = new Regex("InnerClass");

                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);
                navigator.SelectClassDeclaration(regex).SelectLastFieldDeclaration();

                var selection = navigator.State.Peek();
                var fieldDeclaration = selection.CurrentNode as FieldDeclarationSyntax;
                var variableDeclaratorWithName = fieldDeclaration?.DescendantNodes()
                    .OfType<VariableDeclaratorSyntax>()
                    .Where(vd => vd.Identifier.ValueText == "field3")
                    .FirstOrDefault();

                Assert.NotNull(variableDeclaratorWithName);
            }
        }

        [Test]
        public void SelectEnumByName()
        {
            var foundEnumName = "Enum1";
            Regex testRegexE = new Regex(foundEnumName);

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Enum1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);
                navigator.SelectEnumDeclaration(foundEnumName);

                var selection = navigator.State.Peek();
                var enumDeclaration = selection.CurrentNode as EnumDeclarationSyntax;

                Assert.NotNull(enumDeclaration);
                Assert.IsTrue(testRegexE.IsMatch(enumDeclaration.Identifier.Text));
            }
        }

        [Test]
        public void SelectEnumAndEnumMemberByName()
        {
            var foundEnumName = "Enum1";
            var foundEnumMemberName = "blue";
            Regex testRegexEM = new Regex(foundEnumMemberName);

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Enum1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);
                navigator
                    .SelectEnumDeclaration(foundEnumName)
                    .SelectEnumMemberDeclaration(foundEnumMemberName);

                var selection = navigator.State.Peek();
                var enumMemberDeclaration = selection.CurrentNode as EnumMemberDeclarationSyntax;

                Assert.NotNull(enumMemberDeclaration);
                Assert.IsTrue(testRegexEM.IsMatch(enumMemberDeclaration.Identifier.Text));
            }
        }
    }
}