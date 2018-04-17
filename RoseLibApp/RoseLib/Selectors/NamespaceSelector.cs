using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLibApp.RoseLib.Selectors
{
    public class NamespaceSelector : BaseSelector
    {
        public NamespaceSelector(StreamReader reader) : base(reader)
        {

        }

        public NamespaceSelector(SyntaxNode node) : base(node)
        {

        }


        public bool SelectClassDeclaration(string className)
        {
            var @class = CurrentNode?.DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Where(cl => cl.Identifier.ToString() == className).FirstOrDefault();
            
            return NextStep(@class);
        }

        public bool SelectInterfaceDeclaration(string interfaceName)
        {
            var @interface = CurrentNode?.DescendantNodes().OfType<InterfaceDeclarationSyntax>()
                .Where(cl => cl.Identifier.ToString() == interfaceName).FirstOrDefault();

            return NextStep(@interface);
        }

        public bool SelectEnumDeclaration(string enumName)
        {
            var @enum = CurrentNode?.DescendantNodes().OfType<EnumDeclarationSyntax>()
                .Where(en => en.Identifier.ToString() == enumName).FirstOrDefault();

            return NextStep(@enum);
        }

        public bool SelectStructDeclaration(string structName)
        {
            var @struct = CurrentNode?.DescendantNodes().OfType<StructDeclarationSyntax>()
                .Where(st => st.Identifier.ToString() == structName).FirstOrDefault();

            return NextStep(@struct);
        }
    }
}
