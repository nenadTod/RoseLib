using RoseLib.Composers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IronPython.Hosting;
using IronPython.Compiler;
using static IronPython.Modules._ast;
using System.IO;
using System.Xml.Linq;
using Assert = NUnit.Framework.Assert;
using RoseLib.CSPath;
using RoseLib.CSPath.Engine;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Tests.CSPath
{
    public class EngineTests
    {
        [Test]
        public void TestEngineInitialization()
        {
            CPathEngine cPathEngine = new CPathEngine();

            Assert.Pass();
        }

        [Test]
        public void TestEngineRun()
        {
            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CPathEngine cSPathEngine = new CPathEngine();
                var selectedSyntaxNode = cSPathEngine.Evaluate(reader, "/Namespace");


                Assert.True(selectedSyntaxNode != null);
                Assert.True(selectedSyntaxNode is NamespaceDeclarationSyntax);
            }
        }
    }
}
