using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoseLib.Model;
using RoseLib.Traversal.Navigators;
using RoseLib.CSPath.Engine.CoR;

namespace RoseLib.CSPath.Engine
{
    public class CPathEngine
    {
        private BaseHandler? cuHandler;
        public CPathEngine()
        {
            Init();
        }

        private void Init()
        {
            cuHandler = new CompilationUnitHandler();

            var nsHandler = new NamespaceHandler();
            cuHandler.NextHandler = nsHandler;

            var typeHandler = new TypeHandler();
            nsHandler.NextHandler = typeHandler;

            var csrTypeHandler = new CSRTypeHandler();
            typeHandler.NextHandler = csrTypeHandler;

            var enumHandler = new EnumHandler();
            csrTypeHandler.NextHandler = enumHandler;
        }
        
        public BaseNavigator Evaluate(StreamReader reader, string path) 
        {
            var csPathModel = CSPathParser.GetModelForCSPath(path);
            
            var cuNavigator = new CompilationUnitNavigator(reader);
            if (csPathModel.Count == 0) { return cuNavigator; }

            Context context = new Context(cuNavigator, csPathModel[0]);
            int processedPathParts = 0;
            do
            {
                context.PathPart = csPathModel[processedPathParts];
                cuHandler!.HandleDescent(context);

                processedPathParts++;
            } while (processedPathParts < csPathModel.Count);

            return (context.Visitor as BaseNavigator)!;
        }
    }
}
