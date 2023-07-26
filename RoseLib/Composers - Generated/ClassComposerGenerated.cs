using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Guards;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoseLib.Traversal;
using RoseLib.CSPath;
using System.Runtime.InteropServices;

namespace RoseLib.Composers
{
    public partial class ClassComposer
    {
        public ClassComposer AddGetOneMethod(string name, string returnType)
        {
            CompositionGuard.ImmediateOrParentOfNodeIs(Visitor.CurrentNode, typeof(ClassDeclarationSyntax)); 

            var fragment = $"[Route(\"{{id:int}}\")] public {returnType} {name}(int id){{return _context.getOne(id);}}".Replace('\r', ' ').Replace('\n', ' ');

            var member = SyntaxFactory.ParseMemberDeclaration(fragment); 
            if (member!.ContainsDiagnostics)
            {
                throw new Exception("Idiom filled with provided parameters not rendered as syntactically valid.");
            }

            var referenceNode = TryGetReferenceAndPopToPivot();
            var newEnclosingNode = AddMemberToCurrentNode(member!, referenceNode); 
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, newEnclosingNode);


            var navigator = BaseNavigator.CreateTempNavigator<CSRTypeNavigator>(Visitor); 

            var memberName = RoslynHelper.GetMemberName(member);
            navigator.SelectMethodDeclaration(memberName); 

            return this;
        }
    }
}
