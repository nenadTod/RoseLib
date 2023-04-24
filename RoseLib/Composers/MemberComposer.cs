using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
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

        public virtual MemberComposer SetAttributes(List<Model.AttributeProperties> modelAttributeList)
        {
            List<AttributeSyntax> attributeSyntaxList = new List<AttributeSyntax>();
            foreach (var attribute in modelAttributeList)
            {
                AttributeArgumentListSyntax? attributeArgumentListSyntax = null;
                if (attribute.AttributeArgumentsAsString != null)
                {
                    var parameterToBePassed = attribute.AttributeArgumentsAsString;
                    attributeArgumentListSyntax = SyntaxFactory.ParseAttributeArgumentList(parameterToBePassed, 0, CSharpParseOptions.Default, false);
                }

                var attributeSyntax = SyntaxFactory.Attribute(SyntaxFactory.ParseName(attribute.Name), attributeArgumentListSyntax);
                attributeSyntaxList.Add(attributeSyntax);
            }

            var attributeList = SyntaxFactory.AttributeList(new SeparatedSyntaxList<AttributeSyntax>().AddRange(attributeSyntaxList));

            PopToPivot();
            var memberNode = (Visitor.CurrentNode as MemberDeclarationSyntax)!;


            var newTypeNode = memberNode.AddAttributeLists(attributeList);
            Visitor.ReplaceNodeAndAdjustState(memberNode, newTypeNode);

            return this;
        }
    }
}
