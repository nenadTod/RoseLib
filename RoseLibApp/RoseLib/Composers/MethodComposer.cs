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
using RoseLibApp.RoseLib.Templates;

namespace RoseLibApp.RoseLib.Composers
{
    public class MethodComposer : MethodSelector<MethodComposer>, IComposer
    {
        const string STATEMENT_ANNOTATION_KIND = "RoseLibStatement";

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
                throw new Exception("Appending parameters can only be done if method node is selected! Call Reset if you want to select the method node.");
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
                throw new Exception("Setting parameters only be done if method node is selected! Call Reset if you want to select the method node.");
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

        public MethodComposer InsertStatementsBefore(params string[] statements)
        {
            if (!(CurrentNode is StatementSyntax))
            {
                throw new Exception("InsertStatementsBefore can only be called if some statement node is selected!");
            }

            var currentStatement = CurrentNode;

            Reset();
            InsertStatements(currentStatement, statements);
            return this;
        }

        public MethodComposer InsertStatementsAfter(params string[] statements)
        {
            if (!(CurrentNode is StatementSyntax))
            {
                throw new Exception("InsertStatementsAfter can only be called if some statement node is selected!");
            }

            var currentStatement = CurrentNode;

            Reset();
            InsertStatements(currentStatement, statements, false);
            return this;
        }

        public MethodComposer InsertStatements(params string[] statements)
        {
            Reset();
            var method = CurrentNode as MethodDeclarationSyntax;
            InsertStatements(method, statements);

            return this;
        }

        public MethodComposer Body(string body)
        {
            Reset();
            var method = CurrentNode as MethodDeclarationSyntax;
            InsertStatements(method, new string[] { body }, true);

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


        private void InsertStatements(MethodDeclarationSyntax method, string[] statements, bool clear = false)
        {
            SyntaxList<StatementSyntax> currentStatements = new SyntaxList<StatementSyntax>();

            if (!clear)
            {
                currentStatements = method.Body.Statements;
            }

            var block = EvaluateStatements(statements);
            SyntaxAnnotation annotation = null;
            List<StatementSyntax> statementNodes = AnnotateStatements(block.Statements, out annotation); 
            
            currentStatements = currentStatements.AddRange(statementNodes);

            var newBody = method.Body.WithStatements(currentStatements);
            var newMethod = method.WithBody(newBody);

            Replace(method, newMethod, null);
            var annotatedNode = CurrentNode.GetAnnotatedNodes(annotation).First();

            if (annotatedNode != null)
            {
                NextStep(annotatedNode);
            }
        }

        private void InsertStatements(SyntaxNode currentStatement, string[] statements, bool before = true)
        {
            var block = EvaluateStatements(statements);
            SyntaxAnnotation annotation = null;
            List<StatementSyntax> statementNodes = AnnotateStatements(block.Statements, out annotation);
            SyntaxNode newNode = null;
         
            if (before)
            {
                newNode = CurrentNode.InsertNodesBefore(currentStatement, statementNodes);
            }
            else
            {
                newNode = CurrentNode.InsertNodesAfter(currentStatement, statementNodes);
            }

            Replace(CurrentNode, newNode, null);
            var annotatedNode = CurrentNode.GetAnnotatedNodes(annotation).First();

            if (annotatedNode != null)
            {
                NextStep(annotatedNode);
            }
        }

        private BlockSyntax EvaluateStatements(string[] statements)
        {
            var statementsTemplate = new CreateStatements();
            statementsTemplate.statements = statements.ToList();
            var methodStr = statementsTemplate.TransformText();
            var tempMethod = CSharpSyntaxTree.ParseText(methodStr).GetRoot();

            return tempMethod.DescendantNodes().OfType<BlockSyntax>().First();
        }

        private List<StatementSyntax> AnnotateStatements(SyntaxList<StatementSyntax> statements, out SyntaxAnnotation lastAnnotation)
        {
            List<StatementSyntax> annotatedStatements = new List<StatementSyntax>();
            lastAnnotation = null;

            foreach(var statement in statements)
            {
                var annotation = new SyntaxAnnotation(STATEMENT_ANNOTATION_KIND, Guid.NewGuid().ToString());
                annotatedStatements.Add(statement.WithAdditionalAnnotations(annotation));
                lastAnnotation = annotation;
            }

            return annotatedStatements;
        }

        public MethodComposer Delete()
        {
            var nodeForRemoval = CurrentNode;
            Reset();

            var @method = CurrentNode;

            if (@method == nodeForRemoval)
            {
                throw new Exception("Root of the composer cannot be deleted. Deletion can be done using parent selector.");
            }
            if (nodeForRemoval == null)
            {
                throw new Exception("You cannot perform delete operation when the value of the current node is null.");

            }

            var newClass = @method.RemoveNode(nodeForRemoval, SyntaxRemoveOptions.KeepExteriorTrivia);
            Replace(@method, newClass, null);

            return this;
        }
    }
}
