using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Language;
using RazorViewTemplateEngine.Core.Interface;
using RazorViewTemplateEngine.Core.Internal;
using RazorViewTemplateEngine.Core.Options;

namespace RazorViewTemplateEngine.Core {
    public class RazorEngine : IRazorEngine {
        
        readonly IRuntimeViewCompiler _runtimeViewCompiler;
        public RazorEngine(IRuntimeViewCompiler runtimeViewCompiler) {
            _runtimeViewCompiler = runtimeViewCompiler;
        }
        
        public async Task<string> CompileDynamicAsync(string relativePath,object model) {
            var compiledViewDescriptor = await  _runtimeViewCompiler.CompileAsync(relativePath);
            bool isAnonymous = model != null && model.IsAnonymous();
            if (isAnonymous) {
                model = new AnonymousTypeWrapper(model);
            }
            IRazorViewPage instance = (IRazorViewPage) Activator.CreateInstance(compiledViewDescriptor.TemplateType);
            instance.Model = model;
            await instance.ExecuteAsync();
            return await instance.ResultAsync();
        }
        public async Task<string> CompileGenericAsync<T>(string relativePath,T model) where T : class {
            var compiledViewDescriptor = await  _runtimeViewCompiler.CompileAsync(relativePath);
            IRazorViewPage<T> instance = (IRazorViewPage<T>) Activator.CreateInstance(compiledViewDescriptor.TemplateType);
            instance.Model = model;
            await instance.ExecuteAsync();
            return await instance.ResultAsync();
        }
        
        
        public static IRazorEngine Create(){
            return Create(null,null,null);
        }
        
        public static IRazorEngine Create(Action<RazorFileSystem,IRuntimeViewCompiler> razorFileSystemAction){
            return Create(razorFileSystemAction,null,null);
        }
        
        /// <summary>
        /// 创建 Razor 引擎
        /// </summary>
        /// <param name="templateOptionsAction">模板配置</param>
        /// <param name="mvcRazorRuntimeCompilationOptionsAction">运行时编译配置</param>
        public static IRazorEngine Create(Action<RazorFileSystem,IRuntimeViewCompiler> razorFileSystemAction,Action<TemplateOptions> templateOptionsAction,
            Action<MvcRazorRuntimeCompilationOptions> mvcRazorRuntimeCompilationOptionsAction){
            var templateOptions = new TemplateOptions();
            templateOptionsAction?.Invoke(templateOptions);
            var mvcRazorRuntimeCompilationOptions = new MvcRazorRuntimeCompilationOptions();
            mvcRazorRuntimeCompilationOptionsAction?.Invoke(mvcRazorRuntimeCompilationOptions);
            
            var engine = RazorProjectEngine.Create(RazorConfiguration.Default, 
                RazorProjectFileSystem.Create("."), builder => {
                builder.SetNamespace(mvcRazorRuntimeCompilationOptions.TemplateNamespace);
            });
            var cSharpCompiler = new CSharpCompiler(new RazorReferenceManager(mvcRazorRuntimeCompilationOptions));
            var razorFileSystem = new RazorFileSystem(templateOptions,mvcRazorRuntimeCompilationOptions);
            var runtimeViewCompiler = new RuntimeViewCompiler(engine,
                cSharpCompiler,
                razorFileSystem,
                mvcRazorRuntimeCompilationOptions);
            razorFileSystemAction?.Invoke(razorFileSystem,runtimeViewCompiler);
            return new RazorEngine(runtimeViewCompiler);
        }
    }
}
