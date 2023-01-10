using IronPython.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static IronPython.Modules._ast;

namespace RoseLib.CSPath
{
    public class CSPathInterpretation
    {
        public static dynamic? GetScopeForCSPath(string CSPath)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var dllDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if(dllDirectory == null)
            {
                return null;
            }

            try
            {
                Directory.SetCurrentDirectory(dllDirectory);

                var engine = Python.CreateEngine();

                ICollection<string> searchPaths = engine.GetSearchPaths();
                searchPaths.Add(".\\CSPath\\Lib");
                searchPaths.Add(".\\lib");
                engine.SetSearchPaths(searchPaths);

                dynamic scope = engine.CreateScope();
                scope.sentence = CSPath;

                string script = File.ReadAllText(".\\CSPath\\cspath_to_model.py");
                engine.Execute(script, scope);

                Directory.SetCurrentDirectory(currentDirectory);

                return scope;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                Directory.SetCurrentDirectory(currentDirectory);
            }

            return null;
        }
    }
}
