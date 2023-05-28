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
using System.Runtime.CompilerServices;
using RoseLib.Model;
using RoseLib.Traversal.Extensions;
using System.Reflection;
using static IronPython.Modules._ast;
using static IronPython.Modules.PythonStruct;

namespace RoseLib.Traversal
{
    public static class TypeSelectionExtensions
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
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

            visitor.NextStep(new SelectedObject(
                @class,
                ExtensionsHelper.GetPathPartForMethod(MethodBase.GetCurrentMethod()!)
                )
            );
            return visitor.ToCSRTypeNavigator();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
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

            visitor.NextStep(new SelectedObject(
                @class,
                ExtensionsHelper.GetPathPartForMethodAndValue(MethodBase.GetCurrentMethod()!, name)
                )
            );
            return visitor.ToCSRTypeNavigator();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
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

            visitor.NextStep(new SelectedObject(
                @class,
                ExtensionsHelper.GetPathPartForMethodAndValue(MethodBase.GetCurrentMethod()!, regex.ToString())
                )
            );
            return visitor.ToCSRTypeNavigator();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
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

            visitor.NextStep(new SelectedObject(
                @struct,
                ExtensionsHelper.GetPathPartForMethodAndValue(MethodBase.GetCurrentMethod()!, name)
                )
            );
            return visitor.ToCSRTypeNavigator();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
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

            visitor.NextStep(new SelectedObject(
                record,
                ExtensionsHelper.GetPathPartForMethodAndValue(MethodBase.GetCurrentMethod()!, name)
                )
            );
            return visitor.ToCSRTypeNavigator();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
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

            visitor.NextStep(new SelectedObject(
                @interface,
                ExtensionsHelper.GetPathPartForMethodAndValue(MethodBase.GetCurrentMethod()!, name)
                )
            );
            return visitor.ToTypeNavigator();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
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

            visitor.NextStep(new SelectedObject(
                 @enum,
                 ExtensionsHelper.GetPathPartForMethodAndValue(MethodBase.GetCurrentMethod()!, name)
                 )
             );
            return visitor.ToEnumNavigator();
        }
    }
}
