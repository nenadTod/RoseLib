using RoseLibApp.RoseLib.Selectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection.Metadata;
using RoseLibApp.RoseLib.Model;

namespace RoseLibApp.RoseLib.Composers
{
    public class MethodComposer : MethodSelector<MethodComposer>, IComposer
    {
        public IComposer ParentComposer { get; set; }

        public MethodComposer(MethodDeclarationSyntax method, IComposer parent):base(method)
        {
            Composer = this;
            ParentComposer = parent;
        }

        public MethodComposer(MethodDeclarationSyntax node) : base(node)
        {
        }

        public MethodComposer(List<MethodDeclarationSyntax> nodes) : base(nodes)
        {
        }

        public MethodComposer Rename(string newName)
        {
            if (!(CurrentNode is MethodDeclarationSyntax))
            {
                throw new Exception("Rename can only be called if method node is selected! Call Reset if you want to select the method node.");
            }

            var id = SyntaxFactory.Identifier(newName);
            var newNode = (CurrentNode as MethodDeclarationSyntax).WithIdentifier(id);
            Replace(CurrentNode, newNode, null);

            return this;
        }

        public MethodComposer ReturnType(string type)
        {
            if (!(CurrentNode is MethodDeclarationSyntax))
            {
                throw new Exception("ReturnType can only be called if method node is selected! Call Reset if you want to select the method node.");
            }

            var id = SyntaxFactory.IdentifierName(type);
            id = id.WithTrailingTrivia(SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " "));
            var newNode = (CurrentNode as MethodDeclarationSyntax).WithReturnType(id);
            Replace(CurrentNode, newNode, null);

            return this;
        }

        public MethodComposer AppendParameters(params RLParameter[] parameters)
        {
            if (!(CurrentNode is MethodDeclarationSyntax))
            {
                throw new Exception("Rename can only be called if method node is selected! Call Reset if you want to select the method node.");
            }
            var existingParams= (CurrentNode as MethodDeclarationSyntax).ParameterList;

            foreach (var param in parameters)
            {
                var type = SyntaxFactory.IdentifierName(param.Type);
                var name = SyntaxFactory.Identifier(param.Name);
                var paramSyntax = SyntaxFactory
                    .Parameter(new SyntaxList<AttributeListSyntax>(), SyntaxFactory.TokenList(), type, name, null);

                existingParams = existingParams.AddParameters(paramSyntax);
            }
            existingParams = existingParams.NormalizeWhitespace();

            var newNode = (CurrentNode as MethodDeclarationSyntax).WithParameterList(existingParams);
            Replace(CurrentNode, newNode, null);

            return this;
        }

        public MethodComposer Parameters(params RLParameter[] parameters)
        {
            if (!(CurrentNode is MethodDeclarationSyntax))
            {
                throw new Exception("Rename can only be called if method node is selected! Call Reset if you want to select the method node.");
            }
            var @params = SyntaxFactory.ParameterList();
            foreach (var param in parameters)
            {
                var type = SyntaxFactory.IdentifierName(param.Type);
                var name = SyntaxFactory.Identifier(param.Name);
                var paramSyntax = SyntaxFactory
                    .Parameter(new SyntaxList<AttributeListSyntax>(), SyntaxFactory.TokenList(), type, name, null);
                @params = @params.AddParameters(paramSyntax);
            }
            @params = @params.NormalizeWhitespace();
            var newNode = (CurrentNode as MethodDeclarationSyntax).WithParameterList(@params);
            Replace(CurrentNode, newNode, null);

            return this;
        }

        public MethodComposer InsertStatementBefore(string statement)
        {
            return this;
        }

        public MethodComposer InsertStatementAfter(string statement)
        {
            return this;
        }

        public MethodComposer InsertStatement(string statement)
        {
            return this;
        }

        public MethodComposer ReplaceBody(string body)
        {
            return this;
        }

        public List<SyntaxNode> Replace(SyntaxNode oldNode, SyntaxNode newNode, List<SyntaxNode> nodesToTrack)
        {
            if (oldNode.GetType() != newNode.GetType())
            {
                throw new Exception("Old and new node must be of the same type");
            }

            var trackedNodes = new List<SyntaxNode>();
            
            if(nodesToTrack != null)
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
                if (!(oldNode is MethodDeclarationSyntax))
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
    }
}
