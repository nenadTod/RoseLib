using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib;
using RoseLib.Composers;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Miscellaneous
{
    public class RoslynHelperTests
    {
        [Test]
        public void TestExtractingClassMemberName()
        {
            string className = "Class1";

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var classMember = navigator
                    .SelectClassDeclaration(className)
                    .State
                    .Peek()
                    .CurrentNode as MemberDeclarationSyntax;

                Assert.IsNotNull(classMember);

                var extractedName = RoslynHelper.GetMemberName(classMember);
                Assert.IsNotNull(extractedName);
                Assert.That(extractedName, Is.EqualTo(className));
            }
        }

        [Test]
        public void TestExtractingNamespaceMemberName()
        {
            string namespaceName = "Tests.TestFiles";

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var namespaceMember = navigator
                    .SelectNamespace(namespaceName)
                    .State
                    .Peek()
                    .CurrentNode as MemberDeclarationSyntax;

                Assert.IsNotNull(namespaceMember);

                var extractedName = RoslynHelper.GetMemberName(namespaceMember);
                Assert.IsNotNull(extractedName);
                Assert.That(extractedName, Is.EqualTo(namespaceName));
            }
        }

        [Test]
        public void TestExtractingFieldMemberName()
        {
            string namespaceName = "field1";

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var namespaceMember = navigator
                    .SelectFieldDeclaration(namespaceName)
                    .State
                    .Peek()
                    .CurrentNode as MemberDeclarationSyntax;

                Assert.IsNotNull(namespaceMember);

                var extractedName = RoslynHelper.GetMemberName(namespaceMember);
                Assert.IsNotNull(extractedName);
                Assert.That(extractedName, Is.EqualTo(namespaceName));
            }
        }
    }
}
