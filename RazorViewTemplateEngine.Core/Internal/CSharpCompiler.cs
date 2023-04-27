using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace RazorViewTemplateEngine.Core.Internal {
    internal sealed class CSharpCompiler
    {
        private CSharpCompilationOptions CompilationOptions { get; }
        private readonly RazorReferenceManager _referenceManager;
        public CSharpCompiler(RazorReferenceManager referenceManager)
        {
            _referenceManager = referenceManager;
            CompilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        }
        public SyntaxTree CreateSyntaxTree(SourceText sourceText)
        {
            return CSharpSyntaxTree.ParseText(sourceText);
        }

        public CSharpCompilation CreateCompilation(string assemblyName)
        {
            return CSharpCompilation.Create(
                assemblyName,
                options: CompilationOptions,
                references: _referenceManager.CompilationReferences);
        }
    }
}
