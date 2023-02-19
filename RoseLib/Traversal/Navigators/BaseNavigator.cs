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
using RoseLib.Exceptions;

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

        protected BaseNavigator()
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

        internal static T CreateTempNavigator<T>(IStatefulVisitor visitor) where T : BaseNavigator, new()
        {
            var navigator = new T();
            navigator.AsVisitor.State = visitor.State;
            return navigator;
        }

        public T StartComposing<T>() where T : BaseComposer
        {
            // TODO: Extend for different kinds of possible composers.
            if (typeof(T).Equals(typeof(ClassComposer)) && ClassComposer.CanProcessCurrentSelection(this))
            {
                return (new ClassComposer(this) as T)!;
            }

            throw new InvalidActionForStateException("The provided composer type cannot process the state.");
        }

    }
}
