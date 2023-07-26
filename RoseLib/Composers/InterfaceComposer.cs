using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Model;
using RoseLib.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.Composers
{
    public partial class InterfaceComposer: TypeComposer
    {
        internal InterfaceComposer(IStatefulVisitor visitor, bool pivotOnParent = false) : base(visitor, pivotOnParent)
        {
        }

        public static new bool CanProcessCurrentSelection(IStatefulVisitor statefulVisitor, bool pivotOnParent)
        {
            if (!pivotOnParent)
            {
                return GenericCanProcessCurrentSelectionCheck(statefulVisitor, typeof(InterfaceDeclarationSyntax), SupportedScope.IMMEDIATE_OR_PARENT);
            }
            else
            {
                return GenericCanProcessCurrentSelectionParentCheck(statefulVisitor, typeof(InterfaceDeclarationSyntax));
            }
        }

        protected override void PrepareStateAndSetStatePivot(bool pivotOnParent)
        {
            if(!pivotOnParent)
            {
                GenericPrepareStateAndSetStatePivot(typeof(InterfaceDeclarationSyntax), SupportedScope.IMMEDIATE_OR_PARENT);
            }
            else
            {
                GenericPrepareStateAndSetParentAsStatePivot(typeof(InterfaceDeclarationSyntax));
            }
        }

        public override InterfaceComposer AddProperty(PropertyProps options)
        {
            return (base.AddPropertyToType<InterfaceDeclarationSyntax>(options) as InterfaceComposer)!;
        }
        public override InterfaceComposer AddMethod(MethodProps options)
        {
            return (base.AddMethodToType<InterfaceDeclarationSyntax>(options) as InterfaceComposer)!;
        }
        public override InterfaceComposer SetAttributes(List<AttributeProps> modelAttributeList)
        {
            base.SetAttributes(modelAttributeList);

            return this;
        }

        public InterfaceComposer Delete()
        {
            base.DeleteForParentNodeOfType<InterfaceDeclarationSyntax>();
            return this;
        }

        protected override bool CanHaveBodylessMethod()
        {
            return true;
        }
    }
}
