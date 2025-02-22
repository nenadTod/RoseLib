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
using static IronPython.Modules._ast;

namespace RoseLib.Composers
{
    public partial class ClassComposer
    {
        public ClassComposer AddGetOneMethod(string name, string returnType)
        {
            CompositionGuard.NodeOrParentIs(Visitor.CurrentNode, typeof(ClassDeclarationSyntax)); 

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

        public ClassComposer AddDBSet(string type, string setName)
        {
            CompositionGuard.NodeOrParentIs(Visitor.CurrentNode, typeof(ClassDeclarationSyntax));
            var fragment = $"public DbSet<{type}> {setName} {{ get; set; }}".Replace('\r', ' ').Replace('\n', ' ').Replace("\u200B", "");
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
            navigator.SelectPropertyDeclaration(memberName);
            return this;
        }

        public ClassComposer AddGetAllRepositoryMethod(string type, string dbsetName)
        {
            CompositionGuard.NodeOrParentIs(Visitor.CurrentNode, typeof(ClassDeclarationSyntax));
            var fragment = $"public IEnumerable<{type}> GetAll(int pageIndex, int pageSize) {{ return RADBContext.{dbsetName}.Skip((pageIndex - 1) * pageSize).Take(pageSize); }}".Replace('\r', ' ').Replace('\n', ' ').Replace("\u200B", "");
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

        public ClassComposer AddUoWDependency(string iRepositoryName, string name)
        {
            CompositionGuard.NodeOrParentIs(Visitor.CurrentNode, typeof(ClassDeclarationSyntax));
            var fragment = $"[Dependency] public {iRepositoryName} {name} {{ get; set; }}".Replace('\r', ' ').Replace('\n', ' ').Replace("\u200B", "");
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
            navigator.SelectPropertyDeclaration(memberName);
            return this;
        }

        public ClassComposer AddPutRESTMethod(string methodName, string methodParameter)
        {
            CompositionGuard.NodeOrParentIs(Visitor.CurrentNode, typeof(ClassDeclarationSyntax));
            var fragment = $"[ResponseType(typeof(void))] public IHttpActionResult {methodName}(int id, {methodParameter}) {{ if(!ModelState.IsValid) {{ return BadRequest(ModelState); }} }}".Replace('\r', ' ').Replace('\n', ' ').Replace("\u200B", "");
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
