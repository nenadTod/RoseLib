using IronPython.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.CSPath
{
    public class CSPathInterpretation
    {
        public static dynamic GetScopeForCSPath(string CSPath)
        {
            var engine = Python.CreateEngine();
            
            ICollection<string> searchPaths = engine.GetSearchPaths();
            searchPaths.Add(".\\CSPath\\Lib");
            engine.SetSearchPaths(searchPaths);

            dynamic scope = engine.CreateScope();
            scope.sentence = CSPath;

            string script = File.ReadAllText(".\\CSPath\\cspath_to_model.py");
            engine.Execute(script, scope);

            return scope;
        }
    }
}
