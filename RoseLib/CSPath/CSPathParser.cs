using IronPython.Hosting;
using RoseLib.CSPath.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static IronPython.Modules._ast;

namespace RoseLib.CSPath
{
    public class CSPathParser
    {
        public static List<PathPart> GetModelForCSPath(string cSPath)
        {
            var rawModelWithSize  = GetRawModelForCSPath(cSPath);

            var rawModel = rawModelWithSize.rawModel;
            var size = rawModelWithSize.rawModelSize;

            var model = new List<PathPart>();
            for(int i = 0; i < size; i++)
            {
                var modelMember = rawModel![i];
                
                // _tx_fqn is a field provided by TextX
                // var modelTypeString = modelMember._tx_fqn as string;

                var pathPart = GetPathPartForRawMember(modelMember);
                model.Add(pathPart);
            }

            return model;
        }
        public static (dynamic? rawModel, int rawModelSize) GetRawModelForCSPath(string CSPath)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var dllDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if(dllDirectory == null)
            {
                return (null, 0);
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

                return (scope.model.path, scope.path_elements_count);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                Directory.SetCurrentDirectory(currentDirectory);
            }

            return (null, 0);
        }

        public static PathPart GetPathPartForRawMember(dynamic modelMember)
        {
            Descend? descend = null;
            if(modelMember.descend != null)
            {
                descend = new Descend(modelMember.descend.tokens);
            }

            Concept? concept = null;
            if (modelMember.concept != null)
            {
                var rawConcept = modelMember.concept;
                Predicate? predicate = null;
                if (rawConcept.predicate != null)
                {
                    var rawPredicate = rawConcept.predicate;
                    
                    // Equating predicate with predicate expression
                    // Due to the grammar and how TextX builds a model, this means that here we will
                    // have the attribute predicate, with an attribute and a value.
                    rawPredicate = rawPredicate.predicateExpression;
                    predicate = new Predicate(rawPredicate.attribute, rawPredicate.value);
                }
                concept = new Concept(rawConcept.name, predicate);

            }

            if(descend == null || concept == null)
            {
                throw new Exception();
            }

            return new PathPart(descend, concept);
        }
    }
}
