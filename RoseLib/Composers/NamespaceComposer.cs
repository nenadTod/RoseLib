using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLib.Exceptions;
using RoseLib.Guards;
using RoseLib.Model;
using RoseLib.Templates;
using RoseLib.Traversal;
using RoseLib.Traversal.Navigators;

namespace RoseLib.Composers
{
    public partial class NamespaceComposer : MemberComposer
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

        public NamespaceComposer AddClass(ClassProps options)
        {
            CompositionGuard.NodeOrParentIs(Visitor.CurrentNode, typeof(NamespaceDeclarationSyntax));

            var template = new EmptyClassTemplate() { Properties = options };
            var code = template.TransformText();
            var cu = SyntaxFactory.ParseCompilationUnit(code).NormalizeWhitespace();
            var newClass = cu.DescendantNodes().OfType<ClassDeclarationSyntax>().First();


            var referenceNode = TryGetReferenceAndPopToPivot();
            var newEnclosingNode = AddMemberToCurrentNode(newClass, referenceNode);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, newEnclosingNode);

            BaseNavigator.CreateTempNavigator<NamespaceNavigator>(Visitor)
                .SelectClassDeclaration(options.ClassName);

            return this;
        }

        public ClassComposer EnterClass()
        {
            var @class = Visitor.State.Peek().CurrentNode as ClassDeclarationSyntax;
            if (@class == null)
            {
                throw new InvalidActionForStateException("Entering classes only possible when positioned on a class declaration syntax instance.");
            }

            return new ClassComposer(Visitor);
        }

        public NamespaceComposer AddInterface(InterfaceProps properties)
        {
            CompositionGuard.NodeOrParentIs(Visitor.CurrentNode, typeof(NamespaceDeclarationSyntax));

            var template = new EmptyInterfaceTemplate() { Properties = properties };
            var code = template.TransformText();
            var cu = SyntaxFactory.ParseCompilationUnit(code).NormalizeWhitespace();
            var newInterface = cu.DescendantNodes().OfType<InterfaceDeclarationSyntax>().First();

            var referenceNode = TryGetReferenceAndPopToPivot();
            var newEnclosingNode = AddMemberToCurrentNode(newInterface, referenceNode);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, newEnclosingNode);

            BaseNavigator.CreateTempNavigator<NamespaceNavigator>(Visitor)
                .SelectInterfaceDeclaration(properties.InterfaceName);

            return this;
        }
        public InterfaceComposer EnterInterface()
        {
            var @interface = Visitor.State.Peek().CurrentNode as InterfaceDeclarationSyntax;
            if (@interface == null)
            {
                throw new InvalidActionForStateException("Entering interfaces only possible when positioned on an interface declaration syntax instance.");
            }

            return new InterfaceComposer(Visitor);
        }

        public NamespaceComposer AddEnum(EnumProps properties)
        {
            CompositionGuard.NodeOrParentIs(Visitor.CurrentNode, typeof(NamespaceDeclarationSyntax));

            var template = new EmptyEnumTemplate() { Properties = properties };
            var code = template.TransformText();
            var cu = SyntaxFactory.ParseCompilationUnit(code).NormalizeWhitespace();
            var newEnum = cu.DescendantNodes().OfType<EnumDeclarationSyntax>().First();

            var referenceNode = TryGetReferenceAndPopToPivot();
            var newEnclosingNode = AddMemberToCurrentNode(newEnum, referenceNode);
            Visitor.ReplaceNodeAndAdjustState(Visitor.CurrentNode!, newEnclosingNode);

            BaseNavigator.CreateTempNavigator<NamespaceNavigator>(Visitor)
                .SelectEnumDeclaration(properties.EnumName);

            return this;
        }
        public EnumComposer EnterEnum()
        {
            var @enum = Visitor.State.Peek().CurrentNode as EnumDeclarationSyntax;
            if (@enum == null)
            {
                throw new InvalidActionForStateException("Entering enums only possible when positioned on an enum declaration syntax instance.");
            }

            return new EnumComposer(Visitor);
        }

        public NamespaceComposer Delete()
        {
            base.DeleteForParentNodeOfType<NamespaceDeclarationSyntax>();
            return this;
        }

        protected SyntaxNode AddMemberToCurrentNode(MemberDeclarationSyntax member, MemberDeclarationSyntax? referenceNode = null)
        {
            SyntaxNode newEnclosingNode;
            var @namespace = (Visitor.CurrentNode as NamespaceDeclarationSyntax)!;
            if (referenceNode == null)
            {
                newEnclosingNode = @namespace.AddMembers(member);
            }
            else
            {
                var currentSelection = referenceNode!;
                var currentMembers = @namespace.Members;
                var indexOfSelected = currentMembers.IndexOf(currentSelection);
                if (indexOfSelected == -1)
                {
                    throw new InvalidStateException("For some reason, reference node not found in members.");
                }
                var updatedMembers = currentMembers.Insert(indexOfSelected + 1, member);
                newEnclosingNode = @namespace.WithMembers(updatedMembers);
            }

            return newEnclosingNode;
        }
    }
}
