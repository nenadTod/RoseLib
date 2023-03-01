using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using RoseLib.Exceptions;
using RoseLib.Model;
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
    public class NamespaceComposer : TypeContainerComposer
    {
        internal NamespaceComposer(IStatefulVisitor visitor, bool pivotOnParent = false) : base(visitor, pivotOnParent)
        {
        }
        protected override void PrepareStateAndSetStatePivot(bool pivotOnParent)
        {
            if (!pivotOnParent)
            {
                GenericPrepareStateAndSetStatePivot(typeof(NamespaceDeclarationSyntax), SupportedScope.IMMEDIATE_OR_PARENT);
            }
            else
            {
                GenericPrepareStateAndSetParentAsStatePivot(typeof(NamespaceDeclarationSyntax));
            }
        }
        public static new bool CanProcessCurrentSelection(IStatefulVisitor statefulVisitor, bool pivotOnParent)
        {
            if (!pivotOnParent)
            {
                return GenericCanProcessCurrentSelectionCheck(statefulVisitor, typeof(NamespaceDeclarationSyntax), SupportedScope.IMMEDIATE_OR_PARENT);
            }
            else
            {
                return GenericCanProcessCurrentSelectionParentCheck(statefulVisitor, typeof(NamespaceDeclarationSyntax));
            }
        }

        public override NamespaceComposer AddClass(ClassProperties options)
        {
            return (base.AddClassToNodeOfType<NamespaceDeclarationSyntax>(options) as NamespaceComposer)!;
        }

        public override NamespaceComposer AddInterface(InterfaceProperties properties)
        {
            return (base.AddInterfaceToNodeOfType<NamespaceDeclarationSyntax>(properties) as NamespaceComposer)!;
        }
        public NamespaceComposer Delete()
        {
            base.DeleteForParentNodeOfType<NamespaceDeclarationSyntax>();
            return this;
        }
    }
}
