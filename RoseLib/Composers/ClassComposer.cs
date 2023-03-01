using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Model;
using RoseLib.Traversal.Navigators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoseLib.Exceptions;
using RoseLib.Traversal;

namespace RoseLib.Composers
{

    [Serializable]
    public class ClassComposer: CSRTypeComposer
    {
        public ClassComposer(IStatefulVisitor visitor) : base(visitor)
        {
        }

        public static new bool CanProcessCurrentSelection(IStatefulVisitor statefulVisitor)
        {
            return GenericCanProcessCurrentSelectionCheck(statefulVisitor, typeof(ClassDeclarationSyntax), SupportedScope.IMMEDIATE_OR_PARENT);
        }
        protected override void PrepareStateAndSetStatePivotIndex()
        {
            GenericPrepareStateAndSetStatePivotIndex(typeof(ClassDeclarationSyntax), SupportedScope.IMMEDIATE_OR_PARENT);
        }

        public override ClassComposer AddField(FieldProperties options)
        {
            return (base.AddFieldToNodeOfType<ClassDeclarationSyntax>(options) as ClassComposer)!;
        }

        public override ClassComposer AddProperty(PropertyProperties options)
        {
            return (base.AddPropertyToType<ClassDeclarationSyntax>(options) as ClassComposer)!;
        }
        public override ClassComposer AddMethod(MethodProperties options)
        {
            return (base.AddMethodToType<ClassDeclarationSyntax>(options) as ClassComposer)!;
        }

        public override ClassComposer AddClass(ClassProperties options)
        {
            return (base.AddClassToNodeOfType<ClassDeclarationSyntax>(options) as ClassComposer)!;
        }

        public override ClassComposer AddInterface(InterfaceProperties properties)
        {
            return (base.AddInterfaceToNodeOfType<ClassDeclarationSyntax>(properties) as ClassComposer)!;
        }

        public ClassComposer UpdateField(FieldProperties options)
        {
            
            var existingFieldDeclaration = Visitor.CurrentNode as FieldDeclarationSyntax;

            if(existingFieldDeclaration == null)
            {
                throw new InvalidActionForStateException("A field must be selected to update it");
            }

            TypeSyntax type = SyntaxFactory.ParseTypeName(options.FieldType);
            var declaration = SyntaxFactory.VariableDeclaration(type,
                    SyntaxFactory.SeparatedList(new[]
                        {
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(options.FieldName))
                        }
                    )
                );

            var newFieldDeclaration = SyntaxFactory.FieldDeclaration(new SyntaxList<AttributeListSyntax> { }, options.ModifiersToTokenList(), declaration);

            Visitor.ReplaceNodeAndAdjustState(existingFieldDeclaration, newFieldDeclaration);

            return this;
        }

        public ClassComposer Delete()
        {
            base.DeleteForParentNodeOfType<ClassDeclarationSyntax>();
            return this;
        }
    }
}
