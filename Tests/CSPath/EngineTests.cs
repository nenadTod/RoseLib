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
using RoseLib.Traversal.Navigators;

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
        public void TestEngineFindNamespace()
        {
            var cSPath = "/Namespace";
            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CPathEngine cSPathEngine = new CPathEngine();
                var navigator = cSPathEngine.Evaluate(reader, cSPath);


                Assert.True(navigator != null);
                Assert.True(navigator is NamespaceNavigator);
                Assert.True(navigator!.GetCSPath().Equals(cSPath));
            }
        }

        [Test]
        public void TestEngineFindNamespaceWithName()
        {
            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CPathEngine cSPathEngine = new CPathEngine();
                var navigator = cSPathEngine.Evaluate(reader, "/Namespace[name='Tests.TestFiles']");


                Assert.True(navigator != null);
                Assert.True(navigator is NamespaceNavigator);
            }
        }

        [Test]
        public void TestEngineFindClassInNamespace()
        {
            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CPathEngine cSPathEngine = new CPathEngine();
                var navigator = cSPathEngine.Evaluate(reader, "/Namespace/Class");


                Assert.True(navigator != null);
                Assert.True(navigator is CSRTypeNavigator);
            }
        }
    }
}
