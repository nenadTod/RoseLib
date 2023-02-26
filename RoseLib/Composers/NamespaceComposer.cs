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
        public NamespaceComposer(IStatefulVisitor visitor) : base(visitor)
        {
        }
   
        public static new bool CanProcessCurrentSelection(IStatefulVisitor statefulVisitor)
        {
            return GenericCanProcessCurrentSelectionCheck(statefulVisitor, typeof(NamespaceDeclarationSyntax), SupporedScope.IMMEDIATE_OR_PARENT);
        }

        public override NamespaceComposer AddClass(ClassProperties options)
        {
            return (this.AddClass<NamespaceDeclarationSyntax>(options) as NamespaceComposer)!;
        }

        public override NamespaceComposer AddInterface(InterfaceProperties properties)
        {
            return (base.AddInterface<NamespaceDeclarationSyntax>(properties) as NamespaceComposer)!;
        }

    }
}
