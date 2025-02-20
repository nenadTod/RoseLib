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
    public partial class BlockComposer
    {
        public BlockComposer AddIf(string text)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(BlockSyntax));
            string fragment = $"if(4>3){{ Console.WriteLine({text}); }}".Replace('\r', ' ').Replace('\n', ' ');
            var block = Visitor.CurrentNode as BlockSyntax;
            var currentStatements = block!.Statements;
            var newStatements = CreateStatementList(new string[] { fragment });
            var allStatements = currentStatements.AddRange(newStatements);
            var updatedBlock = block.WithStatements(allStatements);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, updatedBlock);
            var blockNavigator = BaseNavigator.CreateTempNavigator<BlockNavigator>(Visitor);
            blockNavigator.SelectLastStatementDeclaration();
            return this;
        }

        public BlockComposer AddSaveWithConcurrencyHandling(string findOneExpression)
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(BlockSyntax));
            string fragment = $"try {{ db.SaveChanges(); unitOfWork.Complete(); }} catch(DbUpdateConcurrencyException) {{ if({findOneExpression}) {{ return NotFound(); }} else {{ throw; }} }}".Replace('\r', ' ').Replace('\n', ' ').Replace("\u200B", "");
            var block = Visitor.CurrentNode as BlockSyntax;
            var currentStatements = block!.Statements;
            var newStatements = CreateStatementList(new string[] { fragment });
            var allStatements = currentStatements.AddRange(newStatements);
            var updatedBlock = block.WithStatements(allStatements);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, updatedBlock);
            var blockNavigator = BaseNavigator.CreateTempNavigator<BlockNavigator>(Visitor);
            blockNavigator.SelectLastStatementDeclaration();
            return this;
        }

    }
}