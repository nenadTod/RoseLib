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
    public class InterfaceComposer: TypeComposer
    {
        public InterfaceComposer(IStatefulVisitor visitor) : base(visitor)
        {
        }

        public static new bool CanProcessCurrentSelection(IStatefulVisitor statefulVisitor)
        {
            return GenericCanProcessCurrentSelectionCheck(statefulVisitor, typeof(InterfaceDeclarationSyntax), SupportedScope.IMMEDIATE_OR_PARENT);
        }
        protected override void PrepareStateAndSetStatePivotIndex()
        {
            GenericPrepareStateAndSetStatePivotIndex(typeof(InterfaceDeclarationSyntax), SupportedScope.IMMEDIATE_OR_PARENT);
        }

        public override InterfaceComposer AddProperty(PropertyProperties options)
        {
            return (base.AddPropertyToType<InterfaceDeclarationSyntax>(options) as InterfaceComposer)!;
        }
        public override InterfaceComposer AddMethod(MethodProperties options)
        {
            return (base.AddMethodToType<InterfaceDeclarationSyntax>(options) as InterfaceComposer)!;
        }

        public override InterfaceComposer AddClass(ClassProperties options)
        {
            return (base.AddClassToNodeOfType<InterfaceDeclarationSyntax>(options) as InterfaceComposer)!;
        }

        public override InterfaceComposer AddInterface(InterfaceProperties properties)
        {
            return (base.AddInterfaceToNodeOfType<InterfaceDeclarationSyntax>(properties) as InterfaceComposer)!;
        }

        public InterfaceComposer Delete()
        {
            base.DeleteForParentNodeOfType<InterfaceDeclarationSyntax>();
            return this;
        }
    }
}
