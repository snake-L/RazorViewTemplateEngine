using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RazorViewTemplateEngine.Core {
    public class CompilationFailureException : Exception {
        public CompilationFailureException(List<Diagnostic> diagnostics,string generatedCode)
        {
            Errors = diagnostics;
            GeneratedCode = generatedCode;
        }

        public IEnumerable<Diagnostic> Errors { get; set; }

        public string GeneratedCode { get; set; }

        public override string Message {
            get {
                string errors = string.Join("\n", this.Errors.Where(w => w.IsWarningAsError || w.Severity == DiagnosticSeverity.Error));
                return "编译模板失败: " + errors;
            }
        }
    }
}
