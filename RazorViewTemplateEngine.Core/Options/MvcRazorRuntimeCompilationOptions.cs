using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using RazorViewTemplateEngine.Core.Internal;

namespace RazorViewTemplateEngine.Core.Options
{
    public sealed class MvcRazorRuntimeCompilationOptions {
        public HashSet<Assembly> ReferenceAssemblies { get; internal set; }
        public HashSet<MetadataReference> MetadataReferences { get; internal set; }
        public List<string> Usings { get; internal set; } 
        public string TemplateNamespace { get; set; } = "RazorViewTemplateEngine.Core";
        internal string Inherits { get; set; } = "RazorViewTemplateEngine.Core.RazorViewPage";
        public MvcRazorRuntimeCompilationOptions() {
            MetadataReferences = new HashSet<MetadataReference>();
            ReferenceAssemblies = new HashSet<Assembly> {
                typeof(object).Assembly,
                    Assembly.Load(new AssemblyName("Microsoft.CSharp")),
                    typeof(RazorViewPage).Assembly,
                    Assembly.Load(new AssemblyName("System.Runtime")),
                    typeof(System.Collections.IList).Assembly,
                    typeof(IEnumerable<>).Assembly,
                    Assembly.Load(new AssemblyName("System.Linq")),
                    Assembly.Load(new AssemblyName("System.Linq.Expressions")),
                    Assembly.Load(new AssemblyName("netstandard"))
            };
            Usings = new List<string>() {
                 "System",
                 "System.Collections.Generic",
                 "System.Linq",
                 "System.Linq.Expressions",
                 "System.Threading.Tasks",
                 "RazorViewTemplateEngine.Core"
            };
        }

        public MvcRazorRuntimeCompilationOptions AddAssemblyReferenceByName(string assemblyName) {
            Assembly assembly = Assembly.Load(new AssemblyName(assemblyName));
            AddAssemblyReference(assembly);
            return this;
        }

        public MvcRazorRuntimeCompilationOptions AddAssemblyReference(Assembly assembly) {
            ReferenceAssemblies.Add(assembly);
            return this;
        }

        public MvcRazorRuntimeCompilationOptions AddAssemblyReference(Type type) {
            AddAssemblyReference(type.Assembly);

            foreach (Type argumentType in type.GenericTypeArguments) {
                AddAssemblyReference(argumentType);
            }
            return this;
        }

        public MvcRazorRuntimeCompilationOptions AddMetadataReference(MetadataReference reference) {
            MetadataReferences.Add(reference);
            return this;
        }

        public MvcRazorRuntimeCompilationOptions AddUsing(string namespaceName) {
            Usings.Add(namespaceName);
            return this;
        }
        private string RenderTypeName(Type type) {
            IList<string> elements = new List<string>()
            {
                type.Namespace,
                RenderDeclaringType(type.DeclaringType),
                type.Name
            };

            string result = string.Join(".", elements.Where(e => !string.IsNullOrWhiteSpace(e)));

            if (result.Contains('`')) {
                result = result.Substring(0, result.IndexOf("`"));
            }

            if (type.GenericTypeArguments.Length == 0) {
                return result;
            }

            return result + "<" + string.Join(",", type.GenericTypeArguments.Select(RenderTypeName)) + ">";
        }
        private string RenderDeclaringType(Type type) {
            if (type == null) {
                return null;
            }

            string parent = RenderDeclaringType(type.DeclaringType);

            if (string.IsNullOrWhiteSpace(parent)) {
                return type.Name;
            }

            return parent + "." + type.Name;
        }
    }
}
