using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoseLibApp.RoseLib.Composers;
using RoseLibApp.RoseLib.Model;
using RoseLibApp.RoseLib.Selectors;
using System;
using System.IO;

namespace RoseLibApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (StreamReader reader = new StreamReader(".\\RoseLib\\Test Cases\\ClassSelector.cs.t"))
            {
                //CompilationUnitComposer cuComposer =  new CompilationUnitComposer(reader);
                //cuComposer.SelectNamespace();
                //cuComposer.Delete();

                //var namespaceComposer = cuComposer.ToNamespaceComposer();
                //namespaceComposer.AddClass(new ClassOptions() { ClassName = "NewClass", IsPartial = true, IsStatic=true});
                //namespaceComposer.SelectClassDeclaration("ClassSelector");
                //namespaceComposer.Delete();

                //var classComposer = namespaceComposer.ToClassComposer();
                //classComposer.RemoveStaticKeyword();
                //classComposer.SetAccessModifier(ClassComposer.AccessModifierTypes.INTERNAL);
                //classComposer.Rename("RenamedClass");
                //classComposer.SelectMethodDeclaration("FindOverloadedConstructorDeclarations");
                //classComposer.SetAccessModifier(ClassComposer.AccessModifierTypes.PRIVATE);

                //classComposer.SelectMethodDeclaration("FindFieldDeclaration");
                //var mc = classComposer.ToMethodComposer();
                //mc.Rename("RenamedMethod");
                //mc.ReturnType("string");
                //mc.Parameters(new RLParameter("first", "string"));
                //mc.AppendParameters(new RLParameter("second", "bool"));
                //mc.SelectLastStatement();
                //mc.InsertStatementsBefore("Console.WriteLine(\"HelloWorld\");");
                //mc.InsertStatementsBefore("Console.WriteLine(\"HelloWorldBefore\");");
                //mc.InsertStatementsAfter("Console.WriteLine(\"HelloWorldAfter\");");

                //classComposer.SelectMethodDeclaration("FindLastFieldDeclaration");
                //var mc1 = classComposer.ToMethodComposer();
                //mc1.Body("Console.WriteLine(\"HelloWorld\");");
                //mc1.InsertStatements("Console.WriteLine(\"Hello\");", "Console.WriteLine(\"World\");");
                //mc1.SelectLastStatement().Delete();


                //classComposer.SelectMethodDeclaration("FindLastPropertyDeclaration");
                //classComposer.Delete();
                //classComposer.SelectLastConstructorDeclaration();
                //classComposer.Delete();
                //classComposer.SelectPropertyDeclaration("Dummy");
                //classComposer.Delete();

                CompilationUnitComposer cuComposer = new CompilationUnitComposer();
                cuComposer.AddUsing("System")
                    .AddUsing("System.IO")
                    .AddNamespace("test.namespace");

                cuComposer.SelectNamespace();
                var nsComposer = cuComposer.ToNamespaceComposer();
                nsComposer.AddClass(new ClassOptions() { ClassName = "TestClass", AccessModifier = RoseLib.Enums.AccessModifierTypes.INTERNAL });
                nsComposer.SelectClassDeclaration("TestClass");

                Console.WriteLine(cuComposer.CurrentNode.NormalizeWhitespace().ToFullString());
            }
        }
    }
}
