using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Traversal.Navigators;
using RoseLib.Traversal.Selectors.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RoseLib.Traversal
{
    public static class NamespaceSelectionExtensions
    {
        public static NamespaceNavigator SelectNamespace<T>(this T visitor) where T: INamespaceSelector
        {
            NavigationGuard.CurrentNodeNotNull(visitor.CurrentNode);
            var @namespace = visitor.CurrentNode?
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault();

            if (@namespace != null)
            {
                visitor.NextStep(@namespace);
            }

            return visitor.ToNamespaceNavigator();
        }

        public static NamespaceNavigator SelectNamespace<T>(this T visitor, string name) where T : INamespaceSelector
        {
            NavigationGuard.CurrentNodeNotNull(visitor.CurrentNode);
            var @namespace = visitor.CurrentNode?
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .Where((nds) => nds.Name.ToFullString().Trim().Equals(name))
                .FirstOrDefault();

            if (@namespace != null)
            {
                visitor.NextStep(@namespace);
            }

            return visitor.ToNamespaceNavigator();
        }

        public static NamespaceNavigator SelectNamespace<T>(this T visitor, Regex regex) where T : INamespaceSelector
        {
            NavigationGuard.CurrentNodeNotNull(visitor.CurrentNode);
            var @namespace = visitor.CurrentNode?
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .Where((nds) => regex.IsMatch(nds.Name.ToFullString()))
                .FirstOrDefault();

            if (@namespace != null)
            {
                visitor.NextStep(@namespace);
            }

            return visitor.ToNamespaceNavigator();
        } 
    }
}
