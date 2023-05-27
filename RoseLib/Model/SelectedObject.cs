using Microsoft.CodeAnalysis;
using RoseLib.CSPath.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoseLib.Model
{
    public class SelectedObject
    {
        public SyntaxNode? CurrentNode { get; }

        public List<SyntaxNode>? CurrentNodesList { get; }

        public PathPart PathPart { get; set; }

        public SelectedObject(SyntaxNode node, PathPart pathPart)
        {
            CurrentNode = node;
            PathPart = pathPart;
        }
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
