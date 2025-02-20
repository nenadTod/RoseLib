using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoseLib.Guards;
using System.Diagnostics.Metrics;
using RoseLib.Traversal;

namespace RoseLib.Composers
{
    public partial class CompilationUnitComposer
    {
        public CompilationUnitComposer AddVSClass(string namespaceName, string className)
        {
            CompositionGuard.NodeOrParentIs(Visitor.CurrentNode, typeof(CompilationUnitSyntax));
            Visitor.PopUntil(typeof(CompilationUnitSyntax));
            var compilationUnit = (Visitor.CurrentNode as CompilationUnitSyntax)!;
            var fragment = $"namespace {namespaceName} {{ internal class {className} {{ }} }}".Replace('\r', ' ').Replace('\n', ' ');
            var parsedCU = SyntaxFactory.ParseSyntaxTree(fragment).GetRoot();
            if (parsedCU!.ContainsDiagnostics)
            {
                throw new Exception("Idiom filled with provided parameters not rendered as syntactically valid.");
            }

            var @namespace = BaseNavigator.CreateTempNavigator<CompilationUnitNavigator>(parsedCU).SelectNamespace().AsVisitor.CurrentNode as NamespaceDeclarationSyntax;
            CompilationUnitSyntax newCompilationUnit = compilationUnit.AddMembers(@namespace!);
            Visitor.SetHead(newCompilationUnit);
            var memberName = RoslynHelper.GetMemberName(@namespace!);
            CompilationUnitNavigator.CreateTempNavigator(Visitor).SelectNamespace(memberName!);
            return this;
        }


        public CompilationUnitComposer AddMigrationBasis(string migrationName)
        {
            CompositionGuard.NodeOrParentIs(Visitor.CurrentNode, typeof(CompilationUnitSyntax));
            Visitor.PopUntil(typeof(CompilationUnitSyntax));
            var compilationUnit = (Visitor.CurrentNode as CompilationUnitSyntax)!;
            var fragment = $"namespace RentApp.Migrations {{ public partial class {migrationName} : DbMigration {{ public override void Up() {{ }} public override void Down() {{ }} }} }}".Replace('\r', ' ').Replace('\n', ' ').Replace("\u200B", "");
            var parsedCU = SyntaxFactory.ParseSyntaxTree(fragment).GetRoot();
            if (parsedCU!.ContainsDiagnostics)
            {
                throw new Exception("Idiom filled with provided parameters not rendered as syntactically valid.");
            }

            var @namespace = BaseNavigator.CreateTempNavigator<CompilationUnitNavigator>(parsedCU).SelectNamespace().AsVisitor.CurrentNode as NamespaceDeclarationSyntax;
            CompilationUnitSyntax newCompilationUnit = compilationUnit.AddMembers(@namespace!);
            Visitor.SetHead(newCompilationUnit);
            var memberName = RoslynHelper.GetMemberName(@namespace!);
            CompilationUnitNavigator.CreateTempNavigator(Visitor).SelectNamespace(memberName!);
            return this;
        }
    }
}
