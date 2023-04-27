using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RazorViewTemplateEngine.Core.Interface;
using RazorViewTemplateEngine.Core.Internal;

namespace RazorViewTemplateEngine.Core {
    public class CompiledViewDescriptor {
        private readonly Assembly _assembly;
        public Type TemplateType { get; }
        internal CompiledViewDescriptor(Assembly assembly, string templateNamespace) {
            _assembly = assembly;
            TemplateType = assembly.GetType(templateNamespace + ".Template");
        }
    }
}
