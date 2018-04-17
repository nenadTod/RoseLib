using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoseLibApp.RoseLib.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RoseLibApp.RoseLib.Selectors
{
    public class BaseSelector
    {
        private Stack<SelectedObject> pastNodes;

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

        protected bool NextStep(SyntaxNode node)
        {
            if (node != null)
            {
                pastNodes.Push(Current);
                Current = new SelectedObject(node);
                return true;
            }

            return false;
            
        }

        protected bool NextStep(List<SyntaxNode> nodes)
        {
            if (nodes != null && nodes.Count != 0)
            {
                pastNodes.Push(Current);
                Current = new SelectedObject(nodes);
                return true;
            }

            return false;
        }

        public bool StepBack()
        {
            if(pastNodes.Peek() != null)
            {
                Current = pastNodes.Pop();
                return true;
            }

            return false;
        }
    }
}
