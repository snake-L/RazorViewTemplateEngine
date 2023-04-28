using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RazorViewTemplateEngine.Core.FileDescriptor;

namespace RazorViewTemplateEngine.Core.Interface {
    public interface IRuntimeViewCompiler {
        /// <summary>
        /// 编译视图对象
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        Task<CompiledViewDescriptor> CompileAsync(string relativePath);

        /// <summary>
        /// 重新编译
        /// </summary>
        void OnReCompile(string relativePath);
    }
}
