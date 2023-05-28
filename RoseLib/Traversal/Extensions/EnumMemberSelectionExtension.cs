using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.CSPath.Model;
using RoseLib.CSPath;
using RoseLib.Guards;
using RoseLib.Traversal.Navigators;
using RoseLib.Traversal.Selectors.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using RoseLib.Model;
using RoseLib.Traversal.Extensions;
using System.Reflection;

namespace RoseLib.Traversal
{
    public static class EnumMemberSelectionExtension
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        [CSPathConfig(Concept = "EnumMember", Attribute = "name")]
        public static BaseNavigator SelectEnumMemberDeclaration<T>(this T visitor, string name) where T : IEnumMemberSelector
        {
            NavigationGuard.CurrentNodeNotNull(visitor.CurrentNode);
            NavigationGuard.NameNotNull(name);

            EnumMemberDeclarationSyntax? foundDeclaration = visitor.CurrentNode?
                .DescendantNodes()
                .OfType<EnumMemberDeclarationSyntax>()
                .Where(emd => 
                {
                    return emd.Identifier.Text.Equals(name);
                })
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            visitor.NextStep(new SelectedObject(
                foundDeclaration,
                ExtensionsHelper.GetPathPartForMethodAndValue(MethodBase.GetCurrentMethod()!, name)
                )
            );

            return visitor.ToBaseNavigator();
        }
    }
}
