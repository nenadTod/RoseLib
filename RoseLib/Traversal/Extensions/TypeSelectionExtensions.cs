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
using static Microsoft.Scripting.Hosting.Shell.ConsoleHostOptions;

namespace RoseLib.Traversal
{
    public static class TypeSelectionExtensions
    {
        public static CSRTypeNavigator SelectClassDeclaration<T>(this T visitor)
            where T : ITypeSelector
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

        public static CSRTypeNavigator SelectClassDeclaration<T>(this T visitor, string className) where T: ITypeSelector
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

        public static CSRTypeNavigator SelectClassDeclaration<T>(this T visitor, Regex regex) where T : ITypeSelector
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


        public static CSRTypeNavigator SelectStructDeclaration<T>(this T visitor, string structName) where T: ITypeSelector
        {
            var @struct = visitor
                .CurrentNode
                ?.DescendantNodes()
                .OfType<StructDeclarationSyntax>()
                .Where(st => st.Identifier.ToString() == structName)
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(@struct);
            return visitor.ToCSRTypeNavigator();
        }

        public static CSRTypeNavigator SelectRecordDeclaration<T>(this T visitor, string recordName) where T : ITypeSelector
        {
            var record = visitor
                .CurrentNode
                ?.DescendantNodes()
                .OfType<RecordDeclarationSyntax>()
                .Where(re => re.Identifier.ToString() == recordName)
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(record);
            return visitor.ToCSRTypeNavigator();
        }

        public static TypeNavigator SelectInterfaceDeclaration<T>(this T visitor, string interfaceName) where T : ITypeSelector
        {
            var @interface = visitor
                .CurrentNode
                ?.DescendantNodes()
                .OfType<InterfaceDeclarationSyntax>()
                .Where(ids => ids.Identifier.ToString() == interfaceName)
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(@interface);
            return visitor.ToTypeNavigator();
        }

        public static EnumNavigator SelectEnumDeclaration<T>(this T visitor, string enumName) where T : ITypeSelector
        {
            var @enum = visitor
                .CurrentNode
                ?.DescendantNodes()
                .OfType<EnumDeclarationSyntax>()
                .Where(eds => eds.Identifier.ToString() == enumName)
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(@enum);
            return visitor.ToEnumNavigator();
        }
    }
}
