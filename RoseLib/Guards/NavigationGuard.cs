using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RoseLib.Guards
{
    public static class NavigationGuard
    {
        public static void CurrentNodeNotNull(SyntaxNode? node)
        {
            if (node == null)
            {
                throw new Exception("Current composer state is not valid: current node is null");
            }
        }

        public static void CurrentNodeListNotEmpty(List<SyntaxNode>? nodeList)
        {
            if (nodeList == null || nodeList.Count == 0)
            {
                throw new Exception("Current composer state is not valid: current node is null");
            }
        }

        public static void NameNotNull(string? name)
        {
            if (name == null)
            {
                throw new Exception("Passed name to search by must not be null");
            }
        }

    }
}
