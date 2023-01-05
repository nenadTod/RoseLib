using Microsoft.CodeAnalysis;
using RoseLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoseLib.ComposersOLD
{
    public interface IComposer
    {
        IComposer ParentComposer { get; set; }
        FileInfo SourceFile { get; set; }
        Stack<SelectedObject> State { get; set; }
        
        List<SyntaxNode> Replace(SyntaxNode oldNode, SyntaxNode newNode, List<SyntaxNode> trackedNodes);
    }
}
