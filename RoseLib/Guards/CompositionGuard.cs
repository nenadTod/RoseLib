using Microsoft.CodeAnalysis;
using RoseLib.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Guards
{
    public static class CompositionGuard
    {
        public static void ImmediateOrParentOfNodeIs(SyntaxNode? node, Type type)
        {
            if (node == null)
            {
                throw new InvalidActionForStateException("Selected node is null");
            }
            if (type == null)
            {
                throw new InvalidUsageException("Type to test against not provided");
            }


            if (node.GetType() == type)
            {
                return;
            }

            if(node.Parent != null && node.Parent.GetType() == type)
            {
                return;
            }

            throw new InvalidActionForStateException("Nor node nor it's parent are of an appropriate type");
        }

        public static void ImmediateNodeIs(SyntaxNode? node, Type type)
        {
            if(node == null)
            {
                throw new InvalidActionForStateException("Selected node is null");
            }
            if (type == null)
            {
                throw new InvalidUsageException("Type to test against not provided");
            }


            if (node.GetType() == type)
            {
                return;
            }

            throw new InvalidActionForStateException("Node is not of an appropriate type");
        }
    }
}
