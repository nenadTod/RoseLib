using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Guards;
using RoseLib.Traversal.Navigators;
using RoseLib.Traversal.Selectors.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Traversal
{
    public static class EnumMemberSelectionExtension
    {
        public static BaseNavigator SelectEnumMemberDeclaration<T>(this T navigator, string enumDeclarationName) where T : IEnumMemberSelector
        {
            NavigationGuard.CurrentNodeNotNull(navigator.CurrentNode);
            NavigationGuard.NameNotNull(enumDeclarationName);

            EnumMemberDeclarationSyntax? foundDeclaration = navigator.CurrentNode?
                .DescendantNodes()
                .OfType<EnumMemberDeclarationSyntax>()
                .Where(emd => 
                {
                    return emd.Identifier.Text.Equals(enumDeclarationName);
                })
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            navigator.NextStep(foundDeclaration);

            return navigator.ToBaseNavigator();
        }
    }
}
