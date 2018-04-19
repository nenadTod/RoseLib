using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoseLibApp.RoseLib.Composers;
using RoseLibApp.RoseLib.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RoseLibApp.RoseLib.Selectors
{
    public class BaseSelector<T> where T: IComposer
    { 
        protected T Composer { get; set; }
        private Stack<SelectedObject> pastNodes = new Stack<SelectedObject>();

        private SelectedObject Current { get; set; }
        public BaseSelector(StreamReader reader)
        {
            var code = reader.ReadToEnd();
            Current =  new SelectedObject(SyntaxFactory.ParseCompilationUnit(code));
        }

        public SyntaxNode CurrentNode { get { return Current.CurrentNode; } }
        public List<SyntaxNode> CurrentNodesList { get { return Current.CurrentNodesList; } }

        public BaseSelector(SyntaxNode node)
        {
            Current = new SelectedObject(node);
        }

        public BaseSelector(List<SyntaxNode> nodes)
        {
            Current = new SelectedObject(nodes);
        }

        protected void NextStep(SyntaxNode node)
        {
            if(node == null)
            {
                throw new InvalidOperationException($"{typeof(T).Name}: Selection failed!");
            }

            pastNodes.Push(Current);
            Current = new SelectedObject(node);
        }

        protected void NextStep(List<SyntaxNode> nodes)
        {
            if (nodes == null)
            {
                throw new InvalidOperationException($"{typeof(T).Name}: Selection failed!");
            }

            pastNodes.Push(Current);
            Current = new SelectedObject(nodes);
        }

        public T StepBack()
        {
            if(pastNodes.Peek() != null)
            {
                Current = pastNodes.Pop();
            }

            return Composer;
        }
    }
}
