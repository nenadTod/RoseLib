﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Composers;
using RoseLib.Exceptions;
using RoseLib.Model;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tests.Composition
{
    public class MethodAndBlockCompositionTests
    {
        [Test]
        public void MethodWithConsoleLog()
        {
            var newMethodName = "TestM";
            Regex testRegexMN = new Regex(newMethodName);

            var newMethodType = "bool";
            Regex testRegexMT = new Regex(newMethodType);

            var newStatement = "Console.WriteLine(\"It works!\");";
            Regex testRegexS = new Regex("Console\\.WriteLine");

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var classComposer = navigator
                    .SelectClassDeclaration("Class1")
                    .StartComposing<ClassComposer>();
                var code = classComposer.AddMethod(new MethodProps() {
                        MethodName = newMethodName,
                        ReturnType = newMethodType,
                        Attributes = new List<RoseLib.Model.AttributeProps>() {
                            new AttributeProps() { Name = "TestAtt" } 
                        }
                    })
                    .EnterMethod()
                    .EnterBody()
                    .InsertStatements(newStatement)
                    .GetCode();

                Assert.IsTrue(testRegexMN.IsMatch(code));
                Assert.IsTrue(testRegexMT.IsMatch(code));
                Assert.IsTrue(testRegexS.IsMatch(code));
                Assert.IsNotNull(classComposer.Visitor.CurrentNode as StatementSyntax);
            }
        }

        [Test]
        public void MethodWithInvalidConsoleLogStatement()
        {
            var newMethodName = "TestM";
            var newMethodType = "bool";
            var newStatement = "Console.WriteLine(\"It works!\")";

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                try
                {
                    navigator
                        .SelectClassDeclaration("Class1")
                        .StartComposing<ClassComposer>()
                        .AddMethod(new MethodProps() { MethodName = newMethodName, ReturnType = newMethodType })
                        .EnterMethod()
                        .SetAttributes(new List<RoseLib.Model.AttributeProps>() { new AttributeProps() { Name = "TestAtt" } })
                        .EnterBody()
                        .InsertStatements(newStatement);

                    Assert.Fail("Syntax error not caught!");
                }
                catch (CodeHasErrorsException)
                {
                    Assert.Pass("Syntax error caught!");
                }
            }
        }

        [Test]
        public void MethodWithNestedBlocks()
        {
            var newMethodName = "TestM";
            Regex testRegexMN = new Regex(newMethodName);

            var newMethodType = "bool";
            Regex testRegexMT = new Regex(newMethodType);

            var newSubblockStatement = "if(4>3){}";
            var newStatement = "Console.WriteLine(\"It works!\");";
            Regex testRegexS = new Regex("Console\\.WriteLine");

            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CompilationUnitNavigator navigator = new CompilationUnitNavigator(reader);

                var classComposer = navigator
                    .SelectClassDeclaration("Class1")
                    .StartComposing<ClassComposer>();

                classComposer.AddMethod(new MethodProps()
                {
                    MethodName = newMethodName,
                    ReturnType = newMethodType,
                    Attributes = new List<RoseLib.Model.AttributeProps>() {
                                new AttributeProps() { Name = "TestAtt" }
                            }
                });

                var methodComposer = classComposer.EnterMethod();

                var bodyComposer = methodComposer.EnterBody();
                bodyComposer.InsertStatements(newSubblockStatement);
                bodyComposer.EnterSubblock();
                bodyComposer.InsertStatements(newStatement);

                var code = bodyComposer.GetCode();

                Assert.IsTrue(testRegexMN.IsMatch(code));
                Assert.IsTrue(testRegexMT.IsMatch(code));
                Assert.IsTrue(testRegexS.IsMatch(code));
                Assert.IsNotNull(classComposer.Visitor.CurrentNode as StatementSyntax);

                classComposer.Visitor.State.Pop();
                Assert.IsNotNull(classComposer.Visitor.CurrentNode as BlockSyntax);

                classComposer.Visitor.State.Pop();
                Assert.IsNotNull(classComposer.Visitor.CurrentNode as StatementSyntax);

                classComposer.Visitor.State.Pop();
                Assert.IsNotNull(classComposer.Visitor.CurrentNode as BlockSyntax);
            }
        }
    }
}
