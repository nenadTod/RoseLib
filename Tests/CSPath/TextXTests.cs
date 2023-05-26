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

namespace Tests.CSPath
{
    public class TextXTests
    {
        [Test]
        public void TestSimpleCSPathExpression()
        {
            var engine = Python.CreateEngine();
            ICollection<string> searchPaths = engine.GetSearchPaths();
            //searchPaths.Add("C:\\Users\\ntodo\\Desktop\\Doktorske\\ironPython\\IronPython.3.4.0-beta1\\Lib");
            searchPaths.Add("C:\\Users\\ntodo\\Desktop\\Doktorske\\ironPython\\IronPython.3.4.0-beta1\\Lib\\site-packages");
            searchPaths.Add("C:\\Python34\\Lib");
            engine.SetSearchPaths(searchPaths);

            dynamic scope = engine.CreateScope();

            var theScript = @"
from textx import metamodel_from_str 

grammar = \
""""""
CSPath:
    path += PathElements
    ;

PathElements:
    RelativeDescend | Descend | Element
    ;

RelativeDescend:
    tokens = /(\/\/)/
    ;

Descend:
    tokens = /(\/)/
    ;

Element:
    name = /[^\/]*/
    ;
            """"""

hello_meta = metamodel_from_str(grammar)

sentence = ""//Namespace/Class/Field""

model = hello_meta.model_from_str(sentence)
";
            engine.Execute(theScript, scope);

            Assert.Pass();

        }

        [Test]
        public void TestRoseLibCSPathIntegration()
        {
            var output = CSPathParser.GetRawModelForCSPath("/Namespace/Class/Field");

            Assert.NotNull(output.rawModel);
            Assert.That(output.rawModelSize, NUnit.Framework.Is.EqualTo(3));
        }

        [Test]
        public void TestCSPathModelExtraction()
        {
            // For now, only strings are fully supported
            // There is no need for other types at the moment
            var model = CSPathParser.GetModelForCSPath("/Namespace[name='test']/Class/Field");

            Assert.NotNull(model);
            Assert.That(model.Count, NUnit.Framework.Is.EqualTo(3));
        }
    }
}
