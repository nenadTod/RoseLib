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
                Assert.True(navigator!.AsVisitor.GetCSPathImpl().Equals(cSPath));
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

        [Test]
        public void TestEngineFindClassByName()
        {
            var csPath = "/Class[name='Class1']";
            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CPathEngine cSPathEngine = new CPathEngine();
                var navigator = cSPathEngine.Evaluate(reader, csPath);


                Assert.True(navigator != null);
                Assert.True(navigator is CSRTypeNavigator);
                Assert.True(navigator!.GetCSPath().Equals(csPath));
            }
        }

        [Test]
        public void TestEngineFindInterfaceByName()
        {
            var csPath = "/Interface[name='INestedInterface1']";
            using (StreamReader reader = new StreamReader(".\\TestFiles\\Interface1.cs"))
            {
                CPathEngine cSPathEngine = new CPathEngine();
                var navigator = cSPathEngine.Evaluate(reader, csPath);


                Assert.True(navigator != null);
                Assert.True(navigator is TypeNavigator);
                Assert.True(navigator!.GetCSPath().Equals(csPath));
            }
        }

        [Test]
        public void TestEngineFindEnumByName()
        {
            var csPath = "/Enum[name='Enum1']";
            using (StreamReader reader = new StreamReader(".\\TestFiles\\Enum1.cs"))
            {
                CPathEngine cSPathEngine = new CPathEngine();
                var navigator = cSPathEngine.Evaluate(reader, csPath);


                Assert.True(navigator != null);
                Assert.True(navigator is EnumNavigator);
                Assert.True(navigator!.GetCSPath().Equals(csPath));
            }
        }

        [Test]
        public void TestEngineFindEnumMemberByName()
        {
            var csPath = "/Enum[name='Enum1']/EnumMember[name='green']";
            using (StreamReader reader = new StreamReader(".\\TestFiles\\Enum1.cs"))
            {
                CPathEngine cSPathEngine = new CPathEngine();
                var navigator = cSPathEngine.Evaluate(reader, csPath);


                Assert.True(navigator != null);
                Assert.True(navigator is BaseNavigator);
                Assert.True(navigator!.GetCSPath().Equals(csPath));
            }
        }

        [Test]
        public void TestEngineFindMethodInClassByName()
        {
            var csPath = "/Class[name='Class1']/Method[name='Method1']";
            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CPathEngine cSPathEngine = new CPathEngine();
                var navigator = cSPathEngine.Evaluate(reader, csPath);


                Assert.True(navigator != null);
                Assert.True(navigator is MethodNavigator);
                Assert.True(navigator!.GetCSPath().Equals(csPath));
            }
        }

        [Test]
        public void TestEngineFindPropInClassByName()
        {
            var csPath = "/Class[name='Class1']/Property[name='Prop1']";
            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CPathEngine cSPathEngine = new CPathEngine();
                var navigator = cSPathEngine.Evaluate(reader, csPath);


                Assert.True(navigator != null);
                Assert.True(navigator is PropertyNavigator);
                Assert.True(navigator!.GetCSPath().Equals(csPath));
            }
        }

        [Test]
        public void TestEngineFindFieldInClassByName()
        {
            var csPath = "/Class[name='Class1']/Field[name='field1']";
            using (StreamReader reader = new StreamReader(".\\TestFiles\\Class1.cs"))
            {
                CPathEngine cSPathEngine = new CPathEngine();
                var navigator = cSPathEngine.Evaluate(reader, csPath);


                Assert.True(navigator != null);
                Assert.True(navigator is FieldNavigator);
                Assert.True(navigator!.GetCSPath().Equals(csPath));
            }
        }
    }
}
