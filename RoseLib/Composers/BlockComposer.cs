using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Guards;
using RoseLib.Templates;
using RoseLib.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Composers
{
    public class BlockComposer : BaseComposer
    {
        public BlockComposer(IStatefulVisitor visitor, bool pivotOnParent = false) : base(visitor, pivotOnParent)
        {
        }

        #region Transition methods
        protected override void PrepareStateAndSetStatePivot(bool pivotOnParent)
        {
            if (pivotOnParent)
            {
                throw new NotSupportedException("Block does not have descendants which composer can handle.");
            }

            GenericPrepareStateAndSetStatePivot(typeof(BlockSyntax), SupportedScope.IMMEDIATE);
        }
        #endregion

        #region Block change methods

        public BlockComposer InsertStatements(params string[] statements)
        {
            CompositionGuard.ImmediateNodeIs(Visitor.CurrentNode, typeof(BlockSyntax));
            var block = Visitor.CurrentNode as BlockSyntax;

            var currentStatements = block!.Statements;

            var newStatements = CreateStatementList(statements);
            var allStatements = currentStatements.AddRange(newStatements);
            
            var updatedBlock = block.WithStatements(allStatements);
            
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, updatedBlock);

            return this;
        }


        #endregion

        #region Helper Methods
        private SyntaxList<StatementSyntax> CreateStatementList(string[] statements)
        {
            var statementsTemplate = new StatementsTemplate();
            statementsTemplate.statements = statements.ToList();
            var methodStr = statementsTemplate.TransformText();
            var tempMethod = CSharpSyntaxTree.ParseText(methodStr).GetRoot();

            var tempBlock = tempMethod.DescendantNodes().OfType<BlockSyntax>().First();

            CompositionGuard.CodeIsSyntacticallyValid(tempBlock);

            return tempBlock.Statements;
        }
        #endregion
    }
}
