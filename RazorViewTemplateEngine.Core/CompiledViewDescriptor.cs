using System;
using System.Reflection;

namespace RazorViewTemplateEngine.Core {
    public class CompiledViewDescriptor : IDisposable {
        private Assembly _assembly;
        public Type TemplateType { get; private set; }
        internal CompiledViewDescriptor(Assembly assembly, string templateNamespace) {
            _assembly = assembly;
            TemplateType = assembly.GetType(templateNamespace + ".Template");
        }
        public void Dispose() {
            _assembly = null;
            TemplateType = null;
            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }
}
