using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.CSPath.Model;
using RoseLib.CSPath;
using RoseLib.Guards;
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
        [CSPathConfig(Concept = "Class")]
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

        [CSPathConfig(Concept = "Class", Attribute = "name")]
        public static CSRTypeNavigator SelectClassDeclaration<T>(this T visitor, string name) where T: ITypeSelector
        {
            NavigationGuard.CurrentNodeNotNull(visitor.CurrentNode);
            var @class = visitor
                .CurrentNode
                ?.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(cl => cl.Identifier.ToString() == name)
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(@class);
            return visitor.ToCSRTypeNavigator();
        }

        [CSPathConfig(Concept = "Class", Attribute = "regex")]
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

        [CSPathConfig(Concept = "Struct", Attribute = "name")]
        public static CSRTypeNavigator SelectStructDeclaration<T>(this T visitor, string name) where T: ITypeSelector
        {
            var @struct = visitor
                .CurrentNode
                ?.DescendantNodes()
                .OfType<StructDeclarationSyntax>()
                .Where(st => st.Identifier.ToString() == name)
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(@struct);
            return visitor.ToCSRTypeNavigator();
        }

        [CSPathConfig(Concept = "Record", Attribute = "name")]
        public static CSRTypeNavigator SelectRecordDeclaration<T>(this T visitor, string name) where T : ITypeSelector
        {
            var record = visitor
                .CurrentNode
                ?.DescendantNodes()
                .OfType<RecordDeclarationSyntax>()
                .Where(re => re.Identifier.ToString() == name)
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(record);
            return visitor.ToCSRTypeNavigator();
        }

        [CSPathConfig(Concept = "Interface", Attribute = "name")]
        public static TypeNavigator SelectInterfaceDeclaration<T>(this T visitor, string name) where T : ITypeSelector
        {
            var @interface = visitor
                .CurrentNode
                ?.DescendantNodes()
                .OfType<InterfaceDeclarationSyntax>()
                .Where(ids => ids.Identifier.ToString() == name)
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(@interface);
            return visitor.ToTypeNavigator();
        }

        [CSPathConfig(Concept = "Enum", Attribute = "name")]
        public static EnumNavigator SelectEnumDeclaration<T>(this T visitor, string name) where T : ITypeSelector
        {
            var @enum = visitor
                .CurrentNode
                ?.DescendantNodes()
                .OfType<EnumDeclarationSyntax>()
                .Where(eds => eds.Identifier.ToString() == name)
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(@enum);
            return visitor.ToEnumNavigator();
        }
    }
}
