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
        private BaseHandler? firstHanlder;
        public CPathEngine()
        {
            Init();
        }

        private void Init()
        {
            firstHanlder = new CompilationUnitHandler();
        }
        
        public SyntaxNode Evaluate(StreamReader reader, string path) 
        {
            var csPathModel = CSPathParser.GetModelForCSPath(path);
            
            var cuNavigator = new CompilationUnitNavigator(reader);
            if (csPathModel.Count == 0) { return cuNavigator.AsVisitor.CurrentNode!; }

            Context context = new Context(cuNavigator, csPathModel[0]);
            int processedPathParts = 0;
            do
            {
                context.PathPart = csPathModel[processedPathParts];
                firstHanlder!.HandleDescent(context);

                processedPathParts++;
            } while (processedPathParts < csPathModel.Count);

            return context.Visitor.CurrentNode!;
        }
    }
}
