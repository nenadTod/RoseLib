using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Scripting.Interpreter;
using RoseLib.Enums;
using RoseLib.Exceptions;
using RoseLib.Guards;
using RoseLib.Model;
using RoseLib.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Composers
{
    public partial class ConstructorComposer : MemberComposer
    {
        public ConstructorComposer(IStatefulVisitor visitor, bool pivotOnParent = false) : base(visitor, pivotOnParent)
        {
        }

        #region Transition methods
        public static new bool CanProcessCurrentSelection(IStatefulVisitor statefulVisitor, bool pivotOnParent)
        {
            if (pivotOnParent)
            {
                throw new NotSupportedException("Metod does not have descendants which composer can handle. It's body does.");
            }
            return GenericCanProcessCurrentSelectionCheck(statefulVisitor, typeof(ConstructorDeclarationSyntax), SupportedScope.IMMEDIATE);
        }

        protected override void PrepareStateAndSetStatePivot(bool pivotOnParent)
        {
            if (pivotOnParent)
            {
                throw new NotSupportedException("Metod does not have descendants which composer can handle. It's body does.");
            }
            
            GenericPrepareStateAndSetStatePivot(typeof(ConstructorDeclarationSyntax), SupportedScope.IMMEDIATE);
        }
        #endregion

        public BlockComposer EnterBody()
        {
            CompositionGuard.NodeIs(Visitor.CurrentNode, typeof(ConstructorDeclarationSyntax));
            
            var constructor = Visitor.State.Peek().CurrentNode as ConstructorDeclarationSyntax;
            if (constructor == null)
            {
                throw new InvalidActionForStateException("Entering body possible when positioned on a method declaration syntax instance.");
            }

            if(constructor.Body == null)
            {
                throw new InvalidOperationException("Cannot enter a body of a bodyless constructor.");
            }

            Visitor.NextStep(new SelectedObject(constructor.Body));

            return new BlockComposer(Visitor);
        }
    }
}
