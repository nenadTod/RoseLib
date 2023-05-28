using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.CSPath;
using RoseLib.Guards;
using RoseLib.Model;
using RoseLib.Traversal.Extensions;
using RoseLib.Traversal.Navigators;
using RoseLib.Traversal.Selectors.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RoseLib.Traversal
{
    public static class NamespaceSelectionExtensions
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        [CSPathConfig(Concept = "Namespace")]
        public static NamespaceNavigator SelectNamespace<T>(this T visitor) where T: INamespaceSelector
        {
            NavigationGuard.CurrentNodeNotNull(visitor.CurrentNode);
            var @namespace = visitor.CurrentNode?
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .FirstOrDefault();

            visitor.NextStep(new SelectedObject(
                @namespace,
                ExtensionsHelper.GetPathPartForMethod(MethodBase.GetCurrentMethod()!)
                )
            );
            

            return visitor.ToNamespaceNavigator();
        }

        [CSPathConfig(Concept = "Namespace", Attribute="name")]
        public static NamespaceNavigator SelectNamespace<T>(this T visitor, string name) where T : INamespaceSelector
        {
            NavigationGuard.CurrentNodeNotNull(visitor.CurrentNode);
            var @namespace = visitor.CurrentNode?
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .Where((nds) => nds.Name.ToFullString().Trim().Equals(name))
                .FirstOrDefault();

            visitor.NextStep(new SelectedObject(
                @namespace,
                ExtensionsHelper.GetPathPartForMethodAndValue(MethodBase.GetCurrentMethod()!, name)
                )
            );

            return visitor.ToNamespaceNavigator();
        }

        [CSPathConfig(Concept = "Namespace", Attribute = "regex")]
        public static NamespaceNavigator SelectNamespace<T>(this T visitor, Regex regex) where T : INamespaceSelector
        {
            NavigationGuard.CurrentNodeNotNull(visitor.CurrentNode);
            var @namespace = visitor.CurrentNode?
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>()
                .Where((nds) => regex.IsMatch(nds.Name.ToFullString()))
                .FirstOrDefault();

            visitor.NextStep(new SelectedObject(
                @namespace,
                ExtensionsHelper.GetPathPartForMethodAndValue(MethodBase.GetCurrentMethod()!, regex.ToString())
                )
            );

            return visitor.ToNamespaceNavigator();
        } 
    }
}
