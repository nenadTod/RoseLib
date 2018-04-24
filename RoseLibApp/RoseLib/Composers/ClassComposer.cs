using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLibApp.RoseLib.Enums;
using RoseLibApp.RoseLib.Model;
using RoseLibApp.RoseLib.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLibApp.RoseLib.Composers
{
    public class ClassComposer: ClassStructSelector<ClassComposer>, IComposer
    {
        public IComposer ParentComposer { get; set; }

        
        public ClassComposer(ClassDeclarationSyntax @class, IComposer parentComposer) : base(@class)
        {
            Composer = this;
            ParentComposer = parentComposer;
        }

        public ClassComposer(ClassDeclarationSyntax @class):base(@class)
        {
            Composer = this;
        }
        
        public ClassComposer AddMethod(MethodOptions options)
        {
            if (!IsAtRoot())
            {
                throw new Exception("A class must be selected (which is also a root to the composer) to add a method to it.");
            }

            TypeSyntax returnType = SyntaxFactory.ParseTypeName(options.ReturnType);
            var method = SyntaxFactory.MethodDeclaration(returnType, options.MethodName).WithModifiers(options.ModifiersToTokenList());

            var @params = SyntaxFactory.ParameterList();
            foreach (var param in options.Parameters)
            {
                var type = SyntaxFactory.IdentifierName(param.Type);
                var name = SyntaxFactory.Identifier(param.Name);
                var paramSyntax = SyntaxFactory
                    .Parameter(new SyntaxList<AttributeListSyntax>(), SyntaxFactory.TokenList(), type, name, null);
                @params = @params.AddParameters(paramSyntax);
            }
            @params = @params.NormalizeWhitespace();
            method = method.WithParameterList(@params);

            method = method.WithBody(SyntaxFactory.Block());

            var newNode = (CurrentNode as ClassDeclarationSyntax).AddMembers(method);
            Replace(CurrentNode, newNode, null);
            
            return this;
        }

        public ClassComposer AddField(FieldOptions options)
        {
            if (!IsAtRoot())
            {
                throw new Exception("A class must be selected (which is also a root to the composer) to add a field to it.");
            }

            TypeSyntax type = SyntaxFactory.ParseTypeName(options.FieldType);
            var declaration = SyntaxFactory.VariableDeclaration(type,
                    SyntaxFactory.SeparatedList(new[] 
                        {
                            SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(options.FieldName))
                        }
                    )
                );
            

            var fieldDeclaration = SyntaxFactory.FieldDeclaration( new SyntaxList<AttributeListSyntax>{ }, options.ModifiersToTokenList(), declaration);

            var newNode = (CurrentNode as ClassDeclarationSyntax).AddMembers(fieldDeclaration);
            Replace(CurrentNode, newNode, null);

            return this;
        }

        public ClassComposer AddProperty(PropertyOptions options)
        {
            if (!IsAtRoot())
            {
                throw new Exception("A class must be selected (which is also a root to the composer) to add a field to it.");
            }

            PropertyDeclarationSyntax @property = SyntaxFactory
                .PropertyDeclaration(SyntaxFactory.ParseTypeName(options.PropertyType), options.PropertyName)
                .WithModifiers(options.ModifiersToTokenList());
            
            @property = @property.AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                    ));
            @property = @property.AddAccessorListAccessors(
                SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                ));

            var newNode = (CurrentNode as ClassDeclarationSyntax).AddMembers(@property);
            Replace(CurrentNode, newNode, null);

            return this;
        }

        public ClassComposer Rename(string newName)
        {
            if(!(CurrentNode is ClassDeclarationSyntax))
            {
                throw new Exception("Rename can only be called if class node is selected! Call Reset if you want to select the class node.");
            }

            var id = SyntaxFactory.Identifier(newName);
            var newNode = (CurrentNode as ClassDeclarationSyntax).WithIdentifier(id);
            newNode = RenameConstuctors(newNode, id) as ClassDeclarationSyntax;
            Replace(CurrentNode, newNode, null);

            return this;
        }

        public ClassComposer AddStaticKeyword()
        {
            dynamic twm = CurrentNode;
            SyntaxTokenList modifiers = twm.Modifiers;

            if(modifiers.Where(m => m.Kind() == SyntaxKind.StaticKeyword).Any())
            {
                return this;
            }

            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
            SyntaxNode newNode = twm.WithModifiers(modifiers);
            Replace(twm, newNode, null);

            return this;
        }

        public ClassComposer RemoveStaticKeyword()
        {
            dynamic twm = CurrentNode;
            SyntaxTokenList modifiers = twm.Modifiers;
            for (int i = modifiers.Count - 1; i >= 0; i--)
            {
                var m = modifiers.ElementAt(i);
                if (m.Kind() == SyntaxKind.StaticKeyword)
                {
                    modifiers = modifiers.RemoveAt(i);
                    break;
                }
            }

            SyntaxNode newNode = twm.WithModifiers(modifiers);
            Replace(twm, newNode, null);

            return this;
        }

        public ClassComposer SetAccessModifier(AccessModifierTypes newType)
        {
            dynamic twm = CurrentNode;
            SyntaxTokenList modifiers = twm.Modifiers;
            for (int i = modifiers.Count - 1; i >= 0; i--)
            {
                var m = modifiers.ElementAt(i);
                switch (m.Kind())
                {
                    case SyntaxKind.PrivateKeyword:
                    case SyntaxKind.ProtectedKeyword:
                    case SyntaxKind.InternalKeyword:
                    case SyntaxKind.PublicKeyword:
                        modifiers = modifiers.RemoveAt(i);
                        break;
                }
            }

            switch (newType)
            {
                case AccessModifierTypes.PRIVATE:
                    modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
                    break;
                case AccessModifierTypes.PROTECTED:
                    modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
                    break;
                case AccessModifierTypes.PRIVATE_PROTECTED:
                    modifiers = SyntaxFactory.TokenList(modifiers.Concat(new[] { SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.ProtectedKeyword) }));
                    break;
                case AccessModifierTypes.INTERNAL:
                    modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.InternalKeyword));
                    break;
                case AccessModifierTypes.PROTECTED_INTERNAL:
                    modifiers = SyntaxFactory.TokenList(modifiers.Concat(new[] { SyntaxFactory.Token(SyntaxKind.ProtectedKeyword), SyntaxFactory.Token(SyntaxKind.InternalKeyword) }));
                    break;
                case AccessModifierTypes.PUBLIC:
                    modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                    break;
            }

            
            SyntaxNode newNode = twm.WithModifiers(modifiers);
            Replace(twm, newNode, null);

            return this;
        }

        public ClassComposer Delete()
        {
            var nodeForRemoval = CurrentNode;
            Reset();

            var @class = CurrentNode;

            if (@class == nodeForRemoval)
            {
                throw new Exception("Root node of the composer cannot be deleted. Deletion can be done using parent selector.");
            }
            if(nodeForRemoval == null)
            {
                throw new Exception("You cannot perform delete operation when the value of the current node is null.");

            }

            var newClass = @class.RemoveNode(nodeForRemoval, SyntaxRemoveOptions.KeepExteriorTrivia);
            Replace(@class, newClass, null);

            return this;
        }

        public List<SyntaxNode> Replace(SyntaxNode oldNode, SyntaxNode newNode, List<SyntaxNode> nodesToTrack)
        {
            if (oldNode.GetType() != newNode.GetType())
            {
                throw new Exception("Old and new node must be of the same type");
            }

            var trackedNodes = new List<SyntaxNode>();

            if (nodesToTrack != null)
            {
                trackedNodes.AddRange(nodesToTrack);
            }

            Reset();

            var newRoot = CurrentNode;

            if (ParentComposer != null)
            {
                trackedNodes.Add(CurrentNode);
                trackedNodes = ParentComposer.Replace(oldNode, newNode, trackedNodes);
                var tempNode = trackedNodes.LastOrDefault();

                if (tempNode != null)
                {
                    newRoot = tempNode;
                    trackedNodes.Remove(newRoot);
                }
            }
            else
            {
                if (!(oldNode is ClassDeclarationSyntax))
                {
                    newRoot = newRoot.ReplaceNode(oldNode, newNode);
                }
                else
                {
                    newRoot = newNode;
                }
            }

            ReplaceHead(newRoot);
            return trackedNodes;
        }
               
        private SyntaxNode RenameConstuctors(SyntaxNode root, SyntaxToken identifier)
        {
            var constructorCount = root.DescendantNodes().OfType<ConstructorDeclarationSyntax>().Count();
            var newRoot = root;

            for (var current = 0; current < constructorCount; current++)
            {
                var constructors = newRoot.DescendantNodes().OfType<ConstructorDeclarationSyntax>();
                var ctor = constructors.ElementAt(current);

                var newCtor = ctor.WithIdentifier(identifier);
                newRoot = newRoot.ReplaceNode(ctor, newCtor);
            }

            return newRoot;
        }
        
    }
}
