using System;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RazorViewTemplateEngine.Core.Interface;
using RazorViewTemplateEngine.Core.Internal;
using RazorViewTemplateEngine.Core.Options;

namespace RazorViewTemplateEngine.Core.DependencyInjection {
    public static class ServiceCollectionExtensions {

        /// <summary>
        /// 注入 RazorViewTemplateEngine
        /// </summary>
        /// <param name="templateOptionsAction">模板配置</param>
        /// <param name="mvcRazorRuntimeCompilationOptionsAction">运行时编译配置</param>
        public static IServiceCollection AddRazorViewTemplateEngine(this IServiceCollection services,Action<TemplateOptions> templateOptionsAction = null,
            Action<MvcRazorRuntimeCompilationOptions> mvcRazorRuntimeCompilationOptionsAction = null) {
            services.AddRazorViewTemplateEngineCore(templateOptionsAction,mvcRazorRuntimeCompilationOptionsAction);
            return services;
        }

        private static void AddRazorViewTemplateEngineCore(this IServiceCollection services,Action<TemplateOptions> templateOptionsAction = null,
            Action<MvcRazorRuntimeCompilationOptions> mvcRazorRuntimeCompilationOptionsAction = null) {
            var templateOptions = new TemplateOptions();
            templateOptionsAction?.Invoke(templateOptions);
            var mvcRazorRuntimeCompilationOptions = new MvcRazorRuntimeCompilationOptions();
            mvcRazorRuntimeCompilationOptionsAction?.Invoke(mvcRazorRuntimeCompilationOptions);
            services.AddSingleton(provider => {
                return RazorProjectEngine.Create(RazorConfiguration.Default,
                    RazorProjectFileSystem.Create("."), builder => {
                        builder.SetNamespace(provider
                            .GetRequiredService<MvcRazorRuntimeCompilationOptions>()
                            .TemplateNamespace);
                    });
            });
            services.AddSingleton(templateOptions);
            services.AddSingleton(mvcRazorRuntimeCompilationOptions);
            services.AddSingleton<IRuntimeViewCompiler, RuntimeViewCompiler>();
            services.AddSingleton<RazorReferenceManager>();
            services.AddSingleton<RazorFileSystem>();
            services.AddSingleton<CSharpCompiler>();
            services.AddSingleton<IRazorEngine, RazorEngine>();
        }
    }
}