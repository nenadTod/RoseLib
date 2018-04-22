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
                CompilationUnitComposer cuComposer =  new CompilationUnitComposer(reader);
                cuComposer.SelectNamespace();
                //cuComposer.Delete();
                
                var namespaceComposer = cuComposer.ToNamespaceComposer();
                namespaceComposer.SelectClassDeclaration("ClassSelector");
                //namespaceComposer.Delete();
                
                var classComposer = namespaceComposer.ToClassComposer();
                classComposer.RemoveStaticKeyword();
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
                
                Console.WriteLine(cuComposer.CurrentNode.ToFullString());
            }
        }
    }
}
