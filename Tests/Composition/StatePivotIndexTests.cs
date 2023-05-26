using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Composers;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

namespace Tests.Composition
{
    public class StatePivotIndexTests
    {

        [Test]
        public void FieldSelectionAndClassComposer()
        {
            var fieldToFind = "field2";

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var classComposer = navigator
                    .SelectFieldDeclaration(fieldToFind)
                    .StartComposing<ClassComposer>();

                var head = classComposer.Visitor.State.Pop();
                Assert.True(head.CurrentNode?.GetType() == typeof(FieldDeclarationSyntax));
                var behindTheHead = classComposer.Visitor.State.Pop();
                Assert.True(behindTheHead.CurrentNode?.GetType() == typeof(ClassDeclarationSyntax));
                Assert.That(classComposer.StatePivotIndex, Is.EqualTo(1));
            }
        }

        [Test]
        public void ClassSelectionAndClassComposer()
        {
            var classToFind = "InnerClass1";

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var classComposer = navigator
                    .SelectClassDeclaration(classToFind)
                    .StartComposing<ClassComposer>();

                var head = classComposer.Visitor.State.Pop();
                Assert.True(head.CurrentNode?.GetType() == typeof(ClassDeclarationSyntax));
                Assert.That(classComposer.StatePivotIndex, Is.EqualTo(1));
            }
        }
    }
}