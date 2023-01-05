using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib
{
    internal static class RoslynHelper
    {
        internal static int GetNodeDepth(this SyntaxNode node)
        {
            if(node == null)
            {
                throw new ArgumentNullException("Cannot get depth of null");
            }
            
            var depth = 0;
            while(node.Parent != null)
            {
                depth++;
                node = node.Parent;
            }

            return depth;
        }

        internal static IEnumerable<T>? GetClosestDepthwise<T>(this IEnumerable<T> nodes) where T : SyntaxNode
        {
            if(nodes == null || nodes.Count() == 0)
            {
                return new List<T>(); // Not to break anything. 
            }

            var nodesWithDepth = nodes.Select(sn => new Tuple<T, int>(sn, sn.GetNodeDepth()));

            if (nodesWithDepth == null)
            {
                return null;
            }

            var minDepth = nodesWithDepth.Min(tuple => tuple.Item2);
            return nodesWithDepth
                .Where(tuple => tuple.Item2 == minDepth)
                .Select(tuple => tuple.Item1);
        }
    }
}
