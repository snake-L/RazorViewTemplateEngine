using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RazorViewTemplateEngine.Core.Interface {
    public interface IRuntimeViewCompiler {
        /// <summary>
        /// 编译视图对象
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        Task<CompiledViewDescriptor> CompileAsync(string relativePath);
    }
}
