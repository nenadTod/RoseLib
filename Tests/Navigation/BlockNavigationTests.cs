using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Navigation
{
    public class BlockNavigationTests
    {
        [Test]
        public void SelectLastStatement()
        { 
            string testMethodName = "Method1";

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);
                navigator
                    .SelectMethodDeclaration(testMethodName)
                    .SelectLastStatementDeclaration();


                var selection = navigator.State.Peek();
                var returnStatement = selection.CurrentNode as ReturnStatementSyntax;

                Assert.NotNull(returnStatement);
            }
        }
    }
}
