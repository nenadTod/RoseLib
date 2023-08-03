using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using RoseLib.Guards;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoseLib.Traversal;
using System.Text.RegularExpressions;

namespace RoseLib.Composers
{ 
    public partial class NamespaceComposer
    {

        public NamespaceComposer AddController(string resourceName, string resourcePath)
        {
            CompositionGuard.NodeOrParentIs(Visitor.CurrentNode, typeof(NamespaceDeclarationSyntax));
            var fragment = $@"[RoutePrefix(""api/{resourcePath}"")] public class {resourceName}Controller: ApiController {{ private {resourceName}Context _context; public {resourceName}Controller({resourceName}Context context){{_context = context;}}[Route("""")] public IEnumerable<{resourceName}> GetAll(){{return _context.getAll();}}}}".Replace('\r', ' ').Replace('\n', ' ');
            var member = SyntaxFactory.ParseMemberDeclaration(fragment);
            if (member!.ContainsDiagnostics)
            {
                throw new Exception("Idiom filled with provided parameters not rendered as syntactically valid.");
            }

            var referenceNode = TryGetReferenceAndPopToPivot();
            var newEnclosingNode = AddMemberToCurrentNode(member!, referenceNode);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, newEnclosingNode);
            var navigator = BaseNavigator.CreateTempNavigator<NamespaceNavigator>(Visitor);
            var name = RoslynHelper.GetMemberName(member);
            navigator.SelectClassDeclaration(name);
            return this;
        }
    }
}
