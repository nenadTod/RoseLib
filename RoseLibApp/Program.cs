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
                var namespaceComposer = cuComposer.ToNamespaceComposer();
                namespaceComposer.SelectClassDeclaration("ClassSelector");

                var classComposer = namespaceComposer.ToClassComposer();
                classComposer.Rename("RenamedClass");
                classComposer.SelectMethodDeclaration("FindFieldDeclaration");

                var mc = classComposer.ToMethodComposer();
                mc.Rename("RenamedMethod");
                mc.ReturnType("string");
                mc.Parameters(new RLParameter("first", "string"));
                mc.AppendParameters(new RLParameter("second", "bool"));

                Console.WriteLine(cuComposer.CurrentNode.ToFullString());
            }
        }
    }
}
