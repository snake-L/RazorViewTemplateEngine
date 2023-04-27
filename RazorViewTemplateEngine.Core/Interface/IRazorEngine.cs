using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RazorViewTemplateEngine.Core.Interface {
    public interface IRazorEngine {
        Task<string> CompileDynamicAsync(string relativePath,object model);
        Task<string> CompileGenericAsync<T>(string relativePath, T model) where T : class;
    }
}
