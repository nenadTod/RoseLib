using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoseLibApp.RoseLib.Composers;
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
                CompilationUnitComposer cuc = new CompilationUnitComposer(reader);
                cuc.SelectNamespace();
                var namespaceComposer = cuc.ToNamespaceComposer();
                namespaceComposer.SelectClassDeclaration("ClassSelector");
                var cc = namespaceComposer.ToClassComposer();
                cc.Rename("Ninja");

                Console.WriteLine(cuc.CurrentNode.ToFullString());
            }
        }
    }
}
