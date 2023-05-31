using IronPython.Compiler.Ast;
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

        public PathPart? PathPart { get; set; }

        public SelectedObject(SyntaxNode? node, PathPart? pathPart)
        {
            if (node == null)
            {
                throw new InvalidOperationException($"{GetType()}: Selection failed!");
            }

            CurrentNode = node;
            PathPart = pathPart;
        }
        public SelectedObject(SyntaxNode? node)
        {
            if (node == null)
            {
                throw new InvalidOperationException($"{GetType()}: Selection failed!");
            }

            CurrentNode = node;
        }

        public SelectedObject(List<SyntaxNode> nodeList, PathPart? pathPart)
        {
            if (nodeList == null)
            {
                throw new InvalidOperationException($"{GetType()}: Selection failed!");
            }

            CurrentNodesList = nodeList;
            PathPart = pathPart;
        }
    }
}
