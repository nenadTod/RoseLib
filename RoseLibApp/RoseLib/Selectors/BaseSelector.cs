using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoseLibApp.RoseLib.Selectors
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
