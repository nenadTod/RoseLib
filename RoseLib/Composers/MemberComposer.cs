using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Composers
{
    public abstract class MemberComposer : BaseComposer
    {
        protected MemberComposer(IStatefulVisitor visitor, bool pivotOnParent) : base(visitor, pivotOnParent)
        {
        }

        protected MemberDeclarationSyntax? TryGetReferenceAndPopToPivot()
        {
            var enclosingNode = Visitor.GetNodeAtIndex((int)StatePivotIndex!);
            var isAtBase = enclosingNode == Visitor.CurrentNode;
            var referenceNode = isAtBase ? null : Visitor.CurrentNode as MemberDeclarationSyntax;

            PopToPivot();
            return referenceNode;
        }

        protected void PopToPivot()
        {
            Visitor.PopToIndex((int)StatePivotIndex!);
        }
    }
}
