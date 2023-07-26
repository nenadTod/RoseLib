using IronPython.Compiler.Ast;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Guards;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Composers
{
    public partial class BlockComposer
    {   
        public BlockComposer AddIfBlock(string clause)
        {
            CompositionGuard.ImmediateNodeIs(Visitor.CurrentNode, typeof(BlockSyntax));
            
            string fragment = $"if({clause}){{Console.WriteLine(\"Everything's gonna be alright now!\");}}".Replace('\r', ' ').Replace('\n', ' ');

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
