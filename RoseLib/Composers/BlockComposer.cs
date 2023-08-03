using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Exceptions;
using RoseLib.Guards;
using RoseLib.Templates;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Composers
{

    public partial class BlockComposer : BaseComposer
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
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(BlockSyntax));
            var block = Visitor.CurrentNode as BlockSyntax;

            var currentStatements = block!.Statements;

            var newStatements = CreateStatementList(statements);
            var allStatements = currentStatements.AddRange(newStatements);

            var updatedBlock = block.WithStatements(allStatements);
            
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, updatedBlock);

            var blockNavigator = BaseNavigator.CreateTempNavigator<BlockNavigator>(Visitor);
            blockNavigator.SelectLastStatementDeclaration();

            return this;
        }


        #endregion

        #region Navigation 
        public BlockComposer EnterSubblock()
        {
            var statement = Visitor.State.Peek().CurrentNode as StatementSyntax;
            if (statement == null)
            {
                throw new InvalidActionForStateException("Entering subblocks only possible when positioned on a statement syntax instance.");
            }

            var subblock = statement
                .DescendantNodes()
                .OfType<BlockSyntax>()
                .GetClosestDepthwise()
                ?.FirstOrDefault();

            this.Visitor.NextStep(subblock);

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

            CompositionGuard.IsSyntacticallyValid(tempBlock);

            return tempBlock.Statements;
        }
        #endregion
    }
}
