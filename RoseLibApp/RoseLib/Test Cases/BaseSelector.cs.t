using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RoseLibApp.RoseLib.Selectors.Test
{
    public class BaseSelector
    {
        private SyntaxNode currentNode;
        private List<SyntaxNode> currentNodesList;

        public SyntaxNode CurrentNode
        {
            get { return currentNode; }
            protected set {
                currentNodesList = null;
                currentNode = value;
            }
        }
        public List<SyntaxNode> CurrentNodesList
        {
            get { return currentNodesList; }
            set {
                currentNode = null;
                currentNodesList = value;
            }
        }


        public BaseSelector()
        {
            currentNode = null;
            currentNodesList = null;
        }

        public BaseSelector(StreamReader reader)
        {
            var code = reader.ReadToEnd();
            CurrentNode = SyntaxFactory.ParseCompilationUnit(code);
        }

        public BaseSelector(SyntaxNode node)
        {
            currentNode = node;
        }

        public BaseSelector(List<SyntaxNode> nodes)
        {
            currentNodesList = nodes;
        }
    }
}
