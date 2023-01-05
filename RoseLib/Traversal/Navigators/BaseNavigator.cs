using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using RoseLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoseLib.Composers;
using RoseLib.Traversal.Selectors.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoseLib.Traversal.Navigators
{
    public abstract class BaseNavigator : IBaseSelector
    {
        protected BaseNavigator? ParentNavigator { get; set; }
        public IStatefulVisitor AsVisitor => this;
        public FileInfo? SourceFile { get; set; }

        private Stack<SelectedObject> state = new Stack<SelectedObject>();
        public Stack<SelectedObject> State
        {
            get
            {
                if (ParentNavigator != null)
                {
                    return ParentNavigator.State;
                }
                else
                {
                    return state;
                }
            }

            set
            {
                if (ParentNavigator != null)
                {
                    ParentNavigator.State = value;
                }
                else
                {
                    state = value;
                }
            }
        }
        CompilationUnitSyntax? IStatefulVisitor.TrackedRoot { get; set; }

        public BaseNavigator()
        {
        }

        public BaseNavigator(BaseNavigator? parentNavigator)
        {
            ParentNavigator = parentNavigator;
        }

        public BaseNavigator(string path)
        {
            using (var reader = new StreamReader(path))
            {
                var code = reader.ReadToEnd();
                State.Push(new SelectedObject(SyntaxFactory.ParseCompilationUnit(code)));
            }

            SourceFile = new FileInfo(path);

        }

        public BaseNavigator(StreamReader reader)
        {
            var code = reader.ReadToEnd();
            State.Push(new SelectedObject(SyntaxFactory.ParseCompilationUnit(code)));
        }
    }
}
