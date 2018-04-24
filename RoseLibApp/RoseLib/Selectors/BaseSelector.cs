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
        private Stack<SelectedObject> nodes = new Stack<SelectedObject>();

        public SyntaxNode CurrentNode => nodes.Peek()?.CurrentNode;
        public List<SyntaxNode> CurrentNodesList => nodes.Peek()?.CurrentNodesList;

        protected BaseSelector()
        {
        }

        public BaseSelector(StreamReader reader)
        {
            var code = reader.ReadToEnd();
            nodes.Push(new SelectedObject(SyntaxFactory.ParseCompilationUnit(code)));
        }

        public BaseSelector(SyntaxNode node)
        {
            nodes.Push(new SelectedObject(node));
        }

        public BaseSelector(List<SyntaxNode> nodes)
        {
            this.nodes.Push(new SelectedObject(nodes));
        }

        public bool IsAtRoot()
        {
            return nodes.Count == 1;
        }

        protected void NextStep(SyntaxNode node)
        {
            if(node == null)
            {
                throw new InvalidOperationException($"{typeof(T).Name}: Selection failed!");
            }

            nodes.Push(new SelectedObject(node));
        }

        protected void NextStep(List<SyntaxNode> nodes)
        {
            if (nodes == null)
            {
                throw new InvalidOperationException($"{typeof(T).Name}: Selection failed!");
            }

            this.nodes.Push(new SelectedObject(nodes));
        }

        public T Reset()
        {
            while (nodes.Count > 1)
            {
                nodes.Pop();
            }

            return Composer;
        }

        public T StepBack()
        {
            if(nodes.Peek() != null && nodes.Count > 1)
            {
                nodes.Pop();
            }

            return Composer;
        }

        protected void SetHead(SyntaxNode node)
        {
            nodes.Clear();
            nodes.Push(new SelectedObject(node));
        }

        protected void ReplaceHead(SyntaxNode node)
        {
            nodes.Pop();
            nodes.Push(new SelectedObject(node));
        }
    }
}
