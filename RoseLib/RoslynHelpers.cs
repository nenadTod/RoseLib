using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib
{
    public static class RoslynHelper
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

        public static string? GetMemberName(MemberDeclarationSyntax member)
        {
            if (member == null)
            {
                return null;
            }

            switch (member)
            {
                case NamespaceDeclarationSyntax namespaceDeclarationSyntax:
                    return namespaceDeclarationSyntax.Name.ToString();
                case ClassDeclarationSyntax classDeclaration:
                    return classDeclaration.Identifier.Text;
                case InterfaceDeclarationSyntax interfaceDeclaration:
                    return interfaceDeclaration.Identifier.Text;
                case EnumDeclarationSyntax enumDeclaration:
                    return enumDeclaration.Identifier.Text;
                case StructDeclarationSyntax structDeclaration:
                    return structDeclaration.Identifier.Text;
                case MethodDeclarationSyntax methodDeclarationSyntax:
                    return methodDeclarationSyntax.Identifier.Text;
                case PropertyDeclarationSyntax propertyDeclaration:
                    return propertyDeclaration.Identifier.Text;
                case FieldDeclarationSyntax fieldDeclaration:
                    return fieldDeclaration.DescendantNodes()
                        .OfType<VariableDeclaratorSyntax>()
                        .FirstOrDefault()
                        ?.Identifier.Text;
                case null:
                    return null;
                default:
                    throw new ArgumentException($"Extracting name not supported for type ${member.GetType()}");

            }
        }
    }
    internal class CommentsRemover : CSharpSyntaxRewriter
    {
        public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
        {
            switch (trivia.Kind())
            {
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                    return default; // new SyntaxTrivia()  // if C# <= 7.0
                default:
                    return trivia;
            }
        }
    }
}
