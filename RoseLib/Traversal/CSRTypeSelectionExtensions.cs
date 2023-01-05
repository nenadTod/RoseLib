using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Traversal.Navigators;
using RoseLib.Traversal.Selectors.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RoseLib.Traversal
{
    // Enums, Interfaces? 
    public static class TypeSelectionExtensions
    {
        public static CSRTypeNavigator SelectClassDeclaration<T>(this T visitor)
            where T : ICSRTypeSelector
        {
            NavigationGuard.CurrentNodeNotNull(visitor.CurrentNode);
            var @class = visitor.CurrentNode?
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(@class);
            return visitor.ToCSRTypeNavigator();
        }

        public static CSRTypeNavigator SelectClassDeclaration<T>(this T visitor, string className) where T: ICSRTypeSelector
        {
            NavigationGuard.CurrentNodeNotNull(visitor.CurrentNode);
            var @class = visitor
                .CurrentNode
                ?.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(cl => cl.Identifier.ToString() == className)
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(@class);
            return visitor.ToCSRTypeNavigator();
        }

        public static CSRTypeNavigator SelectClassDeclaration<T>(this T visitor, Regex regex) where T : ICSRTypeSelector
        {
            NavigationGuard.CurrentNodeNotNull(visitor.CurrentNode);
            var @class = visitor
                .CurrentNode
                ?.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(cl => regex.IsMatch(cl.Identifier.ToString()))
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(@class);
            return visitor.ToCSRTypeNavigator();
        }


        public static ICSRTypeSelector SelectStructDeclaration<T>(this T visitor, string structName) where T: ICSRTypeSelector
        {
            var @struct = visitor
                .CurrentNode
                ?.DescendantNodes()
                .OfType<StructDeclarationSyntax>()
                .Where(st => st.Identifier.ToString() == structName)
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(@struct);
            return visitor;
        }

        public static ICSRTypeSelector SelectRecordDeclaration<T>(this T visitor, string recordName) where T : ICSRTypeSelector
        {
            var @struct = visitor
                .CurrentNode
                ?.DescendantNodes()
                .OfType<RecordDeclarationSyntax>()
                .Where(st => st.Identifier.ToString() == recordName)
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(@struct);
            return visitor;
        }

    }
}
