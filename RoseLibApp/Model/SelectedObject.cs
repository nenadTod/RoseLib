using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoseLibApp.RoseLib.Model
{
    public class SelectedObject
    {
        public SyntaxNode CurrentNode { get; }

        public List<SyntaxNode> CurrentNodesList { get; }

        public SelectedObject(SyntaxNode node)
        {
            CurrentNode = node;
        }

        public SelectedObject(List<SyntaxNode> nodeList)
        {
            CurrentNodesList = nodeList;
        }
    }
}
